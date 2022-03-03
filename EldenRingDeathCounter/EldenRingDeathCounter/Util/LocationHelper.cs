using EldenRingDeathCounter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EldenRingDeathCounter.Util
{
    public class LocationHelper
    {
        private readonly static LocationHelper instance = new();
        private readonly Dictionary<string, ILocation> locations = new();
        private readonly string resourcePath = "../../../Resources/Regions/";
        
        private LocationHelper()
        {
            BuildUp();
        }

        public static LocationHelper Instance { get { return instance; } }


        private void BuildUp()
        {
            foreach (string file in Directory.EnumerateFiles(resourcePath, "*", SearchOption.AllDirectories))
            {
                string regionName = Path.GetFileNameWithoutExtension(file);

                var region = new Region() { Name = regionName};
                var location = new Location() { Name = regionName, Region = region };
                locations.Add(regionName.Replace(" ", "").ToLower(), location);

                if(regionName.Equals("Legacy Dungeons"))
                {
                    region.LegacyDungeon = true;
                }
                if (regionName.Equals("Special"))
                {
                    region.Special = true;
                }
                if (regionName.Equals("Undergound"))
                {
                    region.Underground = true;
                }

                using (StreamReader reader = File.OpenText(file))
                {
                    while (!reader.EndOfStream)
                    {
                        string locationName = reader.ReadLine();
                        string key = locationName.Replace(" ", "").ToLower();

                        location = new Location() { Name = locationName, Region = region };

                        if(locations.ContainsKey(key))
                        {
                            location.MultiRegion = true;
                            locations[key] = location;

                        } else
                        {
                            locations.Add(key, location);
                        }
                    }
                }
            }
        }

        public bool TryGetLocation(string key, out ILocation location)
        {
            return locations.TryGetValue(key, out location);
        }

        public bool ValidLocation(string key)
        {
            return locations.ContainsKey(key);
        }
    }
}
