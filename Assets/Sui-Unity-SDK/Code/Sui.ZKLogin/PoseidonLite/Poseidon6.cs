using System.Collections.Generic;
using System.Numerics;
using OpenDive.Crypto.PoseidonLite;
using OpenDive.Crypto.PoseidonLite.Constants;

public class Poseidon6
{
    public static BigInteger Hash(object[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c6 = new Dictionary<string, object> { ["C"] = C6.C, ["M"] = C6.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c6);
        // IRVIN: Explicitly constraint it to return the first value of the hash
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }

    public static BigInteger Hash(BigInteger[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c6 = new Dictionary<string, object> { ["C"] = C6.C, ["M"] = C6.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c6);
        // IRVIN: Explicitly constraint it to return the first value of the hash
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }
}
