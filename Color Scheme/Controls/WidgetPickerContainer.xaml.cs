using HSVColorPickers;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Color_Scheme.Controls
{
    public sealed partial class WidgetPickerContainer<T> : WidgetPickerContainer
        where T : IHSVPicker, IColorPicker, new()
    {
        public WidgetPickerContainer() : this(new T()) { }
        private WidgetPickerContainer(T content) : base(content, content) { }
    }

    public partial class WidgetPickerContainer : UserControl, IColorPicker
    {

        readonly IHSVPicker HSVPicker;
        readonly IColorPicker ColorPicker;

        //@Override
        public string Type => this.ColorPicker.Type;
        public Control Self => this;
        public Color Color
        {
            get => this.ColorPicker.Color;
            set
            {
                this.ColorPicker.Color = value;
                this.ColorChangedCore(null, value);
            }
        }

        //@Delegate
        public event ColorChangedHandler ColorChanged { remove => this.ColorPicker.ColorChanged -= value; add => this.ColorPicker.ColorChanged += value; }
        public event ColorChangedHandler ColorChangedStarted { remove => this.ColorPicker.ColorChangedStarted -= value; add => this.ColorPicker.ColorChangedStarted += value; }
        public event ColorChangedHandler ColorChangedDelta { remove => this.ColorPicker.ColorChangedDelta -= value; add => this.ColorPicker.ColorChangedDelta += value; }
        public event ColorChangedHandler ColorChangedCompleted { remove => this.ColorPicker.ColorChangedCompleted -= value; add => this.ColorPicker.ColorChangedCompleted += value; }

        protected WidgetPickerContainer(IColorPicker colorPicker, IHSVPicker hsvPicker)
        {
            this.InitializeComponent();

            this.ColorPicker = colorPicker;
            this.HSVPicker = hsvPicker;
            this.ContentPresenter.Content = this.ColorPicker.Self;

            this.ColorChanged += this.ColorChangedCore;
            this.ColorChangedStarted += this.ColorChangedCore;
            this.ColorChangedDelta += this.ColorChangedCore;
            this.ColorChangedCompleted += this.ColorChangedCore;

            this.RHyperlink.Click += (s, e) => WidgetPickerContainer.Paste(this.RRun.Text);
            this.GHyperlink.Click += (s, e) => WidgetPickerContainer.Paste(this.GRun.Text);
            this.BHyperlink.Click += (s, e) => WidgetPickerContainer.Paste(this.BRun.Text);
            this.HHyperlink.Click += (s, e) => WidgetPickerContainer.Paste(this.HRun.Text);
            this.SHyperlink.Click += (s, e) => WidgetPickerContainer.Paste(this.SRun.Text);
            this.VHyperlink.Click += (s, e) => WidgetPickerContainer.Paste(this.VRun.Text);
        }

        private void ColorChangedCore(object sender, Color color)
        {
            HSV hsv = this.HSVPicker.HSV;

            this.RRun.Text = $"{color.R}".PadLeft(3, ' ');
            this.GRun.Text = $"{color.G}".PadLeft(3, ' ');
            this.BRun.Text = $"{color.B}".PadLeft(3, ' ');

            this.HRun.Text = $"{(int)hsv.H}".PadLeft(3, ' ');
            this.SRun.Text = $"{(int)hsv.S}".PadLeft(3, ' ');
            this.VRun.Text = $"{(int)hsv.V}".PadLeft(3, ' ');
        }

        //@Static
        public static void Paste(string value)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(value);
            Clipboard.SetContent(dataPackage);
        }

    }
}