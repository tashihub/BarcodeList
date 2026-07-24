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

        /// <summary>
        /// trueの場合、90度回転して描画する(横に長すぎて画面幅に収まらないバーコードを、
        /// 画面の縦方向を使って最小バー幅を確保するため)。呼び出し側でGraphicsViewの
        /// WidthRequest/HeightRequestも縦長になるよう入れ替えて設定すること。
        /// </summary>
        public bool Rotated { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.White;
            canvas.FillRectangle(dirtyRect);

            if (Matrix == null || Matrix.Width == 0 || Matrix.Height == 0)
                return;

            canvas.FillColor = Colors.Black;

            if (Rotated)
            {
                var moduleWidth = dirtyRect.Width / Matrix.Height;
                var moduleLength = dirtyRect.Height / Matrix.Width;

                for (var x = 0; x < Matrix.Width; x++)
                {
                    for (var y = 0; y < Matrix.Height; y++)
                    {
                        if (Matrix[x, y])
                        {
                            canvas.FillRectangle(
                                dirtyRect.X + y * moduleWidth,
                                dirtyRect.Y + x * moduleLength,
                                moduleWidth + 1,
                                moduleLength + 1);
                        }
                    }
                }

                return;
            }

            var normalModuleWidth = dirtyRect.Width / Matrix.Width;
            var normalModuleHeight = dirtyRect.Height / Matrix.Height;

            for (var y = 0; y < Matrix.Height; y++)
            {
                for (var x = 0; x < Matrix.Width; x++)
                {
                    if (Matrix[x, y])
                    {
                        canvas.FillRectangle(
                            dirtyRect.X + x * normalModuleWidth,
                            dirtyRect.Y + y * normalModuleHeight,
                            normalModuleWidth + 1,
                            normalModuleHeight + 1);
                    }
                }
            }
        }
    }
}
