using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using ImgToJson.ImageMap;
using Newtonsoft.Json;

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
    Colors:
    {
        ""ColorHexValue"":
        [
            [
                [x1, y1],
                [x2, y2],
            ],
            . . .
        ],
        . . .
    }
}

Each color hex value will contain a list of rectangle coordinates, indicating the top-left and bottom-right
corners for a rectangle of the corresponding color that should be drawn.

Note that the output, in a json file, is about 7x the size of a .png image. This should not be used as a
low-cost storage format, but it is useful as a means to draw an image on a canvas where specific colors
might be changed without need filters or image manipulation.
            ";

        /// <summary>
        /// A program designed to generate a JSON string for an image file. The output JSON string is intended to be
        /// used as a low-cost format for rendering on an HTML canvas, particularly in the context of low color count
        /// images.
        /// </summary>
        /// <param name="args">See the help string for more information.</param>
        public static void Main(string[] args)
        {
            if (args.Length == 0 || args[0].Trim('-') == "help")
            {
                Console.Out.WriteLine(HelpString);
                return;
            }

            string imgPath = args[0];

            if (!File.Exists(imgPath))
            {
                Console.Out.WriteLine("File does not exist at the provided path.");
                return;
            }

            using (FileStream imageFile = File.OpenRead(imgPath))
            {
                // https://stackoverflow.com/questions/49190596/c-sharp-rgb-values-from-bitmap-data
                Bitmap bitmap = new Bitmap(imageFile);
                Rectangle bitmapContainer = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                BitmapData bitmapData = bitmap.LockBits(bitmapContainer, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                int stride = bitmapData.Stride;
                byte[] pixelData = new byte[stride * bitmap.Height];
                Marshal.Copy(bitmapData.Scan0, pixelData, 0, pixelData.Length);
                bitmap.UnlockBits(bitmapData);

                JsonImageOutput output = JsonImageMapper.JsonImageData(bitmap, pixelData, stride);
                string outputStr = JsonConvert.SerializeObject(output);
                Console.Out.WriteLine(outputStr);
            }
        }
    }
}
