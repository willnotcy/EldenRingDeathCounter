using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EldenRingDeathCounter.Model
{
    public interface ILocation
    {
        public string Name { get; set; }
        public IRegion Region { get; set; }
    }
}
