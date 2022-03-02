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

        public bool TryDetectDeath(Image<Rgba32> bmp, out Image<Rgba32> debug, out string debugReading)
        {
            if(TryCropImage(bmp, GetBounds(bmp), out Image<Rgba32> cropped))
            {
                return TryDetect(bmp, (location) => { return location.Equals("YOUDIED"); }, TargetRed ,out string result, out debug, out debugReading);
            }

            debug = null;
            debugReading = "";
            return false;
        }

        private Rectangle GetBounds(Image<Rgba32> bmp)
        {
            return new Rectangle((int)(bmp.Width - (bmp.Width * 0.61)), (int)(bmp.Height - (bmp.Height * 0.535)), (int)(bmp.Width * 0.22), (int)(bmp.Height * 0.07));
        }
    }
}
