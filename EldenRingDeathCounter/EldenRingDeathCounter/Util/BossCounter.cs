using EldenRingDeathCounter.Model;
using Microsoft.Isam.Esent.Collections.Generic;

namespace EldenRingDeathCounter.Util
{
    public class BossCounter
    {
        private static readonly BossCounter instance = new();
        private PersistentDictionary<string, int> bossCounts;

        private BossCounter()
        {
            bossCounts = new("bossCounts");
        }

        public static BossCounter Instance { get { return instance; } }

        public bool TryGetCount(IBoss boss, out int count)
        {
            string key = KeyFromBoss(boss);
            count = -1;

            if (key.Equals(""))
            {
                return false;
            }

            if (bossCounts.ContainsKey(key))
            {
                count = bossCounts[key];
                bossCounts.Flush();
                return true;
            }

            bossCounts.Add(key, 0);
            bossCounts.Flush();

            return false;
        }

        public bool TryIncrementCount(IBoss boss)
        {
            string key = KeyFromBoss(boss);

            if(key.Equals(""))
            {
                return false;
            }

            if (bossCounts.ContainsKey(key))
            {
                bossCounts[key] = bossCounts[key] + 1;
                bossCounts.Flush();
                return true;
            }

            bossCounts.Add(key, 1);
            bossCounts.Flush();

            return false;
        }

        private string KeyFromBoss(IBoss boss)
        {
            return boss is null ? "" : $"{boss.Name}{boss.Region.Name}".ToLower().Trim().Replace(" ", "");
        }
    }
}
