using viol3.SuiWorks.Transactions;
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

namespace viol3.SuiWorks.Accounts
{
    
    public class SuiLocalAccount : ISuiAccount
    {
        protected Account _account;
        protected SuiClient _client;

        public string GetPrivateKey()
        {
            return _account.PrivateKey.ToHex();
        }

        public string GetSuiAddress()
        {
            return _account.SuiAddress().ToHex();
        }

        public SuiAccountType GetAccountType()
        {
            return SuiAccountType.Local;
        }

        public Account GetAccount()
        {
            return _account;
        }

        public string Serialize()
        {
            return GetPrivateKey();
        }

        public void Deserialize(string data)
        {
            _account = new Account(data);
        }
    }
}

