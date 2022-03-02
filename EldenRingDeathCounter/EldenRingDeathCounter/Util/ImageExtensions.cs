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
        private static readonly Vector4 Red = new Vector4(1, 0, 0, 1);
        private static readonly Vector4 TargetRed = new Vector4(0.66f, 0, 0, 1);
        private static readonly Vector4 TargetWhite = new Vector4(0.811f, 0.811f, 0.819f, 1);

        public static IImageProcessingContext RedFilter(this IImageProcessingContext context, float threshold)
            => context.ProcessPixelRowsAsVector4(r =>
            {
                for (int x = 0; x < r.Length; x++)
                {
                    // Filter on target
                    var p = r[x];

                    var redDifference = p - TargetRed;
                    var redDistance = redDifference.Length();

                    var whiteDifference = p - TargetWhite;
                    var whiteDistance = whiteDifference.Length();

                    r[x] = (redDistance < threshold) || (whiteDistance < threshold) ? Black : White;
                }
                //ColorFilter(context, threshold, TargetRed);
            });


        public static IImageProcessingContext WhiteFilter(this IImageProcessingContext context, float threshold)
         => context.ProcessPixelRowsAsVector4(r =>
         {
            ColorFilter(context, threshold, TargetWhite);
         });


        private static IImageProcessingContext ColorFilter(this IImageProcessingContext context, float threshold, Vector4 targetColor)
            => context.ProcessPixelRowsAsVector4(r =>
            {
                for (int x = 0; x < r.Length; x++)
                {
                    // Filter on target
                    var p = r[x];

                    var difference = p - targetColor;
                    var distance = difference.Length();

                    r[x] = distance < threshold ? Black : White;
                }
            });

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
