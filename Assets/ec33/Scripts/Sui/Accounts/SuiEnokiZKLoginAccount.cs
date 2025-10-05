using ec33.SuiWorks.Transactions;
using Newtonsoft.Json;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Rpc.Client;
using Sui.Rpc.Models;
using Sui.Transactions;
using Sui.ZKLogin;
using Sui.ZKLogin.Utils;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

namespace ec33.SuiWorks.Accounts
{
    public class SuiEnokiZKLoginAccount : ISuiAccount
    {
        protected Account _ephemeralAccount;
        protected SuiClient _client;

        public async Task<BigInteger> GetBalanceAsync()
        {
            RpcResult<Balance> result = await _client.GetBalanceAsync(Sui.ZKLogin.AccountAddress.FromHex(EnokiZKLogin.GetSuiAddress()));
            return result.Result.TotalBalance;
        }

        public string GetPrivateKey()
        {
            return "[ZKLOGIN]";
        }

        public string GetSuiAddress()
        {
            return EnokiZKLogin.GetSuiAddress();
        }

        public void LoadClient(SuiClient client)
        {
            _client = client;
        }

        public string Serialize()
        {
            SuiZKLoginAccountData accountData = new SuiZKLoginAccountData();
            accountData.enokiZKLoginUser = EnokiZKLogin.GetZKLoginUser();
            accountData.enokiZKPResponse = EnokiZKLogin.GetZKP();
            accountData.ephemeralPrivateKey = EnokiZKLogin.GetEphemeralAccount().PrivateKey.ToHex();
            accountData.maxEpoch = EnokiZKLogin.GetMaxEpoch();
            return JsonConvert.SerializeObject(accountData);
        }

        public void Deserialize(string data)
        {
            SuiZKLoginAccountData accountData = JsonConvert.DeserializeObject<SuiZKLoginAccountData>(data);
            EnokiZKLogin.LoadZKLoginUser(accountData.enokiZKLoginUser);
            EnokiZKLogin.LoadZKPResponse(accountData.enokiZKPResponse);
            EnokiZKLogin.LoadEphemeralKey(new Account(accountData.ephemeralPrivateKey));
            EnokiZKLogin.LoadMaxEpoch(accountData.maxEpoch);
        }

        public async Task<RpcResult<TransactionBlockResponse>> SignAndExecuteTransaction(TransactionBlock txBlock, TransactionBlockResponseOptions opts = null)
        {
            return await EnokiZKLogin.SignAndExecuteTransactionBlock(txBlock);
        }

    }

    public class SuiZKLoginAccountData
    {
        public EnokiZKLoginUser enokiZKLoginUser;
        public EnokiZKPResponse enokiZKPResponse;
        public string ephemeralPrivateKey;
        public int maxEpoch;
    }
}

