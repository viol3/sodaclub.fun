namespace Sui.ZKLogin.SDK
{
    using System;
    using System.Text;
    using System.Numerics;
    using System.Threading.Tasks;
    using Sui.Cryptography;
    using Sui.Cryptography.Ed25519;

    /// <summary>
    /// TODO: Implement ZKLoginPublicKey.
    /// TODO: See how this is used and whether it can just be extended from the core PublicKey cass
    /// </summary>
    public class ZkLoginPublicKey : PublicKey
    {
        public ZkLoginPublicKey(byte[] public_key) : base(public_key)
        {
        }

        public ZkLoginPublicKey(string public_key) : base(public_key)
        {
        }

        //private byte[] data;
        //private SuiGraphQLClient client;

        //public class ZkLoginOptions
        //{
        //    public SuiGraphQLClient Client { get; set; }
        //}

        ///// <summary>
        ///// Create a new ZkLoginPublicIdentifier object
        ///// </summary>
        ///// <param name="value">zkLogin public identifier as byte array or base-64 encoded string</param>
        ///// <param name="options">Optional client configuration</param>
        //public ZkLoginPublicKey(object value, ZkLoginOptions options = null)
        //{
        //    this.client = options?.Client;

        //    if (value is string strValue)
        //    {
        //        this.data = Convert.FromBase64String(strValue);
        //    }
        //    else if (value is byte[] byteValue)
        //    {
        //        this.data = byteValue;
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Unsupported value type");
        //    }
        //}

        /// <summary>
        /// Checks if two zkLogin public identifiers are equal
        /// </summary>
        //public override bool Equals(PublicKey publicKey)
        //{
        //    return base.Equals(publicKey);
        //}

        ///// <summary>
        ///// Return the byte array representation of the zkLogin public identifier
        ///// </summary>
        //public override byte[] ToRawBytes()
        //{
        //    return data;
        //}

        /// <summary>
        /// Return the Sui address associated with this ZkLogin public identifier
        /// </summary>
        public int Flag()
        {
            return SignatureSchemeToFlag.ZkLogin;
        }

        /// <summary>
        /// Verifies that the signature is valid for the provided message
        /// </summary>
        public async Task<bool> Verify(byte[] message, object signature)
        {
            throw new Exception("does not support");
        }

        /// <summary>
        /// Verifies that the signature is valid for the provided PersonalMessage
        /// </summary>
        public async Task<bool> VerifyPersonalMessage(byte[] message, object signature)
        {
            //var parsedSignature = ParseSerializedZkLoginSignature(signature);
            //string address = new ZkLoginPublicIdentifier(parsedSignature.PublicKey).ToSuiAddress();

            //return await GraphqlVerifyZkLoginSignature(new VerifyZkLoginSignatureRequest
            //{
            //    Address = address,
            //    Bytes = Convert.ToBase64String(message),
            //    Signature = parsedSignature.SerializedSignature,
            //    IntentScope = "PERSONAL_MESSAGE",
            //    Client = this.client
            //});
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the signature is valid for the provided Transaction
        /// </summary>
        public async Task<bool> VerifyTransaction(byte[] transaction, object signature)
        {
            //var parsedSignature = ParseSerializedZkLoginSignature(signature);
            //string address = new ZkLoginPublicIdentifier(parsedSignature.PublicKey).ToSuiAddress();

            //return await GraphqlVerifyZkLoginSignature(new VerifyZkLoginSignatureRequest
            //{
            //    Address = address,
            //    Bytes = Convert.ToBase64String(transaction),
            //    Signature = parsedSignature.SerializedSignature,
            //    IntentScope = "TRANSACTION_DATA",
            //    Client = this.client
            //});
            throw new NotImplementedException();
        }

        /// <summary>
        /// Derive the public identifier for zklogin based on address seed and iss.
        /// </summary>
        public static ZkLoginPublicKey ToZkLoginPublicIdentifier(
            BigInteger addressSeed,
            //string iss,
            string iss
            //ZkLoginOptions options = null
        )
        {
            // Consists of iss_bytes_len || iss_bytes || padded_32_byte_address_seed.
            byte[] addressSeedBytesBigEndian = ZKLogin.SDK.Utils.ToPaddedBigEndianBytes(addressSeed, 32);
            byte[] issBytes = Encoding.UTF8.GetBytes(iss);

            byte[] tmp = new byte[1 + issBytes.Length + addressSeedBytesBigEndian.Length];
            tmp[0] = (byte)issBytes.Length;
            Buffer.BlockCopy(issBytes, 0, tmp, 1, issBytes.Length);
            Buffer.BlockCopy(addressSeedBytesBigEndian, 0, tmp, 1 + issBytes.Length, addressSeedBytesBigEndian.Length);

            //return new ZkLoginPublicKey(tmp, options);
            return new ZkLoginPublicKey(tmp);
        }
    }

    public class VerifyZkLoginSignatureRequest
    {
        public string Address { get; set; }
        public string Bytes { get; set; }
        public string Signature { get; set; }
        public string IntentScope { get; set; }
        //public SuiGraphQLClient Client { get; set; }
    }

    public class ParsedSignature
    {
        public string SerializedSignature { get; set; }
        public string SignatureScheme { get; set; }
        public ZkLoginInfo ZkLogin { get; set; }
        public byte[] Signature { get; set; }
        public byte[] PublicKey { get; set; }
    }

    public class ZkLoginInfo
    {
        public ZkLoginInputs Inputs { get; set; }
        public long MaxEpoch { get; set; }
        public string UserSignature { get; set; }
        public string Iss { get; set; }
        public BigInteger AddressSeed { get; set; }
    }

    public class ZkLoginInputs
    {
        public string IssBase64Details { get; set; }
        public string AddressSeed { get; set; }
    }

    // Helper methods
    public static class ZkLoginHelpers
    {
        public static async Task<bool> GraphqlVerifyZkLoginSignature(VerifyZkLoginSignatureRequest request)
        {
            //if (request.Client == null)
            //{
            //    request.Client = new SuiGraphQLClient("https://sui-mainnet.mystenlabs.com/graphql");
            //}

            //var response = await request.Client.Query(new
            //{
            //    query = "query Zklogin($bytes: Base64!, $signature: Base64!, $intentScope: ZkLoginIntentScope!, $author: SuiAddress!) { verifyZkloginSignature(bytes: $bytes, signature: $signature, intentScope: $intentScope, author: $author) { success errors } }",
            //    variables = new
            //    {
            //        bytes = request.Bytes,
            //        signature = request.Signature,
            //        intentScope = request.IntentScope,
            //        author = request.Address
            //    }
            //});

            //// Assuming response has similar structure to TypeScript version
            //return response.Data?.VerifyZkloginSignature?.Success == true &&
            //       (response.Data?.VerifyZkloginSignature?.Errors?.Length ?? 1) == 0;

            throw new NotImplementedException();
        }

        public static ParsedSignature ParseSerializedZkLoginSignature(object signature)
        {
            byte[] bytes;
            if (signature is string strSig)
            {
                bytes = Convert.FromBase64String(strSig);
            }
            else if (signature is byte[] byteSig)
            {
                bytes = byteSig;
            }
            else
            {
                throw new ArgumentException("Invalid signature format");
            }

            if (bytes[0] != SignatureSchemeToFlag.ZkLogin)
            {
                throw new Exception("Invalid signature scheme");
            }

            byte[] signatureBytes = new byte[bytes.Length - 1];
            Buffer.BlockCopy(bytes, 1, signatureBytes, 0, bytes.Length - 1);

            //var parsedSig = ParseZkLoginSignature(signatureBytes);
            //string iss = JwtUtils.ExtractClaimValue<string>(parsedSig.Inputs.IssBase64Details, "iss");

            //var publicIdentifier = ZkLoginPublicKey.ToZkLoginPublicIdentifier(
            //    BigInteger.Parse(parsedSig.Inputs.AddressSeed),
            //    iss
            //);

            //return new ParsedSignature
            //{
            //    SerializedSignature = Convert.ToBase64String(bytes),
            //    SignatureScheme = "ZkLogin",
            //    ZkLogin = new ZkLoginInfo
            //    {
            //        Inputs = parsedSig.Inputs,
            //        MaxEpoch = parsedSig.MaxEpoch,
            //        UserSignature = parsedSig.UserSignature,
            //        Iss = iss,
            //        AddressSeed = BigInteger.Parse(parsedSig.Inputs.AddressSeed)
            //    },
            //    Signature = bytes,
            //    //PublicKey = publicIdentifier.ToRawBytes()
            //    PublicKey = publicIdentifier.KeyBytes
            //};

            throw new NotImplementedException();
        }
    }

}