using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EldenRingDeathCounter.Model
{
    public interface IRegion
    {
        public string Name { get; set; }

        public bool Special { get; set; }
        
        public bool LegacyDungeon { get; set; }

        public bool Underground { get; set; }
    }
}
