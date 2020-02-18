using System.Text;
using Xunit;

namespace NativeArray.Tests
{
	public unsafe class StringLengthTest
	{
		[Theory]
		[InlineData("")]
		[InlineData("basic ansi test")]
		[InlineData("CaSe_ĆćĘę-test")]
		[InlineData("0123456789")]
		[InlineData("ąćęłńóśźż")]
		[InlineData("中文")]
		[InlineData("𠜎")]
		[InlineData("�")]
		[InlineData("$¢€𐍈")]
		public void Utf8LengthTest(string text)
		{
			using (NativeArray<byte> nativeArray = NativeArray<byte>.FromString(text, Encoding.UTF8))
			{
				Assert.Equal(text, Encoding.UTF8.GetString(nativeArray.Pointer, nativeArray.Length));
			}
		}

		[Theory]
		[InlineData("")]
		[InlineData("basic ansi test")]
		[InlineData("CaSe_ĆćĘę-test")]
		[InlineData("0123456789")]
		[InlineData("ąćęłńóśźż")]
		[InlineData("中文")]
		[InlineData("𠜎")]
		[InlineData("�")]
		[InlineData("$¢€𐍈")]
		public void Utf16LengthTest(string text)
		{
			using (NativeArray<byte> nativeArray = NativeArray<byte>.FromString(text, Encoding.Unicode))
			{
				Assert.Equal(text, Encoding.Unicode.GetString(nativeArray.Pointer, nativeArray.Length));
			}
		}

		[Theory]
		[InlineData("")]
		[InlineData("basic ansi test")]
		[InlineData("CaSe_ĆćĘę-test")]
		[InlineData("0123456789")]
		[InlineData("ąćęłńóśźż")]
		[InlineData("中文")]
		[InlineData("𠜎")]
		[InlineData("�")]
		[InlineData("$¢€𐍈")]
		public void Utf32LengthTest(string text)
		{
			using (NativeArray<byte> nativeArray = NativeArray<byte>.FromString(text, Encoding.UTF32))
			{
				Assert.Equal(text, Encoding.UTF32.GetString(nativeArray.Pointer, nativeArray.Length));
			}
		}
	}
}
