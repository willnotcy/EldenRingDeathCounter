using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EldenRingDeathCounter.Util
{
    public class DeathDetector
    {
        private readonly int youDiedThreshhold = 10000;
        private bool TryCropImage(Image<Rgba32> bmp, out Image<Rgba32> cropped)
        {
            bmp.Mutate(
                x => x.Crop(new Rectangle((int)(bmp.Width - (bmp.Width * 0.61)), (int)(bmp.Height - (bmp.Height * 0.535)), (int) (bmp.Width * 0.22), (int)(bmp.Height * 0.07))));
            cropped = bmp.Clone();

            return true;
        }

        public bool TryDetectDeath(Image<Rgba32> bmp, out bool dead, out Image<Rgba32> debug, out int debugPixelCount, float threshold = 0.24f)
        {
            if (bmp is null)
            {
                throw new ArgumentNullException(nameof(bmp));
            }
            dead = false;

            if (TryCropImage(bmp, out Image<Rgba32> cropped))
            {
                cropped.Mutate(x => x
                .RedFilter(threshold)
                .Dilate(3));
                //cropped.Mutate(x => x
                //.RedFilter(threshold));
            }

            var blackPixels = ImageExtensions.BlackPixelCount(cropped);

            if (blackPixels > youDiedThreshhold)
            {
                dead = true;
            }

            debug = cropped.Clone();
            debugPixelCount = blackPixels;
            return true;
        }

    }
}
