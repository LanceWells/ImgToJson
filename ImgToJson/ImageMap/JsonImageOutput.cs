using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace ImgToJson.ImageMap
{
  /// <summary>
  /// A JSON-friendly object used to contain image information.
  /// </summary>
  public class ImageOutput
  {
    /// <summary>
    /// The color-aware bitmap information for the image.
    /// </summary>
    public List<JsonImageColor> Colors { get; }

    /// <summary>
    /// The constructor for this type.
    /// </summary>
    /// <param name="colors">The color information to contain by this class.</param>
    public ImageOutput()
    {
      Colors = new List<JsonImageColor>();
    }

    /// <summary>
    /// Adds a color to this image's expected output.
    /// </summary>
    /// <param name="color"></param>
    public void Add(JsonImageColor color)
    {
      Colors.Add(color);
    }

    /// <summary>
    /// Returns this object as a JSON string.
    /// </summary>
    /// <returns></returns>
    public string AsJson()
    {
      Dictionary<string, ColorMapData> colorDict = Colors.ToDictionary(
        (c) => c.Color.ToString(),
        (c) => new ColorMapData(c.Rectangles, c.DrawIndex)
      );

      return JsonConvert.SerializeObject(colorDict);
    }

    /// <summary>
    /// Returns this object as a string that contains minimal extra characters. This option is used
    /// to reduce the overall size of the output.
    /// </summary>
    public string AsMinText()
    {
      Dictionary<string, ColorMapData> colorDict = Colors.ToDictionary(
        (c) => c.Color.ToString(),
        (c) => new ColorMapData(c.Rectangles, c.DrawIndex)
      );

      var builder = new StringBuilder();

      foreach (var kvp in colorDict.OrderBy(kvp => kvp.Value.DrawIndex)) {
        var rectStrings = kvp.Value.Rectangles.Select(r => {
          if (r[0,0] == r[1,0] && r[0,1] == r[1,1]) {
            return $"{r[0,0]},{r[1,1]}";
          }
          return $"{r[0,0]},{r[0,1]},{r[1,0]},{r[1,1]}";
        });
        
        var keyWithoutHash = kvp.Key.TrimStart('#');
        builder.AppendLine($"{keyWithoutHash}:{string.Join("|", rectStrings)}");
      }

      return builder.ToString();
    }

    /// <summary>
    /// A private struct for the express purpose of serializing the output for JSON.
    /// </summary>
    private readonly struct ColorMapData
    {
      public readonly List<int[,]> Rectangles;
      public readonly int DrawIndex;

      public ColorMapData(List<int[,]> rectangles, int drawIndex)
      {
        Rectangles = rectangles;
        DrawIndex = drawIndex;
      }
    }
  }

  /// <summary>
  /// A struct used to represent the rectangles associated with a given color.
  /// </summary>
  public readonly struct JsonImageColor
  {
    /// <summary>
    /// The color that this structure should represent.
    /// </summary>
    public readonly ARGBFormatColor Color;

    /// <summary>
    /// The series of rectangles to draw for this given color.
    /// </summary>
    public readonly List<int[,]> Rectangles;

    /// <summary>
    /// The index that this series of rectangles ought to be drawn at.
    /// </summary>
    public readonly int DrawIndex;

    /// <summary>
    /// A constructor for this object.
    /// </summary>
    /// <param name="color">The color to draw the associated rectangles with.</param>
    /// <param name="rectangles">The rectangles to draw in the provided color.</param>
    /// <param name="drawIndex">The numerical index associated with when this color ought to be drawn.</param>
    public JsonImageColor(ARGBFormatColor color, List<int[,]> rectangles, int drawIndex)
    {
      Color = color;
      Rectangles = rectangles;
      DrawIndex = drawIndex;
    }
  }
}
