using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Color_Scheme.Elements
{
    /// <summary>
    /// Mode for <see cref="Reorder"/>.
    /// </summary>
    public enum ReorderMode
    {
        /// <summary> Normal </summary>
        None,
        /// <summary> Sorter </summary>
        Sorter,
        /// <summary> Sorted </summary>
        Sorted,

        /// <summary> Sorted Upwards </summary>
        SortedUp,
        /// <summary> Sorted Downwards </summary>
        SortedDown,

        /// <summary> Sorted Upwards with Offset </summary>
        SortedUpWithOffset,
        /// <summary> Sorted Downwards with Offset </summary>
        SortedDownWithOffset,
    }

    /// <summary>
    /// Represents a control that reorder ui-element which in canvas.
    /// New some ui-element which  base on it and put these  in a canvas.
    /// </summary>
    public class Reorder : ContentControl
    {

        private const double Extension = 40;
        private double HeaderExtension => 0 - Reorder.Extension;
        private double FooterExtension;

        private int Count;
        private int StatringReorderIndex;
        private double Top;
        private double StatringTop;
        private double Bottom;
        private double StatringBottom;

        readonly Storyboard Storyboard;
        readonly DoubleAnimation Animation = new DoubleAnimation
        {
            From = 0,
            To = 0,
            Duration = TimeSpan.FromSeconds(0.2),
        };


        #region DependencyProperty


        /// <summary> Gets or set the reorder mode for <see cref="Reorder"/>. </summary>
        public ReorderMode ReorderMode
        {
            get => (ReorderMode)base.GetValue(ReorderModeProperty);
            set => base.SetValue(ReorderModeProperty, value);
        }
        /// <summary> Identifies the <see cref = "Reorder.ReorderMode" /> dependency property. </summary>
        public static readonly DependencyProperty ReorderModeProperty = DependencyProperty.Register(nameof(ReorderMode), typeof(ReorderMode), typeof(Reorder), new PropertyMetadata(ReorderMode.None));


        /// <summary> Gets or set the reorder index for <see cref="Reorder"/>. </summary>
        public int ReorderIndex
        {
            get => (int)base.GetValue(ReorderIndexProperty);
            set => base.SetValue(ReorderIndexProperty, value);
        }
        /// <summary> Identifies the <see cref = "Reorder.ReorderIndex" /> dependency property. </summary>
        public static readonly DependencyProperty ReorderIndexProperty = DependencyProperty.Register(nameof(ReorderIndex), typeof(int), typeof(Reorder), new PropertyMetadata(0, (sender, e) =>
        {
            Reorder control = (Reorder)sender;
            if (control.IsLoaded == false) return;

            if (e.NewValue is int value)
            {
                control.SetReorderIndex(value);
            }
        }));


        /// <summary> Gets or set the item height for <see cref="Reorder"/>. </summary>
        public double ItemHeight
        {
            get => (double)base.GetValue(ItemHeightProperty);
            set => base.SetValue(ItemHeightProperty, value);
        }
        /// <summary> Identifies the <see cref = "Reorder.ItemHeight" /> dependency property. </summary>
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(nameof(ItemHeight), typeof(double), typeof(Reorder), new PropertyMetadata(50d, (sender, e) =>
        {
            Reorder control = (Reorder)sender;
            if (control.IsLoaded == false) return;

            if (e.NewValue is double value)
            {
                control.Update(control.ReorderIndex, value);
            }
        }));


        #endregion


        //@Construct
        /// <summary>
        /// Initializes a Reorder. 
        /// </summary> 
        public Reorder()
        {
            Timeline animation = this.Animation;
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, "(Canvas.Top)");
            this.Storyboard = new Storyboard { Children = { animation } };
            base.Loaded += (s, e) => this.Update(this.ReorderIndex, this.ItemHeight);
        }


        /// <summary>
        /// <see cref="UIElement.ManipulationStarted"/> of <see cref="Thumb"/>.
        /// </summary>
        protected void ThumbManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (base.Parent is Canvas canvas)
            {
                int count = 0;
                foreach (UIElement item in canvas.Children)
                {
                    if (item is Reorder reorder)
                    {
                        count++;
                        reorder.Cache();
                    }
                }
                this.Count = count;
                this.FooterExtension = canvas.ActualHeight - this.ItemHeight + Reorder.Extension;

                Canvas.SetZIndex(this, 1);
                this.ReorderMode = ReorderMode.Sorter;
            }
        }
        /// <summary>
        /// <see cref="UIElement.ManipulationDelta"/> of <see cref="Thumb"/>.
        /// </summary>
        protected void ThumbManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (base.Parent is Canvas canvas)
            {
                this.Top = this.GetOffsetWithExtension(this.StatringTop + e.Cumulative.Translation.Y);
                this.Bottom = this.Top + this.ItemHeight;
                Canvas.SetTop(this, this.Top);

                foreach (UIElement item in canvas.Children)
                {
                    if (item is Reorder reorder)
                    {
                        if (this == reorder) continue;
                        reorder.ReorderMode = reorder.GetReordeMode(this.StatringReorderIndex, this.Top, this.Bottom);
                        reorder.SetReordeMode(this.Top, this.Bottom);
                    }
                }
            }
        }
        /// <summary>
        /// <see cref="UIElement.ManipulationCompleted"/> of <see cref="Thumb"/>.
        /// </summary>
        protected void ThumbManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (base.Parent is Canvas canvas)
            {
                foreach (UIElement item in canvas.Children)
                {
                    if (item is Reorder reorder)
                    {
                        reorder.Done(this.Count);
                    }
                }

                Canvas.SetZIndex(this, 0);
            }
        }


        private void Update(int reorderIndex, double itemHeight)
        {
            this.Top = reorderIndex * itemHeight;
            this.Bottom = this.Top + itemHeight;
            Canvas.SetTop(this, this.Top);
        }
        private void Cache()
        {
            this.ReorderMode = ReorderMode.Sorted;

            this.Top = this.ReorderIndex * this.ItemHeight;
            this.Bottom = this.Top + this.ItemHeight;

            this.StatringReorderIndex = this.ReorderIndex;
            this.StatringTop = this.Top;
            this.StatringBottom = this.Bottom;
        }
        private void Done(int count)
        {
            switch (this.ReorderMode)
            {
                case ReorderMode.SortedUp:
                    this.ReorderIndex--;
                    break;
                case ReorderMode.SortedDown:
                    this.ReorderIndex++;
                    break;

                case ReorderMode.Sorter:
                case ReorderMode.SortedUpWithOffset:
                case ReorderMode.SortedDownWithOffset:
                    int reorderIndex = Math.Max(0, Math.Min(count - 1, (int)this.GetReorderIndex()));
                    if (this.ReorderIndex != reorderIndex)
                        this.ReorderIndex = reorderIndex;
                    else
                        this.SetReorderIndex(reorderIndex);
                    break;

                default:
                    break;
            }
            this.ReorderMode = ReorderMode.None;
        }


        #region ReorderIndex


        private double GetReorderIndex() => (this.Top + this.ItemHeight / 2) / this.ItemHeight;
        private void SetReorderIndex(int ReorderIndex)
        {
            double from = Canvas.GetTop(this);
            double to = this.ItemHeight * ReorderIndex;
            if (from == to) return;

            this.Animation.From = from;
            this.Animation.To = to;

            this.Storyboard.Pause(); // Storyboard
            this.Storyboard.Begin(); // Storyboard
        }


        #endregion


        #region ReorderMode


        private ReorderMode GetReordeMode(int statringReorderIndex, double top, double bottom)
        {
            if (statringReorderIndex == this.StatringReorderIndex) return ReorderMode.Sorter;
            else if (statringReorderIndex > this.StatringReorderIndex)
            {
                if (top >= this.StatringBottom) return ReorderMode.Sorted;
                else if (bottom < this.StatringBottom) return ReorderMode.SortedDown;
                else return ReorderMode.SortedDownWithOffset; // if (bottom >= this.StatringBottom)
            }
            else // if (statringReorderIndex < this.StatringReorderIndex)
            {
                if (bottom <= this.StatringTop) return ReorderMode.Sorted;
                else if (top > this.StatringTop) return ReorderMode.SortedUp;
                else return ReorderMode.SortedUpWithOffset; // if (top < this.StatringTop)
            }
        }
        private void SetReordeMode(double top, double bottom)
        {
            switch (this.ReorderMode)
            {
                case ReorderMode.None: return;
                case ReorderMode.Sorter: return;
                case ReorderMode.Sorted: this.Top = this.StatringReorderIndex * this.ActualHeight; break;
                case ReorderMode.SortedUp: this.Top = this.StatringTop - this.ActualHeight; break;
                case ReorderMode.SortedDown: this.Top = this.StatringTop + this.ItemHeight; break;
                case ReorderMode.SortedUpWithOffset: this.Top = this.StatringTop - (bottom - this.StatringTop); break;
                case ReorderMode.SortedDownWithOffset: this.Top = this.StatringTop - (top - this.StatringBottom); break;
                default: break;
            }
            this.Bottom = this.Top + this.ItemHeight;
            Canvas.SetTop(this, this.Top);
        }


        #endregion


        #region Math


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private double GetDamping(double extension)
        {
            // Damping:
            //
            // The more you swipe to the header,
            // the more effort it takes,
            //
            // when offset is greater than the header expansion.
            return Reorder.Extension - Reorder.Extension / (extension / 100.0 + 1.0);
        }
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private double GetOffsetWithExtension(double offset)
        {
            bool isHeader = offset < this.HeaderExtension;
            bool isFooter = offset > this.FooterExtension;

            if (isHeader == false && isFooter == false) return offset;
            double extensionHeader = this.HeaderExtension - offset;
            if (isHeader && isFooter == false) return this.HeaderExtension - this.GetDamping(extensionHeader);
            double extensionFooter = offset - this.FooterExtension;
            if (isHeader == false && isFooter) return this.FooterExtension + this.GetDamping(extensionFooter);

            if (extensionHeader > extensionFooter) return this.HeaderExtension - this.GetDamping(extensionHeader);
            if (extensionHeader < extensionFooter) return this.FooterExtension + this.GetDamping(extensionFooter);
            return offset;
        }


        #endregion

    }
}