using Microsoft.Maui.Graphics;
using ZXing.Common;

namespace BarcodeList.Tool
{
    /// <summary>
    /// ZXing.NetのBitMatrixをGraphicsView上に自前描画するためのIDrawable。
    /// zxing:BarcodeGeneratorViewはEncodingOptions(GS1Format等)を渡せないため、
    /// GS1-128のように特殊なエンコードオプションが必要な場合にこちらを使う。
    /// </summary>
    public class BitMatrixDrawable : IDrawable
    {
        public BitMatrix? Matrix { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.White;
            canvas.FillRectangle(dirtyRect);

            if (Matrix == null || Matrix.Width == 0 || Matrix.Height == 0)
                return;

            canvas.FillColor = Colors.Black;

            var moduleWidth = dirtyRect.Width / Matrix.Width;
            var moduleHeight = dirtyRect.Height / Matrix.Height;

            for (var y = 0; y < Matrix.Height; y++)
            {
                for (var x = 0; x < Matrix.Width; x++)
                {
                    if (Matrix[x, y])
                    {
                        canvas.FillRectangle(
                            dirtyRect.X + x * moduleWidth,
                            dirtyRect.Y + y * moduleHeight,
                            moduleWidth + 1,
                            moduleHeight + 1);
                    }
                }
            }
        }
    }
}
