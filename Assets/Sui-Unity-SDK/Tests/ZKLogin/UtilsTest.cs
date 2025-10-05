using System;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Sui.ZKLogin.SDK;

namespace Sui.Tests.ZkLogin
{
    public class UtilsTest
    {
        [Test]
        public void FindFirstNonZeroIndexTest()
        {
            byte[] bytes = { 0, 0, 0, 1, 2, 3 };
            int actualIndex = Utils.FindFirstNonZeroIndex(bytes);
            Assert.AreEqual(3, actualIndex);
        }

        [Test]
        public void FindFirstNonZeroIndex_ShouldFindCorrectIndex()
        {
            // Test case 1: Leading zeros
            Assert.AreEqual(2, Utils.FindFirstNonZeroIndex(new byte[] { 0, 0, 1, 2 }));

            // Test case 2: No leading zeros
            Assert.AreEqual(0, Utils.FindFirstNonZeroIndex(new byte[] { 1, 2, 3, 4 }));

            // Test case 3: All zeros
            Assert.AreEqual(-1, Utils.FindFirstNonZeroIndex(new byte[] { 0, 0, 0, 0 }));

            // Test case 4: Single zero
            Assert.AreEqual(-1, Utils.FindFirstNonZeroIndex(new byte[] { 0 }));

            // Test case 5: Single non-zero
            Assert.AreEqual(0, Utils.FindFirstNonZeroIndex(new byte[] { 1 }));
        }

        [Test]
        public void ToPaddedBigEndianBytes_ShouldConvertCorrectly()
        {
            // Test case 1: Basic conversion
            Assert.AreEqual(
                new byte[] { 0 },
                ZKLogin.SDK.Utils.ToPaddedBigEndianBytes(BigInteger.Parse("0"), 1)
            );

            // Test case 2: Max single byte
            Assert.AreEqual(
                new byte[] { 255 },
                ZKLogin.SDK.Utils.ToPaddedBigEndianBytes(BigInteger.Parse("255"), 1)
            );

            // Test case 3: Two bytes
            Assert.AreEqual(
                new byte[] { 1, 0 },
                ZKLogin.SDK.Utils.ToPaddedBigEndianBytes(BigInteger.Parse("256"), 2)
            );

            // Test case 4: Max two bytes
            Assert.AreEqual(
                new byte[] { 255, 255 },
                ZKLogin.SDK.Utils.ToPaddedBigEndianBytes(BigInteger.Parse("65535"), 2)
            );
        }

        [Test]
        public void ToPaddedBigEndianBytes_ShouldPadWithZeros()
        {
            // Test case 1: Padding small number to 4 bytes
            byte[] result1 = ZKLogin.SDK.Utils.ToPaddedBigEndianBytes(BigInteger.Parse("255"), 4);
            Assert.AreEqual(4, result1.Length);
            CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 255 }, result1);

            // Test case 2: Padding medium number to 4 bytes
            byte[] result2 = ZKLogin.SDK.Utils.ToPaddedBigEndianBytes(BigInteger.Parse("65535"), 4);
            Assert.AreEqual(4, result2.Length);
            CollectionAssert.AreEqual(new byte[] { 0, 0, 255, 255 }, result2);
        }

        [Test]
        public void ToBigEndianBytes_ShouldRemoveLeadingZeros()
        {
            // Test case 1: Zero should return single zero byte
            CollectionAssert.AreEqual(
                new byte[] { 0 },
                ZKLogin.SDK.Utils.ToBigEndianBytes(BigInteger.Parse("0"), 4)
            );

            // Test case 2: Single byte number
            CollectionAssert.AreEqual(
                new byte[] { 255 },
                ZKLogin.SDK.Utils.ToBigEndianBytes(BigInteger.Parse("255"), 4)
            );

            // Test case 3: Two byte number
            CollectionAssert.AreEqual(
                new byte[] { 1, 0 },
                ZKLogin.SDK.Utils.ToBigEndianBytes(BigInteger.Parse("256"), 4)
            );

            // Test case 4: Max two bytes
            CollectionAssert.AreEqual(
                new byte[] { 255, 255 },
                ZKLogin.SDK.Utils.ToBigEndianBytes(BigInteger.Parse("65535"), 4)
            );
        }

        [Test]
        public void ToBigEndianBytes_ShouldReturnSingleZeroForZero()
        {
            byte[] result = ZKLogin.SDK.Utils.ToBigEndianBytes(BigInteger.Zero, 4);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(0, result[0]);
        }

        [Test]
        public void ToBigEndianBytesTest()
        {
            BigInteger num1 = new BigInteger(255);
            byte[] bigEndianBytes = num1.ToBigEndianBytes(4);
            Assert.AreEqual(string.Join(",", bigEndianBytes), "255");

            BigInteger num2 = new BigInteger(0);
            byte[] bigEndianBytesZero = num2.ToBigEndianBytes(4);
            Assert.AreEqual(string.Join(",", bigEndianBytesZero), "0");
        }

        [Test]
        public void ToPaddedBigEndianBytes_ShouldHandleLargeNumbers()
        {
            // Arrange
            BigInteger num = BigInteger.Parse("1234567890123456789012345678901234567890");
            int width = 32;

            // Act
            byte[] result = ZKLogin.SDK.Utils.ToPaddedBigEndianBytes(num, width);

            // Assert
            Assert.AreEqual(width, result.Length);
            Assert.AreNotEqual(0, result[result.Length - 1]); // Last byte shouldn't be zero
        }

        [Test]
        public void ToPaddedBigEndianBytesTest()
        {
            BigInteger bigInt = new BigInteger(255);
            byte[] paddedBytes = bigInt.ToPaddedBigEndianBytes(4);
            Assert.AreEqual(string.Join(",", paddedBytes), "0,0,0,255");
        }


        [Test]
        public void BytesBEToBigIntTest_ValidBytes()
        {
            byte[] bytes = { 0x01, 0x02, 0x03 };
            BigInteger result = Utils.BytesBEToBigInt(bytes);
            Assert.AreEqual(new BigInteger(0x010203), result);
        }

        [Test]
        public void BytesBEToBigIntTest_EmptyArray()
        {
            byte[] bytes = { };
            BigInteger result = Utils.BytesBEToBigInt(bytes);
            Assert.AreEqual(BigInteger.Zero, result);
        }

        [Test]
        public void BytesBEToBigIntTest_AllZeros()
        {
            byte[] bytes = { 0x00, 0x00, 0x00 };
            BigInteger result = Utils.BytesBEToBigInt(bytes);
            Assert.AreEqual(BigInteger.Zero, result);
        }

        [Test]
        public void BytesBEToBigIntTest_SingleByte()
        {
            byte[] bytes = { 0xFF };
            BigInteger result = Utils.BytesBEToBigInt(bytes);
            Assert.AreEqual(new BigInteger(0xFF), result);
        }

        [Test]
        public void BytesBEToBigIntTest_LargeValue()
        {
            byte[] bytes = { 0xFF, 0xFE, 0xFD, 0xFC };
            BigInteger result = Utils.BytesBEToBigInt(bytes);
            Assert.AreEqual(new BigInteger(0xFFFEFDFC), result);
        }

        [Test]
        public void BytesBEToBigIntTest_NullArray()
        {
            byte[] bytes = null;
            BigInteger result = Utils.BytesBEToBigInt(bytes);
            Assert.AreEqual(BigInteger.Zero, result);
        }

        [Test]
        public void ChunkArrayTest_EvenSplit()
        {
            int[] array = { 1, 2, 3, 4, 5, 6 };
            var chunks = Utils.ChunkArray(array, 2);

            Assert.AreEqual(3, chunks.Count);
            Assert.AreEqual(new List<int> { 1, 2 }, chunks[0]);
            Assert.AreEqual(new List<int> { 3, 4 }, chunks[1]);
            Assert.AreEqual(new List<int> { 5, 6 }, chunks[2]);
        }

        [Test]
        public void ChunkArrayTest_OddSplit()
        {
            int[] array = { 1, 2, 3, 4, 5 };
            var chunks = Utils.ChunkArray(array, 2);

            Assert.AreEqual(3, chunks.Count);
            Assert.AreEqual(new List<int> { 1, 2 }, chunks[0]);
            Assert.AreEqual(new List<int> { 3, 4 }, chunks[1]);
            Assert.AreEqual(new List<int> { 5 }, chunks[2]);
        }

        [Test]
        public void ChunkArrayTest_EmptyArray()
        {
            int[] array = { };
            var chunks = Utils.ChunkArray(array, 2);

            Assert.AreEqual(0, chunks.Count);
        }

        [Test]
        public void ChunkArrayTest_ChunkSizeLargerThanArray()
        {
            int[] array = { 1, 2, 3 };
            var chunks = Utils.ChunkArray(array, 5);

            Assert.AreEqual(1, chunks.Count);
            Assert.AreEqual(new List<int> { 1, 2, 3 }, chunks[0]);
        }

        [Test]
        public void ChunkArrayTest_InvalidChunkSize()
        {
            int[] array = { 1, 2, 3 };

            Assert.Throws<ArgumentException>(() => Utils.ChunkArray(array, 0));
            Assert.Throws<ArgumentException>(() => Utils.ChunkArray(array, -1));
        }

        [Test]
        public void ChunkArrayTest_SingleElementArray()
        {
            int[] array = { 42 };
            var chunks = Utils.ChunkArray(array, 2);

            Assert.AreEqual(1, chunks.Count);
            Assert.AreEqual(new List<int> { 42 }, chunks[0]);
        }

        [Test]
        public void BytesToHex_EmptyArray_ReturnsEmptyString()
        {
            byte[] bytes = Array.Empty<byte>();
            string result = ZKLogin.SDK.Utils.BytesToHex(bytes);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void BytesToHex_SingleByte_ReturnsCorrectHex()
        {
            byte[] bytes = { 0xab };
            string result = ZKLogin.SDK.Utils.BytesToHex(bytes);
            Assert.That(result, Is.EqualTo("ab"));
        }

        [Test]
        public void BytesToHex_MultipleBytesExample_ReturnsCorrectHex()
        {
            byte[] bytes = { 0xca, 0xfe, 0x01, 0x23 };
            string result = ZKLogin.SDK.Utils.BytesToHex(bytes);
            Assert.That(result, Is.EqualTo("cafe0123"));
        }

        [Test]
        public void BytesToHex_ByteWithLeadingZeros_PreservesZeros()
        {
            byte[] bytes = { 0x00, 0x01, 0x02 };
            string result = ZKLogin.SDK.Utils.BytesToHex(bytes);
            Assert.That(result, Is.EqualTo("000102"));
        }

        [Test]
        public void BytesToHex_NullArray_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ZKLogin.SDK.Utils.BytesToHex(null));
        }

        [Test]
        public void BytesToHex_MaxByteValue_ReturnsCorrectHex()
        {
            byte[] bytes = { 0xFF };
            string result = ZKLogin.SDK.Utils.BytesToHex(bytes);
            Assert.That(result, Is.EqualTo("ff"));
        }

        [Test]
        public void BytesToHex_MinByteValue_ReturnsCorrectHex()
        {
            byte[] bytes = { 0x00 };
            string result = ZKLogin.SDK.Utils.BytesToHex(bytes);
            Assert.That(result, Is.EqualTo("00"));
        }

        [Test]
        public void BytesToHex_LargeByteArray_ReturnsCorrectHex()
        {
            byte[] bytes = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc, 0xde, 0xf0 };
            string result = ZKLogin.SDK.Utils.BytesToHex(bytes);
            Assert.That(result, Is.EqualTo("123456789abcdef0"));
        }

        [Test]
        public void HashASCIIStrToField_ClaimNameValue_AUD_Returns_ExpectedHash()
        {
            BigInteger result = Utils.HashASCIIStrToField("sub", 32);
            BigInteger expected = BigInteger.Parse("9102752833182448263444250585012134730074321235810986230287216596098480554553");
            Assert.That(result, Is.EqualTo(expected));

            result = Utils.HashASCIIStrToField("106286931906362609286", 115);
            expected = BigInteger.Parse("2352939069770256775234829048624339326695552050355924727369814598468708852850");
            Assert.That(result, Is.EqualTo(expected));

            result = Utils.HashASCIIStrToField("573120070871-0k7ga6ns79ie0jpg1ei6ip5vje2ostt6.apps.googleusercontent.com", 145);
            expected = BigInteger.Parse("21206880572761796382621760317640694562316540269674130237534171623788609905211");
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void HashASCIIStrToField_EmptyString_Returns_ValidHash()
        {
            Assert.DoesNotThrow(() => Utils.HashASCIIStrToField("", 32));
        }

        [TestCase("test", 3)]
        [TestCase("toolong", 5)]
        public void HashASCIIStrToField_StringLongerThanMaxSize_ThrowsException(string input, int maxSize)
        {
            Assert.Throws<ArgumentException>(() => Utils.HashASCIIStrToField(input, maxSize));
        }

        [Test]
        public void HashASCIIStrToField_StringWithSpecialChars_Returns_ValidHash()
        {
            Assert.DoesNotThrow(() => Utils.HashASCIIStrToField("test@123!", 32));
        }
    }
}