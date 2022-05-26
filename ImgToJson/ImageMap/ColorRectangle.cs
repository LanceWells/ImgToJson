namespace ImgToJson.ImageMap
{
  /// <summary>
  /// A container element used to represent rectangles of a specific color.
  /// </summary>
  public class ColorRectangle
  {
    /// <summary>
    /// The color that this rectangle ought to be colored as.
    /// </summary>
    public ARGBFormatColor ColorIndex;

    /// <summary>
    /// The boundaries for this rectnagle. Note that this should be a 2x2 2-dimensional array.
    /// </summary>
    public readonly int[,] RectangleBoundaries;

    /// <summary>
    /// The size of the array 
    /// </summary>
    public int RectangleSize;

    /// <summary>
    /// The constructor for the ColorRectangle type.
    /// </summary>
    /// <param name="colorInfo">The color information object for this rectangle.</param>
    /// <param name="rectangleBoundaries">The boundaries enclosed by this rectangle.</param>
    public ColorRectangle(ARGBFormatColor colorInfo, int[,] rectangleBoundaries)
    {
      ColorIndex = colorInfo;
      RectangleBoundaries = rectangleBoundaries;

      int width = rectangleBoundaries[1, 0] - rectangleBoundaries[0, 0] + 1;
      int height = rectangleBoundaries[1, 1] - rectangleBoundaries[0, 1] + 1;

      RectangleSize = width * height;
    }

    /// <summary>
    /// Returns true if the provided rectangle is enclosed by this rectangle.
    /// </summary>
    /// <param name="rectangleToCompare">The rectangle to evaluate.</param>
    /// <returns>True if the provided rectangle is enclosed by this rectangle.</returns>
    public bool Encloses(ColorRectangle rectangleToCompare)
    {
      bool upperLeftEnclosed = true;
      upperLeftEnclosed =
              upperLeftEnclosed && RectangleBoundaries[0, 0] <= rectangleToCompare.RectangleBoundaries[0, 0];
      upperLeftEnclosed =
          upperLeftEnclosed && RectangleBoundaries[0, 1] <= rectangleToCompare.RectangleBoundaries[0, 1];

      bool bottomRightEnclosed = true;
      bottomRightEnclosed =
          bottomRightEnclosed && RectangleBoundaries[1, 0] >= rectangleToCompare.RectangleBoundaries[1, 0];
      bottomRightEnclosed =
          bottomRightEnclosed && RectangleBoundaries[1, 1] >= rectangleToCompare.RectangleBoundaries[1, 1];

      return upperLeftEnclosed && bottomRightEnclosed;
    }
  }
}
