# Solnet.Pumpfun
  C# SDK & Client for Pump.fun on Solana

#### Dependencies - Use NET8 tag on nuget to find latest versions of solnet
- NET8.0
- Solnet.RPC
- Solnet.Wallet
- Solnet.Programs


How to use the library to perform a 10 second buy/sell
```
using Solnet.Programs;
using Solnet.Pumpfun;
using Solnet.Rpc;
using Solnet.Wallet;


Account _trader = Account.FromSecretKey("ENTER SECRET KEY HERE");
IRpcClient connection = ClientFactory.GetClient("RPC LINK HERE");
PumpfunClient pumpFun = new PumpfunClient(connection, _trader);

//Buy the token.  Token Address  -  Sol Amount - Slippage Percent  
await pumpFun.Buy("CA/MINT ADDRESS HERE", 0.001m, 10);

await Task.Delay(10000);

PublicKey associatedUser = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(_trader, new PublicKey("CA/MINT ADDRESS HERE"));
var tokenbalance = await connection.GetTokenAccountBalanceAsync(associatedUser);

await Task.Delay(1000);

await pumpFun.Sell("CA/MINT ADDRESS HERE", tokenbalance.Result.Value.AmountDecimal);
```

Quickstart AI logic template
```
using Solnet.Programs;
using Solnet.Pumpfun;
using Solnet.Rpc;
using Solnet.Wallet;


Account _trader = Account.FromSecretKey("ENTER SECRET KEY HERE");
IRpcClient connection = ClientFactory.GetClient("RPC LINK HERE");
PumpfunClient pumpFun = new PumpfunClient(connection, _trader);

bool living = true;
while (living)
{

    //THINK 
    //Add logic here to determine what you should buy next then execute the code below to have your bot trade the tokens
    //if (foundGoodToken)
    //{

        //DO
        try
        {
            //Swap the token.  Token Address  -  Sol Amount  
            await PerformTenSecondTrade("TOKEN ADDRESS", 0.001m, pumpFun, _trader);

        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        //Freeze the loop in the example. you can remove this once you add logic to determine when to buy/sell
        await Task.Delay(1000000000);
   //}
}

static async Task PerformTenSecondTrade(string tokenAddress, decimal sol_amount, PumpfunClient pumpFun, Account _trader)
{
    try
    {
        //Buy the token.  Token Address  -  Sol Amount - Slippage Percent  
        await pumpFun.Buy(tokenAddress, 0.001m, 10);

        await Task.Delay(10000);

        PublicKey associatedUser = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(_trader, new PublicKey(tokenAddress));
        var tokenbalance = await pumpFun.RpcClient.GetTokenAccountBalanceAsync(associatedUser);

        await Task.Delay(1000);

        await pumpFun.Sell(tokenAddress, tokenbalance.Result.Value.AmountDecimal);

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}
```
