using System;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Color_Scheme
{

    public enum MainPageState
    {
        HomeType,
        EunmType,
        ImageType,
        PaletteType,
    }
    public enum LayoutState
    {
        Phone,
        Pad,
        PC
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
                this.HomeLabelControl.SeletedIndex =
                this.EunmLabelControl.SeletedIndex =
                this.ImageLabelControl.SeletedIndex = (int)value;

                this.HomeLabelControl2.SeletedIndex =
                this.EunmLabelControl2.SeletedIndex =
                this.ImageLabelControl2.SeletedIndex = (int)value;

                this.HomePage.State =
                this.EunmPage.State =
                this.ImagePage.State = value;

                this.DropShadowPanel2.Visibility =
                    (value == MainPageState.ImageType) ? Visibility.Visible : Visibility.Collapsed;

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
                this.SplitButton.Visibility =
                this.SplitPanelControl.Visibility =
                this.TitleTextBlock.Visibility =
                    (value == LayoutState.Phone) ? Visibility.Visible : Visibility.Collapsed;

                this.AvatarButton.Visibility =
                this.TitleGrid.Visibility =
                    (value == LayoutState.Phone) ? Visibility.Collapsed : Visibility.Visible;

                this.layoutState = value;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            MainPage.colorAction = (color) => this.FloatActionButtonBrush.Color = this.FloatActionButtonBrush2.Color = color;
            MainPage.Color = MainPage.ThemeColor;
            this.Loaded += (s, e) => this.HomePage.Color = MainPage.ThemeColor;
            this.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;
                this.LayoutState = MainPage.GetLayoutState(e.NewSize.Width);
            };
            this.ThemeButton.Tapped += (s, e) => this.ThemeControl.Theme = (this.ThemeControl.Theme == ElementTheme.Dark) ? ElementTheme.Light : ElementTheme.Dark;
            this.SplitButton.Tapped += (s, e) => this.SplitPanelControl.IsOpen = true;
            this.FloatActionButton2.Tapped += (s, e) => this.ImagePage.BitmapPicker();

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

            this.HomeLabelControl.SeletedChanged += (value) => this.State = (MainPageState)value;
            this.EunmLabelControl.SeletedChanged += (value) => this.State = (MainPageState)value;
            this.ImageLabelControl.SeletedChanged += (value) => this.State = (MainPageState)value;

            this.HomeLabelControl2.SeletedChanged += (value) =>
            {
                SplitPanelControl.IsOpen = false;
                this.State = (MainPageState)value;
            };
            this.EunmLabelControl2.SeletedChanged += (value) =>
            {
                SplitPanelControl.IsOpen = false;
                this.State = (MainPageState)value;
            };
            this.ImageLabelControl2.SeletedChanged += (value) =>
            {
                SplitPanelControl.IsOpen = false;
                this.State = (MainPageState)value;
            };
        }
    }
}



