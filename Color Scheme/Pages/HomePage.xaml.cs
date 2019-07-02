using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Color_Scheme.Library;
using HSVColorPickers;

namespace Color_Scheme.Pages
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
                this.ValuePasteControl.Visibility =
                this.SaturationBorder.Visibility = (value == LayoutState.Phone) ? Visibility.Collapsed : Visibility.Visible;
                this.ValueBorder.Visibility = (value == LayoutState.PC) ? Visibility.Visible : Visibility.Collapsed;
        
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
            get => MainPage.Color;
            set => this.SetColor(value, HSV.RGBtoHSV(value));
        }

        PalettePicker HuePicker = PalettePicker.CreateFormHue();
        PalettePicker SaturationPicker = PalettePicker.CreateFormSaturation();
        PalettePicker ValuePicker = PalettePicker.CreateFormValue();

        public HomePage()
        {
            this.InitializeComponent();
            this.HueBorder.Child = this.HuePicker; 
            this.SaturationBorder.Child = this.SaturationPicker;
            this.ValueBorder.Child = this.ValuePicker;
            this.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;
                this.LayoutState = MainPage.GetLayoutState(e.NewSize.Width);
            };

            this.WheelPicker.HSVChange += (s, value) => this.SetColor(HSV.HSVtoRGB(value), value, isWheelPicker: false);
            this.RGBPicker.ColorChange += (s, value) => this.SetColor(value, HSV.RGBtoHSV(value), isRGBPicker: false);
            this.HSVPicker.HSVChange += (s, value) => this.SetColor(HSV.HSVtoRGB(value), value, isHSVPicker: false);

            this.HuePicker.HSVChange += (s, value) => this.SetColor(HSV.HSVtoRGB(value), value, isHuePicker: false);
            this.SaturationPicker.HSVChange += (s, value) => this.SetColor(HSV.HSVtoRGB(value), value, isSaturationPicker: false);
            this.ValuePicker.HSVChange += (s, value) => this.SetColor(HSV.HSVtoRGB(value), value, isValuePicker: false);

            this.GradientControl0.ColorChange += this.GradientControl_ColorChange;
            this.GradientControl1.ColorChange += this.GradientControl_ColorChange;
            this.GradientControl2.ColorChange += this.GradientControl_ColorChange;
        }

        private void GradientControl_ColorChange(object sender, Color value)
        {
            HSV hsv = HSV.RGBtoHSV(value);
            this.SetColor(value, hsv);
        }

        private void SetColor(Color color, HSV hsv, bool isWheelPicker = true, bool isRGBPicker = true, bool isHSVPicker = true, bool isHuePicker = true, bool isSaturationPicker = true, bool isValuePicker = true)
        {
            this.RedPasteControl.Value = (int)color.R;
            this.GreenPasteControl.Value = (int)color.G;
            this.BluePasteControl.Value = (int)color.B;

            this.HueasteControl.Value = (int)hsv.H;
            this.SaturationPasteControl.Value = (int)hsv.V;
            this.ValuePasteControl.Value = (int)hsv.S;

            if (isWheelPicker) this.WheelPicker.HSV = hsv;
            if (isRGBPicker) this.RGBPicker.Color = color;
            if (isHSVPicker) this.HSVPicker.HSV = hsv;

            if (isHuePicker) this.HuePicker.HSV = hsv;
            if (isSaturationPicker) this.SaturationPicker.HSV = hsv;
            if (isValuePicker) this.ValuePicker.HSV = hsv;

            MainPage.Color = color;
        }
    }
}
