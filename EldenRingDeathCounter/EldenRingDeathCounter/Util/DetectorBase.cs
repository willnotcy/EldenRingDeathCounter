using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Numerics;
using TesserNet;

namespace EldenRingDeathCounter.Util
{
    public abstract class DetectorBase
    {
        protected abstract float XOffset { get; }
        protected abstract float YOffset { get; }
        protected abstract float WidthOffset { get; }
        protected abstract float HeightOffset { get; }

        private readonly ITesseract tesseract = new TesseractPool(new TesseractOptions
        {
            PageSegmentation = PageSegmentation.Line,
            Numeric = false,
            Whitelist = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz',",
        });

        protected virtual float Threshold { get; } = 0.24f;
        protected Image<Rgba32> CropImage(Image<Rgba32> bmp)
        {
            return CropImage(bmp, GetBounds(bmp));
        }

        protected Image<Rgba32> CropImage(Image<Rgba32> bmp, Rectangle bounds)
        {
            bmp.Mutate(
                x => x.Crop(bounds));

            return bmp;
        }

        protected bool TryDetect(Image<Rgba32> bmp, Vector4 targetColor, out string result, out Image<Rgba32> debug, out string debugReading)
        {
            if (bmp is null)
            {
                throw new ArgumentNullException(nameof(bmp));
            }
   
            debugReading = "";
            result = "";

            // Prepare image for OCR
            bmp.Mutate(x => x
            .ColorFilter(Threshold, targetColor)
            .Dilate(2));

            debug = bmp.Clone();

            if (!TryReadImage(bmp, out string reading))
            {
                return false;
            }

            debugReading = reading;
            result = reading;
            Console.WriteLine($"Read: {reading}");

            return true;
        }


        private bool TryReadImage(Image<Rgba32> bmp, out string reading)
        {
            reading = "";
            bmp.Metadata.HorizontalResolution = 400;
            bmp.Metadata.VerticalResolution = 400;

            try
            {
                reading = tesseract.Read(bmp).Trim().Replace(" ", "").ToLower();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (reading.Length > 2)
            {
                return true;
            }

            return false;
        }
        
        protected Rectangle GetBounds(Image<Rgba32> bmp)
        {
            return new Rectangle(GetX(bmp, XOffset), GetY(bmp, YOffset), GetWidth(bmp, WidthOffset), GetHeight(bmp, HeightOffset));
        }

        protected int GetX(Image<Rgba32> bmp, float xOffset)
        {
            return (int)(bmp.Width - (bmp.Width * xOffset));
        }
        protected int GetY(Image<Rgba32> bmp, float yOffset)
        {
            return (int)(bmp.Height - (bmp.Height * yOffset));
        }
        protected int GetWidth(Image<Rgba32> bmp, float widthOffset)
        {
            return (int)(bmp.Width * widthOffset);
        }
        protected int GetHeight(Image<Rgba32> bmp, float heightOffset)
        {
            return (int)(bmp.Height * heightOffset);
        }
    }
}
