using System;
using System.Runtime.InteropServices;

namespace NativeArray
{
	public sealed class CoTaskMemAllocator : IMemoryAllocator
	{
		public static readonly CoTaskMemAllocator Allocator = new CoTaskMemAllocator();

		public IntPtr Allocate(int size)
		{
			return Marshal.AllocCoTaskMem(size);
		}

		public void Free(IntPtr memory)
		{
			Marshal.FreeCoTaskMem(memory);
		}
	}
}
