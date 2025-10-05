using System.Collections.Generic;
using System.Numerics;
using OpenDive.Crypto.PoseidonLite;
using OpenDive.Crypto.PoseidonLite.Constants;

public class Poseidon3
{
    public static BigInteger Hash(object[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c3 = new Dictionary<string, object> { ["C"] = C3.C, ["M"] = C3.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c3);
        // IRVIN: Explicitly constraint it to return the first value of the hash
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }

    public static BigInteger Hash(BigInteger[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c3 = new Dictionary<string, object> { ["C"] = C3.C, ["M"] = C3.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c3);
        // IRVIN: Explicitly constraint it to return the first value of the hash
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }
}
