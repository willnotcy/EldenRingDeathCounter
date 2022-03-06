using EldenRingDeathCounter.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EldenRingDeathCounter.Util
{
    public class BossDetector : DetectorBase
    {
        private static readonly BossHelper _helper = BossHelper.Instance;
        private static readonly Vector4 TargetWhite = new Vector4(0.85f, 0.85f, 0.85f, 1);

        protected override float XOffset => 0.757f;

        protected override float YOffset => 0.222f;

        protected override float WidthOffset => 0.271f;

        protected override float HeightOffset => 0.022f;

        public bool TryDetectBoss(Image<Rgba32> bmp, ILocation location, out IBoss boss, out Image<Rgba32> debug, out string debugReading)
        {
            debug = null;
            debugReading = "";
            boss = null;

            var cropped = CropImage(bmp);
            if (TryDetect(cropped, TargetWhite, out string bossName, out debug, out debugReading))
            {
                return _helper.TryGetBoss(bossName, location, out boss);
            }

            return false;
        }
    }
}
