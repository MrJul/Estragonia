using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static JLeb.Estragonia.VkInterop;

namespace JLeb.Estragonia;

internal static class VkExtensions {

	public static void VerifySuccess(this VkResult result, string functionName) {
		if (result != VkResult.VK_SUCCESS)
			ThrowError(result, functionName);

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void ThrowError(VkResult vkResult, string functionName)
			=> throw new InvalidOperationException($"{functionName} returned Vulkan error {vkResult}");
	}

}
