using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Wallet;

namespace Solnet.Pumpfun
{
    public static class PumpfunProgram
    {
        public static PublicKey ProgramID = new PublicKey("6EF8rrecthR5Dkzon8Nwu78hRvfCKubJ14M5uBEwF6P");
        public static PublicKey FeeReceiver = new PublicKey("CebN5WGQ4jvEPvsVU4EoHEpgzq1VV7AbicfhtW4xC9iM");
        public static PublicKey GlobalAccount = new PublicKey("4wTV1YmiEkRvAtNtsSGPtUrqRYQMe5SKy2uB4Jjaxnjf");
        public static PublicKey RentProgram = new PublicKey("SysvarRent111111111111111111111111111111111");
        public static PublicKey EventAuthority = new PublicKey("Ce6TQqeHC9p8KetsN6JsjHK7UTZk7nasjjnr7XxXp9F1");

        public static TransactionInstruction SetCUlimit(ulong units)
        {
            List<AccountMeta> keys = new List<AccountMeta>();
            byte[] data = new byte[9];
            data.WriteU8(2, 0);
            data.WriteU64(units, 1);
            return new TransactionInstruction
            {
                ProgramId = ComputeBudgetProgram.ProgramIdKey,
                Keys = keys,
                Data = data
            };
        }
        public static TransactionInstruction CreateBuyInstruction( PublicKey tokenMint, PublicKey bonding, PublicKey assocBondingAddr, PublicKey ataUser, PublicKey user, ulong tokenOut, ulong solWithSlippage)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(GlobalAccount, false),
                AccountMeta.Writable(FeeReceiver, false),
                AccountMeta.ReadOnly(tokenMint, false),
                AccountMeta.Writable(bonding, false),
                AccountMeta.Writable(assocBondingAddr, false),
                AccountMeta.Writable(ataUser, false),
                AccountMeta.Writable(user, true),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(RentProgram, false),
                AccountMeta.ReadOnly(EventAuthority, false),
                AccountMeta.ReadOnly(ProgramID, false)
            };

            byte[] instructionBuffer = new byte[] { 0x66, 0x06, 0x3d, 0x12, 0x01, 0xda, 0xeb, 0xea };

            byte[] data = new byte[24];
            int offset = 0;
            data.WriteSpan(instructionBuffer, 0);
            offset += 8;
            data.WriteU64(tokenOut, offset);
            offset += 8;
            data.WriteU64(solWithSlippage, offset);
            offset += 8;


            return new TransactionInstruction
            {
                Keys = keys,
                ProgramId = ProgramID,
                Data = data
            };
        }
        public static TransactionInstruction CreateSellInstruction(PublicKey tokenMint, PublicKey bonding, PublicKey assocBondingAddr, PublicKey ataUser, PublicKey user, ulong tokenOut, decimal min_sol_out = 0)
        {
            // Define the keys
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(GlobalAccount, false),
                AccountMeta.Writable(FeeReceiver, false),
                AccountMeta.ReadOnly(tokenMint, false),
                AccountMeta.Writable(bonding, false),
                AccountMeta.Writable(assocBondingAddr, false),
                AccountMeta.Writable(ataUser, false),
                AccountMeta.Writable(user, true),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(AssociatedTokenAccountProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(EventAuthority, false),
                AccountMeta.ReadOnly(ProgramID, false)
            };

            byte[] instructionBuffer = new byte[] { 0x33, 0xe6, 0x85, 0xa4, 0x01, 0x7f, 0x83, 0xad };


            byte[] data = new byte[24];
            int offset = 0;
            data.WriteSpan(instructionBuffer, 0);
            offset += 8;
            data.WriteU64(tokenOut, offset);
            offset += 8;
            data.WriteU64(SolHelper.ConvertToLamports(min_sol_out), offset);

            return new TransactionInstruction
            {
                Keys = keys,
                ProgramId = ProgramID,
                Data = data
            };
        }
    }
}


