using Color_Scheme.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Globalization;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Color_Scheme
{
    /// <summary> 
    /// Represents a dialog used to setting....
    /// </summary>
    public sealed partial class SettingDialog : ContentDialog
    {

        //@ViewModel
        private ObservableCollection<WidgetType> Items => App.ItemsSource;

        //@Delegate  
        /// <summary> Occurs when Setted. </summary>
        public event EventHandler<string> LanguageSetted;

        readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        string LanguageUseSystemSetting = "Use system setting";

        //@Construct
        /// <summary>
        /// Initializes a SettingDialog. 
        /// </summary>
        public SettingDialog()
        {
            this.InitializeComponent();
            this.ConstructStrings();

            this.ConstructTheme();
            this.ConstructLanguage();

            base.Loaded += (s, e) =>
            {
                IEnumerable<WidgetType> value = this.GetTypes();
                this.Editor.Types = value;
                foreach (WidgetType item in value)
                {
                    this.Items.Add(item);
                }
            };

            this.WidgetApplyButton.Click += (s, e) =>
            {
                this.SetTypes(this.Editor.Types);
            };

            this.LanguageTipButton.Click += async (s, e) =>
            {
                await Windows.ApplicationModel.Core.CoreApplication.RequestRestartAsync(string.Empty);
            };

            this.LocalFolderButton.Click += async (s, e) =>
            {
                IStorageFolder folder = ApplicationData.Current.LocalFolder;
                await Launcher.LaunchFolderAsync(folder);
            };

            base.SecondaryButtonClick += (s, e) => base.Hide();
            base.PrimaryButtonClick += (s, e) => base.Hide();
        }

        public void SetTheme(ElementTheme value)
        {
            this.LocalSettings.Values["Theme"] = (int)value;
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                if (frameworkElement.RequestedTheme == value) return;
                frameworkElement.RequestedTheme = value;
            }
        }
        public ElementTheme GetTheme()
        {
            if (this.LocalSettings.Values.ContainsKey("Theme"))
            {
                if (this.LocalSettings.Values["Theme"] is int item)
                {
                    return (ElementTheme)item;
                }
            }
            return base.RequestedTheme;
        }


        public void SetTypes(IEnumerable<WidgetType> value)
        {
            if (value.Count() == 0)
            {
                this.LocalSettings.Values["HasTypes"] = false;
                this.LocalSettings.Values["Types"] = null;
               
                this.Items.Clear();
            }
            else
            {
                this.LocalSettings.Values["HasTypes"] = true;
                this.LocalSettings.Values["Types"] = value.Select(c => (int)c).ToArray();
            
                this.Items.Clear();
                foreach (WidgetType item in value)
                {
                    this.Items.Add(item);
                }
            }
        }
        public IEnumerable<WidgetType> GetTypes()
        {
            if (this.LocalSettings.Values.ContainsKey("HasTypes"))
            {
                if (this.LocalSettings.Values["HasTypes"] is bool item1)
                {
                    if (item1)
                    {
                        if (this.LocalSettings.Values.ContainsKey("Types"))
                        {
                            if (this.LocalSettings.Values["Types"] is Array item2)
                            {
                                foreach (object item3 in item2)
                                {
                                    if (int.TryParse(item3.ToString(), out int result))
                                    {
                                        if (result < 0) continue;
                                        if (result > 8) continue;
                                        yield return (WidgetType)result;
                                    }
                                }
                                yield break;
                            }
                        }
                    }
                }
            }


            yield return WidgetType.Wheel;
            yield return WidgetType.RGB;
            yield return WidgetType.HSV;
            yield return WidgetType.PaletteHue;
            yield return WidgetType.LinearGradient;
        }

    }


    public sealed partial class SettingDialog : ContentDialog
    {
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            base.SecondaryButtonText = resource.GetString("$SettingPage_Close");
            base.PrimaryButtonText = resource.GetString("$SettingPage_Primary");

            this.TitleTextBlock.Text = resource.GetString("$MainPage_Setting");
            this.ThemeTextBlock.Text = resource.GetString("$SettingPage_Theme");
            this.LightRadioButton.Content = resource.GetString("$SettingPage_Theme_Light");
            this.DarkRadioButton.Content = resource.GetString("$SettingPage_Theme_Dark");
            this.DefaultRadioButton.Content = resource.GetString("$SettingPage_Theme_UseSystem");
            
            this.WidgetTextBlock.Text = resource.GetString("$SettingPage_Widget");
            this.WidgetApplyTextBlock.Text = resource.GetString("$SettingPage_Apply");

            this.LanguageTextBlock.Text = resource.GetString("$SettingPage_Language");
            this.LanguageTipButton.Content = resource.GetString("$SettingPage_LanguageTip");
            this.LanguageUseSystemSetting = resource.GetString("$SettingPage_Language_UseSystemSetting");

            this.LocalFolderTextBlock.Text = resource.GetString("$SettingPage_LocalFolder");
            this.OpenTextBlock.Text = resource.GetString("$SettingPage_LocalFolder_Open");
        }

        private void ConstructTheme()
        {
            ElementTheme theme = this.GetTheme();
            this.SetTheme(theme);

            this.LightRadioButton.IsChecked = (theme == ElementTheme.Light);
            this.DarkRadioButton.IsChecked = (theme == ElementTheme.Dark);
            this.DefaultRadioButton.IsChecked = (theme == ElementTheme.Default);

            this.LightRadioButton.Checked += (s, e) => this.SetTheme(ElementTheme.Light);
            this.DarkRadioButton.Checked += (s, e) => this.SetTheme(ElementTheme.Dark);
            this.DefaultRadioButton.Checked += (s, e) => this.SetTheme(ElementTheme.Default);
        }

        private void ConstructLanguage()
        {
            // Languages
            string groupLanguage = ApplicationLanguages.PrimaryLanguageOverride;
            List<string> languages = new List<string>(ApplicationLanguages.ManifestLanguages);
            languages.Sort();
            languages.Insert(0, string.Empty);

            // Items
            IEnumerable<ContentControl> languages2 =
                from item
                in languages
                select string.IsNullOrEmpty(item) ? new ContentControl
                {
                    Tag = string.Empty,
                    Content = this.LanguageUseSystemSetting
                } : new ContentControl
                {
                    Tag = item,
                    ContentTemplate = this.LanguageTemplate,
                    Content = new CultureInfo(item)
                };

            this.LanguageComboBox.ItemsSource = languages2;
            this.LanguageComboBox.SelectedIndex = languages.IndexOf(groupLanguage);
            this.LanguageComboBox.SelectionChanged += (s, e) =>
            {
                if (this.LanguageComboBox.SelectedItem is ContentControl item)
                {
                    if (item.Tag is string language)
                    {
                        this.SetLanguage(language);
                        this.LanguageSetted?.Invoke(this, language); // Delegate
                        this.ConstructStrings();
                    }
                }
            };
        }

        private void SetLanguage(string language)
        {
            if (ApplicationLanguages.PrimaryLanguageOverride == language) return;
            ApplicationLanguages.PrimaryLanguageOverride = language;

            if (string.IsNullOrEmpty(language) == false)
            {
                if (Window.Current.Content is FrameworkElement frameworkElement)
                {
                    if (frameworkElement.Language != language)
                    {
                        frameworkElement.Language = language;
                    }
                }
            }
        }

    }
}