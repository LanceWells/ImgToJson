using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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
    /// <returns></returns>
    public static ImageOutput JsonImageData(Image bitmap, Rgba32[] bitmapPixelData)
    {
      // First, get the pixel information as a workable object, rather than a byte array.
      PixelMap pixelMap = new PixelMap(bitmap, bitmapPixelData);

      // Now, determine what the largest encompassing rectangle is for each pixel, using that pixel as the top-
      // left corner of the rectangle itself. The idea here is that if we find the largest rectangles, a large
      // number of those similarly-colored rectangles will be encompassed by a larger rectangle. The end goal is
      // to have a low number of rectangles required to fully render the image using an HTML canvas element.
      var rectanglesByColor = new Dictionary<ARGBFormatColor, List<ColorRectangle>>();

      List<ARGBFormatColor> sortedColors = pixelMap.Pixels
        .OrderByDescending((r) => r.Value.Count)
        .Select((r) => r.Key)
        .ToList();

      Dictionary<ARGBFormatColor, int> colorIndex = new Dictionary<ARGBFormatColor, int>();
      for (int i = 0; i < sortedColors.Count; i++)
      {
        colorIndex.Add(sortedColors[i], i);
      }

      for (int xCoord = 0; xCoord < bitmap.Width; xCoord++)
      {
        for (int yCoord = 0; yCoord < bitmap.Height; yCoord++)
        {
          PixelIndex thisPixel = new PixelIndex() { X = xCoord, Y = yCoord };
          ARGBFormatColor? thisColor = pixelMap.LookupPixel(thisPixel);

          List<ARGBFormatColor> availableColors = new List<ARGBFormatColor>();
          if (thisColor.HasValue)
          {
            availableColors = sortedColors.Skip(colorIndex[thisColor.Value]).ToList();

            ColorRectangle rectangle = GetLargestRect(
                pixelMap,
                availableColors,
                xCoord,
                yCoord,
                bitmap.Width,
                bitmap.Height
            );

            if (!rectanglesByColor.ContainsKey(thisColor.Value))
            {
              rectanglesByColor.Add(thisColor.Value, new List<ColorRectangle>());
            }

            rectanglesByColor[thisColor.Value].Add(rectangle);
          }
        }
      }

      ImageOutput output = new ImageOutput();

      foreach (ARGBFormatColor color in sortedColors)
      {
        List<ColorRectangle> colorRectangles = rectanglesByColor[color];
        List<ColorRectangle> limitedRectangles = new List<ColorRectangle>();
        List<ColorRectangle> orderedRectangles = colorRectangles
          .OrderByDescending((r) => r.RectangleSize)
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
        output.Add(new JsonImageColor(color, boundaries, colorIndex[color]));
      }

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
    private static ColorRectangle GetLargestRect(
        PixelMap pixelMap,
        List<ARGBFormatColor> availableColors,
        int xCoord,
        int yCoord,
        int width,
        int height)
    {
      int maxRect = 1;
      int farthestColumn = width - 1;
      int[,] largestRectangle = new int[,]
      {
        { xCoord, yCoord },
        { xCoord, yCoord },
      };

      for (int rowIndex = yCoord; rowIndex < height; rowIndex++)
      {
        for (int columnIndex = xCoord; columnIndex <= farthestColumn; columnIndex++)
        {
          PixelIndex pixelIndex = new PixelIndex()
          {
            X = columnIndex,
            Y = rowIndex,
          };

          bool pixelAtIndex = pixelMap.PixelIsColor(pixelIndex, availableColors);

          if (pixelAtIndex)
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

      return new ColorRectangle(availableColors[0], largestRectangle);
    }
  }
}
