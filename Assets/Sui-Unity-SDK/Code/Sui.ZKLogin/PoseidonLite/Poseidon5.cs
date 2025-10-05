using System.Collections.Generic;
using System.Numerics;
using OpenDive.Crypto.PoseidonLite;
using OpenDive.Crypto.PoseidonLite.Constants;

public class Poseidon5
{
    public static BigInteger Hash(object[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c5 = new Dictionary<string, object> { ["C"] = C5.C, ["M"] = C5.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c5);
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }

    public static BigInteger Hash(BigInteger[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c5 = new Dictionary<string, object> { ["C"] = C5.C, ["M"] = C5.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c5);
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }
}
