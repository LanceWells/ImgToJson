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
        /// <summary>
        /// The bitmap information as a 2D array.
        /// </summary>
        public ARGBFormatColor[,] Pixels;

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
            Dictionary<ARGBFormatColor, int> _colorMap = new Dictionary<ARGBFormatColor, int>();
            ARGBFormatColor[,] pixelMap = new ARGBFormatColor[bitmap.Height, bitmap.Width];

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

                    if (!_colorMap.ContainsKey(color))
                    {
                        int colorIndex = _colorMap.Count;
                        _colorMap.Add(color, colorIndex);
                    }

                    pixelMap[xIndex, yIndex] = color;
                    offset += 4;
                }

                rowOffset += bitmapStride;
            }

            Pixels = pixelMap;
        }
    }
}
