using ec33.SuiWorks.Transactions;
using Newtonsoft.Json;
using Sui.Accounts;
using Sui.Cryptography;
using Sui.Cryptography.Ed25519;
using Sui.Rpc;
using Sui.Rpc.Client;
using Sui.Rpc.Models;
using Sui.Transactions;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

namespace ec33.SuiWorks.Accounts
{
    
    public class SuiLocalAccount : ISuiAccount
    {
        protected Account _account;
        protected SuiClient _client;

        public async Task<BigInteger> GetBalanceAsync()
        {
            RpcResult<Balance> result = await _client.GetBalanceAsync(_account);
            return result.Result.TotalBalance;
        }

        public string GetPrivateKey()
        {
            return _account.PrivateKey.ToHex();
        }

        public string GetSuiAddress()
        {
            return _account.SuiAddress().ToHex();
        }

        public void LoadClient(SuiClient client)
        {
            _client = client;
        }

        public string Serialize()
        {
            return GetPrivateKey();
        }

        public void Deserialize(string data)
        {
            _account = new Account(data);
        }

        public async Task<RpcResult<TransactionBlockResponse>> SignAndExecuteTransaction(TransactionBlock txBlock, TransactionBlockResponseOptions opts = null)
        {
            return await _client.SignAndExecuteTransactionBlockAsync(txBlock, _account, opts);
        }
    }
}

