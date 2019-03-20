using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI.Xaml.Controls;

namespace 配色pro.Controls
{
    public sealed partial class StrawControl : UserControl
    {         
        //Control
        public float ControlWidth;
        public float ControlHeight;
        private Vector2 position;
        public Vector2 Position
        {
            get => this.position;
            set
            {
                Canvas.SetLeft(this, value.X - this.ControlWidth / 2);
                Canvas.SetTop(this, value.Y - this.ControlHeight);
                this.position = value;
            }
        }
   
        //Canvas
        public float CanvasWidth;
        public float CanvasHeight;
        public Vector2 CanvasCenter => new Vector2(this.CanvasWidth / 2, this.CanvasHeight / 2);

        public bool CanInvalidate;
        public CanvasControl CanvasControl => this.ThumbnailCanvasControl;
        
        public StrawControl()
        {
            this.InitializeComponent();

            //Control
            this.SizeChanged += (s, e) =>
            {
                this.ControlWidth = (float)e.NewSize.Width;
                this.ControlHeight = (float)e.NewSize.Height;
            };
            //Canvas
            this.ThumbnailCanvasControl.SizeChanged += (s, e) =>
            {
                this.CanvasWidth = (float)e.NewSize.Width;
                this.CanvasHeight = (float)e.NewSize.Height;
            };
        }

        public void Show()
        {
            this.CanInvalidate = true;
            this.ShowStoryboard.Begin();
        }
        public void Fade()
        {
            this.CanInvalidate = false;
            this.FadeStoryboard.Begin();
        }
        public void Invalidate()
        {
            if (this.CanInvalidate)
            {
                this.ThumbnailCanvasControl.Invalidate();
            }
        }

    }
}
