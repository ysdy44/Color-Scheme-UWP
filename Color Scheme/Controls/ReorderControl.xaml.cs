using Color_Scheme.Elements;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Color_Scheme.Controls
{
    public sealed partial class ReorderControl : Reorder
    {

        public WidgetType Type { get; set; }
        public object Header { get => this.ContentPresenter.Content; set => this.ContentPresenter.Content = value; }
        public string Text { get => this.TextBlock.Text; set => this.TextBlock.Text = value; }
        public bool Visible { get => this.SymbolIcon.Symbol == Symbol.Pin; set => this.SymbolIcon.Symbol = value ? Symbol.Pin : Symbol.UnPin; }

        public ReorderControl()
        {
            this.InitializeComponent();
            this.PinButton.Click += (s, e) => this.Visible = !this.Visible;
            this.Thumb.ManipulationMode = ManipulationModes.TranslateY;
            this.Thumb.ManipulationStarted += base.ThumbManipulationStarted;
            this.Thumb.ManipulationDelta += base.ThumbManipulationDelta;
            this.Thumb.ManipulationCompleted += base.ThumbManipulationCompleted;
        }

    }
}