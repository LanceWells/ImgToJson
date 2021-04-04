import React, { useEffect, useRef } from 'react';
import * as testImage from './image2.json';

type testImageTypes = keyof typeof testImage.Colors;

const imageScale: number = 4;

function DrawImage(drawContext: CanvasRenderingContext2D, ref: HTMLCanvasElement) {
  const colors = Object.keys(testImage.Colors);

  drawContext.clearRect(0, 0, ref.width, ref.height);

  for (let i = 0; i < colors.length; i++) {
    const colorText = colors[i];
    const color = colorText as testImageTypes;

    const colorIndices = testImage.Colors[color];
    drawContext.fillStyle = colorText;

    colorIndices.forEach(colorIndex => {
      const x1 = colorIndex[0][0];
      const x2 = colorIndex[1][0];
      const y1 = colorIndex[0][1];
      const y2 = colorIndex[1][1];

      drawContext.fillRect(
        x1 * imageScale,
        y1 * imageScale,
        (x2 - x1 + 1) * imageScale,
        (y2 - y1 + 1) * imageScale,
      );
    });
  }
}

function PixelCanvas() {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    const context = canvas?.getContext('2d');

    if (context && canvasRef.current) {
      DrawImage(context, canvasRef.current);
    }
  }, [canvasRef]);

  return (
    <div className="App">
      <canvas ref={canvasRef} width={256} height={256} />
    </div>
  );
}

export default PixelCanvas;
