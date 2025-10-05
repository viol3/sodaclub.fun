using ec33.SuiWorks.Transactions;
using NBitcoin;
using Sui.Rpc;
using Sui.Rpc.Client;
using Sui.Rpc.Models;
using Sui.Transactions;
using System.Threading.Tasks;
using UnityEngine;

namespace ec33.SuiWorks.Accounts
{
    public class SuiAccountManager
    {
        private ISuiAccount _currentAccount;
        private SuiClient _client;
        private TransactionKit _transactionKit;

        private TransactionBlockResponseOptions _opts;

        public SuiAccountManager(string network)
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
            CreateTransactionBlockResponseOptionsInternal();
        }

        public SuiAccountManager(string network, SuiClient client)
        {
            if (_client != null)
            {
                Debug.Log("Sui client already created.");
                return;
            }
            _client = client;
            CreateTransactionBlockResponseOptionsInternal();
        }

        public void Login()
        {

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
            };
            LoadTransactionBlockResponseOptions(opts);
        }

        public async Task<RpcResult<TransactionBlockResponse>> TransferSui(float suiAmount, string recipientAddress)
        {
            TransactionBlock txBlock = _transactionKit.GetTransferSuiTransaction(suiAmount, recipientAddress);
            return await _currentAccount.SignAndExecuteTransaction(txBlock, _opts);
        }
    }
}

