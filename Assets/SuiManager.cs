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
using UnityEngine.UIElements;

public class SuiManager : GenericSingleton<SuiManager>
{
    SuiClient _client;
    Account _account;

    private string _deathCardPackageId = "0xff475855d5eea18bd4350c7ef788b377ce3ef47e3677b78b3b91c91ea1335a04";
    private string _deathCardChestId = "0x71e29a34c279548030f27b82dbc8657bf6077662fdaf2df7788d23dcfc3ba989";
    private string _deathCardModuleFunc = "::lucky_game::play";
    private string _randomPackageId = "0x0000000000000000000000000000000000000000000000000000000000000008";

    SuiStructTag _sui_coin = new SuiStructTag("0x2::sui::SUI");
    SuiStructTag _coin = new SuiStructTag("0x2::coin::Coin");
    private void Start()
    {
        _client = new SuiClient(Constants.TestnetConnection);
        _account = new Account("0xb1751db9ab3ce30b1253eeec9ad68cd4db7ef7008debcdc7beae68f222789303");
        Debug.Log(_account.PrivateKey.KeyHex);
        Debug.Log(_account.SuiAddress().KeyHex);
        Test();
    }

    async void Test()
    {
        RpcResult<Balance> result = await _client.GetBalanceAsync(_account);
        BigInteger balance = result.Result.TotalBalance;
        decimal suiBalance = (decimal)balance / 1_000_000_000m;
        Debug.Log(suiBalance);
        await PlayDeathCard(0.1m, 8);
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

    async Task<bool> PlayDeathCard(decimal amount, byte cardCount)
    {
        TransactionBlock tx_block = new TransactionBlock();
        ulong requestedAmountLong = (ulong)(amount * 1_000_000_000m);
        List<TransactionArgument> splitArgs = tx_block.AddMoveCallTx
        (
            SuiMoveNormalizedStructType.FromStr("0x2::pay::split"),
            new SerializableTypeTag[] { new SerializableTypeTag("0x2::sui::SUI") },
            new TransactionArgument[]
            {
                tx_block.gas,  // Insert coin object ID here
                tx_block.AddPure(new U64(requestedAmountLong)) // Insert split amount here
            }
        );
        Debug.Log($"{_deathCardPackageId}{_deathCardModuleFunc}");
        
        tx_block.AddMoveCallTx
        (
            SuiMoveNormalizedStructType.FromStr($"{_deathCardPackageId}{_deathCardModuleFunc}"),
            new SerializableTypeTag[] { new SerializableTypeTag("0x2::random::Random"), new SerializableTypeTag("0xff475855d5eea18bd4350c7ef788b377ce3ef47e3677b78b3b91c91ea1335a04::lucky_game::CHEST"), new SerializableTypeTag("0x2::sui::SUI") },
            new TransactionArgument[]
            {
                    tx_block.AddObjectInput("0x8"),
                    tx_block.AddObjectInput(_deathCardChestId),
                    splitArgs[0],
                    tx_block.AddPure(new U8(cardCount))
            }
        );
        RpcResult<TransactionBlockResponse> result_task = await _client.SignAndExecuteTransactionBlockAsync
        (
            tx_block,
            _account
        );
        if (result_task.Error != null)
        {
            Debug.Log("PlayDeathCard Error => " + result_task.Error.Message);
            return false;
        }
        Debug.Log(result_task.Result);
        foreach (var m_event in result_task.Result.Events)
        {
            Debug.Log("Value => " + m_event.ParsedJson.GetValue("value"));
        }
        return true;
    }

}
