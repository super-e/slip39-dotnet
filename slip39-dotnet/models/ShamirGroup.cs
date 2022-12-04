using System;
using System.Collections.Generic;
using System.Text;

namespace slip39_dotnet.models
{
    public class ShamirGroup
    {
        private int minMembers = 1;
        private int maxMembers = 16;
        private int minThreshold = 1;

        public string GroupName { get; private set; }
        public int TotalShares { get; private set; }
        public int Threshold { get; private set; }

        public ShamirPoint GroupSecret { get; set; }

        public ShamirGroup(string groupName, int members, int threshold)
        {
            if (members < minMembers || members > maxMembers) throw new ArgumentOutOfRangeException(nameof(members), $"{nameof(members)} parameter has value {members}, it should be between {minMembers} and {maxMembers}.");
            if (threshold < minThreshold || threshold > members) throw new ArgumentOutOfRangeException(nameof(threshold), $"{nameof(threshold)} parameter has value {threshold}, it should be between {minThreshold} and number of members ({members}).");
            if (groupName == null) GroupName = $"{threshold} out of {members} group";


            GroupName = groupName;
            TotalShares = members;
            Threshold = threshold;
        }

        
    }
}
