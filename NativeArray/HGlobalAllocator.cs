using System;
using System.Runtime.InteropServices;

namespace NativeArray
{
	public sealed class HGlobalAllocator : IMemoryAllocator
	{
		public static readonly HGlobalAllocator Allocator = new HGlobalAllocator();

		public IntPtr Allocate(int size)
		{
			return Marshal.AllocHGlobal(size);
		}

		public IntPtr Reallocate(IntPtr memory, int newSize)
		{
			Free(memory);
			return Allocate(newSize);
		}

		public void Free(IntPtr memory)
		{
			Marshal.FreeHGlobal(memory);
		}
	}
}
