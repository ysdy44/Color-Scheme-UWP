using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Numerics;

namespace 配色pro.Library
{
    public class CanvasDraw
    {       

        static readonly CanvasTextFormat RulerTextFormat = new CanvasTextFormat() { FontSize = 12, HorizontalAlignment = CanvasHorizontalAlignment.Center, VerticalAlignment = CanvasVerticalAlignment.Center, };

        public static void RulerDraw(CanvasDrawingSession ds, Vector2 position, float scale, float controlWidth, float controlHeight)
        {
            //line
            ds.DrawLine(20, 20, controlWidth, 20, Windows.UI.Colors.Gray);//Horizontal
            ds.DrawLine(20, 20, 20, controlHeight, Windows.UI.Colors.Gray);//Vertical

            //space
            float space = (float)(10 * scale);
            while (space < 10) space *= 5;
            while (space > 100) space /= 5;
            float spaceFive = space * 5;

            //Horizontal
            for (float X = (float)position.X; X < controlWidth; X += space) ds.DrawLine(X, 10, X, 20, Windows.UI.Colors.Gray);
            for (float X = (float)position.X; X > 20; X -= space) ds.DrawLine(X, 10, X, 20, Windows.UI.Colors.Gray);
            //Vertical
            for (float Y = (float)position.Y; Y < controlHeight; Y += space) ds.DrawLine(10, Y, 20, Y, Windows.UI.Colors.Gray);
            for (float Y = (float)position.Y; Y > 20; Y -= space) ds.DrawLine(10, Y, 20, Y, Windows.UI.Colors.Gray);

            //Horizontal
            for (float X = (float)position.X; X < controlWidth; X += spaceFive) ds.DrawLine(X, 10, X, 20, Windows.UI.Colors.Gray);
            for (float X = (float)position.X; X > 20; X -= spaceFive) ds.DrawLine(X, 10, X, 20, Windows.UI.Colors.Gray);
            //Vertical
            for (float Y = (float)position.Y; Y < controlHeight; Y += spaceFive) ds.DrawLine(10, Y, 20, Y, Windows.UI.Colors.Gray);
            for (float Y = (float)position.Y; Y > 20; Y -= spaceFive) ds.DrawLine(10, Y, 20, Y, Windows.UI.Colors.Gray);

            //Horizontal
            for (float X = (float)position.X; X < controlWidth; X += spaceFive) ds.DrawText(((int)(Math.Round((X - position.X) / scale))).ToString(), X, 10, Windows.UI.Colors.Gray, CanvasDraw.RulerTextFormat);
            for (float X = (float)position.X; X > 20; X -= spaceFive) ds.DrawText(((int)(Math.Round((X - position.X) / scale))).ToString(), X, 10, Windows.UI.Colors.Gray, CanvasDraw.RulerTextFormat);
            //Vertical
            for (float Y = (float)position.Y; Y < controlHeight; Y += spaceFive) ds.DrawText(((int)(Math.Round((Y - position.Y) / scale))).ToString(), 10, Y, Windows.UI.Colors.Gray, CanvasDraw.RulerTextFormat);
            for (float Y = (float)position.Y; Y > 20; Y -= spaceFive) ds.DrawText(((int)(Math.Round((Y - position.Y) / scale))).ToString(), 10, Y, Windows.UI.Colors.Gray, CanvasDraw.RulerTextFormat);
        }

    }
}
