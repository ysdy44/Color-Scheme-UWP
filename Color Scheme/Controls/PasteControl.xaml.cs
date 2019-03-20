using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;

namespace Color_Scheme.Controls
{
    public sealed partial class PasteControl : UserControl
    {
        public string Label { get; set; }
        public string Unit { get; set; }
        private int value;
        public int Value
        {
            get => this.value;
            set
            {
                this.TextBlock.Text = this.Label + ":  " + value.ToString() + this.Unit;
                this.value = value;
            }
        }

        public PasteControl()
        {
            this.InitializeComponent();
            this.Button.Tapped += (s, e) =>
            {
                DataPackage dataPackage = new DataPackage();
                dataPackage.SetText(this.Value.ToString());
                Clipboard.SetContent(dataPackage);
            };
        }
    }
}
