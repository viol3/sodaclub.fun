module lucky_game::lucky_game
{
    use sui::object::{Self, UID};
    use sui::balance::{Self, Balance};
    use sui::tx_context::{Self, TxContext};
    use sui::transfer;
    use sui::coin::{Self, Coin};
    use sui::sui::SUI;
    use sui::random::{Self, Random};
    use sui::event;

    public struct DiceValue has copy, drop
    {
        value: u8,
    }

    public struct Commit has key
    {
        id: UID,
        player: address,
        amount: u64,
        input: u8,
        diceValue: u8
    }

    public struct CHEST has key
    {
        id: UID,
        chestBalance: Balance<SUI>,
        owner: address,
    }

    const PUBLISHER: address = @0x0d9b5ca4ebae5f4a7bd3f17e4e36cd6f868d8f0c5a7f977f94f836631fe0288d;

    public fun create(ctx: &mut TxContext)
    {
        assert!(tx_context::sender(ctx) == PUBLISHER, 100);
        let chest = CHEST
        {
            id: object::new(ctx),
            chestBalance: balance::zero(),
            owner: ctx.sender(),
        };
        transfer::share_object(chest);
    }

    public fun topup(chest: &mut CHEST, mut coins: Coin<SUI>, ctx: &mut TxContext)
    {
        assert!(tx_context::sender(ctx) == chest.owner, 1);
        coin::put(&mut chest.chestBalance, coins);
    }

    public fun withdraw(chest: &mut CHEST, amount: u64, ctx: &mut TxContext)
    {
        assert!(tx_context::sender(ctx) == chest.owner, 1);
        assert!(amount > 0, 6); // Sıfırdan büyük miktar kontrolü
        assert!(chest.chestBalance.value() >= amount, 7); // Yeterli bakiye kontrolü

        // Chest'ten belirtilen miktarı çek
        let coin = coin::take(&mut chest.chestBalance, amount, ctx);

        // Kullanıcıya gönder
        transfer::public_transfer(coin, chest.owner);
    }

    public entry fun commit(r: &Random, chest: &mut CHEST, mut user_coin: Coin<SUI>, input: u8, ctx: &mut TxContext)
    {
        let amount = coin::value(&user_coin);
        assert!(amount >= 100_000_000, 2);
        assert!(input >= 2 && input <= 8, 3);

        // Coin'i chest'e koy
        coin::put(&mut chest.chestBalance, user_coin);
        let mut generator = random::new_generator(r, ctx);
        let result = random::generate_u8_in_range(&mut generator, 1, input);
        let commit = Commit
        {
            id: object::new(ctx),
            player: tx_context::sender(ctx),
            amount,
            input,
            diceValue: result,
        };
        transfer::transfer(commit, tx_context::sender(ctx));
    }

    entry fun reveal(chest: &mut CHEST, commit: Commit, ctx: &mut TxContext)
    {
        assert!(commit.player == tx_context::sender(ctx), 10);
        let result = commit.diceValue;
        let input_u64 = commit.input as u64;
        let mut factor = ((100000000 / (10000 - (10000 / input_u64))) - 10000) / 100;
        factor = factor - 3;
        let bonus = (commit.amount * factor) / 100;

        // Balance kontrolü (doğru kullanım)
        assert!(balance::value(&chest.chestBalance) >= bonus, 4);

        if (result != 1)
        {
            let pay_bonus = coin::take(&mut chest.chestBalance, bonus, ctx);
            let mut refund = coin::take(&mut chest.chestBalance, commit.amount, ctx);
            coin::join(&mut refund, pay_bonus);
            transfer::public_transfer(refund, commit.player);
        };
        event::emit(DiceValue { value:result });
        let Commit { id, player: _, amount: _, input: _, diceValue: _ } = commit;
        object::delete(id);

    }

}
