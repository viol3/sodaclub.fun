using OpenDive.BCS;
using Sui.Transactions;
using System.Collections.Generic;

namespace ec33.SuiWorks.Transactions
{
    public class TransactionKit
    {
        public TransactionBlock GetTransferSuiTransaction(float suiAmount, string recipientAddress)
        {
            TransactionBlock tx_block = new TransactionBlock();

            // Split coins from the gas coin to create a new coin with specified amount
            // This example splits 10,000,000 MIST (0.01 SUI) from the gas coin
            // Note: 1 SUI = 1,000,000,000 MIST
            List<TransactionArgument> splitArgs = tx_block.AddSplitCoinsTx
            (
                tx_block.gas,
                new TransactionArgument[]
                {
                    tx_block.AddPure(new U64((ulong)(suiAmount * 1_000_000_000))) // Insert split amount here(0.01 Sui)
                }
            );

            tx_block.AddTransferObjectsTx
            (
                new TransactionArgument[]
                {
                    splitArgs[0] // Insert split amount here
                },
                Sui.Accounts.AccountAddress.FromHex(recipientAddress)
            );

            return tx_block;
        }
    }
}


