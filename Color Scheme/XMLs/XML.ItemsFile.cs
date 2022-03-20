using Color_Scheme.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Color_Scheme
{
    /// <summary>
    /// Provide constant and static methods for XElement.
    /// </summary>
    public static partial class XML
    {

        public static async Task<IEnumerable<Item>> ConstructItemsFile()
        {
            StorageFile file = null;
            bool isLocalFilterExists = await FileUtil.IsFileExistsInLocalFolder("Items.xml");

            if (isLocalFilterExists)
            {
                // Read the file from the local folder.
                file = await ApplicationData.Current.LocalFolder.GetFileAsync("Items.xml");
            }
            else
            {
                // Read the file from the package.
                file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///XMLs/Items.xml"));

                // Copy to the local folder.
                await file.CopyAsync(ApplicationData.Current.LocalFolder);
            }

            if (file is null) return null;

            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                try
                {
                    XDocument document = XDocument.Load(stream);

                    IEnumerable<Item> source = XML.LoadItems(document);
                    return source;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static async Task SaveItemsFile(IEnumerable<Item> items)
        {
            XDocument document = XML.SaveItems(items);

            // Save the xml file.      
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Items.xml", CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (Stream stream = fileStream.AsStream())
                {
                    document.Save(stream);
                }
            }
        }

    }
}