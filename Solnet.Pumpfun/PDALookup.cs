using Solnet.Programs;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Pumpfun
{
    public static class PDALookup
    {
        public static PublicKey FindGlobalPDA()
        {
            PublicKey.TryFindProgramAddress(new List<byte[]>()
            {
                Encoding.UTF8.GetBytes("global")
            },
            PumpfunProgram.ProgramID,
            out PublicKey globalAddress,
            out _);

            return globalAddress;
        }

        public static PublicKey FindBondingPDA(PublicKey mintAddress)
        {
            PublicKey.TryFindProgramAddress(new List<byte[]>()
            {
                new byte[]{ 98, 111, 110, 100, 105, 110, 103, 45, 99, 117, 114, 118, 101 },
                mintAddress.KeyBytes
            },
            PumpfunProgram.ProgramID,
            out PublicKey bondingAddress,
            out _);

            return bondingAddress;
        }

        public static PublicKey FindAssociatedBondingPDA(PublicKey bondingAddress, PublicKey mintAddress)
        {
            PublicKey.TryFindProgramAddress(new List<byte[]>()
            {
                bondingAddress.KeyBytes,
                TokenProgram.ProgramIdKey,
                mintAddress.KeyBytes
            },
            AssociatedTokenAccountProgram.ProgramIdKey,
            out PublicKey asscociated_bondingAddress,
            out _);

            return asscociated_bondingAddress;
        }
    }
}
