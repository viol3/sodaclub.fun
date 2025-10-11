using Sui.Accounts;

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

