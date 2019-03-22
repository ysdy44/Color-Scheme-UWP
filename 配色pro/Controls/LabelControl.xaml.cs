using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace 配色pro.Controls
{
    public sealed partial class LabelControl : UserControl
    {

        #region DependencyProperty


        private int seletedIndex;
        public int SeletedIndex
        {
            get => this.seletedIndex;
            set
            {
                if (value == this.TabIndex)
                {
                    this.SeletedPath.Visibility = Visibility.Visible;
                    this.UsualPath.Visibility = Visibility.Collapsed;
                    this.Line.Visibility = Visibility.Visible;
                }
                else
                {
                    this.SeletedPath.Visibility = Visibility.Collapsed;
                    this.UsualPath.Visibility = Visibility.Visible;
                    this.Line.Visibility = Visibility.Collapsed;
                }
                this.seletedIndex = value;
            }
        }


        /// <summary>Usual IconPath' geometry data</summary>
        public Geometry UsualData { get => this.UsualPath.Data; set => this.UsualPath.Data = value; }
        /// <summary>Seleted IconPath' geometry data</summary>
        public Geometry SeletedData { get => this.SeletedPath.Data; set => this.SeletedPath.Data = value; }


        #endregion

        //Delegate
        public delegate void IndexChangedHandler();
        public event IndexChangedHandler RefreshChanged = null;
        public event IndexChangedHandler SeletedChanged = null;

        public LabelControl()
        {
            this.InitializeComponent();

            this.Button.Tapped += (s, e) =>
            {
                if (this.SeletedIndex != this.TabIndex)
                    this.SeletedChanged?.Invoke(); //Delegate
                else
                    this.RefreshChanged?.Invoke(); //Delegate
            };
        }
    }
}
