using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using 配色pro.Library;

namespace 配色pro.Model
{
    public class ColorBrushCode
    {
        //颜色
        public String Name { get; set; }
        public Color Color { get; set; }
        public SolidColorBrush Brush { get => new SolidColorBrush(Color); }

        //参数
        public string R { get => "R：" + Color.R.ToString(); }
        public string G { get => "G：" + Color.G.ToString(); }
        public string B { get => "B：" + Color.B.ToString(); }

        //十六进制代码
        public string Code { get => "#" + Color.R.ToString("x2") + Color.G.ToString("x2") + Color.B.ToString("x2"); }

        //字体颜色（自适应：笔刷偏白字体就更黑，笔刷偏黑字体就更白）
        //  public SolidColorBrush Foreground { get => Color.R + Color.G + Color.B < 384 ? new SolidColorBrush(Color.FromArgb(255, (byte)(255 - (255 - Color.R) / 2), (byte)(255 - (255 - Color.G) / 2), (byte)(255 - (255 - Color.B) / 2))) : new SolidColorBrush(Color.FromArgb(255, (byte)(Color.R / 2), (byte)(Color.G / 2), (byte)(Color.B / 2))); }
        public SolidColorBrush Foreground { get => Color.R + Color.G + Color.B < 384 ? new SolidColorBrush(Method.ChangeL(Color, 0.5)) : new SolidColorBrush(Method.ChangeL(Color, -0.5)); }

    }
}
