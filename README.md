# About
ImgToJson is a conversion tool used to translate images into raw JSON. The output JSON is designed to be HTML-Canvas friendly, and organizes the data into rectangle/color tuples. From here, swapping a specific color in an existing image is trivial at runtime in a frontend environment. This is all without the need for filters or for image conversion.

# Usage

## Options
* `-help`/`--help`: Show this menu

## Args
* [0]: imagePath: The path to the image to get the JSON format string for.

## Output
A JSON-format string for the image. The format will be:
```
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
```

Each color hex value will contain a list of rectangle coordinates, indicating the top-left and bottom-right
corners for a rectangle of the corresponding color that should be drawn.

Note that the output, in a json file, is about 7x the size of a .png image. This should not be used as a
low-cost storage format, but it is useful as a means to draw an image on a canvas where specific colors
might be changed without need filters or image manipulation.

## Return Codes
* 0: No error. Note that this help menu will return '0'.
* 1: The provided file does not exist at the specified path.
