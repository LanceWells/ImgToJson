using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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
    public PixelMap(Image bitmap, Rgba32[] bitmapPixelData)
    {
      var pixelMap = new Dictionary<ARGBFormatColor, List<PixelIndex>>();
      // int offset = bitmap.Width;
      int rowOffset = 0;

      for (int y = 0; y < bitmap.Height; y++) {
        for (int x = 0; x < bitmap.Width; x++) {
          var pixel = bitmapPixelData[y * bitmap.Width + x];
          ARGBFormatColor color = new ARGBFormatColor(pixel.A, pixel.R, pixel.G, pixel.B);

          if (color.A == 0) {
            continue;
          }

          var index = new PixelIndex { X = x, Y = y };
          if (!pixelMap.ContainsKey(color)) {
            pixelMap.Add(color, new List<PixelIndex>());
          }

          _pixelStorage.Add(index, color);
          pixelMap[color].Add(index);
        }

        rowOffset++;
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
    public int X;

    /// <summary>
    /// The y-value in a 2D matrix for this pixel.
    /// </summary>
    public int Y;
  }
}
