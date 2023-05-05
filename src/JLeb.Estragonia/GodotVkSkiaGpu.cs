using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Unicode;
using Avalonia;
using Avalonia.Platform;
using Avalonia.Skia;
using Godot;
using SkiaSharp;
using static JLeb.Estragonia.VkConstants;

namespace JLeb.Estragonia;

/// <summary>Bridges the Godot Vulkan renderer with a Skia context used by Avalonia.</summary>
internal sealed class GodotVkSkiaGpu : ISkiaGpu {

	private readonly GRContext _grContext;
	private readonly uint _queueFamilyIndex;

	public bool IsLost
		=> _grContext.IsAbandoned;

	public unsafe GodotVkSkiaGpu() {
		var gdRd = RenderingServer.GetRenderingDevice();

		var vkInstance = (IntPtr) gdRd.GetDriverResource(RenderingDevice.DriverResource.Instance, default, 0UL);
		var vkPhysicalDevice = (IntPtr) gdRd.GetDriverResource(RenderingDevice.DriverResource.PhysicalDevice, default, 0UL);
		var vkDevice = (IntPtr) gdRd.GetDriverResource(RenderingDevice.DriverResource.Device, default, 0UL);
		var vkQueue = (IntPtr) gdRd.GetDriverResource(RenderingDevice.DriverResource.Queue, default, 0UL);
		var vkQueueFamilyIndex = (uint) gdRd.GetDriverResource(RenderingDevice.DriverResource.QueueFamilyIndex, default, 0UL);

		var vkLibrary = NativeLibrary.Load(OperatingSystem.IsWindows() ? "vulkan-1" : "libvulkan");
		var vkGetInstanceProcAddr =
			(delegate* unmanaged[Cdecl]<IntPtr, byte*, IntPtr>) NativeLibrary.GetExport(vkLibrary, "vkGetInstanceProcAddr");
		var vkGetDeviceProcAddr =
			(delegate* unmanaged[Cdecl]<IntPtr, byte*, IntPtr>) NativeLibrary.GetExport(vkLibrary, "vkGetDeviceProcAddr");

		IntPtr GetVkProcAddress(string name, IntPtr instance, IntPtr device) {
			Span<byte> utf8Name = stackalloc byte[128];

			// The stackalloc buffer should always be sufficient for proc names
			if (Utf8.FromUtf16(name, utf8Name[..^1], out _, out var bytesWritten) != OperationStatus.Done)
				throw new InvalidOperationException($"Invalid proc name {name}");

			utf8Name[bytesWritten] = 0;

			fixed (byte* utf8NamePtr = utf8Name) {
				return device != IntPtr.Zero
					? vkGetDeviceProcAddr(device, utf8NamePtr)
					: vkGetInstanceProcAddr(instance, utf8NamePtr);
			}
		}

		var vkContext = new GRVkBackendContext {
			VkInstance = vkInstance,
			VkPhysicalDevice = vkPhysicalDevice,
			VkDevice = vkDevice,
			VkQueue = vkQueue,
			GraphicsQueueIndex = vkQueueFamilyIndex,
			GetProcedureAddress = GetVkProcAddress
		};

		if (GRContext.CreateVulkan(vkContext) is not { } grContext)
			throw new InvalidOperationException("Couldn't create Vulkan context");

		_grContext = grContext;
		_queueFamilyIndex = vkQueueFamilyIndex;
	}

	object? IOptionalFeatureProvider.TryGetFeature(Type featureType)
		=> null;

	IDisposable IPlatformGraphicsContext.EnsureCurrent()
		=> EmptyDisposable.Instance;

	ISkiaGpuRenderTarget? ISkiaGpu.TryCreateRenderTarget(IEnumerable<object> surfaces)
		=> surfaces.OfType<GodotSkiaSurface>().FirstOrDefault() is { } surface
			? new GodotSkiaRenderTarget(surface, _grContext)
			: null;

	public GodotSkiaSurface CreateSurfaceFromTexture(Texture2D gdTexture) {
		var gdRdTexture = RenderingServer.TextureGetRdTexture(gdTexture.GetRid());
		if (!gdRdTexture.IsValid)
			throw new InvalidOperationException("Couldn't get Godot rendering device texture");

		var gdRd = RenderingServer.GetRenderingDevice();

		var vkImage = gdRd.GetDriverResource(RenderingDevice.DriverResource.Image, gdRdTexture, 0UL);
		if (vkImage == 0UL)
			throw new InvalidOperationException("Couldn't get Vulkan image from Godot texture");

		var vkFormat = (uint) gdRd.GetDriverResource(RenderingDevice.DriverResource.ImageNativeTextureFormat, gdRdTexture, 0UL);
		if (vkFormat == 0U)
			throw new InvalidOperationException("Couldn't get Vulkan format from Godot texture");

		// The flags should correspond to what Godot's Vulkan renderer uses for a ViewportTexture.
		// https://github.com/godotengine/godot/blob/master/drivers/vulkan/rendering_device_vulkan.cpp
		var grVkImageInfo = new GRVkImageInfo {
			CurrentQueueFamily = _queueFamilyIndex,
			Format = vkFormat,
			Image = vkImage,
			ImageLayout = (uint) VkImageLayout.SHADER_READ_ONLY_OPTIMAL,
			ImageTiling = (uint) VkImageTiling.OPTIMAL,
			ImageUsageFlags = (uint) (
				VkImageUsageFlagBits.SAMPLED_BIT |
				VkImageUsageFlagBits.TRANSFER_SRC_BIT |
				VkImageUsageFlagBits.TRANSFER_DST_BIT |
				VkImageUsageFlagBits.COLOR_ATTACHMENT_BIT
			),
			LevelCount = 1,
			SampleCount = 1,
			Protected = false,
			SharingMode = (uint) VkSharingMode.EXCLUSIVE
		};

		var skSurface = SKSurface.Create(
			_grContext,
			new GRBackendRenderTarget(gdTexture.GetWidth(), gdTexture.GetHeight(), 1, grVkImageInfo),
			GRSurfaceOrigin.TopLeft,
			SKColorType.Rgba8888,
			new SKSurfaceProperties(SKPixelGeometry.RgbHorizontal)
		);

		if (skSurface is null)
			throw new InvalidOperationException("Couldn't create Skia surface from Vulkan image");

		return new GodotSkiaSurface(skSurface, gdTexture);
	}

	ISkiaSurface ISkiaGpu.TryCreateSurface(PixelSize size, ISkiaGpuRenderSession? session) {
		size = new PixelSize(Math.Max(size.Width, 1), Math.Max(size.Height, 1));

		var viewport = new SubViewport {
			RenderTargetClearMode = SubViewport.ClearMode.Never,
			TransparentBg = true,
			RenderTargetUpdateMode = SubViewport.UpdateMode.Disabled,
			Size = new Vector2I(size.Width, size.Height)
		};

		return CreateSurfaceFromTexture(viewport.GetTexture());
	}

	public void Dispose()
		=> _grContext.Dispose();

}
