using System;
using System.Numerics;
using NUnit.Framework;
using Sui.Cryptography.Ed25519;
using Sui.ZKLogin.SDK;
using UnityEngine;

namespace Sui.Tests.ZkLogin
{
    [TestFixture]
    public class NonceTest : MonoBehaviour
    {
        string pkBase64 = "suiprivkey1qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq509duq";
        string pubKeyBase64Expected = "O2onvM62pC1io6jQKm8Nc2UyFXcd4kOmOsBIoYtZ2ik=";

        /// <summary>
        /// PrivateKey:
        ///     {"schema":"ED25519","privateKey":"lHezoWY/4pRWe+iajFHw62hQjmVQ6wlL+C8CJxw4bY0="}
        /// PublicKey:
        ///     "/CTTrykDrvNxtl0WfBo3Q+H/L9VLJAzwXAJew6cMP70="
        /// </summary>
        // Test Case 1: Empty private key, small BigInteger
        [Test]
        public void GenerateNonceTest_1()
        {
            PrivateKey pk = new PrivateKey(new byte[32]);
            string pubKey = pk.PublicKey().KeyBase64;
            Assert.AreEqual(pubKeyBase64Expected, pubKey);

            int maxEpoch = 0;
            BigInteger randomness = new BigInteger(91593735651025872);

            string nonce = NonceGenerator.GenerateNonce(
                (PublicKey)pk.PublicKey(),
                maxEpoch,
                randomness
            );

            string nonceExpected = "SoeqFjFH8qNU8t4z1oh2aWp1jJI"; 
            Assert.AreEqual(nonceExpected, nonce, "RAND: " + randomness.ToString());
        }

        [Test]
        // Test Case 2: Empty private key, small BigInteger
        public void GenerateNonceTest_2()
        {
            PrivateKey pk = new PrivateKey(new byte[32]);
            string pubKey = pk.PublicKey().KeyBase64;

            Assert.AreEqual(pubKeyBase64Expected, pubKey);

            int maxEpoch = 0;
            BigInteger randomness = new BigInteger(915937356510258724);

            string nonce = NonceGenerator.GenerateNonce(
                (PublicKey)pk.PublicKey(),
                maxEpoch,
                randomness
            );

            string nonceExpected = "smaC7ju0NrM0birjNuUhZspaBOQ";
            Debug.Log("RAND: LOG: " + randomness);

            Assert.AreEqual(nonceExpected, nonce, "RAND: " + randomness.ToString());
        }

        [Test]
        // Test Case 3: Empty private key, large BigInteger
        public void GenerateNonceTest_3()
        {
            PrivateKey pk = new PrivateKey(new byte[32]);
            string pubKey = pk.PublicKey().KeyBase64;
            Assert.AreEqual(pubKeyBase64Expected, pubKey);

            int maxEpoch = 0;
            BigInteger randomness = BigInteger.Parse("144441570523660387698699922682251371601");

            string nonce = NonceGenerator.GenerateNonce(
                (PublicKey)pk.PublicKey(),
                maxEpoch,
                randomness
            );

            string nonceExpected = "Au6gNOIjTVy5y7yaoq63UVNWNOc";
            Assert.AreEqual(nonceExpected, nonce, "RAND: " + randomness.ToString());
        }

        [Test]
        // Test Case 4: Empty private key, large BigInteger
        public void GenerateNonceTest_4()
        {
            PrivateKey pk = new PrivateKey(new byte[32]);
            string pubKey = pk.PublicKey().KeyBase64;
            Assert.AreEqual(pubKeyBase64Expected, pubKey);

            int maxEpoch = 0;
            BigInteger randomness = BigInteger.Parse("52795003160875479850680435799259259156");

            string nonce = NonceGenerator.GenerateNonce(
                (PublicKey)pk.PublicKey(),
                maxEpoch,
                randomness
            );

            string nonceExpected = "5OR1kjf9JnXLjqOFoCf3oWNYUuI";
            Assert.AreEqual(nonceExpected, nonce, "RAND: " + randomness.ToString());
        }

        [Test]
        // Test Case 5: Sample private key, bigger epoch number and large BigInteger for randomness
        public void GenerateNonceTest_5()
        {
            PrivateKey pk = new PrivateKey("d9zN88TckfIma6bORNvc55gYyNExHMfYWDPackyptVE=");
            string pubKey = pk.PublicKey().KeyBase64;
            string pubKeyExpectedB64 = "MAJEmJmINxz1EUAh5WAkA14HuK+UmGR/mh0KFwqWsh4=";
            Assert.AreEqual(pubKeyExpectedB64, pubKey);

            int maxEpoch = 31;
            BigInteger randomness = BigInteger.Parse("135690536260876761130952245550993691844");

            string nonce = NonceGenerator.GenerateNonce(
                (PublicKey)pk.PublicKey(),
                maxEpoch,
                randomness
            );

            string nonceExpected = "XHL72OBEiaVtQkO_i_3BWB3dDEw";
            Assert.AreEqual(nonceExpected, nonce, "RAND: " + randomness.ToString());
        }

        /// <summary>
        /// Verifies that converting an empty byte array returns BigInteger.Zero.
        /// This is an edge case test to ensure the method handles empty input gracefully.
        /// </summary>
        [Test]
        [Description("Validates conversion of an empty byte array to BigInteger")]
        public void ToBigIntBETest_EmptyArray()
        {
            byte[] bytes = { };
            BigInteger toBigintBE = NonceGenerator.ToBigIntBE(bytes);
            Assert.AreEqual(BigInteger.Zero, toBigintBE,
                "An empty byte array should convert to BigInteger.Zero");
        }

        /// <summary>
        /// Tests conversion of a single byte to BigInteger.
        /// Validates that the method correctly interprets a single byte value
        /// without any endianness concerns.
        /// </summary>
        [Test]
        [Description("Verifies correct conversion of a single byte to BigInteger")]
        public void ToBigIntBETest_SingleByte()
        {
            byte[] bytes = new byte[] { 0x12 };
            BigInteger toBigintBE = NonceGenerator.ToBigIntBE(bytes);
            Assert.AreEqual(new BigInteger(0x12), toBigintBE,
                "Single byte 0x12 should convert to equivalent BigInteger value");
        }

        /// <summary>
        /// Tests conversion of multiple bytes to BigInteger using big-endian ordering.
        /// Validates that bytes are interpreted in the correct order: from most significant
        /// byte (leftmost) to least significant byte (rightmost).
        /// Example: [0x12, 0x34, 0x56, 0x78] should become 0x12345678
        /// </summary>
        [Test]
        [Description("Validates big-endian conversion of multiple bytes to BigInteger")]
        public void ToBigIntBETest_MultiByte()
        {
            byte[] bytes = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            BigInteger toBigintBE = NonceGenerator.ToBigIntBE(bytes);
            Assert.AreEqual(new BigInteger(0x12345678), toBigintBE,
                "Bytes should be interpreted in big-endian order (MSB first)");
        }

        /// <summary>
        /// Tests handling of large numbers with all bytes having significant values.
        /// Validates correct interpretation of a byte sequence where each byte contributes
        /// to the final value, including handling of byte values above 0x7F.
        /// Example: [0xff, 0xee, ..., 0x88] = 0xffeeddccbbaa9988
        /// </summary>
        [Test]
        [Description("Tests conversion of bytes representing a large number with no leading zeros")]
        public void ToBigIntBETest_MultiByte_LeadingZeros()
        {
            byte[] bytes = new byte[] { 0xff, 0xee, 0xdd, 0xcc, 0xbb, 0xaa, 0x99, 0x88 };
            BigInteger toBigintBE = NonceGenerator.ToBigIntBE(bytes);
            Assert.AreEqual(new BigInteger(0xffeeddccbbaa9988), toBigintBE,
                "Should correctly handle large numbers with all significant bytes");
        }

        /// <summary>
        /// Tests handling of bytes with leading zeros.
        /// Validates that leading zeros are properly handled in big-endian conversion,
        /// ensuring they don't affect the numerical value but are respected in the 
        /// byte ordering.
        /// Example: [0x00, 0x01, 0x02, 0x03] should become 0x010203
        /// </summary>
        [Test]
        [Description("Verifies correct handling of leading zeros in byte array")]
        public void ToBigIntBETest_LargeByteArray()
        {
            byte[] bytes = new byte[] { 0x00, 0x01, 0x02, 0x03 };
            BigInteger toBigintBE = NonceGenerator.ToBigIntBE(bytes);
            Assert.AreEqual(new BigInteger(0x010203), toBigintBE,
                "Leading zeros should be properly handled in conversion");
        }

        /// <summary>
        /// Tests conversion of maximum single byte value (0xFF = 255).
        /// Validates correct handling of the edge case where a single byte
        /// contains its maximum possible value (all bits set).
        /// Important for ensuring proper handling of unsigned byte values.
        /// </summary>
        [Test]
        [Description("Tests conversion of maximum possible single byte value")]
        public void ToBigIntBETest_MaximumSingleByteValue()
        {
            byte[] bytes = new byte[] { 0xff };
            BigInteger toBigintBE = NonceGenerator.ToBigIntBE(bytes);
            Assert.AreEqual(new BigInteger(0xff), toBigintBE,
                "Maximum byte value 0xFF should convert to 255");
        }

        /// <summary>
        /// Verifies that when calling RandomBytes with no parameters, 
        /// it returns an array of exactly 32 bytes, which is the default length.
        /// This test ensures the default behavior remains consistent.
        /// </summary>
        [Test]
        [Description("Ensures the default RandomBytes call returns 32 bytes")]
        public void RandomBytes_DefaultLength_Returns32Bytes()
        {
            // Act
            byte[] result = NonceGenerator.RandomBytes();

            // Assert
            Assert.AreEqual(32, result.Length);
        }

        /// <summary>
        /// Validates that RandomBytes correctly handles custom length requests.
        /// Tests if the returned array matches the specified length.
        /// This ensures the method correctly respects user-specified lengths.
        /// </summary>
        [Test]
        [Description("Verifies RandomBytes returns the exact number of bytes requested")]
        public void RandomBytes_CustomLength_ReturnsCorrectLength()
        {
            // Arrange
            int customLength = 16;

            // Act
            byte[] result = NonceGenerator.RandomBytes(customLength);

            // Assert
            Assert.AreEqual(customLength, result.Length);
        }

        /// <summary>
        /// Tests the randomness of the generated bytes by comparing two consecutive calls.
        /// While there's a theoretical possibility of getting identical arrays,
        /// the probability is astronomically low (1 in 2^256 for 32 bytes).
        /// This test helps ensure the cryptographic randomness of the implementation.
        /// </summary>
        [Test]
        [Description("Confirms that consecutive calls return different random values")]
        public void RandomBytes_CalledTwice_ReturnsDifferentValues()
        {
            // Act
            byte[] firstResult = NonceGenerator.RandomBytes();
            byte[] secondResult = NonceGenerator.RandomBytes();

            // Assert
            bool areEqual = true;
            for (int i = 0; i < firstResult.Length; i++)
            {
                if (firstResult[i] != secondResult[i])
                {
                    areEqual = false;
                    break;
                }
            }
            Assert.IsFalse(areEqual, "Two consecutive calls should return different random values");
        }

        /// <summary>
        /// Verifies that requesting zero bytes returns an empty array rather than null.
        /// This test ensures consistent behavior with edge cases and follows the principle
        /// that array-returning methods should return empty arrays rather than null.
        /// </summary>
        [Test]
        [Description("Confirms that requesting 0 bytes returns an empty array")]
        public void RandomBytes_ZeroLength_ReturnsEmptyArray()
        {
            // Act
            byte[] result = NonceGenerator.RandomBytes(0);

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        /// <summary>
        /// Validates that the method properly handles invalid input by throwing
        /// an appropriate exception when given a negative length.
        /// This test ensures robust error handling and input validation.
        /// </summary>
        [Test]
        [Description("Verifies that negative length input throws ArgumentOutOfRangeException")]
        public void RandomBytes_NegativeLength_ThrowsArgumentException()
        {
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => NonceGenerator.RandomBytes(-1));
        }
    }
}