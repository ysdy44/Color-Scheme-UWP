using System;
using Color_Scheme.Elements;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Color_Scheme
{
    /// <summary> 
    /// Represents a dialog used to about....
    /// </summary>
    public sealed partial class AboutDialog : ContentDialog
    {
        //@Construct
        /// <summary>
        /// Initializes a AboutDialog. 
        /// </summary>
        public AboutDialog()
        {
            this.InitializeComponent();
            this.ConstructStrings();

            base.Closed += (s, e) => this.AccentDecorator.State = StoryState.Stop;
            base.Opened += (s, e) => this.AccentDecorator.State = StoryState.Begin;
            

            base.SecondaryButtonClick += (s, e) => base.Hide();
            base.PrimaryButtonClick += (s, e) => base.Hide();
        }
    }

    public sealed partial class AboutDialog : ContentDialog
    {
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            base.SecondaryButtonText = resource.GetString("$SettingPage_Close");
            base.PrimaryButtonText = resource.GetString("$SettingPage_Primary");

            this.VersionTextBlock.Text = resource.GetString("$Version");

            this.GithubTextBlock.Text = resource.GetString("Github");
            string githubLink = resource.GetString("$GithubLink");
            this.GithubHyperlinkButton.Content = githubLink;
            this.GithubHyperlinkButton.NavigateUri = new Uri(githubLink);

            this.FeedbackTextBlock.Text = resource.GetString("Feedback");
            string feedbackLink = resource.GetString("$FeedbackLink");
            this.FeedbackHyperlinkButton.Content = feedbackLink;
            this.FeedbackHyperlinkButton.NavigateUri = new Uri("mailto:" + feedbackLink);
        }

    }
}