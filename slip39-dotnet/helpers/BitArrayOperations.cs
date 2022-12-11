using slip39_dotnet.models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace slip39_dotnet.helpers
{
    public static class BitArrayOperations
    {
        static public BitArray ConvertFromInt(long input, byte bitLength)
        {
            if (input < 0 || input >= 1 << bitLength) throw new ArgumentOutOfRangeException(nameof(input), $"{nameof(input)} parameter should be between 0 and {1 << bitLength} for this value of {nameof(bitLength)} ({bitLength})");
            var result = new BitArray(BitConverter.GetBytes(input));
             
            result.Length = bitLength;
            if (BitConverter.IsLittleEndian) result.Reverse();
            return result;
        }

        static public BitArray GetPaddedShareValue(ShamirPoint input)
        {
            var inputBitLength = input.N * 8;
            var outputBitLength = inputBitLength + (inputBitLength % 10 == 0 ? 0 : (10 - (inputBitLength % 10)));
            var result = new BitArray(input.byteArrayValue);
            result.Length = outputBitLength;
            if (BitConverter.IsLittleEndian) result.Reverse();
            return result;
        }

        static public ShamirPoint GetValueRemovingPadding(BitArray input)
        {
            var inputBitLength = input.Length;
            var outputBitLength = inputBitLength - (inputBitLength % 8);
            input.Reverse();
            var b = new BitArray(outputBitLength);
            for(int i = 0; i< outputBitLength; i++)
            {
                b[i] = input[i]; 
            }
            return new ShamirPoint(b.ConvertToByteArray());
        }

        static public int getIntFromBitArray(BitArray bitArray)
        {

            if (bitArray.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.");

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];

        }
    }
}
