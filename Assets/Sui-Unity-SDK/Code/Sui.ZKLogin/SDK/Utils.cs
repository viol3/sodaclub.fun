//
//  Utils.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sui.Cryptography.Ed25519;
using UnityEngine;

namespace Sui.ZKLogin.SDK
{
    public static class Utils
    {
        private const int MAX_KEY_CLAIM_NAME_LENGTH = 32;
        private const int MAX_KEY_CLAIM_VALUE_LENGTH = 115;
        private const int MAX_AUD_VALUE_LENGTH = 145;
        private const int PACK_WIDTH = 248;

        // Cache for common calculations
        private static readonly Dictionary<int, int> _chunkSizeCache = new Dictionary<int, int>();

        /// <summary>
        /// Finds the index of the first non-zero byte in a byte array.
        /// </summary>
        /// <param name="bytes">The byte array to search</param>
        /// <returns>The index of the first non-zero byte, or -1 if all bytes are zero</returns>
        public static int FindFirstNonZeroIndex(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
                if (bytes[i] != 0)
                    return i;
            return -1;
        }

        /// <summary>
        /// Converts a BigInteger to a byte array padded to specified width.
        /// </summary>
        /// <param name="num">The number to convert</param>
        /// <param name="width">The desired width in bytes</param>
        /// <returns>Byte array padded to specified width</returns>
        public static byte[] ToPaddedBigEndianBytes(this BigInteger num, int width)
        {
            // Convert to hex string and pad
            string hex = num.ToString("X");
            hex = hex.PadLeft(width * 2, '0');

            // Take only the last width*2 characters to match desired byte length
            hex = hex.Substring(Math.Max(0, hex.Length - width * 2));

            return Utilities.Utils.HexStringToByteArray(hex);
        }

        /// <summary>
        /// Converts a BigInteger to a big-endian byte array, removing leading zeros but ensuring at least one byte is returned.
        /// </summary>
        /// <param name="num">The number to convert</param>
        /// <param name="width">The maximum width in bytes</param>
        /// <returns>A byte array representing the number with minimal leading zeros</returns>
        public static byte[] ToBigEndianBytes(this BigInteger num, int width)
        {
            byte[] bytes = ToPaddedBigEndianBytes(num, width);

            int firstNonZeroIndex = FindFirstNonZeroIndex(bytes);

            if (firstNonZeroIndex == -1)
                return new byte[] { 0 };

            return bytes.Skip(firstNonZeroIndex).ToArray();
        }

        /// <summary>
        /// TODO: Note: This method would depend on your PublicKey implementation
        /// Converts a public key to its extended ephemeral form in Sui format.
        /// </summary>
        /// <param name="publicKey">The public key to convert.</param>
        /// <returns>The public key in Sui format as a string.</returns>
        public static string GetExtendedEphemeralPublicKey(PublicKey publicKey)
        {
            return publicKey.ToSuiPublicKey();
        }

        /// <summary>
        /// Splits an array into chunks of a specified size. If the array is not evenly divisible
        /// by chunkSize, the first chunk will be smaller than chunkSize.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="chunkSize">The size of each chunk.</param>
        /// <returns>A list of chunks, where each chunk is a list of elements.</returns>
        public static List<List<T>> ChunkArray<T>(T[] array, int chunkSize)
        {
            var reversed = array.Reverse().ToArray();
            var chunks = new List<List<T>>();
            int totalChunks = (array.Length + chunkSize - 1) / chunkSize;

            for (int i = 0; i < totalChunks; i++)
            {
                chunks.Add(reversed.Skip(i * chunkSize)
                                  .Take(chunkSize)
                                  .Reverse()
                                  .ToList());
            }

            chunks.Reverse();
            return chunks;
        }

        public static BigInteger BytesBEToBigInt(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return BigInteger.Zero;

            string hex = BitConverter.ToString(bytes).Replace("-", "");
            return BigInteger.Parse("0" + hex, System.Globalization.NumberStyles.HexNumber);
        }
        
        /// <summary>
        /// TODO: Add test.
        /// Hashes an ASCII string to a field element.
        /// </summary>
        /// <param name="str">The ASCII string to hash.</param>
        /// <param name="maxSize">The maximum allowed size of the string.</param>
        /// <param name="poseidonHash">The hashing function to use.</param>
        /// <returns>A hashed BigInteger value.</returns>
        public static BigInteger HashASCIIStrToField(string str, int maxSize)
        {
            if (str.Length > maxSize)
                throw new ArgumentException($"String {str} is longer than {maxSize} chars");

            var strPadded = str.PadRight(maxSize, '\u0000')
                .Select(c => (int)c)
                .ToArray();

            int chunkSize = PACK_WIDTH / 8;
            var packed = SDK.Utils.ChunkArray(strPadded, chunkSize)
                .Select(chunk => SDK.Utils.BytesBEToBigInt(chunk.Select(i => (byte)(i & 0xFF)).ToArray()))
                .ToArray();

            Debug.Log("PACKED LENGTH: " + packed.Length);

            return PoseidonHasher.PoseidonHash(packed);
        }

        /// <summary>
        /// Generates an address seed based on provided parameters.
        /// TODO: Add test.
        /// </summary>
        /// <param name="salt">The salt value as a string or BigInteger.</param>
        /// <param name="name">The name value to hash.</param>
        /// <param name="value">The value to hash.</param>
        /// <param name="aud">The audience value to hash.</param>
        /// <param name="maxNameLength">The maximum name length (default is MAX_KEY_CLAIM_NAME_LENGTH).</param>
        /// <param name="maxValueLength">The maximum value length (default is MAX_KEY_CLAIM_VALUE_LENGTH).</param>
        /// <param name="maxAudLength">The maximum audience length (default is MAX_AUD_VALUE_LENGTH).</param>
        /// <returns>The generated address seed as a BigInteger.</returns>
        public static BigInteger GenAddressSeed(
            string salt,
            string name,
            string value,
            string aud,
            int maxNameLength = MAX_KEY_CLAIM_NAME_LENGTH,
            int maxValueLength = MAX_KEY_CLAIM_VALUE_LENGTH,
            int maxAudLength = MAX_AUD_VALUE_LENGTH)
        {
            BigInteger bigIntSalt = BigInteger.Parse(salt);
            return GenAddressSeed(
                bigIntSalt,
                name,
                value,
                aud,
                maxNameLength,
                maxValueLength,
                maxAudLength
            );
        }

        /// <summary>
        /// Generates an address seed based on provided parameters.
        /// TODO: Add test.
        /// </summary>
        /// <param name="salt">The salt value as a BigInteger.</param>
        /// <param name="name">The name value to hash.</param>
        /// <param name="value">The value to hash.</param>
        /// <param name="aud">The audience value to hash.</param>
        /// <param name="maxNameLength">The maximum name length (default is MAX_KEY_CLAIM_NAME_LENGTH).</param>
        /// <param name="maxValueLength">The maximum value length (default is MAX_KEY_CLAIM_VALUE_LENGTH).</param>
        /// <param name="maxAudLength">The maximum audience length (default is MAX_AUD_VALUE_LENGTH).</param>
        /// <returns>The generated address seed as a BigInteger.</returns>
        public static BigInteger GenAddressSeed(
            BigInteger salt,
            string name,
            string value,
            string aud,
            int maxNameLength = MAX_KEY_CLAIM_NAME_LENGTH,
            int maxValueLength = MAX_KEY_CLAIM_VALUE_LENGTH,
            int maxAudLength = MAX_AUD_VALUE_LENGTH)
        {

            var saltArr = new BigInteger[] { salt };

            var saltHash = PoseidonHasher.PoseidonHash(
                saltArr
            );

            return SDK.PoseidonHasher.PoseidonHash(
                new List<BigInteger>
                {
                    HashASCIIStrToField(name, maxNameLength),
                    HashASCIIStrToField(value, maxValueLength),
                    HashASCIIStrToField(aud, maxAudLength),
                    saltHash
                }.ToArray()
            );
        }

        /// <summary>
        /// Converts a byte array to a hexadecimal string using a pre-computed lookup table.
        /// </summary>
        /// <param name="bytes">The byte array to convert.</param>
        /// <returns>The hexadecimal representation of the byte array.</returns>
        /// <example>
        /// byte[] data = new byte[] { 0xca, 0xfe, 0x01, 0x23 };
        /// string hex = ByteUtils.BytesToHex(data); // Returns "cafe0123"
        /// </example>
        public static string BytesToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}