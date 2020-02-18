using System;
using System.Linq;
using Xunit;

namespace NativeArray.Tests
{
	public class ArrayConstructorTest
	{
		[Theory]
		[InlineData(new byte[] { })]
		[InlineData(new byte[] { 1, 2, 3 })]
		[InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })]
		public void ArrayTest(byte[] array)
		{
			using (NativeArray<byte> nativeArray = new NativeArray<byte>(array))
			{
				Assert.Equal(array.Length, nativeArray.Length);
				Assert.True(nativeArray.SequenceEqual(array));
			}
		}

		[Fact]
		public void NullTest()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				// ReSharper disable once UnusedVariable
				using (NativeArray<byte> nativeArray = new NativeArray<byte>(null))
				{
				}
			});
		}
	}
}
