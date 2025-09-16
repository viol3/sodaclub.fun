module lucky_game_legacy::lucky_game_legacy
{
    use sui::object;
    use sui::balance::{Self, Balance};
    use sui::tx_context::{Self, TxContext};
    use sui::transfer;
    use sui::coin::{Self, Coin};
    use sui::sui::SUI;
    use sui::random::{Self, Random};
    use sui::event;

    public struct FactorEvent has copy, drop
    {
        value: u64,
    }

    public struct DiceValue has copy, drop
    {
        value: u8,
    }

    public struct CHEST has key
    {
        id: UID,
        chestBalance: Balance<SUI>,
        owner: address,
    }

    const RANDOM_OBJECT: address = @0x8;
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

    public fun topup(chest: &mut CHEST, mut coins: Coin<SUI>)
    {
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


    public fun play(r: &Random, chest: &mut CHEST, mut user_coin: Coin<SUI>, input: u8, ctx: &mut TxContext)
    {
        let amount = coin::value(&user_coin);
        assert!(amount >= 100_000_000, 2);

        let rid = object::id(r);
        assert!(object::id_to_address(&rid) == RANDOM_OBJECT, 99);
        assert!(input >= 2 && input <= 8, 3);
        let randomValue = rollDice(r, input, ctx);

        if (randomValue == 1)
        {
            coin::put(&mut chest.chestBalance, user_coin);
            event::emit(FactorEvent { value: 0 });
        }
        else
        {
            let input_u64 = input as u64;
            let mut factor = ((100000000 / (10000 - (10000 / input_u64))) - 10000) / 100;
            //let mut factor = ((1 / (1 - (1/input))) - 1)*100;
            factor = factor - 3;//house fee
            let bonus = (amount * factor) / 100;
            assert!(chest.chestBalance.value() >= bonus, 4);
            let pay_bonus = coin::take(&mut chest.chestBalance, bonus, ctx);
            coin::join(&mut user_coin, pay_bonus);
            transfer::public_transfer(user_coin, tx_context::sender(ctx));
            event::emit(FactorEvent { value: factor });
        };
        event::emit(DiceValue { value:randomValue });
    }

    entry fun rollDice(r: &Random, max: u8, ctx: &mut TxContext): u8
    {
        assert!(max >= 2 && max <= 8, 100);
        let mut generator = random::new_generator(r, ctx);
        let result = random::generate_u8_in_range(&mut generator, 1, max);
        result
    }
}
