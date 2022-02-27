using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EldenRingDeathCounter.Util
{
    public static class ImageExtensions
    {
        private static readonly Vector4 White = new Vector4(1, 1, 1, 1);
        private static readonly Vector4 Black = new Vector4(0, 0, 0, 1);
        private static readonly Vector4 TargetRed = new Vector4(0.66f, 0, 0, 1);


        public static IImageProcessingContext RedFilter(this IImageProcessingContext context, float threshold)
         => context.ProcessPixelRowsAsVector4(r =>
         {
             for (int x = 0; x < r.Length; x++)
             {
                 var p = r[x];

                 var difference = p - TargetRed;
                 var distance = difference.Length();

                 r[x] = distance < threshold ? Black : White;
             }
         });


        public static int BlackPixelCount(Image<Rgba32> bmp)
        {
            int count = 0;
            int antiCount = 0;
            HashSet<Rgba32> pixels = new HashSet<Rgba32>();

            bmp.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(y);

                    for (int x = 0; x < pixelRow.Length; x++)
                    { 
                        // Get a reference to the pixel at position x
                        ref Rgba32 pixel = ref pixelRow[x];
                        pixels.Add(pixel);
                        if (pixel.R == 0)
                        {
                            count++;
                        } 
                        else
                        {
                            antiCount++;
                        }
                    }
                }

            });

            return count;
        }

        public static IImageProcessingContext Dilate(this IImageProcessingContext context, int radius)
            => context.BoxBlur(radius)
            .ProcessPixelRowsAsVector4(r =>
            {
                for (int x = 0; x < r.Length; x++)
                {
                    Vector4 c = r[x];
                    r[x] = c.X == 1 && c.Y == 1 && c.Z == 1 ? White : Black;
                }
            });
    }
}
