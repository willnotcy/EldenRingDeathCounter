using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EldenRingDeathCounter.Util
{
    public class LocationDetector : DetectorBase
    {
        bool TryDetectLocation(Image<Rgba32> bmp, out string location, out Image<Rgba32> debug, out string debugReading)
        {
            return TryDetect(bmp, (location) => { return location.Equals("Stormhill");  }, out location, out debug, out debugReading);
        }
    }
}
