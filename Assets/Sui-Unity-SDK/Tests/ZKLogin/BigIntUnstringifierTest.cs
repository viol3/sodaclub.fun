using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using OpenDive.Crypto.PoseidonLite.Constants;
using OpenDive.Crypto.PoseidonLite;
using System.Numerics;

namespace Sui.Tests.ZkLogin
{
    public class BigIntUnstringifierTest : MonoBehaviour
    {
        [Test]
        public void UnstrinifyBigInt1()
        {
            var constant1 = new Dictionary<string, object>
            {
                ["C"] = C1.C,
                ["M"] = C1.M
            };

            var result = BigIntUnstringifier.UnstringifyBigInts(constant1);

            // Access C array
            var cArray = (List<BigInteger>)result["C"];

            // Access M array (list of lists)
            var mArray = (List<List<BigInteger>>)result["M"];
        }
    }

}