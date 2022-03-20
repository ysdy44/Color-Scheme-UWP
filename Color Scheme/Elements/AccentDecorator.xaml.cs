using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Color_Scheme.Elements
{
    /// <summary>
    /// State of <see cref="Storyboard"/>.
    /// </summary>
    public enum StoryState
    {
        /// <summary> <see cref="Storyboard.Stop()"/> </summary>
        Stop,
        /// <summary> <see cref="Storyboard.Begin()"/> </summary>
        Begin,
        /// <summary> <see cref="Storyboard.Pause()"/> </summary>
        Pause,
        /// <summary> <see cref="Storyboard.Resume()"/> </summary>
        Resume
    }

    /// <summary>
    /// Represents a decorator with accent color.
    /// </summary>
    public sealed partial class AccentDecorator : UserControl
    {

        #region DependencyProperty


        /// <summary> Gets or sets the state of<see cref = "AccentDecorator" />. </summary>
        public StoryState State
        {
            get => (StoryState)base.GetValue(StateProperty);
            set => base.SetValue(StateProperty, value);
        }
        /// <summary> Identifies the <see cref = "AccentDecorator.State" /> dependency property. </summary>
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(nameof(State), typeof(StoryState), typeof(AccentDecorator), new PropertyMetadata(StoryState.Stop, (sender, e) =>
        {
            AccentDecorator control = (AccentDecorator)sender;

            if (e.NewValue is StoryState value)
            {
                switch (value)
                {
                    case StoryState.Stop:
                        control.Storyboard.Stop();
                        break;
                    case StoryState.Begin:
                        control.Storyboard.Begin();
                        break;
                    case StoryState.Pause:
                        control.Storyboard.Pause();
                        break;
                    case StoryState.Resume:
                        control.Storyboard.Resume();
                        break;
                    default:
                        break;
                }
            }
        }));


        #endregion

        //@Construct
        /// <summary>
        /// Initializes a AccentDecorator. 
        /// </summary> 
        public AccentDecorator()
        {
            this.InitializeComponent();
            base.Tapped += (s, e) =>
            {
                switch (this.State)
                {
                    case StoryState.Pause:
                        this.State = StoryState.Resume;
                        break;
                    default:
                        this.State = StoryState.Pause;
                        break;
                }
            };
        }

    }
}