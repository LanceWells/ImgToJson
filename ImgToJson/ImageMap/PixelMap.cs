using System.Collections.Generic;
using System.Drawing;

namespace ImgToJson.ImageMap
{
    /// <summary>
    /// A pixel container used to represent a bitmap with X/Y coordinates and the associated color information at each
    /// coordinate.
    /// </summary>
    public class PixelMap
    {
        private Dictionary<PixelIndex, ARGBFormatColor> _pixelStorage = new Dictionary<PixelIndex, ARGBFormatColor>();

        /// <summary>
        /// The bitmap information as a 2D array.
        /// </summary>
        public Dictionary<ARGBFormatColor, List<PixelIndex>> Pixels { get; }

        /// <summary>
        /// Returns true if the given pixel location contains any of the colors in the provided list.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="colorFilter"></param>
        /// <returns></returns>
        public bool PixelIsColor(PixelIndex index, List<ARGBFormatColor> colorFilter)
        {
            ARGBFormatColor? color = LookupPixel(index);
            return color != null && colorFilter.Contains(color.Value);
        }

        /// <summary>
        /// Returns the color of the pixel at the provided index. If the pixel is invalid, returns null.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ARGBFormatColor? LookupPixel(PixelIndex index)
        {
            ARGBFormatColor? color = null;
            if (_pixelStorage.ContainsKey(index))
            {
                color = _pixelStorage[index];
            }

            return color;
        }

        /// <summary>
        /// The constructor for this type.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap information for the associated image.
        /// </param>
        /// <param name="bitmapPixelData">
        /// The raw pixel data for the image in question. This should come from the LockBits function.
        /// </param>
        /// <param name="bitmapStride">
        /// The stride for the associated bitmap.
        /// </param>
        public PixelMap(Bitmap bitmap, byte[] bitmapPixelData, int bitmapStride)
        {
            //ARGBFormatColor[,] pixelMap = new ARGBFormatColor[bitmap.Height, bitmap.Width];
            var pixelMap = new Dictionary<ARGBFormatColor, List<PixelIndex>>();

            int rowOffset = 0;
            for (int yIndex = 0; yIndex < bitmap.Height; yIndex++)
            {
                int offset = rowOffset;
                for (int xIndex = 0; xIndex < bitmap.Width; xIndex++)
                {
                    byte b = bitmapPixelData[offset];
                    byte g = bitmapPixelData[offset + 1];
                    byte r = bitmapPixelData[offset + 2];
                    byte a = bitmapPixelData[offset + 3];
                    ARGBFormatColor color = new ARGBFormatColor(a, r, g, b);

                    PixelIndex index = new PixelIndex { XValue = xIndex, YValue = yIndex };
                    if (!pixelMap.ContainsKey(color))
                    {
                        pixelMap.Add(color, new List<PixelIndex>());
                    }

                    //pixelMap[xIndex, yIndex] = color;
                    _pixelStorage.Add(index, color);
                    pixelMap[color].Add(index);

                    offset += 4;
                }

                rowOffset += bitmapStride;
            }

            Pixels = pixelMap;
        }
    }

    /// <summary>
    /// A structure used to represent the index of a specific pixel.
    /// </summary>
    public struct PixelIndex
    {
        /// <summary>
        /// The x-value in a 2D matrix for this pixel.
        /// </summary>
        public int XValue;

        /// <summary>
        /// The y-value in a 2D matrix for this pixel.
        /// </summary>
        public int YValue;
    }
}
