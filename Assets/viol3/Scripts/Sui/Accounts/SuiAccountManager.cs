using viol3.SuiWorks.Transactions;
using NBitcoin;
using Newtonsoft.Json;
using Sui.Rpc;
using Sui.Rpc.Client;
using Sui.Rpc.Models;
using Sui.Transactions;
using Sui.ZKLogin.Enoki;
using System.Threading.Tasks;
using UnityEngine;
using Sui.ZKLogin.Enoki.Utils;
using Unity.VisualScripting;
using Sui.Cryptography;
using Sui.Accounts;
using System.Numerics;
using System.Threading;

namespace viol3.SuiWorks.Accounts
{
    public class SuiAccountManager
    {
        private static SuiAccountManager _instance;
        public static SuiAccountManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("SuiAccountManager is not initialized. Call SuiAccountManager.Initialize() first.");
                }
                return _instance;
            }
        }

        private const string ACCOUNT_PREF = "SUI_ACCOUNT_PREF";

        private ISuiAccount _currentAccount;
        private SuiClient _client;
        private TransactionKit _transactionKit;

        private TransactionBlockResponseOptions _opts;

        public static void Initialize(string network, string googleClientId = null, string enokiPublicKey = null, string redirectUri = null)
        {
            if (_instance != null)
            {
                Debug.LogWarning("SuiAccountManager has already been initialized.");
                return;
            }
            _instance = new SuiAccountManager(network, googleClientId, enokiPublicKey, redirectUri);
        }

        public static void Initialize(string network, SuiClient client, string googleClientId = null, string enokiPublicKey = null, string redirectUri = null)
        {
            if (_instance != null)
            {
                Debug.LogWarning("SuiAccountManager has already been initialized.");
                return;
            }
            _instance = new SuiAccountManager(network, client, googleClientId, enokiPublicKey, redirectUri);
        }


        public SuiAccountManager(string network, string googleClientId = null, string enokiPublicKey = null, string redirectUri = null)
        {
            if (_client != null)
            {
                Debug.Log("Sui client already created.");
                return;
            }
            switch (network)
            {
                case "mainnet":
                    _client = new SuiClient(Constants.MainnetConnection);
                    break;
                case "testnet":
                    _client = new SuiClient(Constants.TestnetConnection);
                    break;
                case "devnet":
                    _client = new SuiClient(Constants.DevnetConnection);
                    break;
                case "localnet":
                    _client = new SuiClient(Constants.LocalnetConnection);
                    break;
                default:
                    Debug.LogWarning($"Unknown network:{network}, creating client with testnet...");
                    _client = new SuiClient(Constants.TestnetConnection);
                    break;
            }
            EnokiZKLogin.Init("testnet", enokiPublicKey);
#if UNITY_WEBGL && !UNITY_EDITOR
        GoogleOAuthWebGLJwtFetcher googleOAuthWebGLJwtFetcher = new GameObject("GoogleOAuthWebGLJwtFetcher").AddComponent<GoogleOAuthWebGLJwtFetcher>();
        googleOAuthWebGLJwtFetcher.SetGoogleClientId(googleClientId);
        EnokiZKLogin.LoadJwtFetcher(googleOAuthWebGLJwtFetcher);
#else
            EnokiZKLogin.LoadJwtFetcher(new GoogleOAuthDesktopJwtFetcher(googleClientId, redirectUri));
#endif
            CreateTransactionBlockResponseOptionsInternal();
            LoadAccount();
        }

        public SuiAccountManager(string network, SuiClient client, string googleClientId = null, string enokiPublicKey = null, string redirectUri = null)
        {
            if (_client != null)
            {
                Debug.Log("Sui client already created.");
                return;
            }
            _client = client;
            EnokiZKLogin.Init("testnet", enokiPublicKey);
#if UNITY_WEBGL && !UNITY_EDITOR
        GoogleOAuthWebGLJwtFetcher googleOAuthWebGLJwtFetcher = new GameObject("GoogleOAuthWebGLJwtFetcher").AddComponent<GoogleOAuthWebGLJwtFetcher>();
        googleOAuthWebGLJwtFetcher.SetGoogleClientId(googleClientId);
        EnokiZKLogin.LoadJwtFetcher(googleOAuthWebGLJwtFetcher);
#else
            EnokiZKLogin.LoadJwtFetcher(new GoogleOAuthDesktopJwtFetcher(googleClientId, redirectUri));
#endif
            CreateTransactionBlockResponseOptionsInternal();
            LoadAccount();
        }

        private void LoadAccount()
        {
            string savedAccountData = PlayerPrefs.GetString(ACCOUNT_PREF);
            if (string.IsNullOrEmpty(savedAccountData))
            {
                return;
            }
            if(savedAccountData.StartsWith("0x"))
            {
                Debug.Log("local account loading...");
                _currentAccount = new SuiLocalAccount();
                _currentAccount.Deserialize(savedAccountData);
            }
            else
            {
                Debug.Log("zklogin account loading..");
                _currentAccount = new SuiEnokiZKLoginAccount();
                EnokiZKLoginSaveableData saveableData = JsonConvert.DeserializeObject<EnokiZKLoginSaveableData>(savedAccountData);
                _currentAccount.Deserialize(savedAccountData);
            }
        }

        public bool IsEnokiZKLogin()
        { 
            if(_currentAccount == null)
            {
                return false;
            }
            return _currentAccount.GetAccountType() == SuiAccountType.EnokiZKLogin;
        }

        public bool IsPrivateKeyValid(string privateKey)
        {
            try
            {
                Account account = new Account(privateKey);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetPrivateKey()
        {
            return _currentAccount.GetPrivateKey();
        }

        public string GetSuiAddress()
        {
            return _currentAccount.GetSuiAddress();
        }

        public bool IsLoggedIn()
        {
            return _currentAccount != null;
        }

        public async Task Validate()
        {
            if (_currentAccount == null)
            {
                return;
            }
            if(_currentAccount.GetAccountType() == SuiAccountType.Local)
            {
                return;
            }
            await EnokiZKLogin.ValidateMaxEpoch();
            if(!EnokiZKLogin.IsLogged())
            {
                Logout();
            }
        }

        public async Task<bool> LoginWithGoogle(CancellationToken cancellationToken)
        {
            
            try
            {
                EnokiZKPResponse response = await EnokiZKLogin.Login(cancellationToken);
                if (response == null) 
                {
                    return false;
                }
                EnokiZKLoginSaveableData saveableData = EnokiZKLogin.GetSaveableData();
                string serializedSaveableData = JsonConvert.SerializeObject(saveableData);
                PlayerPrefs.SetString(ACCOUNT_PREF, serializedSaveableData);
                _currentAccount = new SuiEnokiZKLoginAccount();
                _currentAccount.Deserialize(serializedSaveableData);
                return true;
            }
            catch(System.Exception ex)
            {
                MessageBox.Instance.Show("There was an error while logging.");
                Debug.LogError(ex.Message + "\r\n" + ex.StackTrace);
                return false;
            }
        }

        public bool LoginWithLocal(string privateKeyHex)
        {
            _currentAccount = new SuiLocalAccount();
            _currentAccount.Deserialize(privateKeyHex);
            PlayerPrefs.SetString(ACCOUNT_PREF, privateKeyHex);
            return true;
        }

        public void Logout()
        {
            if (!IsLoggedIn())
            {
                Debug.LogWarning("Cannot logout because not logged in yet.");
                return;
            }
            if (_currentAccount.GetAccountType() == SuiAccountType.EnokiZKLogin) 
            {
                EnokiZKLogin.Logout();
            }
            _currentAccount = null;
            PlayerPrefs.DeleteKey(ACCOUNT_PREF);
        }

        public string GeneratePrivateKey()
        {
            return new Account().PrivateKey.ToHex();
        }

        public void LoadTransactionKit(TransactionKit transactionKit)
        { 
            _transactionKit = transactionKit; 
        }

        public void LoadTransactionBlockResponseOptions(TransactionBlockResponseOptions opts)
        {
            _opts = opts;
        }

        private void CreateTransactionBlockResponseOptionsInternal()
        {
            TransactionBlockResponseOptions opts = new TransactionBlockResponseOptions()
            {
                ShowInput = false,
                ShowBalanceChanges = true,
                ShowEffects = true,
                ShowObjectChanges = true,
                ShowEvents = true,
            };
            LoadTransactionBlockResponseOptions(opts);
        }

        public async Task<float> GetSuiBalance()
        {
            RpcResult<Balance> balanceResult = await _client.GetBalanceAsync(new AccountAddress(_currentAccount.GetSuiAddress()));
            return GetFloatFromBigInteger(balanceResult.Result.TotalBalance);
        }

        public async Task<RpcResult<TransactionBlockResponse>> TransferSui(float suiAmount, string recipientAddress)
        {
            TransactionBlock txBlock = _transactionKit.GetTransferSuiTransaction(suiAmount, recipientAddress);
            return await SignAndExecuteTransactionBlockAsync(txBlock);
        }

        public async Task<RpcResult<TransactionBlockResponse>> SignAndExecuteTransactionBlockAsync(TransactionBlock txBlock)
        {
            if (_currentAccount.GetAccountType() == SuiAccountType.Local)
            {
                return await _client.SignAndExecuteTransactionBlockAsync(txBlock, _currentAccount.GetAccount(), _opts);
            }
            else
            {
                return await EnokiZKLogin.SignAndExecuteTransactionBlock(txBlock);
            }
            
        }

        public static float GetFloatFromBigInteger(BigInteger value)
        {
            decimal suiValue = (decimal)value / 1_000_000_000m;
            return (float)suiValue;
        }
    }
}

