using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EldenRingDeathCounter.Model
{
    public class Boss : IBoss
    {
        public string Name { get; set; }
        public IRegion Region { get; set; }
        public string Variant { get; set; }

        public int CompareTo(IBoss other)
        {
            if (!this.Name.Equals(other.Name)) return -1;
            if (this.Region.Name.Equals(other.Region.Name)) return 0;
            return 1;
        }

        public override string ToString()
        {
            return $"{Name} ({Region.Name})";
        }
    }
}
