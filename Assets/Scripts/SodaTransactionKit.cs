using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Rpc.Models;
using Sui.Transactions;
using Sui.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using viol3.SuiWorks.Transactions;

namespace Soda.Sui
{
    public class SodaTransactionKit : TransactionKit
    {
        private string _deathCardPackageId = "0x017317c7a076f98ef42819dadb4391766719f56aea04e3d87ffe2f47c8d39a87";
        private string _deathCardChestId = "0x8e9382584c8ab518d321317d97ec8ceb570667ef2fb5a5d98ad1b97e21c19271";
        private string _randomPackageId = "0x8";
        private string _deathCardCommitFunc = "::lucky_game::commit";
        private string _deathCardRevealFunc = "::lucky_game::reveal";

        public TransactionBlock Commit(decimal amount, byte cardCount)
        {
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
                SuiMoveNormalizedStructType.FromStr($"{_deathCardPackageId}{_deathCardCommitFunc}"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                    tx_block.AddObjectInput(_randomPackageId),
                    tx_block.AddObjectInput(_deathCardChestId),
                    splitArgs[0],
                    tx_block.AddPure(new U8(cardCount)),
                }
            );
            return tx_block;

        }

        public TransactionBlock Reveal(string ownedCommit)
        {
            TransactionBlock tx_block = new TransactionBlock();
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{_deathCardPackageId}{_deathCardRevealFunc}"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                tx_block.AddObjectInput(_deathCardChestId),
                tx_block.AddObjectInput(ownedCommit),
                }
            );

            return tx_block;


        }
    }
}

