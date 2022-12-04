using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace slip39_dotnet.helpers
{
    public class ChecksumOperations
    {
        static private int[] _gen = new int[] {
             0xE0E040,
             0x1C1C080,
             0x3838100,
             0x7070200,
             0xE0E0009,
             0x1C0C2412,
             0x38086C24,
             0x3090FC48,
             0x21B1F890,
             0x3F3F120, }
        ;
        static public int _rs1024Polymod(IEnumerable<int> values)
        {
            var chk = 1;

            foreach (var v in values)
            {
                int b = chk >> 20;
                chk = (chk & 0xFFFFF) << 10 ^ v;
                for (var i = 0; i < 10; i++)
                {
                    int bb = ((b >> i) & 1);
                    int cc = bb != 0 ? _gen[i] : 0;

                    chk ^= cc;
                }
            }
            return chk;
        }

        static public List<int> GetChecksum(string salt, IEnumerable<int> data)
        {
            var saltInts = new List<int>();
            foreach (char v in salt)
            {
                saltInts.Add((int) v);
            }
            var input = new int[saltInts.Count + data.Count() + 3];

            var j = 0;
            foreach (var v in saltInts)
            {
                input[j++] = v;
            }
            foreach (var v in data)
            {
                input[j++] = v;
            }

             
            var polymod = _rs1024Polymod(input) ^ 1;
            var result = new List<int>();
            for (int i = 0; i< 3; i ++)
            {
                result.Add((polymod >> 10 * (2 - i)) & 1023);
            }
            return result;
        }

        static public bool VerifyChecksum(string salt, IEnumerable<int> data)
        {
            var saltInts = new List<int>();
            foreach (char v in salt)
            {
                saltInts.Add((int)v);
            }
            var input = saltInts.Append(data);
            return _rs1024Polymod(input) == 1;
        }
    }
}
