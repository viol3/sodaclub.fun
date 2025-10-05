//
//  Address.cs
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
using System.Text;
using Blake2Fast;
using OpenDive.Utils.Jwt;
using Sui.Cryptography;
using UnityEngine;

namespace Sui.ZKLogin.SDK
{
    /// <summary>
    /// A utility class used to compute A Sui address from the:
    /// UserSalt, and JWT token values (ClaimName, ClaimValue, Aud, ISS).
    /// TODO: See how this is implemented / used in ZK Login TS. Can the SDK and outer accout `address` class be reconciled?
    /// </summary>
    public static class Address
    {
        public const int MAX_HEADER_LEN_B64 = 248;
        public const int MAX_PADDED_UNSIGNED_JWT_LEN = 64 * 25;

        /// <summary>
        /// Computes a ZkLogin address from an address seed and issuer.
        /// </summary>
        public static string ComputeZkLoginAddressFromSeed(
            BigInteger seed,
            string iss,
            bool legacyAddress = true
            )
        {
            byte[] addressSeedBytesBigEndian = legacyAddress
                ? Utils.ToBigEndianBytes(seed, 32)
                : Utils.ToPaddedBigEndianBytes(seed, 32);

            if (iss == "accounts.google.com")
            {
                iss = "https://accounts.google.com";
            }
            Debug.Log("ISS: .... " + iss);
            byte[] addressParamBytes = Encoding.UTF8.GetBytes(iss); // WORKS
            Debug.Log(string.Join(", ", addressParamBytes));
            byte[] tmp = new byte[2 + addressSeedBytesBigEndian.Length + addressParamBytes.Length];

            tmp[0] = SignatureSchemeToFlag.ZkLogin;
            tmp[1] = (byte)addressParamBytes.Length;
            Buffer.BlockCopy(addressParamBytes, 0, tmp, 2, addressParamBytes.Length);
            Buffer.BlockCopy(addressSeedBytesBigEndian, 0, tmp, 2 + addressParamBytes.Length, addressSeedBytesBigEndian.Length);

            byte[] hash = Blake2b.ComputeHash(32, tmp);

            // Convert to hex and normalize
            Debug.Log(" BLAKE2HASH LENGTH: " + hash.Length + " HASH: " + hash.ToString());

            Accounts.AccountAddress address = new Accounts.AccountAddress(hash);
            return address.KeyHex;

        }

        /// <summary>
        /// Performs length checks on JWT components.
        /// </summary>
        public static void LengthChecks(string jwt)
        {
            string[] parts = jwt.Split('.');
            if (parts.Length < 2)
                throw new ArgumentException("Invalid JWT format");

            string header = parts[0];
            string payload = parts[1];

            if (header.Length > MAX_HEADER_LEN_B64)
                throw new Exception("Header is too long");

            long L = (header.Length + 1 + payload.Length) * 8;
            long K = (512 + 448 - ((L % 512) + 1)) % 512;
            long padded_unsigned_jwt_len = (L + 1 + K + 64) / 8;

            if (padded_unsigned_jwt_len > MAX_PADDED_UNSIGNED_JWT_LEN)
                throw new Exception("JWT is too long");
        }

        // <summary>
        /// Converts a JWT to a ZkLogin address.
        /// </summary>
        public static string JwtToAddress(string jwt, string userSalt, bool legacyAddress = false)
        {
            LengthChecks(jwt);

            JWT decodedJWT = JWTDecoder.DecodeJWT(jwt);

            return JwtToAddress(decodedJWT, userSalt, legacyAddress);
        }

        public static string JwtToAddress(JWT jwt, string userSalt, bool legacyAddress = false)
        {
            JWTPayload payload = jwt.Payload;

            if (string.IsNullOrEmpty(payload.Sub)
                || string.IsNullOrEmpty(payload.Iss)
                || string.IsNullOrEmpty(payload.Aud))
                throw new ArgumentException("Missing jwt data");

            // Check if Aud is an array by checking if it contains a comma
            // This is a simple way to detect multiple audience values
            if (payload.Aud.Contains(","))
                throw new ArgumentException("Not supported aud. Aud is an array, string was expected.");

            return ComputeZkLoginAddress(new ZkLoginAddressOptions
            {
                UserSalt = userSalt,
                ClaimName = "sub",
                ClaimValue = payload.Sub,
                Aud = payload.Aud,
                Iss = payload.Iss,
                LegacyAddress = legacyAddress
            });
        }

        public static string ComputeZkLoginAddress(ZkLoginAddressOptions options)
        {
            var seed = Utils.GenAddressSeed(
                options.UserSalt,
                options.ClaimName,
                options.ClaimValue,
                options.Aud
            );

            return ComputeZkLoginAddressFromSeed(
                seed,
                options.Iss,
                options.LegacyAddress
            );
        }
    }

    public class ZkLoginAddressOptions
    {
        public string ClaimName { get; set; }
        public string ClaimValue { get; set; }
        public string UserSalt { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; }
        public bool LegacyAddress { get; set; }
    }
}