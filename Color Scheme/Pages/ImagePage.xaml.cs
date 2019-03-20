using Color_Scheme.Library;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

                MainPage.Color = ImagePage.GetColorFromBitmap(this.Bitmap, this.Transformer.BitmapPoint(point)) ?? Colors.White;
            };
            this.Operator.Single_Delta += (point) =>
            {
                this.StrawControl.Position = point;

                this.CanvasControl.Invalidate();
                this.StrawControl.Invalidate();

                MainPage.Color = ImagePage.GetColorFromBitmap(this.Bitmap, this.Transformer.BitmapPoint(point)) ?? Colors.White;
            };
            this.Operator.Single_Complete += (point) =>
            {
                this.StrawControl.Fade();
                this.StrawControl.Position = point;

                this.CanvasControl.Invalidate();
                this.StrawControl.Invalidate();

                MainPage.Color = ImagePage.GetColorFromBitmap(this.Bitmap, this.Transformer.BitmapPoint(point)) ?? Colors.White;
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
            this.Operator.Right_Complete += (point) =>    this.CanvasControl.Invalidate();
  
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
            this.Operator.Double_Complete += (center, space) =>  this.CanvasControl.Invalidate();

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

        /// <summary> Change <see cref = "CanvasBitmap" />. </summary>
        public async void BitmapPicker()
        {
            CanvasBitmap bitmap = await ImagePage.GetCanvasBitmap(this.CanvasControl, await ImagePage.GetStorageFile());
            if (bitmap == null) return;

            this.Bitmap = bitmap;
            this.Transformer.Position = this.ControlCenter;
            this.ImageState = ImagePageState.ImageType;
            this.CanvasControl.Invalidate();
        }


        public static async Task<StorageFile> GetStorageFile() => await new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail,
            SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            FileTypeFilter =
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".bmp"
            }
        }.PickSingleFileAsync();

        public static async Task<CanvasBitmap> GetCanvasBitmap(ICanvasResourceCreator creator, StorageFile file)
        {
            if (file == null) return null;

            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                return await CanvasBitmap.LoadAsync(creator, stream);
            }
        }
        
        public static Color? GetColorFromBitmap(CanvasBitmap bitmap, Vector2 vector)
        {
            int x = (int)vector.X;
            int y = (int)vector.Y;

            if (x > 0 && x < bitmap.SizeInPixels.Width && y > 0 && y < bitmap.SizeInPixels.Height)
            {
                return bitmap.GetPixelColors(x, y, 1, 1).Single();
            }
            return null;
        }
    }
}
