using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Color_Scheme
{
    public sealed partial class MainPage : Page
    {

        // FlowDirection
        private void ConstructFlowDirection()
        {
            bool isRightToLeft = System.Globalization.CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

            base.FlowDirection = isRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

        // Strings
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this.TitleTextBlock.Text = resource.GetString("$DisplayName");

            {
                this.WidgetToolTip.Content = this.WidgetTextBlock.Text = resource.GetString("$MainPage_Widget");
                this.EnumToolTip.Content = this.EnumTextBlock.Text = resource.GetString("$MainPage_Enum");
                this.BitmapToolTip.Content = this.BitmapTextBlock.Text = resource.GetString("$MainPage_Bitmap");
                this.SettingToolTip.Content = this.SettingTextBlock.Text = resource.GetString("$MainPage_Setting");
                this.AboutToolTip.Content = resource.GetString("$MainPage_About");

                this.PCSplitToolTip.Content = resource.GetString("$MainPage_Split");
                this.PhoneSplitToolTip.Content = resource.GetString("$MainPage_Split");
            }

            {
                this.CopyToolTip.Content = resource.GetString("$MainPage_CopyBitmap");
                this.PasteToolTip.Content = resource.GetString("$MainPage_PasteBitmap");
                this.AddToolTip2.Content = this.AddToolTip.Content = resource.GetString("$MainPage_AddBitmap");
            }
        }

    }
}