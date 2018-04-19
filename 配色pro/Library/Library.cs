using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Provider;
using System.Threading.Tasks;

namespace 配色pro.Library
{
    class Library
    {
        public static async Task<FileUpdateStatus> SaveToPngImage(WriteableBitmap bitmap, PickerLocationId location, string fileName)
        {
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = location
            };
            savePicker.FileTypeChoices.Add("Png Image", new[] { ".png" });
            savePicker.SuggestedFileName = fileName;
            StorageFile sFile = await savePicker.PickSaveFileAsync();
            if (sFile != null)
            {
                CachedFileManager.DeferUpdates(sFile);


                using (var fileStream = await sFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
                    Stream pixelStream = bitmap.PixelBuffer.AsStream();
                    byte[] pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                              (uint)bitmap.PixelWidth,
                              (uint)bitmap.PixelHeight,
        .0,
        .0,
                              pixels);
                    await encoder.FlushAsync();
                }


                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(sFile);
                return status;
            }
            return FileUpdateStatus.Failed;
        }




        //效率；中
        public static Color[,] 图转组(WriteableBitmap writeableBitmap)
        #region Color[,] 图转组()
        {
            //《图转化成流》
            /////////////////////////////////////////////////////////////////////////////
            int width = writeableBitmap.PixelWidth; //获取宽度
            int height = writeableBitmap.PixelHeight;//获取高度

            Stream stream = writeableBitmap.PixelBuffer.AsStream();//Bitmap=>Stream
            byte[] buffer = new byte[stream.Length]; //创建新Buffer，长度是Stream的长度
            stream.Read(buffer, 0, buffer.Length); //Stream.Read写入，由Buffer引用（从零到其长度）

            //《流转化成组》
            /////////////////////////////////////////////////////////////////////////////

            Color[,] color = new Color[height, width]; //声明颜色二维数组，用来存储图片

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int Num = (y * width + x) * 4;//计算第一个点的位置

                    color[y, x] = Color.FromArgb(buffer[Num + 3], buffer[Num + 2], buffer[Num + 1], buffer[Num]);//改变颜色，buffer=>color[,]
                }
            }
            return color; //返回颜色二维数组
        }
        #endregion


        //效率；中
        public static WriteableBitmap 组转图(Color[,] color)
        #region WriteableBitmap 组转图()
        {
            //《组转化成流》
            //////////////////////////////////////////////////////////////////////////////////////////////////
            int height = color.GetLength(0); //获取二维数组的2    { 1,1,1,1,1 }
            int width = color.GetLength(1); //获取二维数组的5     { 1,1,1,1,1 }

            Byte[] buffer = new Byte[color.Length * 4]; //创建流，长度是数组长度的4倍，存储BGRA四个分量

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color cooo = color[y, x]; //临时颜色

                    int Num = (y * width + x) * 4;//计算第一个点的位置

                    buffer[Num] = cooo.B;//蓝色B
                    buffer[Num + 1] = cooo.G;//绿色G
                    buffer[Num + 2] = cooo.R;//红色R
                    buffer[Num + 3] = cooo.A;//透明度Alpha
                }
            }
            //《流转化成图》
            //////////////////////////////////////////////////////////////////////////////////////////////////
            WriteableBitmap writeableBitmap = new WriteableBitmap(width, height);//创建新Bitmap，宽高是图片宽高
            Stream stream = writeableBitmap.PixelBuffer.AsStream(); //Bitmap=>Stream
            stream.Seek(0, SeekOrigin.Begin);//Stream.Seek寻找（从零开始）
            stream.Write(buffer, 0, buffer.Length);//Stream.Write写入，写入Buffer（从零到其长度）

            return writeableBitmap;
        }
        #endregion
        //效率；中（缩小后的图）（图层栏的缩略图）：缩放为height*width大小
        public static WriteableBitmap 组转图(Color[,] color, int height, int width)
        #region WriteableBitmap 组转图()
        {
            //《组转化成流》
            //////////////////////////////////////////////////////////////////////////////////////////////////
            int 倍height = color.GetLength(0) / height; //获得组与图的高度缩放倍数
            int 倍width = color.GetLength(1) / width; //获得组与图的宽度缩放倍数

            Byte[] buffer = new Byte[width * height * 4]; //创建流，长度是数组长度的4倍，存储BGRA四个分量

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color cooo = color[倍height * y, 倍width * x]; //临时颜色

                    int Num = (y * width + x) * 4;//计算第一个点的位置

                    buffer[Num] = cooo.B;//蓝色B
                    buffer[Num + 1] = cooo.G;//绿色G
                    buffer[Num + 2] = cooo.R;//红色R
                    buffer[Num + 3] = cooo.A;//透明度Alpha
                }
            }
            //《流转化成图》
            //////////////////////////////////////////////////////////////////////////////////////////////////
            WriteableBitmap writeableBitmap = new WriteableBitmap(width, height);//创建新Bitmap，宽高是图片宽高
            Stream stream = writeableBitmap.PixelBuffer.AsStream(); //Bitmap=>Stream
            stream.Seek(0, SeekOrigin.Begin);//Stream.Seek寻找（从零开始）
            stream.Write(buffer, 0, buffer.Length);//Stream.Write写入，写入Buffer（从零到其长度）
                                                   // stream.WriteAsync

            return writeableBitmap;
        }
        #endregion





        //效率；中 上图压住下图
        public static Color[,] 组压组转组(Color[,] 上组, Color[,] 下组)
        #region Color[,] 组压组转组()
        {
            int height = 下组.GetLength(0); //获取二维数组的2    { 1,1,1,1,1 }
            int width = 下组.GetLength(1); //获取二维数组的5     { 1,1,1,1,1 }

            Color[,] 中间组 = new Color[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x < 上组.GetLength(1) && y < 上组.GetLength(0)) //判断：不超过上组边界（在下图大于上图时）
                    {
                        中间组[y, x] = Method.ColorBlend(上组[y, x], 下组[y, x]);
                    }
                }
            }
            return 中间组; //返回中间组
        }
        #endregion
        //效率；中 上图压住下图（包含了下图层的透明度 byte alpha 0~100）
        public static Color[,] 组压组转组(Color[,] 上组, Color[,] 下组, int 下alpha)
        #region Color[,] 组压组转组()
        {
            int height = 下组.GetLength(0); //获取二维数组的2    { 1,1,1,1,1 }
            int width = 下组.GetLength(1); //获取二维数组的5     { 1,1,1,1,1 }

            Color[,] 中间组 = new Color[height, width];

            //下图层透明度为100
            if (下alpha == 100)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x < 上组.GetLength(1) && y < 上组.GetLength(0)) //判断：不超过上组边界（在下图大于上图时）
                        {
                            中间组[y, x] = Method.ColorBlend(上组[y, x], 下组[y, x]);
                        }
                    }
                }
            }
            //下图层透明度为其他
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x < 上组.GetLength(1) && y < 上组.GetLength(0)) //判断：不超过上组边界（在下图大于上图时）
                        {
                            double 下组A = (double)下alpha / 100.0d * (double)下组[y, x].A;

                            Color 下组色 = Color.FromArgb((byte)下组A, 下组[y, x].R, 下组[y, x].G, 下组[y, x].B);

                            中间组[y, x] = Method.ColorBlend(上组[y, x], 下组色);
                        }
                    }
                }
            }

            return 中间组; //返回中间组(byte)(
        }
        #endregion
        //效率；中 上图压住下图（包含了下图层的透明度 byte alpha 0~100）
        public static Color[,] 组压组转组不安全(Color[,] 上组, Color[,] 下组, int 下alpha)
        #region Color[,] 组压组转组不安全()
        {
            int height = 下组.GetLength(0); //获取二维数组的2    { 1,1,1,1,1 }
            int width = 下组.GetLength(1); //获取二维数组的5     { 1,1,1,1,1 }

            Color[,] 中间组 = new Color[height, width];

            //下图层透明度为100
            if (下alpha == 100)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        中间组[y, x] = Method.ColorBlend(上组[y, x], 下组[y, x]);
                    }
                }
            }
            //下图层透明度为其他
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double 下组A = (double)下alpha / 100.0d * (double)下组[y, x].A;

                        Color 下组色 = Color.FromArgb((byte)下组A, 下组[y, x].R, 下组[y, x].G, 下组[y, x].B);

                        中间组[y, x] = Method.ColorBlend(上组[y, x], 下组色);
                    }
                }
            }

            return 中间组; //返回中间组(byte)(
        }
        #endregion
        //效率；中 上图压住下图（包含了上下图层的透明度 byte alpha 0~100）
        public static Color[,] 组压组转组(Color[,] 上组, int 上alpha, Color[,] 下组, int 下alpha)
        #region Color[,] 组压组转组()
        {
            int height = 下组.GetLength(0); //获取二维数组的2    { 1,1,1,1,1 }
            int width = 下组.GetLength(1); //获取二维数组的5     { 1,1,1,1,1 }

            Color[,] 中间组 = new Color[height, width];

            //上下图层透明度为100
            if (上alpha == 100 && 下alpha == 100)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x < 上组.GetLength(1) && y < 上组.GetLength(0)) //判断：不超过上组边界（在下图大于上图时）
                        {
                            中间组[y, x] = Method.ColorBlend(上组[y, x], 下组[y, x]);
                        }
                    }
                }
            }
            //上图层透明度为100
            else if (上alpha == 100)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x < 上组.GetLength(1) && y < 上组.GetLength(0)) //判断：不超过上组边界（在下图大于上图时）
                        {
                            中间组[y, x] = Method.ColorBlend(上组[y, x], Color.FromArgb((byte)((double)下alpha / 100.0d * (double)下组[y, x].A), 下组[y, x].R, 下组[y, x].G, 下组[y, x].B));
                        }
                    }
                }
            }
            //下图层透明度为100
            else if (下alpha == 100)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x < 上组.GetLength(1) && y < 上组.GetLength(0)) //判断：不超过上组边界（在下图大于上图时）
                        {
                            中间组[y, x] = Method.ColorBlend(Color.FromArgb((byte)((double)上alpha / 100.0d * (double)上组[y, x].A), 上组[y, x].R, 上组[y, x].G, 上组[y, x].B), 下组[y, x]);
                        }
                    }
                }
            }
            //下图层透明度为其他
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x < 上组.GetLength(1) && y < 上组.GetLength(0)) //判断：不超过上组边界（在下图大于上图时）
                        {
                            //《原文》
                            //double 上组A = (double)上alpha / 100.0d * (double)上组[y, x].A;
                            //double 下组A = (double)下alpha / 100.0d * (double)下组[y, x].A;

                            // Color 上组色 = Color.FromArgb((byte)上组A, 上组[y, x].R, 上组[y, x].G, 上组[y, x].B);
                            //Color 下组色 = Color.FromArgb((byte)下组A, 下组[y, x].R, 下组[y, x].G, 下组[y, x].B);

                            //中间组[y, x] = Method.正常叠加(上组色, 下组色);

                            //《精简》
                            中间组[y, x] = Method.ColorBlend(Color.FromArgb((byte)((double)上alpha / 100.0d * (double)上组[y, x].A), 上组[y, x].R, 上组[y, x].G, 上组[y, x].B), Color.FromArgb((byte)((double)下alpha / 100.0d * (double)下组[y, x].A), 下组[y, x].R, 下组[y, x].G, 下组[y, x].B));
                        }
                    }
                }
            }
            return 中间组; //返回中间组(byte)

        }
        #endregion
        //效率；高 上图压住下图（包含了上下图层的透明度 byte alpha 0~100）
        public static Color[,] 组压组转组不安全(Color[,] 上组, int 上alpha, Color[,] 下组, int 下alpha)
        #region Color[,] 组压组转组不安全()
        {
            int height = 下组.GetLength(0); //获取二维数组的2    { 1,1,1,1,1 }
            int width = 下组.GetLength(1); //获取二维数组的5     { 1,1,1,1,1 }

            Color[,] 中间组 = new Color[height, width];

            //上下图层透明度为100
            if (上alpha == 100 && 下alpha == 100)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        中间组[y, x] = Method.ColorBlend(上组[y, x], 下组[y, x]);
                    }
                }
            }
            //上图层透明度为100
            else if (上alpha == 100)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        中间组[y, x] = Method.ColorBlend(上组[y, x], Color.FromArgb((byte)((double)下alpha / 100.0d * (double)下组[y, x].A), 下组[y, x].R, 下组[y, x].G, 下组[y, x].B));
                    }
                }
            }
            //下图层透明度为100
            else if (下alpha == 100)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        中间组[y, x] = Method.ColorBlend(Color.FromArgb((byte)((double)上alpha / 100.0d * (double)上组[y, x].A), 上组[y, x].R, 上组[y, x].G, 上组[y, x].B), 下组[y, x]);
                    }
                }
            }
            //下图层透明度为其他
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        中间组[y, x] = Method.ColorBlend(Color.FromArgb((byte)((double)上alpha / 100.0d * (double)上组[y, x].A), 上组[y, x].R, 上组[y, x].G, 上组[y, x].B), Color.FromArgb((byte)((double)下alpha / 100.0d * (double)下组[y, x].A), 下组[y, x].R, 下组[y, x].G, 下组[y, x].B));
                    }
                }
            }
            return 中间组; //返回中间组(byte)

        }
        #endregion



        //效率；高 自己算自己的图层的透明度，得到的视觉上的图层
        public static Color[,] 组变透明(Color[,] 组, int alpha)
        #region Color[,] 组变透明()
        {
            int height = 组.GetLength(0); //获取二维数组的2    { 1,1,1,1,1 }
            int width = 组.GetLength(1); //获取二维数组的5     { 1,1,1,1,1 }

            Color[,] 中间组 = new Color[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double A = (double)alpha / 100.0d * (double)组[y, x].A;

                    中间组[y, x].A = (byte)(A);
                    中间组[y, x].R = 组[y, x].R;
                    中间组[y, x].G = 组[y, x].G;
                    中间组[y, x].B = 组[y, x].B;
                }

            }
            return 中间组; //返回中间组
        }
        #endregion


        //效率；高 剪裁图片（暴力裁为相应的小，适合缩小）
        public static Color[,] 组裁剪转组(Color[,] 上组, int height, int width)
        #region Color[,] 组裁剪转组()
        {
            Color[,] 中间组 = new Color[height, width];

            int 上height = 上组.GetLength(0); //获取二维数组的2    { 1,1,1,1,1 }
            int 上width = 上组.GetLength(1); //获取二维数组的5     { 1,1,1,1,1 }

            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    if (x < 上width && y < 上height) //判断：不超过上组边界（在中间组大于上图时）
                    {
                        中间组[y, x] = 上组[y, x]; //上组不透明，直接用上图代替中间组
                    }
                    else 中间组[y, x] = Color.FromArgb(0, 0, 0, 0);
                }
            }
            return 中间组; //返回中间组
        }
        #endregion
        //效率；高 剪裁图片（具有纵横位移的裁剪图片方法）（选图变换）裁到width*height大小的画布上
        public static Color[,] 组裁剪转组(Color[,] 上组, int height, int width, int Horizontal, int Vertical)//（选图，画布高，画布宽，横位移，纵位移）
        #region Color[,] 组裁剪转组()
        {
            Color[,] 中间组 = new Color[height, width];

            int 上height = 上组.GetLength(0); //获取二维数组的2    { 1,1,1,1,1 }
            int 上width = 上组.GetLength(1); //获取二维数组的5     { 1,1,1,1,1 }

            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    int 差Horizontal = x - Horizontal; //横的差距
                    int 差Vertical = y - Vertical; //纵的差距

                    if (差Horizontal >= 0 && 差Horizontal < 上width && 差Vertical >= 0 && 差Vertical < 上height) //判断：不超过上组边界（在中间组大于上图时）
                    {
                        中间组[y, x] = 上组[差Vertical, 差Horizontal]; //上组不透明，直接用上图代替中间组
                    }
                    else 中间组[y, x] = Color.FromArgb(0, 0, 0, 0);

                }
            }

            return 中间组; // 返回中间组
        }
        #endregion

    }
}
