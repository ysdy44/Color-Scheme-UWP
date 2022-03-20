using HSVColorPickers;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Color_Scheme.Controls
{
    internal class WidgetTypeContainer : ContentControl
    {

        //@Delegate
        public static event ColorChangedHandler ColorChanged;

        IColorPicker ColorPicker;
        static Color Color = Colors.DodgerBlue;
        public static readonly IDictionary<WidgetType, IColorPicker> ColorPickers = new Dictionary<WidgetType, IColorPicker>();

        #region DependencyProperty

        /// <summary> Gets or sets the type of<see cref = "WidgetTypeContainer" />. </summary>
        public WidgetType Type
        {
            get => (WidgetType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "WidgetTypeContainer.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(WidgetType), typeof(WidgetTypeContainer), new PropertyMetadata(WidgetType.None, (sender, e) =>
        {
            WidgetTypeContainer control = (WidgetTypeContainer)sender;

            if (e.NewValue is WidgetType value)
            {
                control.SetWidget(value);
            }
        }));

        #endregion

        public WidgetTypeContainer()
        {
            base.Unloaded += (s, e) => this.SetWidget(WidgetType.None);
        }

        private void SetWidget(WidgetType value)
        {
            if (WidgetTypeContainer.ColorPickers.ContainsKey(value))
            {
                IColorPicker picker = WidgetTypeContainer.ColorPickers[value];
                picker.ColorChanged -= this.ColorChangedCore;
                picker.ColorChangedStarted -= this.ColorChangedCore;
                picker.ColorChangedDelta -= this.ColorChangedCore;
                picker.ColorChangedCompleted -= this.ColorChangedCore;
                WidgetTypeContainer.ColorPickers.Remove(value);
            }
            if (this.ColorPicker != null)
            {
                this.ColorPicker.ColorChanged -= this.ColorChangedCore;
                this.ColorPicker.ColorChangedStarted -= this.ColorChangedCore;
                this.ColorPicker.ColorChangedDelta -= this.ColorChangedCore;
                this.ColorPicker.ColorChangedCompleted -= this.ColorChangedCore;
                base.Content = null;
            }
            this.ColorPicker = WidgetTypeContainer.GetWidget(value);
            if (this.ColorPicker != null)
            {
                {
                    WidgetTypeContainer.ColorPickers.Add(value, this.ColorPicker);
                    this.ColorPicker.Color = WidgetTypeContainer.Color;
                }

                this.ColorPicker.ColorChanged += this.ColorChangedCore;
                this.ColorPicker.ColorChangedStarted += this.ColorChangedCore;
                this.ColorPicker.ColorChangedDelta += this.ColorChangedCore;
                this.ColorPicker.ColorChangedCompleted += this.ColorChangedCore;
                base.Content = this.ColorPicker;
            }
        }

        private void ColorChangedCore(object sender, Color value)
        {
            WidgetTypeContainer.ColorChanged?.Invoke(this, value); // Delegate
            WidgetTypeContainer.SetColor(value, this.Type);
        }

        //@Static
        private static IColorPicker GetWidget(WidgetType value)
        {
            switch (value)
            {
                case WidgetType.Wheel: return new WidgetPickerContainer<WheelPicker>();
                case WidgetType.Circle: return new WidgetPickerContainer<CirclePicker>();

                case WidgetType.RGB: return new RGBPicker();
                case WidgetType.HSV: return new HSVPicker();

                case WidgetType.PaletteHue: return new PaletteHuePicker();
                case WidgetType.PaletteSaturation: return new PaletteSaturationPicker();
                case WidgetType.PaletteValue: return new PaletteValuePicker();

                case WidgetType.LinearGradient: return new LinearGradientWidget();

                default: return null;
            }
        }
        public static void SetColor(Color value, WidgetType type = WidgetType.None)
        {
            if (WidgetTypeContainer.Color == value) return;
            WidgetTypeContainer.Color = value;

            foreach (var item in WidgetTypeContainer.ColorPickers)
            {
                if (item.Key != type)
                {
                    item.Value.Color = value;
                }
            }
        }

    }
}