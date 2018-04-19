using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Foundation;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System.Collections.ObjectModel;
using Windows.Storage.Pickers;
using Windows.Graphics.Imaging;
using Windows.System;
using 配色pro.Library;
using 配色pro.Model;

namespace 配色pro.Control
{
    public sealed partial class MainCanvasControl : UserControl
    {
        public CanvasBitmap ImageBitmap;


        #region DependencyProperty：依赖属性
         


        //刷新
        public int Refresh
        {
            get { return (int)GetValue(RefreshProperty); }
            set { SetValue(RefreshProperty, value); }
        }

        public static readonly DependencyProperty RefreshProperty =
            DependencyProperty.Register("Refresh", typeof(int), typeof(MainCanvasControl), new PropertyMetadata(0, new PropertyChangedCallback(RefreshOnChang)));

        private static void RefreshOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MainCanvasControl Con = (MainCanvasControl)sender;

            Con.ImageCanvas.Invalidate();//刷新画布内容 
        }




        public int Image
        {
            get { return (int)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(int), typeof(MainCanvasControl), new PropertyMetadata(0, new PropertyChangedCallback(ImageOnChang)));
        private static async void ImageOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MainCanvasControl Con = (MainCanvasControl)sender;

            //文件选择器
            FileOpenPicker openPicker = new FileOpenPicker();
            //选择视图模式
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            //初始位置
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            //添加文件类型
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            StorageFile file = await openPicker.PickSingleFileAsync();//打开选择器

            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    Con.ImageBitmap = await CanvasBitmap.LoadAsync(Con.ImageCanvas, stream);
                    Con.LoadImage();//加载图片
                }
            }
        }




        //数据包
        public DataPackageView ImagePackage
        {
            get { return (DataPackageView)GetValue(ImagePackageProperty); }
            set { SetValue(ImagePackageProperty, value); }
        }

        public static readonly DependencyProperty ImagePackageProperty =
            DependencyProperty.Register("ImagePackage", typeof(DataPackageView), typeof(MainCanvasControl), new PropertyMetadata(null, new PropertyChangedCallback(ImagePackageOnChang)));

        private async static void ImagePackageOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MainCanvasControl Con = (MainCanvasControl)sender;
            DataPackageView dataPackageView = e.NewValue as DataPackageView;

            if (dataPackageView != null)
            {
                //切换到画布枢轴面板
                App.Model.SelectedIndex = 2;

                if (dataPackageView.Contains(StandardDataFormats.Bitmap) == true)
                {
                    IRandomAccessStreamReference imageReceived = null;
                    imageReceived = await dataPackageView.GetBitmapAsync();
                    if (imageReceived != null)
                    {
                        using (IRandomAccessStream stream = await imageReceived.OpenReadAsync())
                        {
                            try
                            {
                                Con.ImageBitmap = await CanvasBitmap.LoadAsync(Con.ImageCanvas, stream);
                                Con.LoadImage();//加载图片
                            }
                            catch (Exception)
                            {
                                App.Tip("Fall");

                                //切换到画布枢轴面板
                                App.Model.SelectedIndex = 2;
                            }
                        }
                    }
                }
            }
        }





        //文件
        public StorageFile ImageFile
        {
            get { return (StorageFile)GetValue(ImageFileProperty); }
            set { SetValue(ImageFileProperty, value); }
        }

        public static readonly DependencyProperty ImageFileProperty =
            DependencyProperty.Register("ImageFile", typeof(StorageFile), typeof(MainCanvasControl), new PropertyMetadata(null, new PropertyChangedCallback(ImageFileOnChang)));

        private async static void ImageFileOnChang(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MainCanvasControl Con = (MainCanvasControl)sender;
            StorageFile file = e.NewValue as StorageFile;

            if (file!=null)
            {
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    Con.ImageBitmap = await CanvasBitmap.LoadAsync(Con.ImageCanvas, stream);
                    Con.LoadImage();
                }
            }
        }

        #endregion

        public MainCanvasControl()
        {
            this.InitializeComponent();
        }

        #region Global：全局

        private void ImageCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            App.Model.GridWidth = e.NewSize.Width;
            App.Model.GridHeight = e.NewSize.Height;
        }
        private async void ImagePickerButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //文件选择器
            FileOpenPicker openPicker = new FileOpenPicker();
            //选择视图模式
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            //初始位置
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            //添加文件类型
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            StorageFile file = await openPicker.PickSingleFileAsync();//打开选择器

            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    ImageBitmap = await CanvasBitmap.LoadAsync(ImageCanvas, stream);
                    LoadImage();//加载图片
                }
            }
        }
        #endregion
        


        #region Point：指针事件


        Point Point;//指针移动事件，即使点的位置

        bool isSingle = false;//是否单指
        bool isDouble = false;
        bool isRight = false;

        public List<uint> FingerCollection = new List<uint> { };//可视指集
        Point FingerSingle;//单数点
        Point FingerDouble;//双数点
        Point FingerCenter { get => new Point((FingerSingle.X + FingerDouble.X) / 2, (FingerSingle.Y + FingerDouble.Y) / 2); }//中心点
        double FingerDistance { get => Math.Sqrt((FingerSingle.X - FingerDouble.X) * (FingerSingle.X - FingerDouble.X) + (FingerSingle.Y - FingerDouble.Y) * (FingerSingle.Y - FingerDouble.Y)); }//双指距离

        Point FingerStart;//起始点
        Point FingerStartSingle;//起始单数点
        Point FingerStartDouble;//起始双数点

        //指针进入
        public void Control_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
        }
        //指针退出
        public void Control_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Control_PointerReleased(sender, e);
        }



        //指针按下
        public void Control_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Point p = Judge.Position(e, Control);

            if (Judge.IsTouch(e, Control))
            {
                FingerCollection.Add(e.Pointer.PointerId);
                if (e.Pointer.PointerId % 2 == 0) FingerDouble = p;
                if (e.Pointer.PointerId % 2 == 1) FingerSingle = p;

                if (FingerCollection.Count > 1)
                {
                    if (isDouble == false) //如果双指未开始
                    {
                        FingerStartDouble = FingerDouble;
                        FingerStartSingle = FingerSingle;
                    }
                }
                else
                {
                    if (isSingle == false)//如果单指未开始
                        FingerStart = p;
                }
            }
            else
            {
                if (Judge.IsRight(e, Control))
                {
                    if (isRight == false)
                    {
                        isRight = true;
                        Right_Start(p);
                    }
                }
                if (Judge.IsLeft(e, Control) || Judge.IsPen(e, Control))
                {
                    if (isSingle == false)
                    {
                        isSingle = true;
                        Single_Start(p);
                    }
                }
            }
        }
        //指针松开
        public void Control_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Point p = Judge.Position(e, Control);

            if (isRight == true)
            {
                isRight = false;
                Right_Complete(p);
            }
            if (isDouble == true)
            {
                isDouble = false;
                Double_Complete(p, FingerDistance);

                isSingle = false;//阻止单指结束
            }
            else if (isSingle == true)
            {
                isSingle = false;
                Single_Complete(p);
            }

            FingerCollection.Clear();
        }




        //指针移动
        public void Control_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Point p = Point = Judge.Position(e, Control);


            if (Judge.IsTouch(e, Control))
            {
                if (e.Pointer.PointerId % 2 == 0) FingerDouble = p;
                if (e.Pointer.PointerId % 2 == 1) FingerSingle = p;

                if (FingerCollection.Count > 1)
                {
                    if (isDouble == false) //如果双指未开始
                    {
                        if (Math.Abs(FingerStartDouble.X - FingerDouble.X) > 2 ||
                            Math.Abs(FingerStartDouble.Y - FingerDouble.Y) > 2 ||
                            Math.Abs(FingerStartSingle.X - FingerSingle.X) > 2 ||
                            Math.Abs(FingerStartSingle.Y - FingerSingle.Y) > 2)//中点移动4
                        {
                            isDouble = true;
                            Double_Start(FingerCenter, FingerDistance);
                        }
                    }
                    else if (isDouble == true)
                    {
                        Double_Delta(FingerCenter, FingerDistance);
                    }
                }
                else
                {
                    if (isSingle == false) //如果单指未开始
                    {
                        var d = Method.两点距(FingerStart, p);
                        if (d > 2 && d < 12)//点移动4到20
                        {
                            isSingle = true;
                            Single_Start(p);
                        }
                    }
                    else if (isSingle == true)
                    {
                        Single_Delta(p);
                    }
                }
            }
            else
            {
                if (isRight == true)
                    Right_Delta(p);

                if (isSingle == true)
                    Single_Delta(p);
            }
        }
        //滚轮变化
        private void Control_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            Wheel_Changed(Judge.Position(e, Control), Judge.WheelDelta(e, Control));
        }


        #endregion

        #region Event：封装事件


        //单指&&左键&&笔
        private void Single_Start(Point p)
        {
            if (ImageBitmap != null)
            {
                StrawColor(p);
                StrawMargin(p);
                StrawImage(p);

                StrawShow.Begin();
            }
        }
        private void Single_Delta(Point p)
        {
            if (ImageBitmap != null)
            {
                StrawColor(p);
                StrawMargin(p);
                StrawImage(p);
            }
        }
        private void Single_Complete(Point p)
        {
            if (ImageBitmap != null)
            {
                StrawColor(p);
                StrawMargin(p);
                StrawImage(p);

                StrawFade.Begin();
            }
        }


        #endregion

        #region Move：移动事件


        Point CanvasPointStart;//初始点在画布上的位置

        double DoubleDistanceStart;//开始双指距离
        double CanvasWidthStart;// 开始画布宽度
        double CanvasHeightStart;// 开始画布高度

        double WheelScale = 1;//滚轮缩放比例


        //双指
        private void Double_Start(Point p, double d)
        {
            CanvasPointStart = App.Model.ScreenToCanvas(p);//映射到画布上的点
            DoubleDistanceStart = d;
            CanvasWidthStart = App.Model.CanvasWidth;
            CanvasHeightStart = App.Model.CanvasHeight;
        }
        private void Double_Delta(Point p, double d)
        {
            //缩放
            double scale = d / DoubleDistanceStart;
            var Width = CanvasWidthStart * scale;
            var Height = CanvasHeightStart * scale;
            if ((Width > 70 && Height > 70) || scale > 1)//防止画布过小崩溃
            {
                App.Model.CanvasWidth = Width;
                App.Model.CanvasHeight = Height;

                //移动对齐到画布上的点
                App.Model.X = p.X - CanvasPointStart.X * App.Model.XS;
                App.Model.Y = p.Y - CanvasPointStart.Y * App.Model.YS;
            }

            ImageCanvas.Invalidate();
        }
        private void Double_Complete(Point p, double d)
        {
        }

        //缓存：性能优化
        double RightCacheX;
        double RightCacheY;
        //右键
        private void Right_Start(Point p)
        {
            CanvasPointStart = App.Model.ScreenToCanvas(p);

            //缓存
            RightCacheX = App.Model.X;
            RightCacheY = App.Model.Y;
        }
        private void Right_Delta(Point p)
        {
            //移动对齐
            App.Model.X = p.X - CanvasPointStart.X * App.Model.XS;
            App.Model.Y = p.Y - CanvasPointStart.Y * App.Model.YS;

            //缓存
            if (Math.Abs(RightCacheX - App.Model.X) + Math.Abs(RightCacheY - App.Model.Y) > 10)
            {
                RightCacheX = App.Model.X;
                RightCacheY = App.Model.Y;
                ImageCanvas.Invalidate();
            }
        }
        private void Right_Complete(Point p)
        {
            ImageCanvas.Invalidate();
        }


        //滚轮
        private void Wheel_Changed(Point p, double d)
        {
            //缩放
            if (d > 0) WheelScale = 1.1;
            else if (d < 0) WheelScale = 1 / 1.1;

            var Width = App.Model.CanvasWidth * WheelScale;
            var Height = App.Model.CanvasHeight * WheelScale;
            if ((Width > 70 && Height > 70) || WheelScale > 1)//防止画布过小崩溃
            {
                App.Model.CanvasWidth = Width;
                App.Model.CanvasHeight = Height;

                //当滚轮被按下时被识别为右键按下
                if (isRight == true) Right_Delta(p);
                else
                {
                    App.Model.X = (p.X - (p.X - App.Model.X) * WheelScale);
                    App.Model.Y = (p.Y - (p.Y - App.Model.Y) * WheelScale);
                }
            }

            ImageCanvas.Invalidate();
        }


        #endregion



        #region Straw：吸管


        //Grid：拖动
        Thickness StrawGridThickness = new Thickness();
        private void StrawMargin(Point p)
        {
            StrawGridThickness.Top = Point.Y - StrawGrid.Height;
            StrawGridThickness.Left = Point.X - StrawGrid.Width / 2;
            StrawGrid.Margin = StrawGridThickness;
        }



        //Color：变色
        private void StrawColor(Point p)//主取色
        {
            Point CP = App.Model.ScreenToCanvas(p);

            int X = (int)CP.X;
            int Y = (int)CP.Y;
            if (X > 0 && X < App.Model.Width && Y > 0 && Y < App.Model.Height)
            {
                //当前图层还是全局图层
                App.Model.Color = ImageBitmap.GetPixelColors(X, Y, 1, 1).Single();
            }
        }

        //Image：缩略图
        Point StrawPoint;//吸管吸取的源图点
        private void StrawImage(Point p)
        {
            StrawPoint = App.Model.ScreenToImage(p);

            StrawCanvas.Invalidate();
        }
        private void StrawCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (ImageBitmap != null)
            {

                args.DrawingSession.DrawImage(
                    new ScaleEffect
                    {
                        Source = ImageBitmap,
                        Scale = new Vector2((float)App.Model.XS * 2f)
                    },
                (float)(-2 * StrawPoint.X + StrawCanvasGrid.ActualWidth / 2),
               (float)(-2 * StrawPoint.Y + StrawCanvasGrid.ActualHeight / 2));
            }
        }


        #endregion

        #region Virtual & Animated：虚拟 & 动画


        private void ImageCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (ImageBitmap != null)
            {
                Matrix3x2 m = App.Model.Matrix;

                args.DrawingSession.DrawImage(
                    new Transform2DEffect
                    {
                        Source = ImageBitmap,
                        TransformMatrix = m,
                        InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    });

                if (App.Model.isLine)  LineDraw(args.DrawingSession);
                RulerDraw(args.DrawingSession);
            }
        }





        #endregion

        #region Second ：第二界面


        //Line：网格线
        Color LineColor = Color.FromArgb(90, 255, 255, 255);//字体颜色
        Color LineSignColor = Color.FromArgb(180, 255, 255, 255);//高亮字体颜色
        //Line：画网格线
        private void LineDraw(CanvasDrawingSession ds)
        {
            //左上右下的位置
            float LineL = (float)App.Model.X;
            float LineT = (float)App.Model.Y;
            float LineR = LineL + (float)App.Model.CanvasWidth;
            float LineB = LineT + (float)App.Model.CanvasHeight;


            //间隔
            float space = (float)(10 * App.Model.YS);
            while (space < 10) space *= 5; //大则小
            while (space > 100) space /= 5;//小则大
            float spaceFive = space * 5;//五倍


            //线循环
            for (float X = 0; X < App.Model.CanvasWidth; X += space)
            {
                float xx = LineL + X;
                ds.DrawLine(xx, LineT, xx, LineB, LineColor);
            }
            for (float Y = 0; Y < App.Model.CanvasHeight; Y += space)
            {
                float yy = LineT + Y;
                ds.DrawLine(LineL, yy, LineR, yy, LineColor);
            }


            //实线循环
            for (float X = 0; X < App.Model.CanvasWidth; X += spaceFive)
            {
                float xx = LineL + X;
                ds.DrawLine(xx, LineT, xx, LineB, LineColor);
            }
            for (float Y = 0; Y < App.Model.CanvasHeight; Y += spaceFive)
            {
                float yy = LineT + Y;
                ds.DrawLine(LineL, yy, LineR, yy, LineColor);
            }
        }

        //Ruler：画标尺线
        Color RulerColor = Color.FromArgb(127, 127, 127, 127);//字体颜色
        Color RulerSignColor = Colors.Gray;//高亮字体颜色
        float RulerSpace = 20;//标尺刻度空间
        float RulerHalf = 10;//一半的标尺刻度空间
        CanvasTextFormat RulerTextFormat = new CanvasTextFormat()//字体格式
        {
            FontSize = 12,
            HorizontalAlignment = CanvasHorizontalAlignment.Center,
            VerticalAlignment = CanvasVerticalAlignment.Center,
        };
        //Ruler：画标尺线
        private void RulerDraw(CanvasDrawingSession ds)
        {
            float GridWidth = (float)App.Model.GridWidth;
            float GridHeight = (float)App.Model.GridHeight;

            //画背景颜色
            ds.DrawLine(RulerSpace, RulerSpace, GridWidth, RulerSpace, RulerColor);//水平线
            ds.DrawLine(RulerSpace, RulerSpace, RulerSpace, GridHeight, RulerColor);//垂直线


            //间隔
            float space = (float)(10 * App.Model.YS);
            while (space < 10) space *= 5; //大则小
            while (space > 100) space /= 5;//小则大
            float spaceFive = space * 5;//五倍


            //水平线循环
            for (float X = (float)App.Model.X; X < GridWidth; X += space) ds.DrawLine(X, RulerHalf, X, RulerSpace, RulerColor);
            for (float X = (float)App.Model.X; X > RulerSpace; X -= space) ds.DrawLine(X, RulerHalf, X, RulerSpace, RulerColor);
            //垂直线循环
            for (float Y = (float)App.Model.Y; Y < App.Model.GridHeight; Y += space) ds.DrawLine(RulerHalf, Y, RulerSpace, Y, RulerColor);
            for (float Y = (float)App.Model.Y; Y > RulerSpace; Y -= space) ds.DrawLine(RulerHalf, Y, RulerSpace, Y, RulerColor);


            //水平实线循环
            for (float X = (float)App.Model.X; X < GridWidth; X += spaceFive) ds.DrawLine(X, RulerHalf, X, RulerSpace, RulerSignColor);
            for (float X = (float)App.Model.X; X > RulerSpace; X -= spaceFive) ds.DrawLine(X, RulerHalf, X, RulerSpace, RulerSignColor);
            //垂直实线循环
            for (float Y = (float)App.Model.Y; Y < App.Model.GridHeight; Y += spaceFive) ds.DrawLine(RulerHalf, Y, RulerSpace, Y, RulerSignColor);
            for (float Y = (float)App.Model.Y; Y > RulerSpace; Y -= spaceFive) ds.DrawLine(RulerHalf, Y, RulerSpace, Y, RulerSignColor);


            //水平文字循环
            for (float X = (float)App.Model.X; X < GridWidth; X += spaceFive) ds.DrawText(((int)(Math.Round((X - App.Model.X) / App.Model.XS))).ToString(), X, RulerHalf, RulerSignColor, RulerTextFormat);
            for (float X = (float)App.Model.X; X > RulerSpace; X -= spaceFive) ds.DrawText(((int)(Math.Round((X - App.Model.X) / App.Model.XS))).ToString(), X, RulerHalf, RulerSignColor, RulerTextFormat);
            //垂直文字循环
            for (float Y = (float)App.Model.Y; Y < App.Model.GridHeight; Y += spaceFive) ds.DrawText(((int)(Math.Round((Y - App.Model.Y) / App.Model.YS))).ToString(), RulerHalf, Y, RulerSignColor, RulerTextFormat);
            for (float Y = (float)App.Model.Y; Y > RulerSpace; Y -= spaceFive) ds.DrawText(((int)(Math.Round((Y - App.Model.Y) / App.Model.YS))).ToString(), RulerHalf, Y, RulerSignColor, RulerTextFormat);

        }





        #endregion




        private void LoadImage()
        {
            ImageCanvas.Draw -= ImageCanvas_Draw;

            App.Model.Width = (int)ImageBitmap.SizeInPixels.Width;
            App.Model.Height = (int)ImageBitmap.SizeInPixels.Height;

            App.Model.CanvasWidth = App.Model.Width;
            App.Model.CanvasHeight = App.Model.Height;

            ImageCanvas.Invalidate();
            ImagePickerButton.Visibility = Visibility.Collapsed;

            ImageCanvas.Draw += ImageCanvas_Draw;
        }

    }
}
