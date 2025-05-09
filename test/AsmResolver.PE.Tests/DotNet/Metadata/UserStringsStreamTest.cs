using System.Linq;
using AsmResolver.PE.DotNet.Metadata;
using Xunit;

namespace AsmResolver.PE.Tests.DotNet.Metadata
{
    public class UserStringsStreamTest
    {
        private static void AssertDoesNotHaveString(byte[] streamData, string needle)
        {
            var stream = new SerializedUserStringsStream(streamData);
            Assert.False(stream.TryFindStringIndex(needle, out _));
        }

        private static void AssertHasString(byte[] streamData, string needle)
        {
            var stream = new SerializedUserStringsStream(streamData);
            Assert.True(stream.TryFindStringIndex(needle, out uint actualIndex));
            Assert.Equal(needle, stream.GetStringByIndex(actualIndex));
        }

        [Theory]
        [InlineData("")]
        [InlineData("ABC")]
        [InlineData("DEF")]
        public void FindExistingString(string value) => AssertHasString([
                0x00,
                0x07, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x00,
                0x07, 0x44, 0x00, 0x45, 0x00, 0x46, 0x00, 0x00
            ],
            value);

        [Theory]
        [InlineData("BC")]
        [InlineData("F")]
        public void FindOverlappingExistingString(string value) => AssertHasString([
                0x00,
                0x07, 0x41, 0x05, 0x42, 0x00, 0x43, 0x00, 0x00,
                0x07, 0x44, 0x00, 0x45, 0x03, 0x46, 0x00, 0x00
            ],
            value);

        [Theory]
        [InlineData("ABC")]
        [InlineData("DEF")]
        public void FindExistingStringButNotZeroTerminated(string value) => AssertDoesNotHaveString([
                0x00,
                0x07, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0xff,
                0x07, 0x44, 0x00, 0x45, 0x00, 0x46, 0x00, 0xff
            ],
            value);

        [Fact]
        public void FindExistingIncompleteString() => AssertDoesNotHaveString([
                0x00,
                0x07, 0x41, 0x00, 0x42, 0x00
            ],
            "ABC");

        [Theory]
        [InlineData("CDE")]
        [InlineData("XXX")]
        public void FindNonExistingString(string value) => AssertDoesNotHaveString([
                0x00,
                0x07, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0xff,
                0x07, 0x44, 0x00, 0x45, 0x00, 0x46, 0x00, 0xff
            ],
            value);

        [Fact]
        public void EnumerateStrings()
        {
            var stream = new SerializedUserStringsStream([
                0x00,
                0x07, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x00,
                0x07, 0x44, 0x00, 0x45, 0x00, 0x46, 0x00, 0x00
            ]);

            Assert.Equal([
                (1, "ABC"),
                (9, "DEF")
            ], stream.EnumerateStrings());
        }

        [Fact]
        public void EnumerateStringsMalicious()
        {
            var stream = new SerializedUserStringsStream([
                0x00,
                0x07, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x00,
                0x20, 0x44, 0x00, 0x45, 0x00, 0x46, 0x00,
            ]);

            Assert.Equal([
                (1, "ABC"),
                (9, "DEF")
            ], stream.EnumerateStrings());
        }

        [Fact]
        public void EnumerateStringsNullBytes()
        {
            var stream = new SerializedUserStringsStream([
                0x00,
                0x07, 0x41, 0x00, 0x42, 0x00, 0x43, 0x00, 0x00,
                0x00, 0x00,
                0x00, 0x00,
                0x07, 0x44, 0x00, 0x45, 0x00, 0x46, 0x00, 0x00
            ]);

            Assert.Equal([
                (1, "ABC"),
                (9, ""),
                (11, ""),
                (13, "DEF")
            ], stream.EnumerateStrings());
        }
    }
}
