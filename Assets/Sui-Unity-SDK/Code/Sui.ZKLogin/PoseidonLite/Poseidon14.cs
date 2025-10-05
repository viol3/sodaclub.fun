using System.Collections.Generic;
using System.Numerics;
using OpenDive.Crypto.PoseidonLite;
using OpenDive.Crypto.PoseidonLite.Constants;

public class Poseidon14
{
    public static BigInteger Hash(object[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c14 = new Dictionary<string, object> { ["C"] = C14.C, ["M"] = C14.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c14);
        // IRVIN: Explicitly constraint it to return the first value of the hash
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }

    public static BigInteger Hash(BigInteger[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c14 = new Dictionary<string, object> { ["C"] = C14.C, ["M"] = C14.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c14);
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }
}
