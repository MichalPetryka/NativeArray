using System;
using System.Collections.Generic;
using Xunit;

namespace NativeArray.Tests
{
	public class PrimitiveTest
	{
		[Theory]
		// Fixed point
		[InlineData(byte.MinValue, byte.MaxValue)]
		[InlineData(sbyte.MinValue, sbyte.MaxValue)]
		[InlineData(short.MinValue, short.MaxValue)]
		[InlineData(ushort.MinValue, ushort.MaxValue)]
		[InlineData(int.MinValue, int.MaxValue)]
		[InlineData(uint.MinValue, uint.MaxValue)]
		[InlineData(long.MinValue, long.MaxValue)]
		[InlineData(ulong.MinValue, ulong.MaxValue)]
		// Floating point
		[InlineData(float.MinValue, float.MaxValue)]
		[InlineData(double.MinValue, double.MaxValue)]
		// Others
		[InlineData(true, false)]
		[InlineData(' ', 'a')]
		public void GenericTest<T>(T first, T second) where T : unmanaged
		{
			Assert.False(EqualityComparer<T>.Default.Equals(first, second));
			using (NativeArray<T> nativeArray = new NativeArray<T>(11))
			{
				int next = 0;
				for (int i = 0; i < nativeArray.Length; i++)
				{
					if (next == 0)
					{
						nativeArray[i] = first;
						next = 1;
					}
					else
					{
						nativeArray[i] = second;
						next = 0;
					}
				}

				int next2 = 0;
				for (int i = 0; i < nativeArray.Length; i++)
				{
					if (next2 == 0)
					{
						Assert.Equal(first, nativeArray[i]);
						next2 = 1;
					}
					else
					{
						Assert.Equal(second, nativeArray[i]);
						next2 = 0;
					}
				}
			}
		}

		[Fact]
		public void IntPtrTest()
		{
			IntPtr first = IntPtr.Zero;
			IntPtr second = new IntPtr(int.MaxValue);
			Assert.NotEqual(first,  second);
			using (NativeArray<IntPtr> nativeArray = new NativeArray<IntPtr>(11))
			{
				int next = 0;
				for (int i = 0; i < nativeArray.Length; i++)
				{
					if (next == 0)
					{
						nativeArray[i] = first;
						next = 1;
					}
					else
					{
						nativeArray[i] = second;
						next = 0;
					}
				}

				int next2 = 0;
				for (int i = 0; i < nativeArray.Length; i++)
				{
					if (next2 == 0)
					{
						Assert.Equal(first, nativeArray[i]);
						next2 = 1;
					}
					else
					{
						Assert.Equal(second, nativeArray[i]);
						next2 = 0;
					}
				}
			}
		}
	}
}
