using System.Linq;
using Xunit;

namespace NativeArray.Tests
{
	public class EnumeratorTest
	{
		[Theory]
		[InlineData(0)]
		[InlineData(100)]
		public void CountTest(int count)
		{
			using (NativeArray<byte> nativeArray = new NativeArray<byte>(count))
			{
				// ReSharper disable once UseCollectionCountProperty
				Assert.Equal(count, nativeArray.Count());
			}
		}

		[Theory]
		[InlineData(new byte[] { })]
		[InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9})]
		public void ArrayTest(byte[] array)
		{
			using (NativeArray<byte> nativeArray = new NativeArray<byte>(array))
			{
				int i = 0;
				foreach (byte b in nativeArray)
				{
					Assert.Equal(array[i++], b);
				}
			}
		}

		[Fact]
		public void AnyEmptyTest()
		{
			using (NativeArray<byte> nativeArray = new NativeArray<byte>(0))
			{
				Assert.False(nativeArray.Any());
			}
		}

		[Fact]
		public void AnyNotEmptyTest()
		{
			using (NativeArray<byte> nativeArray = new NativeArray<byte>(100))
			{
				Assert.True(nativeArray.Any());
			}
		}
	}
}
