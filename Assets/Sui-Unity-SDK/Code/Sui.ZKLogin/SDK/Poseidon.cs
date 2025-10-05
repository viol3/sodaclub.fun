//
//  Poseidon.cs
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
using System.Linq;
using UnityEngine;

namespace Sui.ZKLogin.SDK
{
    public static class PoseidonHasher
    {
        public static readonly BigInteger BN254_FIELD_SIZE = BigInteger.Parse("21888242871839275222246405745257275088548364400416034343698204186575808495617");

        private static readonly Func<BigInteger[], int, BigInteger>[] PoseidonNumToHashFN = {
            Poseidon1.Hash,
            Poseidon2.Hash,
            Poseidon3.Hash,
            Poseidon4.Hash,
            Poseidon5.Hash,
            Poseidon6.Hash,
            Poseidon7.Hash,
            Poseidon8.Hash,
            Poseidon9.Hash,
            Poseidon10.Hash,
            Poseidon11.Hash,
            Poseidon12.Hash,
            Poseidon13.Hash,
            Poseidon14.Hash,
            Poseidon15.Hash,
            Poseidon16.Hash,
        };

        /// <summary>
        /// Runs Poseidon Hash.
        /// The inputs can either be a int, long, or string array.
        /// NOTE that a long is also consider a BigInteger but explicitly cast it.
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static BigInteger PoseidonHash(object[] inputs)
        {
            var bigIntInputs = inputs.Select(x => {
                var b = ToBigInteger(x);
                if (b < 0 || b >= BN254_FIELD_SIZE)
                    throw new ArgumentException($"Element {b} not in the BN254 field");
                return b;
            }).ToArray();

            // Call Poseidon hash according to the length of the input
            if (bigIntInputs.Length <= 16 && PoseidonNumToHashFN.Length >= bigIntInputs.Length)
            {
                return PoseidonNumToHashFN[bigIntInputs.Length - 1](bigIntInputs, 1);
            }
            else if (bigIntInputs.Length <= 32) // If it's a small length input
            {
                var hash1 = PoseidonHash(bigIntInputs.Take(16).Cast<object>().ToArray());
                var hash2 = PoseidonHash(bigIntInputs.Skip(16).Cast<object>().ToArray());
                return PoseidonHash(new object[] { hash1, hash2 });
            }

            throw new ArgumentException($"Unable to hash a vector of length {bigIntInputs.Length}");
        }

        public static BigInteger PoseidonHash(BigInteger[] inputs)
        {
            Debug.Log("=========: bigIntInputs Length: " + inputs.Length);

            // Call Poseidon hash according to the length of the input
            if (inputs.Length <= 16 && PoseidonNumToHashFN.Length >= inputs.Length)
            {
                Debug.Log("!!!!!!!!!!");
                return PoseidonNumToHashFN[inputs.Length - 1](inputs, 1);
            }
            else if (inputs.Length <= 32) // If it's a small length input
            {
                Debug.Log("*************");
                var hash1 = PoseidonHash(inputs.Take(16).Cast<object>().ToArray());
                var hash2 = PoseidonHash(inputs.Skip(16).Cast<object>().ToArray());
                return PoseidonHash(new BigInteger[] { hash1, hash2 });
            }

            throw new ArgumentException($"Unable to hash a vector of length {inputs.Length}");
        }

        /// <summary>
        /// Utility function to parse either an int, long, or string into a BigInteger
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static BigInteger ToBigInteger(object input)
        {
            return input switch
            {
                BigInteger bi => bi,
                int i => new BigInteger(i),
                long l => new BigInteger(l),
                string s => BigInteger.Parse(s),
                _ => throw new ArgumentException($"Unsupported input type: {input.GetType()}")
            };
        }
    }
}