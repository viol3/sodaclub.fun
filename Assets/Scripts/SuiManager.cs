using Ali.Helper;
using NBitcoin;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Rpc.Client;
using Sui.Rpc.Models;
using Sui.Transactions;
using Sui.Types;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class SuiManager : GenericSingleton<SuiManager>
{
    SuiClient _client;
    Account _account;

    [SerializeField] private string _deathCardPackageId = "";
    [SerializeField] private string _deathCardChestId = "";
    private string _deathCardModuleFunc = "::lucky_game::play";
    private string _randomPackageId = "0x8";

    SuiStructTag _sui_coin = new SuiStructTag("0x2::sui::SUI");
    SuiStructTag _coin = new SuiStructTag("0x2::coin::Coin");

    [HideInInspector]
    public UnityEvent OnDeathCardStarted = new UnityEvent();
    [HideInInspector]
    public UnityEvent<int> OnDeathCardEnded = new UnityEvent<int>();
    [HideInInspector]
    public UnityEvent<float> OnBalanceUpdated = new UnityEvent<float>();
    [HideInInspector]
    public UnityEvent<float> OnBalanceChanged = new UnityEvent<float>();
    private void Start()
    {
        _client = new SuiClient(Constants.TestnetConnection);
        _account = new Account("0xb1751db9ab3ce30b1253eeec9ad68cd4db7ef7008debcdc7beae68f222789303");
        //Debug.Log(_account.PrivateKey.KeyHex);
        //Debug.Log(_account.SuiAddress().KeyHex);
        CheckBalance();
    }

    float GetFloatFromBigInteger(BigInteger value)
    {
        decimal suiValue = (decimal)value / 1_000_000_000m;
        return (float)suiValue;
    }

    async Task CheckBalance()
    {
        RpcResult<Balance> result = await _client.GetBalanceAsync(_account);
        BigInteger balance = result.Result.TotalBalance;
        decimal suiBalance = (decimal)balance / 1_000_000_000m;
        OnBalanceUpdated?.Invoke((float)suiBalance);
    }

    public async void DeathCard(float betAmount, byte cardCount)
    {
        OnDeathCardStarted?.Invoke();
       
        
        int diceValue = await PlayDeathCard((decimal)betAmount, cardCount);
        OnDeathCardEnded?.Invoke(diceValue);
        //await CheckBalance();
    }

    async Task<List<CoinDetails>> GetSUICoins()
    {
        List<CoinDetails> result = new List<CoinDetails>();
        RpcResult<CoinPage> alice_sui_coins = await _client.GetCoinsAsync(_account, _sui_coin);
        for (int i = 0; i < alice_sui_coins.Result.Data.Length; i++)
        {
            result.Add(alice_sui_coins.Result.Data[i]);
        }
        return result;
    }

    async Task<int> PlayDeathCard(decimal amount, byte cardCount)
    {
        Debug.Log("Bet Amount => " + amount);
        TransactionBlock tx_block = new TransactionBlock();
        ulong requestedAmountLong = (ulong)(amount * 1_000_000_000m);
        List<TransactionArgument> splitArgs = tx_block.AddMoveCallTx
        (
            SuiMoveNormalizedStructType.FromStr("0x2::coin::split"),
            new SerializableTypeTag[] { new SerializableTypeTag("0x2::sui::SUI") },
            new TransactionArgument[]
            {
                tx_block.gas,  // Insert coin object ID here
                tx_block.AddPure(new U64(requestedAmountLong)) // Insert split amount here
            }
        );
        tx_block.AddMoveCallTx
        (
            SuiMoveNormalizedStructType.FromStr($"{_deathCardPackageId}{_deathCardModuleFunc}"),
            new SerializableTypeTag[] {  },
            new TransactionArgument[]
            {
                    tx_block.AddObjectInput(_randomPackageId),
                    tx_block.AddObjectInput(_deathCardChestId),
                    splitArgs[0],
                    tx_block.AddPure(new U8(cardCount))
            }
        );
        TransactionBlockResponseOptions transactionBlockResponseOptions = new TransactionBlockResponseOptions();
        transactionBlockResponseOptions.ShowEvents = true;
        transactionBlockResponseOptions.ShowBalanceChanges = true;
        RpcResult<TransactionBlockResponse> result_task = await _client.SignAndExecuteTransactionBlockAsync
        (
            tx_block,
            _account,
            transactionBlockResponseOptions
        );

        if (result_task.Error != null)
        {
            Debug.Log("PlayDeathCard Error => " + result_task.Error.Message);
            return -1;
        }

        if (result_task.Result != null && result_task.Result.BalanceChanges != null && result_task.Result.BalanceChanges.Length > 0)
        {
            BigInteger changeAmountBig = result_task.Result.BalanceChanges[0].Amount;
            float changeAmount = GetFloatFromBigInteger(changeAmountBig);
            OnBalanceChanged?.Invoke(GetFloatFromBigInteger(result_task.Result.BalanceChanges[0].Amount));
        }

        try
        {
            foreach (var m_event in result_task.Result.Events)
            {
                if(m_event.Type.Contains("DiceValue"))
                {
                    string value = m_event.ParsedJson.GetValue("value").ToString();
                    return int.Parse(value);
                }
            }
            return -1;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.StackTrace);
            return -1;
        }
        
        
    }

}
