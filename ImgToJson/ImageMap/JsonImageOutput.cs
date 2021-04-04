using System.Collections.Generic;

namespace ImgToJson.ImageMap
{
    /// <summary>
    /// A JSON-friendly object used to contain image information.
    /// </summary>
    public class JsonImageOutput
    {
        /// <summary>
        /// The color-aware bitmap information for the image.
        /// </summary>
        public Dictionary<ARGBFormatColor, List<int[,]>> Colors;

        /// <summary>
        /// The constructor for this type.
        /// </summary>
        /// <param name="colors">The color information to contain by this class.</param>
        public JsonImageOutput(Dictionary<ARGBFormatColor, List<int[,]>> colors)
        {
            Colors = colors;
        }
    }
}
