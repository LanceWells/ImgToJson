using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImgToJson.ImageMap
{
    /// <summary>
    /// A helper class used to convert a bitmap into a JSON-friendly object.
    /// </summary>
    public static class JsonImageMapper
    {
        /// <summary>
        /// Converts bitmap data into a JSON-friendly object.
        /// </summary>
        /// <param name="bitmap">The raw bitmap data.</param>
        /// <param name="bitmapPixelData">The bitmap pixel data, taken from the lockbits for a bitmap.</param>
        /// <param name="bitmapStride">The stride for the bitmap data.</param>
        /// <returns></returns>
        public static JsonImageOutput JsonImageData(Bitmap bitmap, byte[] bitmapPixelData, int bitmapStride)
        {
            // First, get the pixel information as a workable object, rather than a byte array.
            PixelMap pixelMap = new PixelMap(bitmap, bitmapPixelData, bitmapStride);

            // Now, determine what the largest encompassing rectangle is for each pixel, using that pixel as the top-
            // left corner of the rectangle itself. The idea here is that if we find the largest rectangles, a large
            // number of those similarly-colored rectangles will be encompassed by a larger rectangle. The end goal is
            // to have a low number of rectangles required to fully render the image using an HTML canvas element.
            List<ColorRectangle> rectangles = new List<ColorRectangle>();
            for (int xCoord = 0; xCoord < pixelMap.Pixels.GetLength(1); xCoord++)
            {
                for (int yCooord = 0; yCooord < pixelMap.Pixels.GetLength(0); yCooord++)
                {
                    if (pixelMap.Pixels[xCoord, yCooord].A != 0)
                    {
                        ColorRectangle rectangle = GetLargestRect(pixelMap.Pixels, xCoord, yCooord);
                        rectangles.Add(rectangle);
                    }
                }
            }

            // Now that the rectangles have been measured, trim out any rectangles that are encompassed by another,
            // larger rectangle.
            var imageOutput = new Dictionary<ARGBFormatColor, List<int[,]>>();

            Dictionary<ARGBFormatColor, List<ColorRectangle>> rectanglesByColor = rectangles
                .GroupBy((r) => r.ColorIndex)
                .ToDictionary((r) => r.Key, (r) => r.ToList());

            foreach (ARGBFormatColor rectangleColor in rectanglesByColor.Keys)
            {
                List<ColorRectangle> limitedRectangles = new List<ColorRectangle>();
                List<ColorRectangle> orderedRectangles = rectanglesByColor[rectangleColor]
                    .OrderByDescending(r => r.RectangleSize)
                    .ToList();

                foreach (ColorRectangle rectangle in orderedRectangles)
                {
                    bool anyRectanglesEnclose = limitedRectangles.Any((r) => r.Encloses(rectangle));
                    if (!anyRectanglesEnclose)
                    {
                        limitedRectangles.Add(rectangle);
                    }
                }

                List<int[,]> boundaries = limitedRectangles.Select(l => l.RectangleBoundaries).ToList();
                imageOutput.Add(rectangleColor, boundaries);
            }

            JsonImageOutput output = new JsonImageOutput(imageOutput);
            return output;
        }

        /// <summary>
        /// A helper method used to get the largest containing rectangle with an upper-left corner at the provided x/y
        /// coordinates.
        /// </summary>
        /// <param name="pixelMap">
        /// The pixel map for an image, represented as a 2D array whose each point is an integer representing a
        /// specific color.
        /// </param>
        /// <param name="xCoord">The x coordinate of the upper-left corner of the rectangle to measure.</param>
        /// <param name="yCoord">The y coordinate of the upper-left corner of the rectangle to measure.</param>
        /// <returns>
        /// The largest encompasing rectangle of a specific color whose upper-left corner originates at the provided
        /// x/y coordinates.
        /// </returns>
        private static ColorRectangle GetLargestRect(ARGBFormatColor[,] pixelMap, int xCoord, int yCoord)
        {
            ARGBFormatColor colorIndex = pixelMap[xCoord, yCoord];
            int maxRect = 1;
            int farthestColumn = pixelMap.GetLength(0) - 1;

            int[,] largestRectangle = new int[,]
            {
                { xCoord, yCoord },
                { xCoord, yCoord },
            };

            for (int rowIndex = yCoord; rowIndex < pixelMap.GetLength(1); rowIndex++)
            {
                if (!pixelMap[xCoord, rowIndex].Equals(colorIndex))
                {
                    break;
                }
                for (int columnIndex = xCoord; columnIndex <= farthestColumn; columnIndex++)
                {
                    ARGBFormatColor targetedColorIndex = pixelMap[columnIndex, rowIndex];
                    if (colorIndex.Equals(targetedColorIndex))
                    {
                        int rowCount = (rowIndex - yCoord) + 1;
                        int columnCount = (columnIndex - xCoord) + 1;
                        int rectSize = rowCount * columnCount;

                        if (rectSize > maxRect)
                        {
                            largestRectangle[1, 0] = columnIndex;
                            largestRectangle[1, 1] = rowIndex;
                            maxRect = rectSize;
                        }
                    }
                    else
                    {
                        farthestColumn = columnIndex - 1;
                        break;
                    }
                }
            }

            return new ColorRectangle(colorIndex, largestRectangle);
        }
    }
}
