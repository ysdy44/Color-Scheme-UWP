using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Color_Scheme.Controls
{
    public sealed partial class WidgetEditor : UserControl
    {

        public IEnumerable<WidgetType> Types
        {
            get
            {
                for (int i = 0; i < 8; i++)
                {
                    if (this.WidgetsControl0.Visible) if (this.WidgetsControl0.ReorderIndex == i) yield return this.WidgetsControl0.Type;
                    if (this.WidgetsControl1.Visible) if (this.WidgetsControl1.ReorderIndex == i) yield return this.WidgetsControl1.Type;
                    if (this.WidgetsControl2.Visible) if (this.WidgetsControl2.ReorderIndex == i) yield return this.WidgetsControl2.Type;
                    if (this.WidgetsControl3.Visible) if (this.WidgetsControl3.ReorderIndex == i) yield return this.WidgetsControl3.Type;
                    if (this.WidgetsControl4.Visible) if (this.WidgetsControl4.ReorderIndex == i) yield return this.WidgetsControl4.Type;
                    if (this.WidgetsControl5.Visible) if (this.WidgetsControl5.ReorderIndex == i) yield return this.WidgetsControl5.Type;
                    if (this.WidgetsControl6.Visible) if (this.WidgetsControl6.ReorderIndex == i) yield return this.WidgetsControl6.Type;
                    if (this.WidgetsControl7.Visible) if (this.WidgetsControl7.ReorderIndex == i) yield return this.WidgetsControl7.Type;
                }
            }
            set
            {
                int index = 0;

                if (value != null)
                {
                    foreach (WidgetType item in value)
                    {
                        if (this.WidgetsControl0.Type == item) { this.WidgetsControl0.ReorderIndex = index; this.WidgetsControl0.Visible = true; index++; }
                        if (this.WidgetsControl1.Type == item) { this.WidgetsControl1.ReorderIndex = index; this.WidgetsControl1.Visible = true; index++; }
                        if (this.WidgetsControl2.Type == item) { this.WidgetsControl2.ReorderIndex = index; this.WidgetsControl2.Visible = true; index++; }
                        if (this.WidgetsControl3.Type == item) { this.WidgetsControl3.ReorderIndex = index; this.WidgetsControl3.Visible = true; index++; }
                        if (this.WidgetsControl4.Type == item) { this.WidgetsControl4.ReorderIndex = index; this.WidgetsControl4.Visible = true; index++; }
                        if (this.WidgetsControl5.Type == item) { this.WidgetsControl5.ReorderIndex = index; this.WidgetsControl5.Visible = true; index++; }
                        if (this.WidgetsControl6.Type == item) { this.WidgetsControl6.ReorderIndex = index; this.WidgetsControl6.Visible = true; index++; }
                        if (this.WidgetsControl7.Type == item) { this.WidgetsControl7.ReorderIndex = index; this.WidgetsControl7.Visible = true; index++; }
                    }
                }

                for (int i = 0; i < 8; i++)
                {
                    WidgetType item = (WidgetType)i;
                    if (value != null && value.Any(c => c == item)) continue;

                    if (this.WidgetsControl0.Type == item) { this.WidgetsControl0.ReorderIndex = index; this.WidgetsControl0.Visible = false; index++; }
                    if (this.WidgetsControl1.Type == item) { this.WidgetsControl1.ReorderIndex = index; this.WidgetsControl1.Visible = false; index++; }
                    if (this.WidgetsControl2.Type == item) { this.WidgetsControl2.ReorderIndex = index; this.WidgetsControl2.Visible = false; index++; }
                    if (this.WidgetsControl3.Type == item) { this.WidgetsControl3.ReorderIndex = index; this.WidgetsControl3.Visible = false; index++; }
                    if (this.WidgetsControl4.Type == item) { this.WidgetsControl4.ReorderIndex = index; this.WidgetsControl4.Visible = false; index++; }
                    if (this.WidgetsControl5.Type == item) { this.WidgetsControl5.ReorderIndex = index; this.WidgetsControl5.Visible = false; index++; }
                    if (this.WidgetsControl6.Type == item) { this.WidgetsControl6.ReorderIndex = index; this.WidgetsControl6.Visible = false; index++; }
                    if (this.WidgetsControl7.Type == item) { this.WidgetsControl7.ReorderIndex = index; this.WidgetsControl7.Visible = false; index++; }
                }
            }
        }

        public WidgetEditor()
        {
            this.InitializeComponent();
            this.ConstructStrings();
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.WidgetsControl0.Width =
                this.WidgetsControl1.Width =
                this.WidgetsControl2.Width =
                this.WidgetsControl3.Width =
                this.WidgetsControl4.Width =
                this.WidgetsControl5.Width =
                this.WidgetsControl6.Width =
                this.WidgetsControl7.Width = e.NewSize.Width;
            };
        }
    }


    public sealed partial class WidgetEditor : UserControl
    {
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this.WidgetsControl0.Text = resource.GetString("Colors_Wheel");
            this.WidgetsControl1.Text = resource.GetString("Colors_Circle");
            this.WidgetsControl2.Text = resource.GetString("Colors_RGB");
            this.WidgetsControl3.Text = resource.GetString("Colors_HSV");
            this.WidgetsControl4.Text = resource.GetString("Colors_PaletteHue");
            this.WidgetsControl5.Text = resource.GetString("Colors_PaletteSaturation");
            this.WidgetsControl6.Text = resource.GetString("Colors_PaletteValue");
            this.WidgetsControl7.Text = resource.GetString("Colors_LinearGradient");
        }

    }
}