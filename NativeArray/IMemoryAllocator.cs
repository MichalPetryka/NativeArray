using System;

namespace NativeArray
{
	public interface IMemoryAllocator
	{
		IntPtr Allocate(int size);
		IntPtr Reallocate(IntPtr memory, int newSize);
		void Free(IntPtr memory);
	}
}
