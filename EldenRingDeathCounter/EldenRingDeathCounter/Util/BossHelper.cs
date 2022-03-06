﻿using EldenRingDeathCounter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EldenRingDeathCounter.Util
{
    public class BossHelper
    {
        private readonly static BossHelper instance = new();
        private readonly List<IBoss> bosses = new();
        private readonly string resourcePath = "../../../Resources/Bosses/";
        private readonly string variantRegex = "(?<=[(]).*(?=[)])";
        private readonly string bossRegex = ".*(?=[(])";
        private readonly LocationHelper locationHelper = LocationHelper.Instance;

        public static BossHelper Instance { get { return instance; } }

        private BossHelper()
        {
            BuildUp();
        }

        private void BuildUp()
        {
            foreach (string file in Directory.EnumerateFiles(resourcePath, "*", SearchOption.AllDirectories))
            {
                string regionName = Path.GetFileNameWithoutExtension(file);

                var region = locationHelper.GetRegion(regionName);

                using (StreamReader reader = File.OpenText(file))
                {
                    while(!reader.EndOfStream)
                    {
                        var entry = reader.ReadLine();
                        var name = Regex.Match(entry, bossRegex).Value.Trim();
                        var variant = Regex.Match(entry, variantRegex).Value ?? "";

                        var boss = new Boss() { Name = name, Region = region, Variant = variant };

                        bosses.Add(boss);
                    }
                }
            }
        }

        public bool TryGetBoss(string name, ILocation location, out IBoss boss)
        {
            boss = null;
            var candidates = bosses.Where(b => b.Name.ToLower().Replace(" ", "").Equals(name));
            
            if (candidates.Count() == 0)
            {
                return false;
            }

            if (candidates.Count() == 1)
            {
                boss = candidates.First();
                return true;
            }

            if (location is null)
            {
                return false;
            }

            candidates = candidates.Where(b => b.Region.Name.Equals(location.Region.Name));

            if (candidates.Count() == 1)
            {
                boss = candidates.First();
                return true;
            }

            boss = candidates.Where(b => b.Variant.Equals("") || b.Variant.Equals(location.Name)).FirstOrDefault();

            return boss is not null;               
        }
    }
}
