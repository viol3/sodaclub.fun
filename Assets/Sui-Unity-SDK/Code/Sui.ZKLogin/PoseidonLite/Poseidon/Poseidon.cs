using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Org.BouncyCastle.Utilities;
using UnityEngine;

namespace OpenDive.Crypto.PoseidonLite
{
    public static class Poseidon
    {
        private static readonly BigInteger F = BigInteger.Parse("21888242871839275222246405745257275088548364400416034343698204186575808495617");
        private static readonly int N_ROUNDS_F = 8;
        private static readonly int[] N_ROUNDS_P = { 56, 57, 56, 60, 60, 63, 64, 63, 60, 66, 60, 65, 70, 60, 64, 68 };

        /// <summary>
        /// Power of 5, module big F.
        /// Consider here that BigInt precision matters.
        /// In TypeScript the tests should create a BigInt using quotation marks
        /// so that the input is considered a `BigInt` versus a `number`.
        /// For example:
        /// <code>
        /// // First interpreted as a number (a double-precision 64-bit floating-point value, then BigInt
        /// BigInt(915937356510258724)
        /// // Directly converted to BigInt, bypassing any intermediate number representation
        /// BigInt("915937356510258724")
        /// </code>
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static BigInteger Pow5(BigInteger v)
        {
            BigInteger o = v * v;       // Compute v^2
            return (v * o * o) % F;     // Compute v^5 % F
        }

        private static BigInteger[] Mix(BigInteger[] state, BigInteger[][] M)
        {
            var result = new BigInteger[state.Length];
            for (int x = 0; x < state.Length; x++)
            {
                BigInteger sum = 0;
                for (int y = 0; y < state.Length; y++)
                {
                    BigInteger tempM = M[x][y];
                    BigInteger tempState = state[y];

                    //sum = (sum + M[x][y] * state[y]) % F;
                    sum = (sum + tempM * tempState) % F;
                }
                result[x] = sum;
            }
            return result;
        }

        /// <summary>
        /// Runs Poseidon hash for give number of rounds
        /// </summary>
        /// <param name="inputs">Can be BigInteger, int, long or string</param>
        /// <param name="opt">"Unstringified BigInts</param>
        /// <param name="nOuts"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static BigInteger[] Hash(
            object[] inputs,
            //object opt,
            Dictionary<string, object> opt,
            int nOuts = 1
        )
        {
            BigInteger[] bigIntInputs = inputs.Select(i => i switch
            {
                BigInteger bi => bi,
                int n => new BigInteger(n),
                string s => BigInteger.Parse(s),
                _ => throw new ArgumentException($"Invalid input type: {i.GetType()}")
            }).ToArray();

            if (bigIntInputs.Length <= 0)
                throw new ArgumentException("Not enough inputs");
            if (bigIntInputs.Length > N_ROUNDS_P.Length)
                throw new ArgumentException("Too many inputs");

            int t = bigIntInputs.Length + 1;
            int nRoundsF = N_ROUNDS_F;
            int nRoundsP = N_ROUNDS_P[t - 2];

            // IRVIN: Access C array
            //var cArray = (List<BigInteger>)opt["C"];
            var cArray = (BigInteger[])opt["C"];

            // IRVIN: Access M array (list of lists)
            //var mArray = (List<List<BigInteger>>)opt["M"];
            var mArray = (BigInteger[][])opt["M"];

            if ( mArray.Length != t)
                throw new ArgumentException($"Incorrect M length, expected {t} got {mArray.Length}");

            var state = new BigInteger[t];
            state[0] = 0;
            Array.Copy(bigIntInputs, 0, state, 1, bigIntInputs.Length);

            for (int x = 0; x < nRoundsF + nRoundsP; x++)
            {
                for (int y = 0; y < state.Length; y++)
                {
                    //state[y] = (state[y] + new BigInteger(Convert.FromBase64String(cArray[x * t + y]))) % F;
                    state[y] = (state[y] + cArray[x * t + y]) % F; // IRVIN: Assuming cArray is BigInteger already
                    if (x < nRoundsF / 2 || x >= nRoundsF / 2 + nRoundsP)
                        state[y] = Pow5(state[y]);
                    else if (y == 0)
                        state[y] = Pow5(state[y]);
                }
                //state = Mix(state, mArray.Select(row =>
                //    row.Select(s => new BigInteger(Convert.FromBase64String(s))).ToArray()
                //).ToArray());
                state = Mix(state, mArray);
            }

            if (nOuts == 1)
                return new[] { state[0] };
            if (nOuts <= state.Length)
                return state.Take(nOuts).ToArray();

            throw new ArgumentException($"Invalid number of outputs requested {nOuts}, max {state.Length}");
        }

        public static BigInteger[] Hash(
            BigInteger[] inputs,
            //object opt,
            Dictionary<string, object> opt,
            int nOuts = 1
        )
        {
            //var bigIntInputs = inputs.Select(i => i switch
            //{
            //    BigInteger bi => bi,
            //    int n => new BigInteger(n),
            //    string s => BigInteger.Parse(s),
            //    _ => throw new ArgumentException($"Invalid input type: {i.GetType()}")
            //}).ToArray();
            BigInteger[] bigIntInputs = inputs;

            if (bigIntInputs.Length <= 0)
                throw new ArgumentException("Not enough inputs");
            if (bigIntInputs.Length > N_ROUNDS_P.Length)
                throw new ArgumentException("Too many inputs");

            int t = bigIntInputs.Length + 1;
            int nRoundsF = N_ROUNDS_F;
            int nRoundsP = N_ROUNDS_P[t - 2];

            // IRVIN: Access C array
            //var cArray = (List<BigInteger>)opt["C"];
            string typeStr = opt["C"].GetType().ToString();

            var cArray = (BigInteger[])opt["C"];

            // IRVIN: Access M array (list of lists)
            //var mArray = (List<List<BigInteger>>)opt["M"];
            var mArray = (BigInteger[][])opt["M"];

            if (mArray.Length != t)
                throw new ArgumentException($"Incorrect M length, expected {t} got {mArray.Length}");

            var state = new BigInteger[t];
            state[0] = 0;
            Array.Copy(bigIntInputs, 0, state, 1, bigIntInputs.Length);

            for (int x = 0; x < nRoundsF + nRoundsP; x++)
            {
                for (int y = 0; y < state.Length; y++)
                {
                    //state[y] = (state[y] + new BigInteger(Convert.FromBase64String(cArray[x * t + y]))) % F;
                    state[y] = (state[y] + cArray[x * t + y]) % F; // IRVIN: Assuming cArray is BigInteger already
                    if (x < nRoundsF / 2 || x >= nRoundsF / 2 + nRoundsP)
                        state[y] = Pow5(state[y]);
                    else if (y == 0)
                        state[y] = Pow5(state[y]);
                }
                //state = Mix(state, mArray.Select(row =>
                //    row.Select(s => new BigInteger(Convert.FromBase64String(s))).ToArray()
                //).ToArray());
                state = Mix(state, mArray);
            }

            if (nOuts == 1)
                return new[] { state[0] };
            if (nOuts <= state.Length)
                return state.Take(nOuts).ToArray();

            throw new ArgumentException($"Invalid number of outputs requested {nOuts}, max {state.Length}");
        }
    }
}