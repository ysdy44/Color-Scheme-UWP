using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace 配色pro.Controls
{
    public sealed partial class GradientControl : UserControl
    {
        private Color keftColor;
        public Color LeftColor
        {
            get => this.keftColor;
            set
            {
                this.LeftBrush.Color = value;
                this.LeftStop.Color = value;
                this.keftColor = value;
            }
        }

        private Color rightColor;
        public Color RightColor
        {
            get => this.rightColor;
            set
            {
                this.RightBrush.Color = value;
                this.RightStop.Color = value;
                this.rightColor = value;
            }
        }
        
        public GradientControl()
        {
            this.InitializeComponent();
            this.LeftColor = Colors.White;
            this.RightColor = Colors.White;

            this.LeftBorder.Tapped += (s, e) => this.LeftColor = MainPage.Color;
            this.RightBorder.Tapped += (s, e) => this.RightColor = MainPage.Color;
            
            this.Slider.ValueChangeDelta += (s, e) => 
            {
                double percentage = e / 100.0;
                double unPercentage = 1.0 - percentage;
                double a = unPercentage * this.LeftColor.A + percentage * this.RightColor.A;
                double r = unPercentage * this.LeftColor.R + percentage * this.RightColor.R;
                double g = unPercentage * this.LeftColor.G + percentage * this.RightColor.G;
                double b = unPercentage * this.LeftColor.B + percentage * this.RightColor.B;
                MainPage.Color = Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
            };
        }       
    }
}
