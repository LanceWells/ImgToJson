using System.Drawing;

namespace ImgToJson.ImageMap
{
    /// <summary>
    /// A structure used to represent ARGB values, with a .ToString() method used to represent this color in hex
    /// format.
    /// </summary>
    public readonly struct ARGBFormatColor
    {
        /// <summary>
        /// The alpha channel value for this color.
        /// </summary>
        public readonly int A;

        /// <summary>
        /// The red channel value for this color.
        /// </summary>
        public readonly int R;

        /// <summary>
        /// The green channel value for this color.
        /// </summary>
        public readonly int G;

        /// <summary>
        /// The blue channel value for this color.
        /// </summary>
        public readonly int B;

        /// <summary>
        /// The constructor for this type.
        /// </summary>
        /// <param name="a">The alpha value for the color.</param>
        /// <param name="r">The red value for the color.</param>
        /// <param name="g">The green value for the color.</param>
        /// <param name="b">The blue value for the color.</param>
        public ARGBFormatColor(int a, int r, int g, int b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Get an instance of this color from the system Color type.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static ARGBFormatColor ColorToArgb(Color color)
        {
            return new ARGBFormatColor(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// An override for the ToString() method. Will be used by Newtonsoft JSON serialization.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // https://stackoverflow.com/questions/2395438/convert-system-drawing-color-to-rgb-and-hex-value
            string aHex = A.ToString("X2");
            string rHex = R.ToString("X2");
            string gHex = G.ToString("X2");
            string bHex = B.ToString("X2");

            return $"#{rHex}{gHex}{bHex}{aHex}";
            //return $"rgba({R},{G},{B},{A})";
        }

        /// <summary>
        /// An override for the Equals method.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>True if the provided object equals this object.</returns>
        public override bool Equals(object obj)
        {
            bool areEqual = false;

            if (obj is ARGBFormatColor rgbObj)
            {
                areEqual = true;
                areEqual = areEqual && rgbObj.A == A;
                areEqual = areEqual && rgbObj.R == R;
                areEqual = areEqual && rgbObj.G == G;
                areEqual = areEqual && rgbObj.B == B;
            }

            return areEqual;
        }

        /// <summary>
        /// An override for the GetHashCode method.
        /// </summary>
        /// <returns>The hash value for this object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
