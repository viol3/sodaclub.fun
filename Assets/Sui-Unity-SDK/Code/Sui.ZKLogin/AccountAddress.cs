//
//  AccountAddress.cs
//  Sui-Unity-SDK
//
//  Copyright (c) 2024 OpenDive
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
using System.Text;
using Sui.Cryptography;
using Konscious.Security.Cryptography;

namespace Sui.ZKLogin
{
    /// <summary>
    /// TODO: Look into where and how this is used in ZK Login TS
    /// </summary>
    public class AccountAddress : Accounts.AccountAddress
    {
        /// <summary>
        /// Computes a ZkLogin address from a seed and issuer.
        /// </summary>
        /// <param name="addressSeed">The address seed as BigInteger</param>
        /// <param name="iss">The issuer string</param>
        /// <returns>Normalized Sui address string</returns>
        public Accounts.AccountAddress ComputeZkLoginAddressFromSeed(long addressSeed, string iss)
        {
            // TS: bytesToHex(blake2b(tmp, { dkLen: 32 })).slice(0, SUI_ADDRESS_LENGTH * 2),
            // string hex = BitConverter.ToString(bytes);
            byte[] addressSeedBytesBigEndian = SDK.Utils.ToBigEndianBytes(addressSeed, 32);

            // Normalize Google issuer
            if (iss == "accounts.google.com")
                iss = "https://accounts.google.com"; //TODO: See / ask about implementation for OAuth providers

            byte[] addressParamBytes = Encoding.UTF8.GetBytes(iss);
            byte[] tmp = new byte[2 + addressSeedBytesBigEndian.Length + addressParamBytes.Length];

            // Set signature scheme flag
            tmp[0] = SignatureSchemeToFlag.ZkLogin;

            // Set address param length
            tmp[1] = (byte)addressParamBytes.Length;

            // Copy address param bytes
            Buffer.BlockCopy(addressParamBytes, 0, tmp, 2, addressParamBytes.Length);

            // Copy address seed bytes
            Buffer.BlockCopy(addressSeedBytesBigEndian, 0, tmp, 2 + addressParamBytes.Length, addressSeedBytesBigEndian.Length);

            // Compute Blake2b hash
            var blake2bHash = new HMACBlake2B(32);
            blake2bHash.Initialize();
            byte[] hash = blake2bHash.ComputeHash(tmp);

            //// Convert to hex and normalize
            Accounts.AccountAddress address = new Accounts.AccountAddress(hash);

            return address;
        }
    }
}