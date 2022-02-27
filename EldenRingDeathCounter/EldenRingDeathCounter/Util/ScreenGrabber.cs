using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace EldenRingDeathCounter.Util
{
    public static class ScreenGrabber
    {
        private static readonly BmpDecoder BmpDecoder = new BmpDecoder();

        public static Image<Rgba32> TakeScreenshot()
        {
            Screen screen = Screen.PrimaryScreen;

            using Bitmap bmp = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format32bppRgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size, CopyPixelOperation.SourceCopy);
            }

            return ToImageSharp(bmp);
        }


        /// <summary>
        /// Converts a System.Drawing image to an ImageSharp image.
        /// </summary>
        /// <param name="bmp">The System.Drawing image.</param>
        /// <returns>The ImageSharp image.</returns>
        public static Image<Rgba32> ToImageSharp(Bitmap bmp)
        {
            if (bmp is null)
            {
                throw new ArgumentNullException(nameof(bmp));
            }

            using MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Bmp);
            ms.Position = 0;
            return SixLabors.ImageSharp.Image.Load<Rgba32>(ms, BmpDecoder);
        }
    }
}
