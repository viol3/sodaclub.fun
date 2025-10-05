using ec33.SuiWorks.Transactions;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Rpc.Client;
using Sui.Rpc.Models;
using Sui.Transactions;
using System.Numerics;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;

namespace ec33.SuiWorks.Accounts
{
    public interface ISuiAccount
    {
        public Task<BigInteger> GetBalanceAsync();
        public string GetSuiAddress();
        public string GetPrivateKey();
        public void LoadClient(SuiClient client);
        public string Serialize();
        public void Deserialize(string data);
        public Task<RpcResult<TransactionBlockResponse>> SignAndExecuteTransaction(TransactionBlock txBlock, TransactionBlockResponseOptions opts = null);
    }
}

