using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using TesserNet;

namespace EldenRingDeathCounter.Util
{
    public class DeathDetector
    {
        private readonly ITesseract tesseract = new TesseractPool(new TesseractOptions
        {
            PageSegmentation = PageSegmentation.Line,
            Numeric = true,
            Whitelist = "YOUDIE",
        });

        private bool TryCropImage(Image<Rgba32> bmp, out Image<Rgba32> cropped)
        {
            bmp.Mutate(
                x => x.Crop(new Rectangle((int)(bmp.Width - (bmp.Width * 0.61)), (int)(bmp.Height - (bmp.Height * 0.535)), (int) (bmp.Width * 0.22), (int)(bmp.Height * 0.07))));
            cropped = bmp.Clone();

            return true;
        }

        public bool TryDetectDeath(Image<Rgba32> bmp, out bool dead, out Image<Rgba32> debug, out string debugReading, float threshold = 0.24f)
        {
            if (bmp is null)
            {
                throw new ArgumentNullException(nameof(bmp));
            }
            dead = false;
            debugReading = "";

            if (TryCropImage(bmp, out Image<Rgba32> cropped))
            {
                cropped.Mutate(x => x
                .RedFilter(threshold)
                .Dilate(3));
            }

            debug = cropped.Clone();

            if (!TryReadImage(bmp, out string reading))
            {
                return false;
            }

            debugReading = reading;

            if(reading.Equals("YOUDIED"))
            {
                dead = true;
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
            } catch (Exception e)
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
