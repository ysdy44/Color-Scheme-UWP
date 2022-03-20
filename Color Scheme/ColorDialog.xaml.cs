using HSVColorPickers;
using System;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Color_Scheme
{
    public sealed partial class ColorDialog : ContentDialog
    {

        Grid LayoutRoot;
        Border BackgroundElement;

        public ColorDialog()
        {
            this.InitializeComponent(); 
            this.ConstructStrings();
            this.CopyButton.Tapped += (s, e) => Clipboard2.CopyText(this.TextBox.Text);
            this.PasteButton.Tapped += async (s, e) =>
            {
                string text = await Clipboard2.PasteTextAsync();
                if (string.IsNullOrEmpty(text)) return;
                this.TextBox.Text = text.ToUpper();
            };
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!(this.LayoutRoot is null)) this.LayoutRoot.Tapped -= this.LayoutRoot_Tapped;
            this.LayoutRoot = base.GetTemplateChild(nameof(LayoutRoot)) as Grid;
            if (!(this.LayoutRoot is null)) this.LayoutRoot.Tapped += this.LayoutRoot_Tapped;

            if (!(this.BackgroundElement is null)) this.BackgroundElement.Tapped -= this.BackgroundElement_Tapped;
            this.BackgroundElement = base.GetTemplateChild(nameof(BackgroundElement)) as Border;
            if (!(this.BackgroundElement is null)) this.BackgroundElement.Tapped += this.BackgroundElement_Tapped;
        }

        private void LayoutRoot_Tapped(object sender, TappedRoutedEventArgs e)
        {
            base.Hide();
        }
        private void BackgroundElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

    }


    public sealed partial class ColorDialog : ContentDialog
    {

        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this.CopyToolTip.Content = resource.GetString("$MainPage_Copy");
            this.PasteToolTip.Content = resource.GetString("$MainPage_Paste");
        }

        public Color? GetColor()
        {
            string text = this.TextBox.Text;
            if (string.IsNullOrEmpty(text)) return null;

            try
            {
                int hex = Hex.StringToInt(text.ToUpper());
                return Hex.IntToColor(hex);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public IAsyncOperation<ContentDialogResult> ShowAsync(Color color)
        {
            this.TextBox.Text = Hex.ColorToString(color).ToUpper();
            this.TextBox.SelectAll();
            this.TextBox.Focus(FocusState.Keyboard);
            return base.ShowAsync(ContentDialogPlacement.InPlace);
        }

    }
}