using System;
using Xunit;

namespace NativeArray.Tests
{
	public class ImplictTest
	{
		[Fact]
		public unsafe void NullTest()
		{
			// ReSharper disable ExpressionIsAlwaysNull
			NativeArray<byte> nativeArray = null;
			IntPtr intPtr = nativeArray;
			Assert.Equal(IntPtr.Zero, intPtr);
			byte* bytePtr = nativeArray;
			Assert.True(bytePtr == null);
#if NETCOREAPP3_1
			Span<byte> span = nativeArray;
			Assert.True(Span<byte>.Empty == span);
#endif
			// ReSharper restore ExpressionIsAlwaysNull
		}

		[Fact]
		public unsafe void ValidTest()
		{
			using (NativeArray<byte> nativeArray = new NativeArray<byte>(1000))
			{
				IntPtr intPtr = nativeArray;
				Assert.Equal(new IntPtr(nativeArray.Pointer), intPtr);
				byte* bytePtr = nativeArray;
				Assert.True(nativeArray.Pointer == bytePtr);
#if NETCOREAPP3_1
				Span<byte> span = nativeArray;
				Assert.True(new Span<byte>(nativeArray.Pointer, nativeArray.Length) == span);
#endif
			}
		}
	}
}
