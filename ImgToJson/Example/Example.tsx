import React, { useEffect, useRef } from 'react';
import $ from 'jquery';
import * as rt from 'runtypes';

const ColorMapType = rt.Array(rt.Array(rt.Array(rt.Number)));

const ImageEntry = rt.Record({
    Rectangles: ColorMapType,
    DrawIndex: rt.Number,
});

const ImageLayout = rt.Dictionary(ImageEntry);

const imageScale: number = 4;

type ImageEntryType = rt.Static<typeof ImageEntry>;
type ImageLayoutType = rt.Static<typeof ImageLayout>;

function DrawImage(
    drawContext: CanvasRenderingContext2D,
    imageMap: ImageLayoutType,
): void {
    const entries = Object
        .entries(imageMap)
        .sort((a, b) => a[1].DrawIndex - b[1].DrawIndex);

    for (let i = 0; i < entries.length; i++) {
        const entry: [string, ImageEntryType] = entries[i];
        let colorText: string = entry[0];
        drawContext.fillStyle = colorText;

        entry[1].Rectangles.forEach((r) => {
            const x1 = r[0][0];
            const x2 = r[1][0];
            const y1 = r[0][1];
            const y2 = r[1][1];

            drawContext.fillRect(
                x1 * imageScale,
                y1 * imageScale,
                (x2 - x1 + 1) * imageScale,
                (y2 - y1 + 1) * imageScale,
            )
        });
    }
}

function PixelCanvas() {
    const canvasRef = useRef<HTMLCanvasElement>(null);

    useEffect(() => {
        const canvas = canvasRef.current;
        const context = canvas?.getContext('2d');

        if (canvas && context) {
            context.clearRect(0, 0, canvas.width, canvas.height);
            $.getJSON('./assets/image4.json')
                .then((json) => {

                    if (ImageLayout.guard(json)) {
                        const jsonDict = json as ImageLayoutType;
                        DrawImage(context, jsonDict);
                    }
                })
        }
    }, [canvasRef])

    return (
        <div className="App">
            <canvas ref={canvasRef} width={256} height={256} />
        </div>
    );
}

export default PixelCanvas;
