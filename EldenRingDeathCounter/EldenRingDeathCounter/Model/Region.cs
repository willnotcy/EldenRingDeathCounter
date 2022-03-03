using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EldenRingDeathCounter.Model
{
    public class Region : IRegion
    {
        public string Name { get; set; }

        public bool Special { get; set; } = false;
        public bool LegacyDungeon { get; set; } = false;
        public bool Underground { get; set; } = false;

        public override string ToString()
        {
            return !(Special || LegacyDungeon || Underground) ? $"in {Name}" : "";
        }
    }
}
