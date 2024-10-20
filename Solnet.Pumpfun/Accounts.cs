using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Pumpfun
{
    public class Accounts
    {
        public partial class BondingCurve
        {

            public ulong virtualTokenReserves;
            public ulong virtualSolReserves;
            public ulong realTokenReserves;
            public ulong realSolReserves;
            public ulong tokenTokenSupply;
            public bool complete;
            public static BondingCurve Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 8;
                BondingCurve result = new BondingCurve();
                result.virtualTokenReserves = _data.GetU64(offset);
                offset += 8;
                result.virtualSolReserves = _data.GetU64(offset);
                offset += 8;
                result.realTokenReserves = _data.GetU64(offset);
                offset += 8;
                result.realSolReserves = _data.GetU64(offset);
                offset += 8;
                result.tokenTokenSupply = _data.GetU64(offset);
                offset += 8;
                result.complete = _data.GetBool(offset);

                return result;
            }
        }
    }
}
