using EldenRingDeathCounter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EldenRingDeathCounter.Util
{
    public class LocationHelper
    {
        private readonly static LocationHelper instance = new();
        private readonly List<IRegion> regions = new();

        private LocationHelper()
        {
            BuildUp();
        }

        public static LocationHelper Instance { get { return instance; } }


        private void BuildUp()
        {
            var embeddedRegionFiles = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(t => t.Contains(".Regions.")).ToList();
            
            foreach (string resource in embeddedRegionFiles)
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource)) {
                    string regionName = resource.Replace(".txt", "").Split('.').Last();

                    var mainRegion = new Region() { Name = regionName, Locations = new(), ParentRegion = null, SubRegions = new() };
                    var currentRegion = mainRegion;

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                        {
                            string locationName = reader.ReadLine();

                            if (locationName.Contains("$"))
                            {
                                if (!currentRegion.Name.Equals(mainRegion.Name))
                                {
                                    mainRegion.SubRegions.Add(currentRegion);
                                }

                                var subRegionName = Regex.Match(locationName, @"(?<=[$])(.*)(?=[$])").Value.Trim();

                                currentRegion = new Region { Name = subRegionName, Locations = new(), ParentRegion = mainRegion, SubRegions = new() };

                                continue;
                            }

                            var location = new Location() { Name = locationName, Region = currentRegion };
                            currentRegion.Locations.Add(location);
                        }
                    }
                    mainRegion.SubRegions.Add(currentRegion);
                    regions.Add(mainRegion);
                }
            }
        }

        public bool TryGetLocation(string key, ILocation currentLocation, out ILocation location)
        {
            location = null;

            var regionCandidates = regions.Where(r => Format(r.Name).Equals(key));

            if (regionCandidates.Count() == 1)
            {
                location = regionCandidates.First().Locations.First();
                return true;
            }


            var candidates = regions.SelectMany(r => r.Locations)
                                    .Concat(regions.SelectMany(r => r.SubRegions)
                                                   .SelectMany(sr => sr.Locations))
                                    .Where(l => Format(l.Name).Equals(key));

            if (candidates.Count() == 0)
            {
                return false;
            }

            if (candidates.Count() == 1)
            {
                location = candidates.First();
                return true;
            }

            if(currentLocation == null)
            {
                return false;
            }

            candidates = candidates.Where(l => l.Region.Name.Equals(currentLocation.Region.Name));

            if(candidates.Count() == 1)
            {
                location = candidates.First();
                return true;
            }

            return location is not null;
        }


        public IRegion GetRegion(string name)
        {
            return regions.Where(r => r.Name.Equals(name)).First();
        }

        private string Format(string str)
        {
            return str.Trim().ToLower().Replace(" ", "");
        }
    }
}
