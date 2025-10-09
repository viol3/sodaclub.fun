using viol3.SuiWorks.Transactions;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Rpc.Client;
using Sui.Rpc.Models;
using Sui.Transactions;
using System.Numerics;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;

namespace viol3.SuiWorks.Accounts
{
    public interface ISuiAccount
    {
        public string GetSuiAddress();
        public string GetPrivateKey();
        public SuiAccountType GetAccountType();
        public Account GetAccount();
        public string Serialize();
        public void Deserialize(string data);
    }

    public enum SuiAccountType
    {
       Local, EnokiZKLogin
    }
}

