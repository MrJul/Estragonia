using System;
using System.Diagnostics.CodeAnalysis;

namespace JLeb.Estragonia;

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

/// <summary>Contains some used Vulkan constants.</summary>
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Matches the official Vulkan names")]
[SuppressMessage("ReSharper", "NotAccessedField.Global", Justification = "Used in interop")]
internal static class VkInterop {

	internal const uint VK_QUEUE_FAMILY_IGNORED = ~0U;

	internal enum VkImageLayout {
		// Provided by VK_VERSION_1_0
		UNDEFINED = 0,
		GENERAL = 1,
		COLOR_ATTACHMENT_OPTIMAL = 2,
		DEPTH_STENCIL_ATTACHMENT_OPTIMAL = 3,
		DEPTH_STENCIL_READ_ONLY_OPTIMAL = 4,
		SHADER_READ_ONLY_OPTIMAL = 5,
		TRANSFER_SRC_OPTIMAL = 6,
		TRANSFER_DST_OPTIMAL = 7,
		PREINITIALIZED = 8,

		// Provided by VK_VERSION_1_1
		DEPTH_READ_ONLY_STENCIL_ATTACHMENT_OPTIMAL = 1000117000,
		DEPTH_ATTACHMENT_STENCIL_READ_ONLY_OPTIMAL = 1000117001,

		// Provided by VK_VERSION_1_2
		DEPTH_ATTACHMENT_OPTIMAL = 1000241000,
		DEPTH_READ_ONLY_OPTIMAL = 1000241001,
		STENCIL_ATTACHMENT_OPTIMAL = 1000241002,
		STENCIL_READ_ONLY_OPTIMAL = 1000241003,

		// Provided by VK_VERSION_1_3
		READ_ONLY_OPTIMAL = 1000314000,
		ATTACHMENT_OPTIMAL = 1000314001,

		// Provided by VK_KHR_swapchain
		PRESENT_SRC_KHR = 1000001002,

		// Provided by VK_KHR_video_decode_queue
		VIDEO_DECODE_DST_KHR = 1000024000,
		VIDEO_DECODE_SRC_KHR = 1000024001,
		VIDEO_DECODE_DPB_KHR = 1000024002,

		// Provided by VK_KHR_shared_presentable_image
		SHARED_PRESENT_KHR = 1000111000,

		// Provided by VK_EXT_fragment_density_map
		FRAGMENT_DENSITY_MAP_OPTIMAL_EXT = 1000218000,

		// Provided by VK_KHR_fragment_shading_rate
		FRAGMENT_SHADING_RATE_ATTACHMENT_OPTIMAL_KHR = 1000164003,

		// Provided by VK_KHR_video_encode_queue
		VIDEO_ENCODE_DST_KHR = 1000299000,
		VIDEO_ENCODE_SRC_KHR = 1000299001,
		VIDEO_ENCODE_DPB_KHR = 1000299002,

		// Provided by VK_EXT_attachment_feedback_loop_layout
		ATTACHMENT_FEEDBACK_LOOP_OPTIMAL_EXT = 1000339000,

		// Provided by VK_KHR_maintenance2
		DEPTH_READ_ONLY_STENCIL_ATTACHMENT_OPTIMAL_KHR = DEPTH_READ_ONLY_STENCIL_ATTACHMENT_OPTIMAL,
		DEPTH_ATTACHMENT_STENCIL_READ_ONLY_OPTIMAL_KHR = DEPTH_ATTACHMENT_STENCIL_READ_ONLY_OPTIMAL,

		// Provided by VK_NV_shading_rate_image
		SHADING_RATE_OPTIMAL_NV = FRAGMENT_SHADING_RATE_ATTACHMENT_OPTIMAL_KHR,

		// Provided by VK_KHR_separate_depth_stencil_layouts
		DEPTH_ATTACHMENT_OPTIMAL_KHR = DEPTH_ATTACHMENT_OPTIMAL,
		DEPTH_READ_ONLY_OPTIMAL_KHR = DEPTH_READ_ONLY_OPTIMAL,
		STENCIL_ATTACHMENT_OPTIMAL_KHR = STENCIL_ATTACHMENT_OPTIMAL,
		STENCIL_READ_ONLY_OPTIMAL_KHR = STENCIL_READ_ONLY_OPTIMAL,

		// Provided by VK_KHR_synchronization2
		READ_ONLY_OPTIMAL_KHR = READ_ONLY_OPTIMAL,

		// Provided by VK_KHR_synchronization2
		ATTACHMENT_OPTIMAL_KHR = ATTACHMENT_OPTIMAL
	}

	internal enum VkImageTiling {
		// Provided by VK_VERSION_1_0
		OPTIMAL = 0,
		LINEAR = 1,

		// Provided by VK_EXT_image_drm_format_modifier
		DRM_FORMAT_MODIFIER_EXT = 1000158000
	}

	[Flags]
	internal enum VkImageUsageFlags {
		// Provided by VK_VERSION_1_0
		TRANSFER_SRC_BIT = 0x00000001,
		TRANSFER_DST_BIT = 0x00000002,
		SAMPLED_BIT = 0x00000004,
		STORAGE_BIT = 0x00000008,
		COLOR_ATTACHMENT_BIT = 0x00000010,
		DEPTH_STENCIL_ATTACHMENT_BIT = 0x00000020,
		TRANSIENT_ATTACHMENT_BIT = 0x00000040,
		INPUT_ATTACHMENT_BIT = 0x00000080,

		// Provided by VK_KHR_video_decode_queue
		VIDEO_DECODE_DST_BIT_KHR = 0x00000400,
		VIDEO_DECODE_SRC_BIT_KHR = 0x00000800,
		VIDEO_DECODE_DPB_BIT_KHR = 0x00001000,

		// Provided by VK_EXT_fragment_density_map
		FRAGMENT_DENSITY_MAP_BIT_EXT = 0x00000200,

		// Provided by VK_KHR_fragment_shading_rate
		FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR = 0x00000100,

		// Provided by VK_KHR_video_encode_queue
		VIDEO_ENCODE_DST_BIT_KHR = 0x00002000,
		VIDEO_ENCODE_SRC_BIT_KHR = 0x00004000,
		VIDEO_ENCODE_DPB_BIT_KHR = 0x00008000,

		// Provided by VK_EXT_attachment_feedback_loop_layout
		ATTACHMENT_FEEDBACK_LOOP_BIT_EXT = 0x00080000,

		// Provided by VK_HUAWEI_invocation_mask
		INVOCATION_MASK_BIT_HUAWEI = 0x00040000,

		// Provided by VK_QCOM_image_processing
		SAMPLE_WEIGHT_BIT_QCOM = 0x00100000,
		SAMPLE_BLOCK_MATCH_BIT_QCOM = 0x00200000,

		// Provided by VK_NV_shading_rate_image
		SHADING_RATE_IMAGE_BIT_NV = FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR
	}

	internal enum VkSharingMode {
		// Provided by VK_VERSION_1_0
		EXCLUSIVE = 0,
		CONCURRENT = 1
	}

	// Not exhaustive
	internal enum VkStructureType {
		// Provided by VK_VERSION_1_0
		SUBMIT_INFO = 4,
		FENCE_CREATE_INFO = 8,
		COMMAND_POOL_CREATE_INFO = 39,
		COMMAND_BUFFER_ALLOCATE_INFO = 40,
		COMMAND_BUFFER_BEGIN_INFO = 42,
		IMAGE_MEMORY_BARRIER = 45
	}

	// Provided by VK_VERSION_1_0
	[Flags]
	internal enum VkAccessFlags {
		// Provided by VK_VERSION_1_0
		INDIRECT_COMMAND_READ_BIT = 0x00000001,
		INDEX_READ_BIT = 0x00000002,
		VERTEX_ATTRIBUTE_READ_BIT = 0x00000004,
		UNIFORM_READ_BIT = 0x00000008,
		INPUT_ATTACHMENT_READ_BIT = 0x00000010,
		SHADER_READ_BIT = 0x00000020,
		SHADER_WRITE_BIT = 0x00000040,
		COLOR_ATTACHMENT_READ_BIT = 0x00000080,
		COLOR_ATTACHMENT_WRITE_BIT = 0x00000100,
		DEPTH_STENCIL_ATTACHMENT_READ_BIT = 0x00000200,
		DEPTH_STENCIL_ATTACHMENT_WRITE_BIT = 0x00000400,
		TRANSFER_READ_BIT = 0x00000800,
		TRANSFER_WRITE_BIT = 0x00001000,
		HOST_READ_BIT = 0x00002000,
		HOST_WRITE_BIT = 0x00004000,
		MEMORY_READ_BIT = 0x00008000,
		MEMORY_WRITE_BIT = 0x00010000,

		// Provided by VK_VERSION_1_3
		NONE = 0,

		// Provided by VK_EXT_transform_feedback
		TRANSFORM_FEEDBACK_WRITE_BIT_EXT = 0x02000000,
		TRANSFORM_FEEDBACK_COUNTER_READ_BIT_EXT = 0x04000000,
		TRANSFORM_FEEDBACK_COUNTER_WRITE_BIT_EXT = 0x08000000,

		// Provided by VK_EXT_conditional_rendering
		CONDITIONAL_RENDERING_READ_BIT_EXT = 0x00100000,

		// Provided by VK_EXT_blend_operation_advanced
		COLOR_ATTACHMENT_READ_NONCOHERENT_BIT_EXT = 0x00080000,

		// Provided by VK_KHR_acceleration_structure
		ACCELERATION_STRUCTURE_READ_BIT_KHR = 0x00200000,
		ACCELERATION_STRUCTURE_WRITE_BIT_KHR = 0x00400000,

		// Provided by VK_EXT_fragment_density_map
		FRAGMENT_DENSITY_MAP_READ_BIT_EXT = 0x01000000,

		// Provided by VK_KHR_fragment_shading_rate
		FRAGMENT_SHADING_RATE_ATTACHMENT_READ_BIT_KHR = 0x00800000,

		// Provided by VK_NV_device_generated_commands
		COMMAND_PREPROCESS_READ_BIT_NV = 0x00020000,
		COMMAND_PREPROCESS_WRITE_BIT_NV = 0x00040000,

		// Provided by VK_NV_shading_rate_image
		SHADING_RATE_IMAGE_READ_BIT_NV = FRAGMENT_SHADING_RATE_ATTACHMENT_READ_BIT_KHR,

		// Provided by VK_NV_ray_tracing
		ACCELERATION_STRUCTURE_READ_BIT_NV = ACCELERATION_STRUCTURE_READ_BIT_KHR,
		ACCELERATION_STRUCTURE_WRITE_BIT_NV = ACCELERATION_STRUCTURE_WRITE_BIT_KHR,

		// Provided by VK_KHR_synchronization2
		NONE_KHR = NONE
	}

	// Provided by VK_VERSION_1_0
	[Flags]
	internal enum VkImageAspectFlags {
		// Provided by VK_VERSION_1_0
		COLOR_BIT = 0x00000001,
		DEPTH_BIT = 0x00000002,
		STENCIL_BIT = 0x00000004,
		METADATA_BIT = 0x00000008,

		// Provided by VK_VERSION_1_1
		PLANE_0_BIT = 0x00000010,
		PLANE_1_BIT = 0x00000020,
		PLANE_2_BIT = 0x00000040,

		// Provided by VK_VERSION_1_3
		NONE = 0,

		// Provided by VK_EXT_image_drm_format_modifier
		MEMORY_PLANE_0_BIT_EXT = 0x00000080,
		MEMORY_PLANE_1_BIT_EXT = 0x00000100,
		MEMORY_PLANE_2_BIT_EXT = 0x00000200,
		MEMORY_PLANE_3_BIT_EXT = 0x00000400,

		// Provided by VK_KHR_sampler_ycbcr_conversion
		PLANE_0_BIT_KHR = PLANE_0_BIT,
		PLANE_1_BIT_KHR = PLANE_1_BIT,
		PLANE_2_BIT_KHR = PLANE_2_BIT,

		// Provided by VK_KHR_maintenance4
		NONE_KHR = NONE
	}

	[Flags]
	internal enum VkPipelineStageFlags {
		// Provided by VK_VERSION_1_0
	    TOP_OF_PIPE_BIT = 0x00000001,
	    DRAW_INDIRECT_BIT = 0x00000002,
	    VERTEX_INPUT_BIT = 0x00000004,
	    VERTEX_SHADER_BIT = 0x00000008,
	    TESSELLATION_CONTROL_SHADER_BIT = 0x00000010,
	    TESSELLATION_EVALUATION_SHADER_BIT = 0x00000020,
	    GEOMETRY_SHADER_BIT = 0x00000040,
	    FRAGMENT_SHADER_BIT = 0x00000080,
	    EARLY_FRAGMENT_TESTS_BIT = 0x00000100,
	    LATE_FRAGMENT_TESTS_BIT = 0x00000200,
	    COLOR_ATTACHMENT_OUTPUT_BIT = 0x00000400,
	    COMPUTE_SHADER_BIT = 0x00000800,
	    TRANSFER_BIT = 0x00001000,
	    BOTTOM_OF_PIPE_BIT = 0x00002000,
	    HOST_BIT = 0x00004000,
	    ALL_GRAPHICS_BIT = 0x00008000,
	    ALL_COMMANDS_BIT = 0x00010000,

		// Provided by VK_VERSION_1_3
		NONE = 0,

		// Provided by VK_EXT_transform_feedback
		TRANSFORM_FEEDBACK_BIT_EXT = 0x01000000,

		// Provided by VK_EXT_conditional_rendering
		CONDITIONAL_RENDERING_BIT_EXT = 0x00040000,

		// Provided by VK_KHR_acceleration_structure
		ACCELERATION_STRUCTURE_BUILD_BIT_KHR = 0x02000000,

		// Provided by VK_KHR_ray_tracing_pipeline
		RAY_TRACING_SHADER_BIT_KHR = 0x00200000,

		// Provided by VK_EXT_fragment_density_map
		FRAGMENT_DENSITY_PROCESS_BIT_EXT = 0x00800000,

		// Provided by VK_KHR_fragment_shading_rate
		FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR = 0x00400000,

		// Provided by VK_NV_device_generated_commands
		COMMAND_PREPROCESS_BIT_NV = 0x00020000,

		// Provided by VK_EXT_mesh_shader
		TASK_SHADER_BIT_EXT = 0x00080000,
		MESH_SHADER_BIT_EXT = 0x00100000,

		// Provided by VK_NV_shading_rate_image
		SHADING_RATE_IMAGE_BIT_NV = FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR,

		// Provided by VK_NV_ray_tracing
		RAY_TRACING_SHADER_BIT_NV = RAY_TRACING_SHADER_BIT_KHR,
		ACCELERATION_STRUCTURE_BUILD_BIT_NV = ACCELERATION_STRUCTURE_BUILD_BIT_KHR,

		// Provided by VK_NV_mesh_shader
		TASK_SHADER_BIT_NV = TASK_SHADER_BIT_EXT,
		MESH_SHADER_BIT_NV = MESH_SHADER_BIT_EXT,

		// Provided by VK_KHR_synchronization2
		NONE_KHR = NONE
	}

	[Flags]
	internal enum VkDependencyFlags {
		// Provided by VK_VERSION_1_0
		BY_REGION_BIT = 0x00000001,

		// Provided by VK_VERSION_1_1
		DEVICE_GROUP_BIT = 0x00000004,
		VIEW_LOCAL_BIT = 0x00000002,

		// Provided by VK_EXT_attachment_feedback_loop_layout
		FEEDBACK_LOOP_BIT_EXT = 0x00000008,

		// Provided by VK_KHR_multiview
		VIEW_LOCAL_BIT_KHR = VIEW_LOCAL_BIT,

		// Provided by VK_KHR_device_group
		DEVICE_GROUP_BIT_KHR = DEVICE_GROUP_BIT
	}

	internal enum VkCommandBufferLevel {
		// Provided by VK_VERSION_1_0
		PRIMARY = 0,
		SECONDARY = 1
	}

	[Flags]
	internal enum VkCommandPoolCreateFlags {
		// Provided by VK_VERSION_1_0
		TRANSIENT_BIT = 0x00000001,
		RESET_COMMAND_BUFFER_BIT = 0x00000002,

		// Provided by VK_VERSION_1_1
		PROTECTED_BIT = 0x00000004
	}

	[Flags]
	internal enum VkCommandBufferUsageFlags {
		// Provided by VK_VERSION_1_0
		ONE_TIME_SUBMIT_BIT = 0x00000001,
		RENDER_PASS_CONTINUE_BIT = 0x00000002,
		SIMULTANEOUS_USE_BIT = 0x00000004
	}

	[Flags]
	internal enum VkFenceCreateFlags {
		// Provided by VK_VERSION_1_0
		SIGNALED_BIT = 0x00000001
	}

	internal enum VkResult {
		// Provided by VK_VERSION_1_0
		VK_SUCCESS = 0,
		VK_NOT_READY = 1,
		VK_TIMEOUT = 2,
		VK_EVENT_SET = 3,
		VK_EVENT_RESET = 4,
		VK_INCOMPLETE = 5,
		VK_ERROR_OUT_OF_HOST_MEMORY = -1,
		VK_ERROR_OUT_OF_DEVICE_MEMORY = -2,
		VK_ERROR_INITIALIZATION_FAILED = -3,
		VK_ERROR_DEVICE_LOST = -4,
		VK_ERROR_MEMORY_MAP_FAILED = -5,
		VK_ERROR_LAYER_NOT_PRESENT = -6,
		VK_ERROR_EXTENSION_NOT_PRESENT = -7,
		VK_ERROR_FEATURE_NOT_PRESENT = -8,
		VK_ERROR_INCOMPATIBLE_DRIVER = -9,
		VK_ERROR_TOO_MANY_OBJECTS = -10,
		VK_ERROR_FORMAT_NOT_SUPPORTED = -11,
		VK_ERROR_FRAGMENTED_POOL = -12,
		VK_ERROR_UNKNOWN = -13,

		// Provided by VK_VERSION_1_1
		VK_ERROR_OUT_OF_POOL_MEMORY = -1000069000,
		VK_ERROR_INVALID_EXTERNAL_HANDLE = -1000072003,

		// Provided by VK_VERSION_1_2
		VK_ERROR_FRAGMENTATION = -1000161000,
		VK_ERROR_INVALID_OPAQUE_CAPTURE_ADDRESS = -1000257000,

		// Provided by VK_VERSION_1_3
		VK_PIPELINE_COMPILE_REQUIRED = 1000297000,

		// Provided by VK_KHR_surface
		VK_ERROR_SURFACE_LOST_KHR = -1000000000,
		VK_ERROR_NATIVE_WINDOW_IN_USE_KHR = -1000000001,

		// Provided by VK_KHR_swapchain
		VK_SUBOPTIMAL_KHR = 1000001003,
		VK_ERROR_OUT_OF_DATE_KHR = -1000001004,

		// Provided by VK_KHR_display_swapchain
		VK_ERROR_INCOMPATIBLE_DISPLAY_KHR = -1000003001,

		// Provided by VK_EXT_debug_report
		VK_ERROR_VALIDATION_FAILED_EXT = -1000011001,

		// Provided by VK_NV_glsl_shader
		VK_ERROR_INVALID_SHADER_NV = -1000012000,

		// Provided by VK_KHR_video_queue
		VK_ERROR_IMAGE_USAGE_NOT_SUPPORTED_KHR = -1000023000,
		VK_ERROR_VIDEO_PICTURE_LAYOUT_NOT_SUPPORTED_KHR = -1000023001,
		VK_ERROR_VIDEO_PROFILE_OPERATION_NOT_SUPPORTED_KHR = -1000023002,
		VK_ERROR_VIDEO_PROFILE_FORMAT_NOT_SUPPORTED_KHR = -1000023003,
		VK_ERROR_VIDEO_PROFILE_CODEC_NOT_SUPPORTED_KHR = -1000023004,
		VK_ERROR_VIDEO_STD_VERSION_NOT_SUPPORTED_KHR = -1000023005,

		// Provided by VK_EXT_image_drm_format_modifier
		VK_ERROR_INVALID_DRM_FORMAT_MODIFIER_PLANE_LAYOUT_EXT = -1000158000,

		// Provided by VK_KHR_global_priority
		VK_ERROR_NOT_PERMITTED_KHR = -1000174001,

		// Provided by VK_EXT_full_screen_exclusive
		VK_ERROR_FULL_SCREEN_EXCLUSIVE_MODE_LOST_EXT = -1000255000,

		// Provided by VK_KHR_deferred_host_operations
		VK_THREAD_IDLE_KHR = 1000268000,
		VK_THREAD_DONE_KHR = 1000268001,
		VK_OPERATION_DEFERRED_KHR = 1000268002,
		VK_OPERATION_NOT_DEFERRED_KHR = 1000268003,

		// Provided by VK_KHR_video_encode_queue
		VK_ERROR_INVALID_VIDEO_STD_PARAMETERS_KHR = -1000299000,

		// Provided by VK_EXT_image_compression_control
		VK_ERROR_COMPRESSION_EXHAUSTED_EXT = -1000338000,

		// Provided by VK_EXT_shader_object
		VK_INCOMPATIBLE_SHADER_BINARY_EXT = 1000482000,

		// Provided by VK_KHR_maintenance1
		VK_ERROR_OUT_OF_POOL_MEMORY_KHR = VK_ERROR_OUT_OF_POOL_MEMORY,

		// Provided by VK_KHR_external_memory
		VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR = VK_ERROR_INVALID_EXTERNAL_HANDLE,

		// Provided by VK_EXT_descriptor_indexing
		VK_ERROR_FRAGMENTATION_EXT = VK_ERROR_FRAGMENTATION,

		// Provided by VK_EXT_global_priority
		VK_ERROR_NOT_PERMITTED_EXT = VK_ERROR_NOT_PERMITTED_KHR,

		// Provided by VK_EXT_buffer_device_address
		VK_ERROR_INVALID_DEVICE_ADDRESS_EXT = VK_ERROR_INVALID_OPAQUE_CAPTURE_ADDRESS,

		// Provided by VK_KHR_buffer_device_address
		VK_ERROR_INVALID_OPAQUE_CAPTURE_ADDRESS_KHR = VK_ERROR_INVALID_OPAQUE_CAPTURE_ADDRESS,

		// Provided by VK_EXT_pipeline_creation_cache_control
		VK_PIPELINE_COMPILE_REQUIRED_EXT = VK_PIPELINE_COMPILE_REQUIRED,
		VK_ERROR_PIPELINE_COMPILE_REQUIRED_EXT = VK_PIPELINE_COMPILE_REQUIRED,

		// Provided by VK_EXT_shader_object
		VK_ERROR_INCOMPATIBLE_SHADER_BINARY_EXT = VK_INCOMPATIBLE_SHADER_BINARY_EXT
	}

	// Provided by VK_VERSION_1_0
	internal struct VkInstance {

		public IntPtr Handle;

		public VkInstance(IntPtr handle)
			=> Handle = handle;

	}

	// Provided by VK_VERSION_1_0
	internal struct VkPhysicalDevice {

		public IntPtr Handle;

		public VkPhysicalDevice(IntPtr handle)
			=> Handle = handle;

	}

	// Provided by VK_VERSION_1_0
	internal struct VkDevice {

		public IntPtr Handle;

		public VkDevice(IntPtr handle)
			=> Handle = handle;

	}

	// Provided by VK_VERSION_1_0
	internal struct VkQueue {

		public IntPtr Handle;

		public VkQueue(IntPtr handle)
			=> Handle = handle;

	}

	// Provided by VK_VERSION_1_0
	internal struct VkCommandBuffer {

		public IntPtr Handle;

		public VkCommandBuffer(IntPtr handle)
			=> Handle = handle;

	}

	// Provided by VK_VERSION_1_0
	internal struct VkSemaphore {

		public ulong Handle;

		public VkSemaphore(ulong handle)
			=> Handle = handle;

	}

	// Provided by VK_VERSION_1_0
	internal struct VkFence {

		public ulong Handle;

		public VkFence(ulong handle)
			=> Handle = handle;

	}

	// Provided by VK_VERSION_1_0
	internal struct VkCommandPool {

		public ulong Handle;

		public VkCommandPool(ulong handle)
			=> Handle = handle;

	}

	// Provided by VK_VERSION_1_0
	internal struct VkImage {

		public ulong Handle;

		public VkImage(ulong handle)
			=> Handle = handle;

	}

	// Provided by VK_VERSION_1_0
	internal struct VkCommandPoolCreateInfo {
		public VkStructureType sType;
		public IntPtr pNext;
		public VkCommandPoolCreateFlags flags;
		public uint queueFamilyIndex;
	}

	// Provided by VK_VERSION_1_0
	internal struct VkImageMemoryBarrier {
		public VkStructureType sType;
		public IntPtr pNext;
		public VkAccessFlags srcAccessMask;
		public VkAccessFlags dstAccessMask;
		public VkImageLayout oldLayout;
		public VkImageLayout newLayout;
		public uint srcQueueFamilyIndex;
		public uint dstQueueFamilyIndex;
		public VkImage image;
		public VkImageSubresourceRange subresourceRange;
	}

	// Provided by VK_VERSION_1_0
	internal struct VkImageSubresourceRange {
		public VkImageAspectFlags aspectMask;
		public uint baseMipLevel;
		public uint levelCount;
		public uint baseArrayLayer;
		public uint layerCount;
	}

	// Provided by VK_VERSION_1_0
	internal unsafe struct VkSubmitInfo {
		public VkStructureType sType;
		public IntPtr pNext;
		public uint waitSemaphoreCount;
		public VkSemaphore* pWaitSemaphores;
		public VkPipelineStageFlags* pWaitDstStageMask;
		public uint commandBufferCount;
		public VkCommandBuffer* pCommandBuffers;
		public uint signalSemaphoreCount;
		public VkSemaphore* pSignalSemaphores;
	}

	// Provided by VK_VERSION_1_0
	internal struct VkCommandBufferAllocateInfo {
		public VkStructureType sType;
		public IntPtr pNext;
		public VkCommandPool commandPool;
		public VkCommandBufferLevel level;
		public uint commandBufferCount;
	}

	// Provided by VK_VERSION_1_0
	internal struct VkCommandBufferBeginInfo
	{
		public VkStructureType sType;
		public IntPtr pNext;
		public VkCommandBufferUsageFlags flags;
		public IntPtr pInheritanceInfo;
	}

	// Provided by VK_VERSION_1_0
	internal struct VkFenceCreateInfo
	{
		public VkStructureType sType;
		public IntPtr pNext;
		public VkFenceCreateFlags flags;
	}

}
