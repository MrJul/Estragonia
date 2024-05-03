using System;
using System.Buffers;
using System.Text.Unicode;
using static JLeb.Estragonia.VkInterop;

namespace JLeb.Estragonia;

internal sealed unsafe class VkDeviceApi {

	// Provided by VK_VERSION_1_0
	private readonly delegate* unmanaged[Stdcall]<VkDevice, ref VkCommandPoolCreateInfo, IntPtr, out VkCommandPool, VkResult> _vkCreateCommandPool;
	private readonly delegate* unmanaged[Stdcall]<VkDevice, VkCommandPool, IntPtr, void> _vkDestroyCommandPool;
	private readonly delegate* unmanaged[Stdcall]<VkDevice, ref VkCommandBufferAllocateInfo, VkCommandBuffer*, VkResult> _vkAllocateCommandBuffers;
	private readonly delegate* unmanaged[Stdcall]<VkCommandBuffer, ref VkCommandBufferBeginInfo, VkResult> _vkBeginCommandBuffer;
	private readonly delegate* unmanaged[Stdcall]<VkCommandBuffer, VkResult> _vkEndCommandBuffer;
	private readonly delegate* unmanaged[Stdcall]<VkDevice, VkCommandPool, uint, VkCommandBuffer*, void> _vkFreeCommandBuffers;
	private readonly delegate* unmanaged[Stdcall]<VkCommandBuffer, VkPipelineStageFlags, VkPipelineStageFlags, VkDependencyFlags, uint, IntPtr, uint, IntPtr, uint, VkImageMemoryBarrier*, void> _vkCmdPipelineBarrier;
	private readonly delegate* unmanaged[Stdcall]<VkQueue, uint, VkSubmitInfo*, VkFence, VkResult> _vkQueueSubmit;
	private readonly delegate* unmanaged[Stdcall]<VkDevice, ref VkFenceCreateInfo, IntPtr, out VkFence, VkResult> _vkCreateFence;
	private readonly delegate* unmanaged[Stdcall]<VkDevice, uint, VkFence*, VkResult> _vkResetFences;
	private readonly delegate* unmanaged[Stdcall]<VkDevice, uint, VkFence*, uint, ulong, VkResult> _vkWaitForFences;
	private readonly delegate* unmanaged[Stdcall]<VkDevice, VkFence, VkResult> _vkGetFenceStatus;
	private readonly delegate* unmanaged[Stdcall]<VkDevice, VkFence, IntPtr, void> _vkDestroyFence;

	public VkDeviceApi(VkDevice vkDevice, delegate* unmanaged[Stdcall]<VkDevice, byte*, IntPtr> vkGetDeviceProcAddr) {
		_vkCreateCommandPool =
			(delegate* unmanaged[Stdcall]<VkDevice, ref VkCommandPoolCreateInfo, IntPtr, out VkCommandPool, VkResult>)
			GetVkProcAddress("vkCreateCommandPool");

		_vkDestroyCommandPool =
			(delegate* unmanaged[Stdcall]<VkDevice, VkCommandPool, IntPtr, void>)
			GetVkProcAddress("vkDestroyCommandPool");

		_vkAllocateCommandBuffers =
			(delegate* unmanaged[Stdcall]<VkDevice, ref VkCommandBufferAllocateInfo, VkCommandBuffer*, VkResult>)
			GetVkProcAddress("vkAllocateCommandBuffers");

		_vkBeginCommandBuffer =
			(delegate* unmanaged[Stdcall]<VkCommandBuffer, ref VkCommandBufferBeginInfo, VkResult>)
			GetVkProcAddress("vkBeginCommandBuffer");

		_vkEndCommandBuffer =
			(delegate* unmanaged[Stdcall]<VkCommandBuffer, VkResult>)
			GetVkProcAddress("vkEndCommandBuffer");

		_vkFreeCommandBuffers =
			(delegate* unmanaged[Stdcall]<VkDevice, VkCommandPool, uint, VkCommandBuffer*, void>)
			GetVkProcAddress("vkFreeCommandBuffers");

		_vkCmdPipelineBarrier =
			(delegate* unmanaged[Stdcall]<VkCommandBuffer, VkPipelineStageFlags, VkPipelineStageFlags, VkDependencyFlags, uint, IntPtr, uint, IntPtr, uint, VkImageMemoryBarrier*, void>)
			GetVkProcAddress("vkCmdPipelineBarrier");

		_vkQueueSubmit =
			(delegate* unmanaged[Stdcall]<VkQueue, uint, VkSubmitInfo*, VkFence, VkResult>)
			GetVkProcAddress("vkQueueSubmit");

		_vkCreateFence =
			(delegate* unmanaged[Stdcall]<VkDevice, ref VkFenceCreateInfo, IntPtr, out VkFence, VkResult>)
			GetVkProcAddress("vkCreateFence");

		_vkResetFences =
			(delegate* unmanaged[Stdcall]<VkDevice, uint, VkFence*, VkResult>)
			GetVkProcAddress("vkResetFences");

		_vkWaitForFences =
			(delegate* unmanaged[Stdcall]<VkDevice, uint, VkFence*, uint, ulong, VkResult>)
			GetVkProcAddress("vkWaitForFences");

		_vkGetFenceStatus =
			(delegate* unmanaged[Stdcall]<VkDevice, VkFence, VkResult>)
			GetVkProcAddress("vkGetFenceStatus");

		_vkDestroyFence =
			(delegate* unmanaged[Stdcall]<VkDevice, VkFence, IntPtr, void>)
			GetVkProcAddress("vkDestroyFence");

		IntPtr GetVkProcAddress(string name) {
			Span<byte> utf8Name = stackalloc byte[128];

			if (Utf8.FromUtf16(name, utf8Name[..^1], out _, out var bytesWritten) != OperationStatus.Done)
				throw new InvalidOperationException($"Invalid proc name {name}");

			utf8Name[bytesWritten] = 0;

			IntPtr result;

			fixed (byte* utf8NamePtr = utf8Name)
				result = vkGetDeviceProcAddr(vkDevice, utf8NamePtr);

			if (result == IntPtr.Zero)
				throw new EntryPointNotFoundException($"Vulkan entry point not found for {name}");

			return result;
		}
	}


	public void CreateCommandPool(
		VkDevice device,
		ref VkCommandPoolCreateInfo pCreateInfo,
		IntPtr pAllocator,
		out VkCommandPool pCommandPool
	)
		=> _vkCreateCommandPool(device, ref pCreateInfo, pAllocator, out pCommandPool)
			.VerifySuccess(nameof(CreateCommandPool));

	public void DestroyCommandPool(VkDevice device, VkCommandPool pool, IntPtr pAllocator)
		=> _vkDestroyCommandPool(device, pool, pAllocator);

	public void AllocateCommandBuffers(
		VkDevice device,
		ref VkCommandBufferAllocateInfo pAllocateInfo,
		VkCommandBuffer* pCommandBuffers
	)
		=> _vkAllocateCommandBuffers(device, ref pAllocateInfo, pCommandBuffers)
			.VerifySuccess(nameof(AllocateCommandBuffers));

	public void BeginCommandBuffer(VkCommandBuffer commandBuffer, ref VkCommandBufferBeginInfo pBeginInfo)
		=> _vkBeginCommandBuffer(commandBuffer, ref pBeginInfo)
			.VerifySuccess(nameof(BeginCommandBuffer));

	public void EndCommandBuffer(VkCommandBuffer commandBuffer)
		=> _vkEndCommandBuffer(commandBuffer)
			.VerifySuccess(nameof(EndCommandBuffer));

	public void FreeCommandBuffers(VkDevice device, VkCommandPool commandPool, uint commandBufferCount, VkCommandBuffer* pCommandBuffers)
		=> _vkFreeCommandBuffers(device, commandPool, commandBufferCount, pCommandBuffers);

	public void CmdPipelineBarrier(
		VkCommandBuffer commandBuffer,
		VkPipelineStageFlags srcStageMask,
		VkPipelineStageFlags dstStageMask,
		VkDependencyFlags dependencyFlags,
		uint memoryBarrierCount,
		IntPtr pMemoryBarriers,
		uint bufferMemoryBarrierCount,
		IntPtr pBufferMemoryBarriers,
		uint imageMemoryBarrierCount,
		VkImageMemoryBarrier* pImageMemoryBarriers
	)
		=> _vkCmdPipelineBarrier(
			commandBuffer,
			srcStageMask,
			dstStageMask,
			dependencyFlags,
			memoryBarrierCount,
			pMemoryBarriers,
			bufferMemoryBarrierCount,
			pBufferMemoryBarriers,
			imageMemoryBarrierCount,
			pImageMemoryBarriers);

	public void QueueSubmit(VkQueue queue, uint submitCount, VkSubmitInfo* pSubmits, VkFence fence)
		=> _vkQueueSubmit(queue, submitCount, pSubmits, fence)
			.VerifySuccess(nameof(QueueSubmit));

	public void CreateFence(VkDevice device, ref VkFenceCreateInfo pCreateInfo, IntPtr pAllocator, out VkFence pFence)
		=> _vkCreateFence(device, ref pCreateInfo, pAllocator, out pFence)
			.VerifySuccess(nameof(CreateFence));

	public void ResetFences(VkDevice device, uint fenceCount, VkFence* pFences)
		=> _vkResetFences(device, fenceCount, pFences)
			.VerifySuccess(nameof(ResetFences));

	public void WaitForFences(VkDevice device, uint fenceCount, VkFence* pFences, uint waitAll, ulong timeout)
		=> _vkWaitForFences(device, fenceCount, pFences, waitAll, timeout)
			.VerifySuccess(nameof(WaitForFences));

	public VkResult GetFenceStatus(VkDevice device, VkFence fence)
		=> _vkGetFenceStatus(device, fence);

	public void DestroyFence(VkDevice device, VkFence fence, IntPtr pAllocator)
		=> _vkDestroyFence(device, fence, pAllocator);

}
