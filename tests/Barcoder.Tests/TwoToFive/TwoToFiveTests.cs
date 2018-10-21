using System;
using FluentAssertions;
using Xunit;

namespace Barcoder.Tests
{
    public sealed class TwoToFiveTests
    {
        [Theory]
        [InlineData(false, false, "12345670", "1101101011101010101110101110101011101110111010101010101110101110111010111010101011101110101010101011101110101011101110101101011")]
        [InlineData(true, false, "12345670", "10101110100010101110001110111010001010001110100011100010101010100011100011101101")]
        [InlineData(false, true, "1234567", "1101101011101010101110101110101011101110111010101010101110101110111010111010101011101110101010101011101110101011101110101101011")]
        public void Encode(bool interleaved, bool includeChecksum, string txt, string testResult)
        {
            IBarcodeIntCS code = TwoToFive.Encode(txt, interleaved, includeChecksum);

            code.Bounds.X.Should().Be(testResult.Length);
            code.Bounds.Y.Should().Be(1);

            string encoded = string.Empty;
            int i = 0;
            foreach (var r in testResult)
                encoded += code.At(i++, 0) ? "1" : "0";
            encoded.Should().Be(testResult);
        }

        [Fact]
        public void Encode_InvalidCharacters_ShouldThrowException()
        {
            Action action = () => TwoToFive.Encode("123A", false, false);
            action.Should().Throw<InvalidOperationException>().WithMessage("Can only encode numerical digits (0-9)");
        }

        [Fact]
        public void Encode_InterleavedWithOddDigitCount_ShouldThrowException()
        {
            Action action = () => TwoToFive.Encode("123", true, false);
            action.Should().Throw<InvalidOperationException>().WithMessage("Can only encode an even number of digits in interleaved mode");
        }

        [Fact]
        public void Encode_InterleavedWithEvenDigitCountAndChecksum_ShouldThrowException()
        {
            Action action = () => TwoToFive.Encode("1234", true, true);
            action.Should().Throw<InvalidOperationException>().WithMessage("Can only encode an even number of digits in interleaved mode");
        }
    }
}
