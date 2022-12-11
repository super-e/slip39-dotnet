using slip39_dotnet.helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static slip39_dotnet.helpers.BitArrayOperations;
using static slip39_dotnet.helpers.ChecksumOperations;
using static slip39_dotnet.helpers.WordList;
using static slip39_dotnet.helpers.ExtensionMethods;
using System.Transactions;

namespace slip39_dotnet.models
{
    public class ShamirShare
    {
        private const int _minShareBitLength = 128;
        private BitArray _id { get; set; }
        private BitArray _iterationExponent { get; set; }
        private BitArray _groupIndex { get; set; }
        private BitArray _groupThreshold { get; set; }
        private BitArray _groupCount { get; set; }
        private BitArray _memberIndex { get; set; }
        private BitArray _memberThreshold { get; set; }
        private BitArray _shareValue { get; set; }
        private BitArray _dataPart { get; set; } 

        private List<int> _dataSplit { get; set; }
        private List<int> _checksum { get; set; }
        public ShamirPoint ShareSecretPoint{ get; set; }

        public ShamirShare(int id, int iterationExponent, int groupIndex, int groupThreshold, int groupCount, int memberIndex, int memberThreshold, ShamirPoint shareValue)
        {
            if (shareValue.N * 8 < _minShareBitLength) throw new ArgumentException(nameof(shareValue), $"{nameof(shareValue)} parameter must have at least {_minShareBitLength} bits of security, it has instead {shareValue.N * 8} bits.");
            _id = ConvertFromInt(id, 15);
            _iterationExponent = ConvertFromInt(iterationExponent, 5);
            _groupIndex = ConvertFromInt(groupIndex, 4);
            _groupThreshold = ConvertFromInt(groupThreshold, 4);
            _groupCount = ConvertFromInt(groupCount, 4);
            _memberIndex = ConvertFromInt(memberIndex, 4);
            _memberThreshold = ConvertFromInt(memberThreshold, 4);
            _shareValue = GetPaddedShareValue(shareValue);
            _dataPart = _id.Append(_iterationExponent).Append(_groupIndex).Append(_groupThreshold).Append(_groupCount).Append(_memberIndex).Append(_memberThreshold).Append(_shareValue);
            _dataSplit = _dataPart.Split().ToList<int>();  // DataPart.ToEnumerable().Chunk<bool>(10).Select(x => getIntFromBitArray(new BitArray(x.ToArray<bool>()))).ToList(); // 
            _checksum = GetChecksum("shamir", _dataSplit);
            if (VerifyChecksum("shamir", _dataSplit.Append(_checksum)) != true) throw new Exception($"Checksum validation failed during the creation of the Shamire's share.");
            WordList = _dataSplit.ToList<int>().Select(x => Words.ElementAt(x));
        }

        public ShamirShare(List<string> words)
        {
            _checksum = words.TakeLast(3).Select(x => Words.IndexOf(x)).ToList();
            _dataSplit = words.Take(words.Count - 3).Select(x => Words.IndexOf(x)).ToList();

            if (VerifyChecksum("shamir", _dataSplit.Concat(_checksum)) != true) throw new Exception($"Checksum validation failed.");

            var c = new BitArray((words.Count - 3) * 10);
            int counter = 0;
            foreach (var ds in _dataSplit)
            {
                var d = new BitArray(10);
                for (int i = 9; i >= 0; i--)
                {
                    c[counter++] = (1 & (ds >> i)) == 1 ? true : false;
                }
                
            }

            _id = new BitArray(15);
            for(int i = 0; i < _id.Length; i++)
            {
                _id[i] = c[i];
            }
            _iterationExponent = new BitArray(5);
            for(int i = 0; i < _iterationExponent.Length; i++)
            {
                _iterationExponent[i] = c[15 + i];
            }
            _groupIndex = new BitArray(4);
            for (int i = 0; i < _groupIndex.Length; i++)
            {
                _groupIndex[i] = c[20 + i];
            }
            _groupThreshold= new BitArray(4);
            for (int i = 0; i < _groupThreshold.Length; i++)
            {
                _groupThreshold[i] = c[24 + i];
            }
            _groupCount= new BitArray(4);
            for (int i = 0; i < _groupCount.Length; i++)
            {
                _groupCount[i] = c[28 + i];
            }
            _memberIndex= new BitArray(4);
            for (int i = 0; i< _memberIndex.Length; i++)
            {
                _memberIndex[i] = c[32 + i];
            }
            _memberThreshold = new BitArray(4);
            for (int i = 0; i < _memberThreshold.Length; i++)
            {
                _memberThreshold[i] = c[36 + i];
            }
            _shareValue= new BitArray(c.Length - 40);
            for(int i = 0; i <  _shareValue.Length; i++)
            {
                _shareValue[i] = c[40 + i]; 
            }
            _id.Reverse();
            _iterationExponent.Reverse();
            _groupIndex.Reverse();
            _groupThreshold.Reverse();
            _groupCount.Reverse();
            _memberIndex.Reverse();
            _memberThreshold.Reverse();
            ShareSecretPoint = GetValueRemovingPadding(_shareValue);
            
            //ShareValue.Reverse();
            //Console.WriteLine($"{getIntFromBitArray(Id)}:{getIntFromBitArray(IterationExponent)}:{getIntFromBitArray(GroupIndex)}:{getIntFromBitArray(GroupThreshold)}:{getIntFromBitArray(GroupCount)}:{getIntFromBitArray(MemberIndex)}:{getIntFromBitArray(MemberThreshold)}");
        }

        public ShamirShare(string shareWords) : this(shareWords.Split(' ').ToList())
        {
             
        }

        public readonly IEnumerable<string> WordList;
        public override string ToString()
        {
            return String.Join(" ", WordList);
        }


    }
}
