using 配色pro.Pickers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace 配色pro.Controls
{
    public sealed partial class ShowDialogControl : UserControl
    {
        public static readonly TimeSpan ShowTime = new TimeSpan(0, 0, 0, 0, 200);
        public static readonly TimeSpan FadeTime = new TimeSpan(0, 0, 0, 0, 300);

        //Delegate
        public event ColorChangeHandler ColorChange;

        public Color Color
        {
            get => this.HexPicker.Color;
            set=>  this.HexPicker.Color = value;            
        }

        //Open
        public bool IsOpen
        {
            get => this.isOpen;
            set
            {
                if (value) this.Open();
                else this.Close();

                this.isOpen = value;
            }
        }
        private bool isOpen;

        private void Open()
        {
            this.ShowStoryboard.Begin();//Storyboard
        }
        private async void Close()
        {
            this.FadeStoryboard.Begin();//Storyboard
            await Task.Delay(ShowDialogControl.FadeTime);
        }
        public ShowDialogControl()
        {
            this.InitializeComponent();
            //Storyboard
            this.FadeAnimation.Duration = ShowDialogControl.FadeTime;
            this.ShowFrame.KeyTime = ShowDialogControl.ShowTime;

            this.DismissOverlayBackground.Tapped += (s, e) => this.Visibility = Visibility.Collapsed;
            this.CancelButton.Tapped += (s, e) => this.Visibility = Visibility.Collapsed;
            this.OKButton.Tapped += (s, e) =>
            {
                this.Visibility = Visibility.Collapsed;
                this.ColorChange?.Invoke(this, this.Color); //Delegate
            };
            this.Button.Tapped += (s, e) =>
            {
                DataPackage dataPackage = new DataPackage();
                dataPackage.SetText(Hex.ColorToString(this.Color));
                Clipboard.SetContent(dataPackage);
            };
        }
    }
}
