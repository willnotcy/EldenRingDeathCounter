using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using TesserNet;

namespace EldenRingDeathCounter.Util
{
    public abstract class DetectorBase
    {
        private readonly ITesseract tesseract = new TesseractPool(new TesseractOptions
        {
            PageSegmentation = PageSegmentation.Line,
            Numeric = true,
            Whitelist = "ABCDEFGHIJKLMNOPQRSTUVXYZ",
        });

        private float threshold = 0.24f;
        protected bool TryCropImage(Image<Rgba32> bmp, Rectangle bounds, out Image<Rgba32> cropped)
        {
            bmp.Mutate(
                x => x.Crop(bounds));
            cropped = bmp.Clone();

            return true;
        }

        protected bool TryDetect(Image<Rgba32> bmp, Func<string, bool> matcher, out string result, out Image<Rgba32> debug, out string debugReading)
        {
            if (bmp is null)
            {
                throw new ArgumentNullException(nameof(bmp));
            }
   
            debugReading = "";
            result = "";

            // Prepare image for OCR
            bmp.Mutate(x => x
            .RedFilter(threshold)
            .Dilate(3));

            debug = bmp.Clone();

            if (!TryReadImage(bmp, out string reading))
            {
                return false;
            }

            debugReading = reading;
            Console.WriteLine($"Read: {reading}");

            if (matcher(reading))
            {
                result = reading;
                return true;
            }

            return false;
        }


        private bool TryReadImage(Image<Rgba32> bmp, out string reading)
        {
            reading = "";
            bmp.Metadata.HorizontalResolution = 300;
            bmp.Metadata.VerticalResolution = 300;

            try
            {
                reading = tesseract.Read(bmp).Trim();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (reading.Length > 0)
            {
                return true;
            }

            return false;
        }
    }
}
