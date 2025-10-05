//
//  Nonce.cs
//  Sui-Unity-SDK
//
//  Copyright (c) 2025 OpenDive
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using System;
using System.Numerics;
using System.Security.Cryptography;
using Sui.Cryptography.Ed25519;

namespace Sui.ZKLogin.SDK
{
    /// <summary>
    /// TODO: See if there are any issues with using RNGCryptoServiceProvider on mobile or WebGL
    /// </summary>
    public static class NonceGenerator
    {
        public const int NONCE_LENGTH = 27;

        /// <summary>
        /// Converts a byte array into a BigInteger value,
        /// interpreting the bytes in big-endian order.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static BigInteger ToBigIntBE(byte[] bytes)
        {
            if (bytes.Length == 0 || bytes == null)
                return BigInteger.Zero;

            // Convert to hex and then to BigInteger
            string hex = BitConverter.ToString(bytes).Replace("-", "");
            return BigInteger.Parse("0" + hex, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Generate random number.
        /// </summary>
        /// <returns>A Random big integer number in string format</returns>
        public static string GenerateRandomness()
        {
            return ToBigIntBE(RandomBytes(16)).ToString();
        }

        /// <summary>
        /// Generates cryptographically secure random bytes using RNGCryptoServiceProvider.
        /// This is a similar implementation to the function: `randomBytes` in `@noble/hashes/utils`
        /// </summary>
        /// <param name="bytesLength">Number of random bytes to generate.
        /// Defaults to 32.</param>
        /// <returns>Array of random bytes</returns>
        public static byte[] RandomBytes(int bytesLength = 32)
        {
            if (bytesLength < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(bytesLength), "Number of bytes cannot be negative");
            }

            byte[] randomBytes = new byte[bytesLength];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        /// <summary>
        ///  An application-defined field embedded in the JWT payload, computed
        ///  as the hash of the ephemeral public key, JWT randomness,
        ///  and the maximum epoch (Sui's defined expiry epoch).
        ///
        /// Specifically, a zkLogin compatible nonce is required to passed in as
        /// <code>
        ///     nonce = ToBase64URL(
        ///         Poseidon_BN254([
        ///             ext_eph_pk_bigint / 2^128,
        ///             ext_eph_pk_bigint % 2^128,
        ///             max_epoch,
        ///             jwt_randomness
        ///         ]).to_bytes()[len - 20..]
        ///     )
        /// </code>
        /// where `ext_eph_pk_bigint` is the BigInt representation of ext_eph_pk.
        /// </summary>
        /// 
        /// <param name="publicKey">
        ///     The byte representation of an ephemeral public key (flag || eph_pk).
        ///     Size varies depending on the choice of the signature scheme
        ///     (denoted by the flag, defined in Signatures).
        /// </param>
        /// <param name="maxEpoch">
        ///     The epoch at which the JWT expires. This is u64 used in Sui, and
        ///     is fetched using the Sui Client.
        ///     Validity period of the ephemeral key pair. e.g. `26`
        /// </param>
        /// <param name="randomness">
        ///     Randomness generated.e.g. `91593735651025872471886891147594672981`
        /// </param>
        /// <returns>
        /// A nonce value computed from the parameter, and encoded as a Base64 string.
        /// e.g. `LSLuhEjHLSeRvyI26wfPQSjYNbc`
        /// </returns>
        public static string GenerateNonce(PublicKey publicKey, int maxEpoch, string randomness)
        {
            return GenerateNonce(publicKey, maxEpoch, BigInteger.Parse(randomness));
        }

        public static string GenerateNonce(PublicKey publicKey, int maxEpoch, BigInteger randomness)
        {
            byte[] publicKeyBytes = publicKey.ToSuiBytes();
            BigInteger publicKeyBigInt = ToBigIntBE(publicKeyBytes);

            // Split public key into two 128-bit parts
            //BigInteger eph_public_key_0 = publicKeyBigInt >> 128; // IRVIN: Same as publicKeyBytes / 2n ** 128n;
            //BigInteger eph_public_key_1 = publicKeyBigInt & ((BigInteger.One << 128) - BigInteger.One); // IRVIN: Same as publicKeyBytes % 2n ** 128n;
            BigInteger eph_public_key_0 = publicKeyBigInt / BigInteger.Pow(2, 128);
            BigInteger eph_public_key_1 = publicKeyBigInt % BigInteger.Pow(2, 128);

            BigInteger bigNum = PoseidonHasher.PoseidonHash(new[] {
                eph_public_key_0,
                eph_public_key_1,
                new BigInteger(maxEpoch),
                randomness
            });

            byte[] Z = Utils.ToPaddedBigEndianBytes(bigNum, 20);
            string nonce = JwtUtils.Base64UrlEncode(Z);

            if (nonce.Length != NONCE_LENGTH)
                throw new Exception($"Length of nonce {nonce} ({nonce.Length}) is not equal to {NONCE_LENGTH}");

            return nonce;
        }
    }
}