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
        public List<ILocation> Locations { get; set; }
        public List<IRegion> SubRegions { get; set; }
        public IRegion ParentRegion { get; set; }
    }
}
