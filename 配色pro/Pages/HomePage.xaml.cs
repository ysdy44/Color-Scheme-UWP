using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using 配色pro.Library;
using 配色pro.Pickers;

namespace 配色pro.Pages
{
    public sealed partial class HomePage : Page
    {
        public static readonly GridLength GridLength300Pixel = new GridLength(300, GridUnitType.Pixel);
        public static readonly GridLength GridLength1Star = new GridLength(1, GridUnitType.Star);

        //State
        public MainPageState State { set => this.Visibility = (value == MainPageState.HomeType) ? Visibility.Visible : Visibility.Collapsed; }
                     
        private LayoutState layoutState;
        public LayoutState LayoutState
        {
            get => this.layoutState;
            set
            {
                this.HueasteControl.Visibility =
                this.SaturationPasteControl.Visibility =
                this.LightnessPasteControl.Visibility =
                this.SaturationBorder.Visibility = (value == LayoutState.Phone) ? Visibility.Collapsed : Visibility.Visible;
                this.LightnessBorder.Visibility = (value == LayoutState.PC) ? Visibility.Visible : Visibility.Collapsed;
        
                this.Column1.Width = (value == LayoutState.Pad) ? HomePage.GridLength300Pixel : HomePage.GridLength1Star;
                this.Column2.Width = (value == LayoutState.Phone) ? HomePage.GridLength300Pixel : HomePage.GridLength1Star;
                this.Column3.Width = (value == LayoutState.Pad) ? HomePage.GridLength300Pixel : HomePage.GridLength1Star;

                Grid.SetColumn(this.HueBorder, (value == LayoutState.Phone) ? 2 : 0);
                Grid.SetColumn(this.SaturationBorder, (value == LayoutState.PC) ? 2 : 4);
                
                this.layoutState = value;
            }
        }

        public Color Color
        {
            set => this.SetColor(value, HSL.RGBtoHSL(value));
        }
        
        PalettePicker HuePicker = new PalettePicker(new PaletteHue());
        PalettePicker SaturationPicker = new PalettePicker(new PaletteSaturation());
        PalettePicker LightnessPicker = new PalettePicker(new PaletteLightness());

        public HomePage()
        {
            this.InitializeComponent();
            this.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;
                this.LayoutState = MainPage.GetLayoutState(e.NewSize.Width);
            };

            this.WheelPicker.HSLChange += (s, value) => this.SetColor(HSL.HSLtoRGB(value), value, isHSLPicker: false);
            this.RGBPicker.ColorChange += (s, value) => this.SetColor(value, HSL.RGBtoHSL(value), isRGBPicker: false);
            this.HSLPicker.HSLChange += (s, value) => this.SetColor(HSL.HSLtoRGB(value), value, isHSLPicker: false);

            this.HuePicker.HSLChange += (s, value) => this.SetColor(HSL.HSLtoRGB(value), value, isHuePicker: false);
            this.SaturationPicker.HSLChange += (s, value) => this.SetColor(HSL.HSLtoRGB(value), value, isSaturationPicker: false);
            this.LightnessPicker.HSLChange += (s, value) => this.SetColor(HSL.HSLtoRGB(value), value, isLightnessPicker: false);

            this.HueBorder.Child = this.HuePicker; 
            this.SaturationBorder.Child = this.SaturationPicker;
            this.LightnessBorder.Child = this.LightnessPicker;
        }

        private void SetColor(Color color, HSL hsl, bool isWheelPicker = true, bool isRGBPicker = true, bool isHSLPicker = true, bool isHuePicker = true, bool isSaturationPicker = true, bool isLightnessPicker = true)
        {
            this.RedPasteControl.Value = (int)color.R;
            this.GreenPasteControl.Value = (int)color.G;
            this.BluePasteControl.Value = (int)color.B;

            this.HueasteControl.Value = (int)hsl.H;
            this.SaturationPasteControl.Value = (int)hsl.L;
            this.LightnessPasteControl.Value = (int)hsl.S;

            if (isWheelPicker) this.WheelPicker.HSL = hsl;
            if (isRGBPicker) this.RGBPicker.Color = color;
            if (isHSLPicker) this.HSLPicker.HSL = hsl;

            if (isHuePicker) this.HuePicker.HSL = hsl;
            if (isSaturationPicker) this.SaturationPicker.HSL = hsl;
            if (isLightnessPicker) this.LightnessPicker.HSL = hsl;

            MainPage.Color = color;
        }
    }
}
