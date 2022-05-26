using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using ImgToJson.ImageMap;

namespace ConsoleApp2
{
  /// <summary>
  /// A program designed to generate a JSON string for an image file. The output JSON string is intended to be used
  /// as a low-cost format for rendering on an HTML canvas, particularly in the context of low color count images.
  /// </summary>
  public class Program
  {
    /// <summary>
    /// The help string that will be displayed to the user.
    /// </summary>
    private static readonly string HelpString =
        @"
Usage:
=====

Options:
-------
-help: Show this menu

Args:
----
imagePath: The path to the image to get the JSON format string for.

Output:
------
A JSON-format string for the image. The format will be:
{
    ""ColorHexValue"": {    // This is the hex value of the color to draw. This is also the key for the record.
        ""Rectangles"": [
            [
                [0, 0],     // Top-left corner of the rectangle.
                [47, 47]    // Bottom-right corner of the rectangle.
            ]
        ],
        ""DrawIndex"": 0    // This index may be safely used to determine color draw-order.
    }
    . . .
}

Each color hex value will contain a list of rectangle coordinates, indicating the top-left and bottom-right
corners for a rectangle of the corresponding color that should be drawn.

Note that the output, in a json file, is about 7x the size of a .png image. This should not be used as a
low-cost storage format, but it is useful as a means to draw an image on a canvas where specific colors
might be changed without need filters or image manipulation.

Return Codes:
    0: No error. Note that this help menu will return '0'.
    1: The provided file does not exist at the specified path.
            ";

    /// <summary>
    /// A program designed to generate a JSON string for an image file. The output JSON string is intended to be
    /// used as a low-cost format for rendering on an HTML canvas, particularly in the context of low color count
    /// images.
    /// </summary>
    /// <param name="args">See the help string for more information.</param>
    public static int Main(string[] args)
    {
      if (args.Length == 0 || args[0].Trim('-') == "help")
      {
        Console.Out.WriteLine(HelpString);
        return 0;
      }

      string imgPath = args[0];

      if (!File.Exists(imgPath))
      {
        Console.Out.WriteLine("File does not exist at the provided path.");
        return 1;
      }

      using (FileStream imageFile = File.OpenRead(imgPath))
      {
        // https://docs.sixlabors.com/articles/imagesharp/pixelbuffers.html
        Image<Rgba32> bitmap = Image.Load<Rgba32>(imageFile);

        Rgba32[] pixelData = new Rgba32[bitmap.Width * bitmap.Height];
        bitmap.CopyPixelDataTo(pixelData);

        ImageOutput output = JsonImageMapper.JsonImageData(bitmap, pixelData);
        
        // This should definitely be configurable on input, but I'm pretty sure I'm the only one
        // using this, and I no longer have any need for JSON output so 🤷‍♂️.
        string jsonStr = output.AsMinText();
        Console.Out.WriteLine(jsonStr);
      }

      return 0;
    }
  }
}
