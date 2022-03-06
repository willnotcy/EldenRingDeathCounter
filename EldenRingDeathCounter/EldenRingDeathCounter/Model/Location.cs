using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EldenRingDeathCounter.Model
{
    public class Location : ILocation
    {
        public string Name { get; set; }
        public IRegion Region { get; set; }

        public override string ToString()
        {
            return Name.Equals(Region.Name) ? Name : $"{Name} {Region}";
        }
    }
}
