using Xunit;

namespace NativeArray.Tests
{
	public class CoTaskMemAllocatorTest
	{
		[Theory]
		[InlineData(0)]
		[InlineData(10)]
		[InlineData(10000)]
		[InlineData(10000000)]
		public void AllocationTest(int size)
		{
			using (NativeArray<byte> nativeArray = new NativeArray<byte>(size, CoTaskMemAllocator.Allocator))
			{
				Assert.Equal(nativeArray.Length, size);
			}
		}
	}
}
