using Color_Scheme.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Color_Scheme.Pages
{
    public sealed partial class EnumPage : Page
    {
        //State
        public MainPageState State { set => this.Visibility = (value == MainPageState.EunmType) ? Visibility.Visible : Visibility.Collapsed; }

        public EnumPage()
        {
            this.InitializeComponent();
            this.GridView.IsItemClickEnabled = true;
            this.GridView.ItemClick += (s, e) =>MainPage.Color= ((ColorsItem)e.ClickedItem).Color;
            this.GridView.Loaded +=async (s, e) => this.GridView.ItemsSource =await EnumPage.GetFilterSource();
        }


        //ColorsItem
        public static async Task<IEnumerable<ColorsItem>> GetFilterSource()
        {
            string json = await EnumPage.ReadFromLocalFolder("Enumerate.json");

            if (json == null)
            {
                json = await EnumPage.ReadFromApplicationPackage("ms-appx:///Json/Enumerate.json");
                EnumPage.WriteToLocalFolder(json, "ms-appx:///Json/Enumerate.json");
            }
            IEnumerable<ColorsItem> source = JsonConvert.DeserializeObject<IEnumerable<ColorsItem>>(json);

            return source;
        }


        #region File


        /// <summary> Read json file from Application Package. </summary> 
        public static async Task<string> ReadFromApplicationPackage(string fileName)
        {
            Uri uri = new Uri(fileName);
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            return await FileIO.ReadTextAsync(file);
        }

        /// <summary> Read json file from Local Folder. </summary> 
        public static async Task<string> ReadFromLocalFolder(string fileName)
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
        public static async void WriteToLocalFolder(string json, string fileName)
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
