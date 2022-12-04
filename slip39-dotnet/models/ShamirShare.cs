using slip39_dotnet.helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static slip39_dotnet.helpers.BitArrayOperations;
using static slip39_dotnet.helpers.ChecksumOperations;
using static slip39_dotnet.helpers.WordList;

namespace slip39_dotnet.models
{
    public class ShamirShare
    {
        private const int minShareBitLength = 128;
        private BitArray Id { get; set; }
        private BitArray IterationExponent { get; set; }
        private BitArray GroupIndex { get; set; }
        private BitArray GroupThreshold { get; set; }
        private BitArray GroupCount { get; set; }
        private BitArray MemberIndex { get; set; }
        private BitArray MemberThreshold { get; set; }
        private BitArray ShareValue { get; set; }
        private BitArray DataPart { get; set; } 

        private List<int> DataSplit { get; set; }
        private List<int> Checksum { get; set; }

        public ShamirShare(int id, int iterationExponent, int groupIndex, int groupThreshold, int groupCount, int memberIndex, int memberThreshold, ShamirPoint shareValue)
        {
            if (shareValue.N * 8 < minShareBitLength) throw new ArgumentException(nameof(shareValue), $"{nameof(shareValue)} parameter must have at least {minShareBitLength} bits of security, it has instead {shareValue.N * 8} bits.");
            Id = ConvertFromInt(id, 15);
            IterationExponent = ConvertFromInt(iterationExponent, 5);
            GroupIndex = ConvertFromInt(groupIndex, 4);
            GroupThreshold = ConvertFromInt(groupThreshold, 4);
            GroupCount = ConvertFromInt(groupCount, 4);
            MemberIndex = ConvertFromInt(memberIndex, 4);
            MemberThreshold = ConvertFromInt(memberThreshold, 4);
            ShareValue = GetPaddedShareValue(shareValue);
            DataPart = Id.Append(IterationExponent).Append(GroupIndex).Append(GroupThreshold).Append(GroupCount).Append(MemberIndex).Append(MemberThreshold).Append(ShareValue);
            DataSplit = DataPart.Split().ToList<int>();
            Checksum = GetChecksum("shamir", DataSplit);
            if (VerifyChecksum("shamir", DataSplit.Append(Checksum)) != true) throw new Exception($"Checksum validation failed during the creation of the Shamire's share.");
        }

        public IEnumerable<string> WordList => (DataSplit.Append(Checksum)).ToList<int>().Select(x => Words.ElementAt(x));
        public override string ToString()
        {
            return String.Join(" ", WordList);
        }
    }
}
