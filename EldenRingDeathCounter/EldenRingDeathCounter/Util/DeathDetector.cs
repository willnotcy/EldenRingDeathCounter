using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Numerics;
using TesserNet;

namespace EldenRingDeathCounter.Util
{
    public class DeathDetector : DetectorBase
    {
        private static readonly Vector4 TargetRed = new Vector4(0.66f, 0, 0, 1);

        protected override float XOffset => 0.61f;

        protected override float YOffset => 0.535f;

        protected override float WidthOffset => 0.22f;

        protected override float HeightOffset => 0.07f;

        public bool TryDetectDeath(Image<Rgba32> bmp, out Image<Rgba32> debug, out string debugReading)
        {
            debug = null;
            debugReading = "";

            if (TryCropImage(bmp, out Image<Rgba32> cropped))
            {
                return TryDetect(cropped, (location) => { return location.Equals("youdied"); }, TargetRed ,out string result, out debug, out debugReading);
            }

            return false;
        }
    }
}
