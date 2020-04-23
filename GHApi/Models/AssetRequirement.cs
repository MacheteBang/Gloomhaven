using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GHApi.Models.EventCards
{
    public class AssetRequirement
    {
        public long Id { get; set; }
        public string Requirement { get; set; }

        public AssetRequirement(string requirement)
        {
            Requirement = requirement;
        }
    }
}
