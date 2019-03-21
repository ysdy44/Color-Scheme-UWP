using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;

namespace 配色pro.Library
{
    public class ImagePicker
    {
        public readonly static string JPG = "JPG";
        public readonly static string JPEG = "JPEG";
        public readonly static string PNG = "PNG";
        public readonly static string GIF = "GIF";
        public readonly static string BMP = "BMP";
        public static bool ContainsFormat(String type) => type.Contains(ImagePicker.JPG) || type.Contains(ImagePicker.JPEG) || type.Contains(ImagePicker.PNG) || type.Contains(ImagePicker.GIF) || type.Contains(ImagePicker.BMP);

        //Color
        public static Color? GetColorFromBitmap(CanvasBitmap bitmap, Vector2 vector) => ImagePicker.GetColorFromBitmap(bitmap, (int)vector.X, (int)vector.Y);
        private static Color? GetColorFromBitmap(CanvasBitmap bitmap, int x, int y) => (x > 0 && x < bitmap.SizeInPixels.Width && y > 0 && y < bitmap.SizeInPixels.Height) ? bitmap.GetPixelColors(x, y, 1, 1).Single() : (Color?)null;

        //File
        public static async Task<StorageFile> GetFileFromDrop(DragEventArgs e) => e.DataView.Contains(StandardDataFormats.StorageItems) ? (await ImagePicker.GetFilesFromDrop(e)).First() as StorageFile : null;
        private static async Task<IEnumerable<IStorageFile>> GetFilesFromDrop(DragEventArgs e) => (await e.DataView.GetStorageItemsAsync()).OfType<StorageFile>().Where(ImagePicker.FileTypeWithFormat);
        private static bool FileTypeWithFormat(StorageFile file) => (file == null) ? false : ImagePicker.ContainsFormat(file.FileType.ToUpper());

        //Stream
        public static async Task<IRandomAccessStream> GetStreamFromClipboard() => await ImagePicker.GetStreamFromClipboard(Clipboard.GetContent());
        private static async Task<IRandomAccessStream> GetStreamFromClipboard(DataPackageView package)
        {
            if (package == null) return null;
            if (!package.Contains(StandardDataFormats.Bitmap)) return null;

            RandomAccessStreamReference stream = await package.GetBitmapAsync();
            if (stream == null) return null;

            return await stream.OpenReadAsync();
        }

        //Picker
        public static async Task<StorageFile> GetFileFromPicker() => await new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail,
            SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            FileTypeFilter =
            {
                ImagePicker.JPG,
                ImagePicker.JPEG,
                ImagePicker.PNG,
                ImagePicker.BMP
            }
        }.PickSingleFileAsync();
    }
}
