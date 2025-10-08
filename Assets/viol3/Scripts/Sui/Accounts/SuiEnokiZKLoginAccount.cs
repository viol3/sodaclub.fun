using ec33.SuiWorks.Transactions;
using Newtonsoft.Json;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Rpc.Client;
using Sui.Rpc.Models;
using Sui.Transactions;
using Sui.ZKLogin;
using Sui.ZKLogin.Enoki;
using Sui.ZKLogin.Enoki.Utils;
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
            EnokiZKLoginSaveableData accountData = EnokiZKLogin.GetSaveableData();
            return JsonConvert.SerializeObject(accountData);
        }

        public void Deserialize(string data)
        {
            EnokiZKLoginSaveableData accountData = JsonConvert.DeserializeObject<EnokiZKLoginSaveableData>(data);
            EnokiZKLogin.LoadZKLoginUser(accountData.loginUserResponse);
            EnokiZKLogin.LoadZKPResponse(accountData.zkpResponse);
            EnokiZKLogin.LoadEphemeralKey(new Account(accountData.ephemeralPrivateKeyHex));
            EnokiZKLogin.LoadMaxEpoch(accountData.maxEpoch);
        }

        public async Task<RpcResult<TransactionBlockResponse>> SignAndExecuteTransaction(TransactionBlock txBlock, TransactionBlockResponseOptions opts = null)
        {
            return await EnokiZKLogin.SignAndExecuteTransactionBlock(txBlock);
        }

    }

}

