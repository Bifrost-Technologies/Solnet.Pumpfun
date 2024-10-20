using Solnet.Programs;
using Solnet.Programs.Models;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using Solnet.Wallet;
using static Solnet.Pumpfun.Accounts;

namespace Solnet.Pumpfun
{
    public class PumpfunClient
    {
        public IRpcClient RpcClient { get; set; }
        private Account trader {  get; set; }

        public PumpfunClient(IRpcClient rpc, Account _trader)
        {
            this.RpcClient = rpc;
            this.trader = _trader;
        }
        public async Task<AccountResultWrapper<BondingCurve>> GetBondingCurveAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new AccountResultWrapper<BondingCurve>(res);
            var resultingAccount = BondingCurve.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new AccountResultWrapper<BondingCurve>(res, resultingAccount);
        }
        public void SetTrader(Account _trader)
        {
            this.trader = _trader;
        }
        public async Task Buy(string mint_address, decimal sol_amount,  decimal slippage_percentage, ulong computebudget = 100000, ulong computeprice = 1080000)
        {
            PublicKey mint = new PublicKey(mint_address);
            PublicKey associatedUser = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(trader, mint);

            PublicKey bondingCurveAddress = PDALookup.FindBondingPDA(mint);
            PublicKey associatedCurveAddress = PDALookup.FindAssociatedBondingPDA(bondingCurveAddress, mint);
            BondingCurve? curve = (await GetBondingCurveAsync(bondingCurveAddress)).ParsedResult;
            if (curve != null)
            {
                decimal virtualSol = curve.virtualSolReserves / SolHelper.LAMPORTS_PER_SOL;
                
                decimal virtualToken = curve.virtualTokenReserves / (ulong)1000000;
              
                decimal tokenPrice = virtualSol / virtualToken;
           
                ulong amountOut = (ulong)(sol_amount / tokenPrice) * (ulong)1000000;
           
                decimal slippage = (sol_amount / 100) * slippage_percentage;
                ulong max_sol = SolHelper.ConvertToLamports(sol_amount + slippage);
                TransactionInstruction computeBudget = PumpfunProgram.SetCUlimit(computebudget);
                TransactionInstruction computePrice = ComputeBudgetProgram.SetComputeUnitPrice(computeprice);
                TransactionInstruction buy_instruction = PumpfunProgram.CreateBuyInstruction(mint, bondingCurveAddress, associatedCurveAddress, associatedUser, trader, amountOut, max_sol);

                LatestBlockHash latestBlockHash = (await RpcClient.GetLatestBlockHashAsync()).Result.Value;
                TransactionBuilder tx = new TransactionBuilder();
                tx.SetFeePayer(trader);
                tx.SetRecentBlockHash(latestBlockHash.Blockhash);
                tx.AddInstruction(computeBudget);
                tx.AddInstruction(computePrice);
                tx.AddInstruction(AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(trader, trader, mint));
                tx.AddInstruction(buy_instruction);
                byte[] final_tx = tx.Build(trader);
                string response = (await RpcClient.SendTransactionAsync(final_tx)).RawRpcResponse;
                Console.WriteLine(response);
            }
        }
        public async Task Sell(string mint_address, decimal token_amount, decimal min_sol_out = 0, ulong computebudget = 100000, ulong computeprice = 2080000)
        {
            PublicKey mint = new PublicKey(mint_address);
            PublicKey associatedUser = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(trader, mint);

            PublicKey bondingCurveAddress = PDALookup.FindBondingPDA(mint);
            PublicKey associatedCurveAddress = PDALookup.FindAssociatedBondingPDA(bondingCurveAddress, mint);
            BondingCurve? curve = (await GetBondingCurveAsync(mint_address)).ParsedResult;
            if (curve != null)
            {
                TransactionInstruction computeBudget = PumpfunProgram.SetCUlimit(computebudget);
                TransactionInstruction computePrice = ComputeBudgetProgram.SetComputeUnitPrice(computeprice);
                TransactionInstruction sell_instruction = PumpfunProgram.CreateSellInstruction(mint, bondingCurveAddress, associatedCurveAddress, associatedUser, trader, (ulong)(token_amount * 1000000), min_sol_out);
                LatestBlockHash latestBlockHash = (await RpcClient.GetLatestBlockHashAsync()).Result.Value;
                TransactionBuilder tx = new TransactionBuilder();
                tx.SetFeePayer(trader);
                tx.SetRecentBlockHash(latestBlockHash.Blockhash);
                tx.AddInstruction(computeBudget);
                tx.AddInstruction(computePrice);
                tx.AddInstruction(sell_instruction);
                byte[] final_tx = tx.Build(trader);
                var response = await RpcClient.SendTransactionAsync(final_tx);
                Console.WriteLine(response.RawRpcResponse);
            }
        }
    }
}
