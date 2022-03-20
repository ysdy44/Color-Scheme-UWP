using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

namespace Color_Scheme
{
    public static partial class Clipboard2
    {

        public static void CopyText(string value)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(value);
            Clipboard.SetContent(dataPackage);
        }
        public static async Task<string> PasteTextAsync()
        {
            DataPackageView package = Clipboard.GetContent();
            if (package == null) return null;
            if (package.Contains(StandardDataFormats.Text) == false) return null;

            return await package.GetTextAsync();
        }

        public static async Task<uint> CopyBitmapAsync(byte[] byteList)
        {
            IBuffer buffer = byteList.AsBuffer();
            IRandomAccessStream randomStream = new InMemoryRandomAccessStream();

            DataWriter dataWriter = new DataWriter(randomStream);
            dataWriter.WriteBuffer(buffer, 0, buffer.Length);
            uint length = await dataWriter.StoreAsync();

            Clipboard2.CopyBitmap(RandomAccessStreamReference.CreateFromStream(randomStream));
            return length;
        }
        public static void CopyBitmap(RandomAccessStreamReference value)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetBitmap(value);
            Clipboard.SetContent(dataPackage);
        }
        public static async Task<IRandomAccessStreamReference> PasteBitmapAsync()
        {
            DataPackageView package = Clipboard.GetContent();
            if (package == null) return null;
            if (package.Contains(StandardDataFormats.Bitmap) == false) return null;

            return await package.GetBitmapAsync();
        }

    }
}