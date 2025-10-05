using System.Collections.Generic;
using System.Numerics;
using OpenDive.Crypto.PoseidonLite;
using OpenDive.Crypto.PoseidonLite.Constants;

public class Poseidon13
{
    public static BigInteger Hash(object[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c13 = new Dictionary<string, object> { ["C"] = C13.C, ["M"] = C13.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c13);
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }

    public static BigInteger Hash(BigInteger[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c13 = new Dictionary<string, object> { ["C"] = C13.C, ["M"] = C13.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c13);
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }
}
