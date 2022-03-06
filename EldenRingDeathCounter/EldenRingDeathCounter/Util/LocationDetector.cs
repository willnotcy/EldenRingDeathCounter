using EldenRingDeathCounter.Model;
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
        //private static readonly Vector4 TargetWhite = new Vector4(0.811f, 0.811f, 0.819f, 1);
        private static readonly Vector4 TargetWhite = new Vector4(0.85f, 0.85f, 0.85f, 1);
        private static readonly LocationHelper _helper = LocationHelper.Instance;

        protected override float Threshold => 0.3f;

        protected override float XOffset => 0.654f;

        protected override float YOffset => 0.685f;

        protected override float WidthOffset => 0.306f;

        protected override float HeightOffset => 0.05f;

        protected float XOffset2 => 0.654f;

        protected float YOffset2 => 0.621f;

        protected float WidthOffset2 => 0.306f;

        protected float HeightOffset2 => 0.05f;

        protected float XOffset3 => 0.721f;

        protected float YOffset3 => 0.541f;

        protected float WidthOffset3 => 0.444f;

        protected float HeightOffset3 => 0.093f;

        public bool TryDetectLocation(Image<Rgba32> bmp, ILocation currentLocation, out ILocation location, out Image<Rgba32> debug, out string debugReading)
        {
            debug = null;
            debugReading = "";
            location = null;

            var clone = bmp.Clone();
            var cropped = CropImage(clone, GetBoundsLoc1(bmp));

            if (TryDetectLocationInCrop(cropped, currentLocation, out location, out debug, out debugReading))
            {
                return true;
            }

            clone = bmp.Clone();
            cropped = CropImage(clone, GetBoundsLoc2(bmp));
            if (TryDetectLocationInCrop(cropped, currentLocation, out location, out debug, out debugReading))
            {
                return true;
            }

            clone = bmp.Clone();
            cropped = CropImage(clone, GetBoundsLoc3(bmp));
            return TryDetectLocationInCrop(cropped, currentLocation, out location, out debug, out debugReading);
        }

        private bool TryDetectLocationInCrop(Image<Rgba32> cropped, ILocation currentLocation, out ILocation location, out Image<Rgba32> debug, out string debugReading)
        {
            debug = null;
            debugReading = "";
            location = null;

            if(TryDetect(cropped, TargetWhite, out string locationName, out debug, out debugReading))
            {
                return _helper.TryGetLocation(locationName, currentLocation, out location);
            }

            return false;
        }

        protected Rectangle GetBoundsLoc1(Image<Rgba32> bmp)
        {
            return new Rectangle(GetX(bmp, XOffset), GetY(bmp, YOffset), GetWidth(bmp, WidthOffset), GetHeight(bmp, HeightOffset));
        }

        protected Rectangle GetBoundsLoc2(Image<Rgba32> bmp)
        {
            return new Rectangle(GetX(bmp, XOffset2), GetY(bmp, YOffset2), GetWidth(bmp, WidthOffset2), GetHeight(bmp, HeightOffset2));
        }

        protected Rectangle GetBoundsLoc3(Image<Rgba32> bmp)
        {
            return new Rectangle(GetX(bmp, XOffset3), GetY(bmp, YOffset3), GetWidth(bmp, WidthOffset3), GetHeight(bmp, HeightOffset3));
        }
    }
}
