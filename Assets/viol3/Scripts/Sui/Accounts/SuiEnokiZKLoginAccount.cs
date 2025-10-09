using viol3.SuiWorks.Transactions;
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

namespace viol3.SuiWorks.Accounts
{
    public class SuiEnokiZKLoginAccount : ISuiAccount
    {
        protected Account _ephemeralAccount;

        public string GetPrivateKey()
        {
            return "[ZKLOGIN]";
        }

        public string GetSuiAddress()
        {
            return EnokiZKLogin.GetSuiAddress();
        }

        public SuiAccountType GetAccountType()
        {
            return SuiAccountType.EnokiZKLogin;
        }

        public Account GetAccount()
        {
            return EnokiZKLogin.GetEphemeralAccount();
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

    }

}

