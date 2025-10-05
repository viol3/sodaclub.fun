using System.Collections.Generic;
using System.Numerics;
using OpenDive.Crypto.PoseidonLite;
using OpenDive.Crypto.PoseidonLite.Constants;

public class Poseidon9
{
    public static BigInteger Hash(object[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c9 = new Dictionary<string, object> { ["C"] = C9.C, ["M"] = C9.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c9);
        // IRVIN: Explicitly constraint it to return the first value of the hash
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }

    public static BigInteger Hash(BigInteger[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c9 = new Dictionary<string, object> { ["C"] = C9.C, ["M"] = C9.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c9);
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }
}
