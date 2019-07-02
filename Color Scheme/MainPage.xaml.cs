using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Color_Scheme.Library;

namespace Color_Scheme
{
    public enum MainPageState
    {
        HomeType,
        EunmType,
        ImageType
    }

    public sealed partial class MainPage : Page
    {
        public static readonly Color ThemeColor = Color.FromArgb(255, 0, 100, 255);
        private static Action<Color> colorAction;
        private static Color color;
        public static Color Color
        {
            get => MainPage.color;
            set
            {
                MainPage.colorAction?.Invoke(value);
                MainPage.color = value;
            }
        }
        public static LayoutState GetLayoutState(double width) => (width < 600) ? LayoutState.Phone : (width > 800) ? LayoutState.PC : LayoutState.Pad;

        private MainPageState state;
        public MainPageState State
        {
            get => this.state;
            set
            {
                this.HomePage.State = this.EnumPage.State = this.ImagePage.State = value;

                this.HomeLabelControl.SeletedIndex = this.EnumLabelControl.SeletedIndex = this.ImageLabelControl.SeletedIndex = (int)value;
                this.HomeLabelControl2.SeletedIndex = this.EnumLabelControl2.SeletedIndex = this.ImageLabelControl2.SeletedIndex = (int)value;
                this.PasteDropShadowControl.Visibility = this.ImageDropShadowControl.Visibility = (value == MainPageState.ImageType) ? Visibility.Visible : Visibility.Collapsed;

                if (value == MainPageState.HomeType) this.HomePage.Color = MainPage.Color;

                this.state = value;
            }
        }

        private LayoutState layoutState;
        public LayoutState LayoutState
        {
            get => this.layoutState;
            set
            {
                this.SplitButton.Visibility = this.SplitPanelControl.Visibility = this.TitleTextBlock.Visibility = (value == LayoutState.Phone) ? Visibility.Visible : Visibility.Collapsed;
                this.AvatarButton.Visibility = this.TitleGrid.Visibility = (value == LayoutState.Phone) ? Visibility.Collapsed : Visibility.Visible;

                this.layoutState = value;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            MainPage.colorAction = (color) => this.FloatActionButtonBrush.Color = this.PasteDropShadowControl.Color = this.ImageDropShadowControl.Color = color;
            MainPage.Color = MainPage.ThemeColor;
            this.Loaded += (s, e) => this.HomePage.Color = MainPage.ThemeColor;
            this.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;
                this.LayoutState = MainPage.GetLayoutState(e.NewSize.Width);
            };

            //Drop
            this.AllowDrop = true;
            this.Drop += (s, e) =>
            {
                this.State = MainPageState.ImageType;
                this.ImagePage.BitmapPicker(e);
            };
            this.DragOver += (s, e) =>
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                e.DragUIOverride.IsCaptionVisible = e.DragUIOverride.IsContentVisible = e.DragUIOverride.IsGlyphVisible = true;
            };

            //Key
            this.KeyDown += (s, e) =>
            {
                if (this.ShowDialogControl.IsOpen) return;
                if (e.Key != VirtualKey.V) return;

                this.State = MainPageState.ImageType;
                this.ImagePage.BitmapClipboard();
                e.Handled = true;
            };

            //Button
            this.ThemeButton.Tapped += (s, e) => this.ThemeControl.Theme = (this.ThemeControl.Theme == ElementTheme.Dark) ? ElementTheme.Light : ElementTheme.Dark;
            this.SplitButton.Tapped += (s, e) => this.SplitPanelControl.IsOpen = true;
            this.PasteDropShadowControl.Tapped += (s, e) => this.ImagePage.BitmapClipboard();
            this.ImageDropShadowControl.Tapped += (s, e) => this.ImagePage.BitmapPicker();

            //Avatar
            this.AvatarButton.Tapped += async (s, e) => await Launcher.LaunchUriAsync(new Uri("https://github.com/ysdy44/Color-Scheme-pro-UWP"));
            this.AvatarButton2.Tapped += async (s, e) => await Launcher.LaunchUriAsync(new Uri("https://github.com/ysdy44/Color-Scheme-pro-UWP"));

            //Dialog
            this.ShowDialogControl.Visibility = Visibility.Collapsed;
            this.FloatActionButton.Tapped += (s, e) =>
            {
                this.ShowDialogControl.Visibility = Visibility.Visible;
                this.ShowDialogControl.Color = MainPage.Color;
            };
            this.ShowDialogControl.ColorChange += (s, e) =>
            {
                MainPage.Color = e;
                if (this.State == MainPageState.HomeType) this.HomePage.Color = e;
            };

            //State
            this.State = MainPageState.HomeType;

            this.HomeLabelControl.SeletedChanged += () => this.SeletedChanged(MainPageState.HomeType);
            this.EnumLabelControl.SeletedChanged += () => this.SeletedChanged(MainPageState.EunmType);
            this.ImageLabelControl.SeletedChanged += () => this.SeletedChanged(MainPageState.ImageType);

            this.HomeLabelControl2.SeletedChanged += () => this.SeletedChanged(MainPageState.HomeType);  
            this.EnumLabelControl2.SeletedChanged += () => this.SeletedChanged(MainPageState.EunmType); 
            this.ImageLabelControl2.SeletedChanged += () => this.SeletedChanged(MainPageState.ImageType);           
        }
        private void SeletedChanged(MainPageState value)
        {
            this.SplitPanelControl.IsOpen = false;
            this.State = value;
        }
    }
}