using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static JLeb.Estragonia.VkInterop;

namespace JLeb.Estragonia;

/// <summary>
/// An helper to create Vulkan image barriers.
/// </summary>
internal sealed class VkBarrierHelper : IDisposable {

	private readonly VkDevice _device;
	private readonly VkQueue _queue;
	private readonly VkDeviceApi _deviceApi;
	private readonly uint _queueFamilyIndex;
	private readonly List<ReusableBuffer> _reusableBuffers = new();

	private bool _isDisposed;

	public VkBarrierHelper(VkDevice device, VkQueue queue, VkDeviceApi deviceApi, uint queueFamilyIndex) {
		_device = device;
		_queue = queue;
		_deviceApi = deviceApi;
		_queueFamilyIndex = queueFamilyIndex;
	}

	public unsafe void TransitionImageLayout(
		VkImage image,
		VkImageLayout sourceLayout,
		VkAccessFlags sourceAccessMask,
		VkImageLayout destinationLayout,
		VkAccessFlags destinationAccessMask
	) {
		if (_isDisposed)
			ThrowDisposed();

		var reusableBuffer = GetOrCreateReusableBuffer();

		var fence = reusableBuffer.Fence;
		_deviceApi.ResetFences(_device, 1, &fence);

		var commandBuffer = reusableBuffer.CommandBuffer;

		var beginInfo = new VkCommandBufferBeginInfo {
			sType = VkStructureType.COMMAND_BUFFER_BEGIN_INFO,
			flags = VkCommandBufferUsageFlags.ONE_TIME_SUBMIT_BIT
		};

		_deviceApi.BeginCommandBuffer(commandBuffer, ref beginInfo);

		var barrier = new VkImageMemoryBarrier {
			sType = VkStructureType.IMAGE_MEMORY_BARRIER,
			srcAccessMask = sourceAccessMask,
			dstAccessMask = destinationAccessMask,
			oldLayout = sourceLayout,
			newLayout = destinationLayout,
			srcQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED,
			dstQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED,
			image = image,
			subresourceRange = new VkImageSubresourceRange {
				aspectMask = VkImageAspectFlags.COLOR_BIT,
				baseMipLevel = 0,
				levelCount = 1,
				baseArrayLayer = 0,
				layerCount = 1
			}
		};

		_deviceApi.CmdPipelineBarrier(
			reusableBuffer.CommandBuffer,
			VkPipelineStageFlags.ALL_COMMANDS_BIT,
			VkPipelineStageFlags.ALL_COMMANDS_BIT,
			0,
			0,
			IntPtr.Zero,
			0,
			IntPtr.Zero,
			1,
			&barrier
		);

		_deviceApi.EndCommandBuffer(commandBuffer);

		var submitInfo = new VkSubmitInfo {
			sType = VkStructureType.SUBMIT_INFO,
			waitSemaphoreCount = 0,
			pWaitSemaphores = null,
			pWaitDstStageMask = null,
			commandBufferCount = 1,
			pCommandBuffers = &commandBuffer,
			signalSemaphoreCount = 0,
			pSignalSemaphores = null
		};

		_deviceApi.QueueSubmit(_queue, 1, &submitInfo, fence);
	}

	private ReusableBuffer GetOrCreateReusableBuffer() {
		for (int i = 0; i < _reusableBuffers.Count; ++i) {
			var existingBuffer = _reusableBuffers[i];
			if (existingBuffer.IsAvailable())
				return existingBuffer;
		}

		var newBuffer = new ReusableBuffer(_device, _deviceApi, _queueFamilyIndex);
		_reusableBuffers.Add(newBuffer);
		return newBuffer;
	}

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowDisposed()
		=> throw new ObjectDisposedException(nameof(VkBarrierHelper));

	public void Dispose() {
		if (_isDisposed)
			return;

		_isDisposed = true;

		for (var i = _reusableBuffers.Count - 1; i >= 0; --i)
			_reusableBuffers[i].Dispose();

		_reusableBuffers.Clear();
	}

	/// <summary>
	/// Contains a reusable command pool, command buffer and an associated fence.
	/// </summary>
	private sealed class ReusableBuffer {

		private readonly VkDevice _device;
		private readonly VkDeviceApi _deviceApi;
		private readonly VkCommandPool _commandPool;
		private bool _isDisposed;

		public VkCommandBuffer CommandBuffer { get; }

		public VkFence Fence { get; }

		public bool IsAvailable()
			=> _deviceApi.GetFenceStatus(_device, Fence) == VkResult.VK_SUCCESS;

		public unsafe ReusableBuffer(VkDevice device, VkDeviceApi deviceApi, uint queueFamilyIndex) {
			_device = device;
			_deviceApi = deviceApi;

			var poolCreateInfo = new VkCommandPoolCreateInfo {
				sType = VkStructureType.COMMAND_POOL_CREATE_INFO,
				flags = VkCommandPoolCreateFlags.RESET_COMMAND_BUFFER_BIT,
				queueFamilyIndex = queueFamilyIndex
			};

			deviceApi.CreateCommandPool(device, ref poolCreateInfo, IntPtr.Zero, out _commandPool);

			var bufferAllocateInfo = new VkCommandBufferAllocateInfo {
				sType = VkStructureType.COMMAND_BUFFER_ALLOCATE_INFO,
				commandPool = _commandPool,
				level = VkCommandBufferLevel.PRIMARY,
				commandBufferCount = 1
			};

			VkCommandBuffer commandBuffer;
			deviceApi.AllocateCommandBuffers(_device, ref bufferAllocateInfo, &commandBuffer);
			CommandBuffer = commandBuffer;

			var fenceCreateInfo = new VkFenceCreateInfo
			{
				sType = VkStructureType.FENCE_CREATE_INFO,
				flags = VkFenceCreateFlags.SIGNALED_BIT
			};

			deviceApi.CreateFence(device, ref fenceCreateInfo, IntPtr.Zero, out var fence);
			Fence = fence;
		}

		public unsafe void Dispose() {
			if (_isDisposed)
				return;

			_isDisposed = true;

			var fence = Fence;
			_deviceApi.WaitForFences(_device, 1, &fence, 1, UInt64.MaxValue);
			_deviceApi.DestroyFence(_device, fence, IntPtr.Zero);

			var commandBuffer = CommandBuffer;
			_deviceApi.FreeCommandBuffers(_device, _commandPool, 1, &commandBuffer);

			_deviceApi.DestroyCommandPool(_device, _commandPool, IntPtr.Zero);
		}

	}

}
