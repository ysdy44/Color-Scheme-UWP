using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Color_Scheme.Library;
using HSVColorPickers;

namespace Color_Scheme.Pages
{
    public enum ImagePageState
    {
        NoneType,
        ImageType,
    }

    public sealed partial class ImagePage : Page
    {

        //State
        public MainPageState State { set => this.Visibility = (value == MainPageState.ImageType) ? Visibility.Visible : Visibility.Collapsed; }

        private ImagePageState imageState;
        public ImagePageState ImageState
        {
            get => this.imageState;
            set
            {
                this.Button.Visibility =
                this.Border.Visibility =
                    (value == ImagePageState.NoneType) ? Visibility.Visible : Visibility.Collapsed;

                this.Canvas.Visibility =
                this.CanvasControl.Visibility =
                    (value == ImagePageState.ImageType) ? Visibility.Visible : Visibility.Collapsed;

                this.imageState = value;
            }
        }

        private CanvasBitmap bitmap;
        public CanvasBitmap Bitmap
        {
            get => this.bitmap;
            set
            {
                this.Transformer.Width = (int)value.SizeInPixels.Width;
                this.Transformer.Height = (int)value.SizeInPixels.Height;

                this.bitmap = value;
            }
        }


        //Transformer
        float ControlWidth;
        float ControlHeight;
        Vector2 ControlCenter => new Vector2(this.ControlWidth / 2, this.ControlHeight / 2);
        Transformer Transformer = Transformer.One;


        //Operator: Right
        Vector2 rightStartPoint;
        Vector2 rightStartPosition;

        //Operator: Double
        Vector2 doubleStartCenter;
        Vector2 doubleStartPosition;
        float doubleStartScale;
        float doubleStartSpace;


        public ImagePage()
        {
            this.InitializeComponent();
            this.ImageState = ImagePageState.NoneType;
            this.Button.Tapped += (s, e) => this.BitmapPicker();
            this.SizeChanged += (s, e) =>
            {
                this.ControlWidth = (float)e.NewSize.Width;
                this.ControlHeight = (float)e.NewSize.Height;
            };

            //Drop
            this.AllowDrop = true;
            this.Drop += (s, e) => this.BitmapPicker(e);
            this.DragOver += (s, e) =>
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                e.DragUIOverride.IsCaptionVisible = e.DragUIOverride.IsContentVisible = e.DragUIOverride.IsGlyphVisible = true;
            };

            //Key
            this.KeyDown += (s, e) => this.BitmapClipboard();

            //Draw
            this.StrawControl.CanvasControl.Draw += (s, args) =>
            {
                if (this.Bitmap == null) return;

                args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    Source = this.Bitmap,
                    TransformMatrix =
                      Matrix3x2.CreateTranslation(-this.Transformer.BitmapPoint(this.StrawControl.Position))
                      * Matrix3x2.CreateScale(2)
                      * Matrix3x2.CreateTranslation(this.StrawControl.CanvasCenter)
                });
            };
            this.CanvasControl.Draw += (s, args) =>
            {
                args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    Source = this.Bitmap,
                    TransformMatrix = this.Transformer.Matrix
                }); 

                CanvasDraw.RulerDraw
                (
                    ds: args.DrawingSession,
                    position: this.Transformer.Position,
                    scale: this.Transformer.Scale,
                    controlWidth: this.ControlWidth,
                    controlHeight: this.ControlHeight
                );
            };


            //Operator: Single
            this.Operator.Single_Start += (point) =>
            {
                this.StrawControl.Show();
                this.StrawControl.Position = point;

                this.CanvasControl.Invalidate();
                this.StrawControl.Invalidate();

                MainPage.Color = ImagePicker.GetColorFromBitmap(this.Bitmap, this.Transformer.BitmapPoint(point)) ?? Colors.White;
            };
            this.Operator.Single_Delta += (point) =>
            {
                this.StrawControl.Position = point;

                this.CanvasControl.Invalidate();
                this.StrawControl.Invalidate();

                MainPage.Color = ImagePicker.GetColorFromBitmap(this.Bitmap, this.Transformer.BitmapPoint(point)) ?? Colors.White;
            };
            this.Operator.Single_Complete += (point) =>
            {
                this.StrawControl.Fade();
                this.StrawControl.Position = point;

                this.CanvasControl.Invalidate();
                this.StrawControl.Invalidate();

                MainPage.Color = ImagePicker.GetColorFromBitmap(this.Bitmap, this.Transformer.BitmapPoint(point)) ?? Colors.White;
            };

            //Operator: Right
            this.Operator.Right_Start += (point) =>
            {
                this.rightStartPoint = point;
                this.rightStartPosition = this.Transformer.Position;

                this.CanvasControl.Invalidate();
            };
            this.Operator.Right_Delta += (point) =>
            {
                this.Transformer.Position = this.rightStartPosition - this.rightStartPoint + point;

                this.CanvasControl.Invalidate();
            };
            this.Operator.Right_Complete += (point) => this.CanvasControl.Invalidate();

            //Operator: Double
            this.Operator.Double_Start += (center, space) =>
            {
                this.doubleStartCenter = (center - this.Transformer.Position) / this.Transformer.Scale + this.ControlCenter;
                this.doubleStartPosition = this.Transformer.Position;

                this.doubleStartSpace = space;
                this.doubleStartScale = this.Transformer.Scale;

                this.CanvasControl.Invalidate();
            };
            this.Operator.Double_Delta += (center, space) =>
            {
                this.Transformer.Scale = this.doubleStartScale / this.doubleStartSpace * space;
                this.Transformer.Position = center - (this.doubleStartCenter - this.ControlCenter) * this.Transformer.Scale;

                this.CanvasControl.Invalidate();
            };
            this.Operator.Double_Complete += (center, space) => this.CanvasControl.Invalidate();

            //Operator: Wheel Changed
            this.Operator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                {
                    if (this.Transformer.Scale < 10f)
                    {
                        this.Transformer.Scale *= 1.1f;
                        this.Transformer.Position = point + (this.Transformer.Position - point) * 1.1f;
                    }
                }
                else
                {
                    if (this.Transformer.Scale > 0.1f)
                    {
                        this.Transformer.Scale /= 1.1f;
                        this.Transformer.Position = point + (this.Transformer.Position - point) / 1.1f;
                    }
                }

                this.CanvasControl.Invalidate();
            };
        }

        public async void BitmapPicker() => this.SetBitmap(await ImagePicker.GetFileFromPicker());
        public async void BitmapPicker(DragEventArgs e) => this.SetBitmap(await ImagePicker.GetFileFromDrop(e));

        public async void BitmapClipboard()
        {
            using (IRandomAccessStream stream = await ImagePicker.GetStreamFromClipboard())
            {
                if (stream == null) return;
                this.SetBitmap(await CanvasBitmap.LoadAsync(this.CanvasControl, stream));
            }
        }

        private async void SetBitmap(StorageFile file)
        {
            try
            {
                using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
                {
                    this.SetBitmap(await CanvasBitmap.LoadAsync(this.CanvasControl, stream));
                }
            }
            catch (Exception) { }
        }
        private void SetBitmap(CanvasBitmap bitmap)
        {
            if (bitmap == null) return;
            this.Bitmap = bitmap;

            this.Transformer.Position = this.ControlCenter;
            this.Transformer.Scale = 0.9f * Math.Min(this.ControlWidth / bitmap.SizeInPixels.Width, this.ControlHeight / bitmap.SizeInPixels.Height);

            this.ImageState = ImagePageState.ImageType;
            this.CanvasControl.Invalidate();
        }
    }
}
