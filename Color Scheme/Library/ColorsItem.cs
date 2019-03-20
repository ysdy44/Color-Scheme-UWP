using System;
using Windows.UI;

namespace Color_Scheme.Library
{
    public class ColorsItem
    {
        public string Text;
        public Color Color;

        public string Summny;
        public Color Foreground;

        public ColorsItem(Color color, string text)
        {
            this.Text = text;
            this.Color = color;

            this.Summny = "A:" + color.A + " " + "R:" + color.R + " " + "G:" + color.G + " " + "B:" + color.B;
            this.Foreground = color.A > 64 ? color.R + color.G + color.B < 640 || Math.Abs(color.R - color.G) + Math.Abs(color.G - color.B) + Math.Abs(color.B - color.R) > 100 ? Colors.White : Colors.Black : Colors.Gray;
        }
    }
}
