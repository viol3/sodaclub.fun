using System.Collections.Generic;
using System.Numerics;
using OpenDive.Crypto.PoseidonLite;
using OpenDive.Crypto.PoseidonLite.Constants;

public class Poseidon12
{
    public static BigInteger Hash(object[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c12 = new Dictionary<string, object> { ["C"] = C12.C, ["M"] = C12.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c12);
        // IRVIN: Explicitly constraint it to return the first value of the hash
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }

    public static BigInteger Hash(BigInteger[] inputs, int nOuts = 1)
    {
        Dictionary<string, object> c12 = new Dictionary<string, object> { ["C"] = C12.C, ["M"] = C12.M };
        Dictionary<string, object> c = BigIntUnstringifier.UnstringifyBigInts(c12);
        // IRVIN: Explicitly constraint it to return the first value of the hash
        return Poseidon.Hash(inputs, c, nOuts)[0];
    }
}
