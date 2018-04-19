using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using 配色pro.Model;
using Windows.ApplicationModel.DataTransfer;


namespace 配色pro
{ 
    sealed partial class App : Application
    {
        //明暗主题：实例化
        public static Model.Model Model = new Model.Model();
        
        public static string Path = ApplicationData.Current.LocalFolder.Path + "/Colors.xml";

        public async static void Tip(string s)//全局提示
        {
            App.Model.Tip = s;
            App.Model.TipVisibility = Visibility.Visible;
            await Task.Delay(1000);
            App.Model.TipVisibility = Visibility.Collapsed;
        }




        private async void EnmuList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)  //集合改变
        {
            //1、创建一个XDocument对象  
            XDocument xDoc = new XDocument();
            XDeclaration XDec = new XDeclaration("1.0", "utf-8", "no");
            xDoc.Declaration = XDec;

            //2、创建根节点  
            XElement root = new XElement("Colors");
            xDoc.Add(root);

            //4、循环创建节点  
            foreach (var item in App.Model.EnmuList)
            {
                XElement coo = new XElement("Color");
                root.Add(coo);

                //4、创建元素
                XElement Name = new XElement("Name", item.Name);
                coo.Add(Name);
                XElement A = new XElement("A", item.Color.A);
                coo.Add(A);
                XElement R = new XElement("R", item.Color.R);
                coo.Add(R);
                XElement G = new XElement("G", item.Color.G);
                coo.Add(G);
                XElement B = new XElement("B", item.Color.B);
                coo.Add(B);
            }

            //5、保存
            //     xDoc.Save(App.Path);
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Colors", CreationCollisionOption.ReplaceExisting);
            if (file != null)
            {
                try
                {
                    using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        xDoc.Save(fileStream.AsStream());
                    }
                }
                catch (FileLoadException)
                {

                    //throw;
                }
            }
        }
        public static void DocumentLoad()//加载文档
        {
            XDocument xdoc = XDocument.Load(App.Path);

            var dataInfo = from a in xdoc.Descendants("Color")
                           select new
                           {
                               Name = a.Element("Name").Value,
                               A = a.Element("A"),
                               R = a.Element("R"),
                               G = a.Element("G"),
                               B = a.Element("B"),
                           };

            foreach (var date in dataInfo)
            {
                int a = (int)date.A;
                int r = (int)date.R;
                int g = (int)date.G;
                int b = (int)date.B;

                App.Model.EnmuList.Add(new ColorBrushCode
                {
                    Name = date.Name,
                    Color = Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b)
                });
            }
           
        }
        public static void DocumentNew()  //新建文档
        {
            App.Model.EnmuList.Clear();

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 255, 255), Name = "白" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 223, 223, 223), Name = "灰白" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 191, 191, 191), Name = "白灰" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 159, 159, 159), Name = "浅灰" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 127, 127, 127), Name = "深灰" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 63, 63, 63), Name = "黑灰" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 31, 31, 31), Name = "灰黑" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 0, 0, 0), Name = "黑" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 192, 203), Name = "粉红" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 220, 20, 60), Name = "猩红" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 240, 245), Name = "苍白紫罗兰红" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 219, 112, 147), Name = "淡紫红" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 105, 180), Name = "热情粉红" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 199, 21, 133), Name = "适中紫罗兰红" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 218, 112, 214), Name = "兰花紫" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 216, 191, 216), Name = "苍紫" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 221, 160, 221), Name = "轻紫" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 238, 130, 238), Name = "紫罗兰" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 0, 255), Name = "洋紫" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 139, 0, 139), Name = "深洋紫" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 128, 0, 128), Name = "紫" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 186, 85, 211), Name = "适中兰花紫" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 148, 0, 211), Name = "深紫罗兰" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 75, 0, 130), Name = "靓青" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 138, 43, 226), Name = "蓝紫罗兰" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 147, 112, 219), Name = "适中的紫" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 123, 104, 238), Name = "适中板岩蓝" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 106, 90, 205), Name = "板岩蓝" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 72, 61, 139), Name = "深板岩蓝" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 230, 230, 250), Name = "薰衣草淡紫" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 0, 0, 205), Name = "蓝" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 25, 25, 112), Name = "午夜蓝" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 0, 0, 139), Name = "深蓝" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 0, 0, 128), Name = "海军蓝" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 65, 105, 225), Name = "皇家蓝" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 100, 149, 237), Name = "矢车菊蓝" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 119, 136, 153), Name = "浅石板灰" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 112, 128, 144), Name = "石板灰" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 30, 144, 255), Name = "道奇蓝" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 240, 248, 255), Name = "爱丽丝蓝" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 70, 130, 180), Name = "钢蓝" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 135, 206, 250), Name = "淡天蓝" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 135, 206, 235), Name = "天蓝" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 0, 191, 255), Name = "深天蓝" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 173, 216, 230), Name = "淡蓝" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 176, 216, 230), Name = "火药蓝" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 95, 158, 160), Name = "军校蓝" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 240, 255, 255), Name = "蔚蓝" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 224, 255, 255), Name = "淡青" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 175, 238, 238), Name = "苍白宝石绿" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 0, 255, 255), Name = "青" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 0, 206, 209), Name = "深宝石绿" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 47, 79, 79), Name = "深石板灰" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 0, 139, 139), Name = "深青色" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 0, 128, 128), Name = "水鸭色" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 72, 209, 204), Name = "适中宝石绿" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 32, 178, 170), Name = "浅海洋绿" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 64, 224, 208), Name = "宝石绿" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 127, 255, 212), Name = "碧绿" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 102, 205, 170), Name = "适中碧绿" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 0, 250, 154), Name = "适中春天绿" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 245, 255, 250), Name = "薄荷奶油" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 0, 255, 127), Name = "春天绿" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 60, 179, 113), Name = "适中海洋绿" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 46, 139, 87), Name = "海洋绿" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 144, 238, 144), Name = "浅绿" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 152, 251, 152), Name = "苍白绿" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 143, 188, 143), Name = "深海洋绿" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 50, 205, 50), Name = "柠檬绿" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 0, 255, 0), Name = "柠檬" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 34, 139, 34), Name = "森林绿" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 127, 255, 0), Name = "查特酒绿" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 124, 252, 0), Name = "草坪绿" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 173, 255, 47), Name = "绿黄" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 85, 107, 47), Name = "深橄榄绿" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 154, 205, 50), Name = "黄绿" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 107, 142, 35), Name = "橄榄褐" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 245, 245, 220), Name = "米色" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 250, 250, 210), Name = "浅秋黄" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 255, 240), Name = "象牙白" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 255, 224), Name = "浅黄" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 255, 0), Name = "黄" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 128, 128, 0), Name = "橄榄" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 189, 183, 107), Name = "深卡其布" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 250, 205), Name = "柠檬沙" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 238, 232, 170), Name = "灰秋" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 240, 230, 140), Name = "卡其布" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 215, 0), Name = "金" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 248, 220), Name = "玉米" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 218, 165, 32), Name = "秋" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 184, 134, 11), Name = "深秋" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 250, 240), Name = "白花" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 253, 245, 230), Name = "浅米色" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 245, 222, 179), Name = "小麦" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 228, 181), Name = "鹿皮" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 165, 0), Name = "橙" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 239, 213), Name = "木瓜" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 235, 205), Name = "漂白杏仁" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 222, 173), Name = "耐而节白" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 250, 235, 215), Name = "古白" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 210, 180, 140), Name = "晒" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 222, 184, 135), Name = "树干" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 228, 196), Name = "乳脂" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 140, 0), Name = "深橙色" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 250, 240, 230), Name = "亚麻" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 205, 133, 63), Name = "秘鲁" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 244, 164, 96), Name = "沙棕" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 210, 105, 30), Name = "巧克力" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 245, 238), Name = "海贝" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 160, 82, 45), Name = "土黄褐" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 160, 122), Name = "浅肉" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 160, 122), Name = "珊瑚" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 69, 0), Name = "橙红" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 99, 71), Name = "番茄色" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 228, 225), Name = "雾中玫瑰" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 250, 128, 114), Name = "肉" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 255, 250, 250), Name = "雪" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 240, 128, 128), Name = "浅珊瑚" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 188, 143, 143), Name = "玫瑰棕" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 205, 92, 92), Name = "浅粉红" });

            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 165, 42, 42), Name = "棕" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 178, 34, 34), Name = "火砖" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 139, 0, 0), Name = "深红" });
            App.Model.EnmuList.Add(new ColorBrushCode { Color = Color.FromArgb(255, 128, 0, 0), Name = "栗色" });

        }

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            try
            {
                DocumentLoad();
            }
            catch (FileNotFoundException e)
            {
                DocumentNew();
            }

            App.Model.EnmuList.CollectionChanged += EnmuList_CollectionChanged;
        }
        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
