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
using static JLeb.Estragonia.VkInterop;

namespace JLeb.Estragonia;

/// <summary>Bridges the Godot Vulkan renderer with a Skia context used by Avalonia.</summary>
internal sealed class GodotVkSkiaGpu : ISkiaGpu {

	private readonly RenderingDevice _renderingDevice;
	private readonly GRContext _grContext;
	private readonly uint _queueFamilyIndex;
	private readonly VkBarrierHelper _barrierHelper;

	public bool IsLost
		=> _grContext.IsAbandoned;

	public unsafe GodotVkSkiaGpu() {
		_renderingDevice = RenderingServer.GetRenderingDevice();

		if (_renderingDevice is null)
			throw new NotSupportedException("Estragonia is only supported on Vulkan renderers (Forward+ or Mobile)");

		IntPtr GetIntPtrDriverResource(RenderingDevice.DriverResource resource) {
			var result = (IntPtr) _renderingDevice.GetDriverResource(resource, default, 0UL);

			if (result == IntPtr.Zero)
				throw new InvalidOperationException($"Godot returned null for driver resource {resource}");

			return result;
		}

		var vkInstance = new VkInstance(GetIntPtrDriverResource(RenderingDevice.DriverResource.TopmostObject));
		var vkPhysicalDevice = new VkPhysicalDevice(GetIntPtrDriverResource(RenderingDevice.DriverResource.PhysicalDevice));
		var vkDevice = new VkDevice(GetIntPtrDriverResource(RenderingDevice.DriverResource.LogicalDevice));
		var vkQueue = new VkQueue(GetIntPtrDriverResource(RenderingDevice.DriverResource.CommandQueue));
		var vkQueueFamilyIndex = (uint) _renderingDevice.GetDriverResource(RenderingDevice.DriverResource.QueueFamily, default, 0UL);

		var vkLibrary = NativeLibrary.Load(OperatingSystem.IsWindows() ? "vulkan-1" : "libvulkan", typeof(GodotVkSkiaGpu).Assembly, null);
		var vkGetInstanceProcAddr =
			(delegate* unmanaged[Stdcall]<VkInstance, byte*, IntPtr>) NativeLibrary.GetExport(vkLibrary, "vkGetInstanceProcAddr");
		var vkGetDeviceProcAddr =
			(delegate* unmanaged[Stdcall]<VkDevice, byte*, IntPtr>) NativeLibrary.GetExport(vkLibrary, "vkGetDeviceProcAddr");

		IntPtr GetVkProcAddress(string name, IntPtr instance, IntPtr device) {
			Span<byte> utf8Name = stackalloc byte[128];

			// The stackalloc buffer should always be sufficient for proc names
			if (Utf8.FromUtf16(name, utf8Name[..^1], out _, out var bytesWritten) != OperationStatus.Done)
				throw new InvalidOperationException($"Invalid proc name {name}");

			utf8Name[bytesWritten] = 0;

			fixed (byte* utf8NamePtr = utf8Name) {
				return device != IntPtr.Zero
					? vkGetDeviceProcAddr(new VkDevice(device), utf8NamePtr)
					: vkGetInstanceProcAddr(new VkInstance(instance), utf8NamePtr);
			}
		}

		var deviceApi = new VkDeviceApi(vkDevice, vkGetDeviceProcAddr);

		var vkContext = new GRVkBackendContext {
			VkInstance = vkInstance.Handle,
			VkPhysicalDevice = vkPhysicalDevice.Handle,
			VkDevice = vkDevice.Handle,
			VkQueue = vkQueue.Handle,
			GraphicsQueueIndex = vkQueueFamilyIndex,
			GetProcedureAddress = GetVkProcAddress
		};

		if (GRContext.CreateVulkan(vkContext) is not { } grContext)
			throw new InvalidOperationException("Couldn't create Vulkan context");

		_grContext = grContext;
		_queueFamilyIndex = vkQueueFamilyIndex;
		_barrierHelper = new VkBarrierHelper(vkDevice, vkQueue, deviceApi, vkQueueFamilyIndex);
	}

	object? IOptionalFeatureProvider.TryGetFeature(Type featureType)
		=> null;

	IDisposable IPlatformGraphicsContext.EnsureCurrent()
		=> EmptyDisposable.Instance;

	ISkiaGpuRenderTarget? ISkiaGpu.TryCreateRenderTarget(IEnumerable<object> surfaces)
		=> surfaces.OfType<GodotSkiaSurface>().FirstOrDefault() is { } surface
			? new GodotSkiaRenderTarget(surface, _grContext, _barrierHelper)
			: null;

	public GodotSkiaSurface CreateSurface(PixelSize size, double renderScaling) {
		size = new PixelSize(Math.Max(size.Width, 1), Math.Max(size.Height, 1));

		var gdRdTextureFormat = new RDTextureFormat {
			Format = RenderingDevice.DataFormat.R8G8B8A8Unorm,
			TextureType = RenderingDevice.TextureType.Type2D,
			Width = (uint)size.Width,
			Height = (uint)size.Height,
			Depth = 1,
			ArrayLayers = 1,
			Mipmaps = 1,
			Samples = RenderingDevice.TextureSamples.Samples1,
			UsageBits = RenderingDevice.TextureUsageBits.SamplingBit
				| RenderingDevice.TextureUsageBits.CanCopyFromBit
				| RenderingDevice.TextureUsageBits.CanCopyToBit
				| RenderingDevice.TextureUsageBits.ColorAttachmentBit
		};

		var gdRdTexture = _renderingDevice.TextureCreate(gdRdTextureFormat, new RDTextureView());

		var vkImage = new VkImage(_renderingDevice.GetDriverResource(RenderingDevice.DriverResource.Texture, gdRdTexture, 0UL));
		if (vkImage.Handle == 0UL)
			throw new InvalidOperationException("Couldn't get Vulkan image from Godot texture");

		var vkFormat = (uint) _renderingDevice.GetDriverResource(RenderingDevice.DriverResource.TextureDataFormat, gdRdTexture, 0UL);
		if (vkFormat == 0U)
			throw new InvalidOperationException("Couldn't get Vulkan format from Godot texture");

		var grVkImageInfo = new GRVkImageInfo {
			CurrentQueueFamily = _queueFamilyIndex,
			Format = vkFormat,
			Image = vkImage.Handle,
			ImageLayout = (uint) VkImageLayout.COLOR_ATTACHMENT_OPTIMAL,
			ImageTiling = (uint) VkImageTiling.OPTIMAL,
			ImageUsageFlags = (uint) (
				VkImageUsageFlags.SAMPLED_BIT |
				VkImageUsageFlags.TRANSFER_SRC_BIT |
				VkImageUsageFlags.TRANSFER_DST_BIT |
				VkImageUsageFlags.COLOR_ATTACHMENT_BIT
			),
			LevelCount = 1,
			SampleCount = 1,
			Protected = false,
			SharingMode = (uint) VkSharingMode.EXCLUSIVE
		};

		var skSurface = SKSurface.Create(
			_grContext,
			new GRBackendRenderTarget(size.Width, size.Height, 1, grVkImageInfo),
			GRSurfaceOrigin.TopLeft,
			SKColorType.Rgba8888,
			new SKSurfaceProperties(SKPixelGeometry.RgbHorizontal)
		);

		if (skSurface is null)
			throw new InvalidOperationException("Couldn't create Skia surface from Vulkan image");

		var gdTexture = new Texture2Drd {
			TextureRdRid = gdRdTexture
		};

		var surface = new GodotSkiaSurface(
			skSurface,
			gdTexture,
			vkImage,
			VkImageLayout.UNDEFINED,
			_renderingDevice,
			renderScaling,
			_barrierHelper);

		surface.TransitionLayoutTo(VkImageLayout.COLOR_ATTACHMENT_OPTIMAL);

		return surface;
	}

	ISkiaSurface? ISkiaGpu.TryCreateSurface(PixelSize size, ISkiaGpuRenderSession? session)
		=> session is GodotSkiaGpuRenderSession godotSession
			? CreateSurface(size, godotSession.Surface.RenderScaling)
			: null;

	public void Dispose() {
		_grContext.Dispose();
		_barrierHelper.Dispose();
	}

}
