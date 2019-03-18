using 配色pro.Library;
using 配色pro.Pickers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace 配色pro.Pages
{
    public sealed partial class EunmPage : Page
    {
        //Delegate
        public event ColorChangeHandler ColorChange = null;
                
        public EunmPage()
        {
            this.InitializeComponent();

            this.GridView.Loaded +=async (s, e) => this.GridView.ItemsSource =await this.GetFilterSource();
        }


        //ColorsItem
        private async Task<IEnumerable<ColorsItem>> GetFilterSource()
        {
            string json = await this.ReadFromLocalFolder("Enumerate.json");

            if (json == null)
            {
                json = await this.ReadFromApplicationPackage("ms-appx:///Json/Enumerate.json");
                this.WriteToLocalFolder(json, "ms-appx:///Json/Enumerate.json");
            }
            IEnumerable<ColorsItem> source = JsonConvert.DeserializeObject<IEnumerable<ColorsItem>>(json);

            return source;
        }


        #region File


        /// <summary> Read json file from Application Package. </summary> 
        public async Task<string> ReadFromApplicationPackage(string fileName)
        {
            Uri uri = new Uri(fileName);
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            return await FileIO.ReadTextAsync(file);
        }

        /// <summary> Read json file from Local Folder. </summary> 
        public async Task<string> ReadFromLocalFolder(string fileName)
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return await FileIO.ReadTextAsync(file);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Write json file to Local Folder. </summary> 
        public async void WriteToLocalFolder(string json, string fileName)
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
            }
            catch (Exception)
            {
            }
        }


        #endregion

    }
}
