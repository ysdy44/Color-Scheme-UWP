using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Color_Scheme.Controls
{
    public sealed partial class DropShadowControl : UserControl
    {
        //Content
        public string Glyph { get => this.FontIcon.Glyph; set => this.FontIcon.Glyph = value; }
        public Color Color { get => this.Brush.Color; set => this.Brush.Color = value; }
     
        public DropShadowControl()
        {
            this.InitializeComponent();
            this.Loaded += (s, e) => this.FontIcon.FontFamily = this.FontFamily;
        }
    }
}
