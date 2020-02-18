using System;

namespace NativeArray
{
	public interface IMemoryAllocator
	{
		IntPtr Allocate(int size);
		void Free(IntPtr memory);
	}
}
