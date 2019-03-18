using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace 配色pro.Controls
{
    public sealed partial class SplitPanelControl : UserControl
    {
        public static readonly double OffsetX = 300;

        //Content
        public UIElement ContentChild { set => this.ContentBorder.Child = value; get => this.ContentBorder.Child; }

        public bool IsOpen
        {
            get => this.isOpen;
            set
            {
                this.OpenOpacity = value ? 1.0 : 0.0;

                this.isOpen = value;
            }
        }
        public bool isOpen;

        public double OpenOpacity
        {
            get => this.openOpacity;
            private set
            {
                this.DismissOverlayBackground.Visibility = (value == 0.0) ? Visibility.Collapsed : Visibility.Visible;
                this.DismissOverlayBackground.Opacity = value;
                this.SpliteLeftShadow.Opacity = value;
                this.BehaviorOffset.OffsetX = (value - 1) * this.ContentWidth;

                this.openOpacity = value;
            }
        }
        private double openOpacity;

        public double TranslateX
        {
            set => translateX = value;
            private get
            {
                if (translateX > 0.0d) return 0.0d;
                else if (translateX < -this.ContentWidth) return -this.ContentWidth;
                else return translateX;
            }
        }
        private double translateX;

        double ContentWidth;

        public SplitPanelControl()
        {
            this.InitializeComponent();
            this.ContentBorder.Width = SplitPanelControl.OffsetX;
            this.BehaviorOffset.OffsetX = -SplitPanelControl.OffsetX;
            this.TranslateX = -SplitPanelControl.OffsetX; ;

            this.ContentBorder.SizeChanged += (s, e) => this.ContentWidth = e.NewSize.Width;
            this.DismissOverlayBackground.Tapped += (s, e) => this.IsOpen = false;

            this.PanelGrid.ManipulationMode = ManipulationModes.TranslateX;
            this.PanelGrid.ManipulationStarted += (s, e) =>
            {
                this.BehaviorOffset.Duration = 0.0d;
                this.TranslateX = (this.OpenOpacity - 1) * this.ContentWidth;
            };
            this.PanelGrid.ManipulationDelta += (s, e) =>
            {
                this.translateX += e.Delta.Translation.X;
                this.OpenOpacity = this.TranslateX / this.ContentWidth + 1;
            };
            this.PanelGrid.ManipulationCompleted += (s, e) =>
            {
                this.BehaviorOffset.Duration = 600.0d;
                this.IsOpen = (this.TranslateX > -this.ContentWidth / 2);
            };
        }
    }
}
