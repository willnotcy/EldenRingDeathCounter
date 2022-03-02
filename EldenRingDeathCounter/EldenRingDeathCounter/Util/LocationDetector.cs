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
    public class LocationDetector : DetectorBase
    {
        private static readonly Vector4 TargetWhite = new Vector4(0.811f, 0.811f, 0.819f, 1);

        public bool TryDetectLocation(Image<Rgba32> bmp, out string location, out Image<Rgba32> debug, out string debugReading)
        {
            if (TryCropImage(bmp, GetBounds(bmp), out Image<Rgba32> cropped))
            {
                return TryDetect(bmp, (location) => { return location.Equals("Stormhill"); }, TargetWhite, out location, out debug, out debugReading);
            }

            debug = null;
            debugReading = "";
            location = "";
            return false;
        }

        private Rectangle GetBounds(Image<Rgba32> bmp)
        {
            return new Rectangle((int)(bmp.Width - (bmp.Width * 0.61)), (int)(bmp.Height - (bmp.Height * 0.535)), (int)(bmp.Width * 0.22), (int)(bmp.Height * 0.07));
        }
    }
}
