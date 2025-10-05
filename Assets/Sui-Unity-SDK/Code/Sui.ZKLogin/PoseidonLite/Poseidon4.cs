using System.Collections.Generic;
using System.Numerics;
using OpenDive.Crypto.PoseidonLite;
using OpenDive.Crypto.PoseidonLite.Constants;

public class Poseidon4
{
    public static BigInteger Hash(object[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c4 = new Dictionary<string, object> { ["C"] = C4.C, ["M"] = C4.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c4);
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }

    public static BigInteger Hash(BigInteger[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c4 = new Dictionary<string, object> { ["C"] = C4.C, ["M"] = C4.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c4);
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }
}
