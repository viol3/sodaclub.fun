using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace OpenDive.Crypto.PoseidonLite
{
    /// <summary>
    /// TODO: Add tests for BigIntUnstringier
    /// </summary>
    public static class BigIntUnstringifier
    {
        /// <summary>
        /// TODO: Add documentation
        /// </summary>
        /// <param name="base64Str"></param>
        /// <returns></returns>
        private static BigInteger ConvertBase64ToBigInt(string base64Str)
        {
            byte[] byteArray = Convert.FromBase64String(base64Str);
            string hex = BitConverter.ToString(byteArray).Replace("-", "").ToLower();
            return BigInteger.Parse("0" + hex, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// TODO: Add documentation
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, object> UnstringifyBigInts(Dictionary<string, object> input)
        {
            var result = new Dictionary<string, object>();

            foreach (var kvp in input)
            {
                if (kvp.Key == "C")
                {
                    // Handle C array
                    var cArray = (IEnumerable<object>)kvp.Value;
                    result[kvp.Key] = cArray
                        .Select(item => ConvertBase64ToBigInt((string)item))
                        //.ToList(); // TODO: This is the initial issue-- we are converting to List instead of array.
                        .ToArray();
                }
                else if (kvp.Key == "M")
                {
                    // Handle M array (list of lists)
                    var mArray = (IEnumerable<object>)kvp.Value;
                    result[kvp.Key] = mArray
                        .Select(subArray => ((IEnumerable<object>)subArray)
                            .Select(item => ConvertBase64ToBigInt((string)item))
                            //.ToList()) // TODO: This is the initial issue -- we are converting to List instead of array.
                            .ToArray())
                        //.ToList();
                        .ToArray();
                }
            }

            return result;
        }

        //public static object UnstringifyBigInts(object input)
        //{
        //    if (input is string[] array)
        //    {
        //        return array.Select(UnstringifyBigInts).ToArray();
        //    }
        //    else if (input is Dictionary<string, object> dict)
        //    {
        //        return dict.ToDictionary(
        //            kvp => kvp.Key,
        //            kvp => UnstringifyBigInts(kvp.Value)
        //        );
        //    }
        //    else if (input is string str)
        //    {
        //        byte[] byteArray = Convert.FromBase64String(str);
        //        string hex = BitConverter.ToString(byteArray).Replace("-", "").ToLower();
        //        return BigInteger.Parse($"0x{hex}", System.Globalization.NumberStyles.HexNumber);
        //    }

        //    return input;
        //}

        //public static BigInteger[] UnstringifyBigInts(string[] base64Strings)
        //{
        //    return base64Strings.Select(str =>
        //    {
        //        byte[] byteArray = Convert.FromBase64String(str);
        //        string hex = BitConverter.ToString(byteArray).Replace("-", "").ToLower();
        //        return BigInteger.Parse($"0x{hex}", System.Globalization.NumberStyles.HexNumber);
        //    }).ToArray();
        //}

        //public static BigInteger UnstringifyBigInts(string[] o)
        //{
        //    if (o is object[] array)
        //    {
        //        return array.Select(UnstringifyBigInts).ToArray();
        //    }
        //    else if (o is Dictionary<string, object> dict)
        //    {
        //        return dict.ToDictionary(
        //            kvp => kvp.Key,
        //            kvp => UnstringifyBigInts(kvp.Value)
        //        );
        //    }
        //    else if (o is string str)
        //    {
        //        byte[] byteArray = Convert.FromBase64String(str);
        //        string hex = BitConverter.ToString(byteArray).Replace("-", "").ToLower();
        //        return BigInteger.Parse($"0x{hex}", System.Globalization.NumberStyles.HexNumber);
        //    }

        //    return o;
        //}
    }
}