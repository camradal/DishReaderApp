using System;
using System.Windows.Navigation;
using DishReaderApp.Resources;
using DishReaderApp.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Utilities;

namespace DishReaderApp
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        private int currentIndex;
        private bool navigating = false;

        public DetailsPage()
        {
            InitializeComponent();

            // specify the text explicitly on the app bar using our resource string
            var buttonEmail = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            buttonEmail.Text = Strings.ButtonEmail;

            var buttonRefresh = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            buttonRefresh.Text = Strings.ButtonRefresh;

            var buttonPrevious = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            buttonPrevious.Text = Strings.ButtonPrevious;

            var buttonNext = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
            buttonNext.Text = Strings.ButtonNext;

            webBrowser1.Navigated += webBrowser1_Navigated;
            webBrowser1.LoadCompleted += webBrowser1_LoadCompleted;
        }

        void webBrowser1_Navigated(object sender, NavigationEventArgs e)
        {
            if (!navigating)
            {
                GlobalLoading.Instance.IsLoading = true;
                GlobalLoading.Instance.LoadingText = Strings.Loading;
            }
            navigating = true;
        }

        private void webBrowser1_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (navigating)
            {
                GlobalLoading.Instance.IsLoading = false;
                GlobalLoading.Instance.LoadingText = null;
            }
            navigating = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string selectedIndex;
            if (!App.FastSwitching && State.ContainsKey("url"))
            {
                // recover from tombstoning
                currentIndex = (int)State["currentIndex"];
                webBrowser1.Navigate((Uri)State["url"]);
                EnableOrDisableNavigation(currentIndex);
            }
            else if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                WebBrowserNavigate(int.Parse(selectedIndex));
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (navigating)
            {
                GlobalLoading.Instance.IsLoading = false;
                GlobalLoading.Instance.LoadingText = null;
                navigating = false;
            }

            // remember for tombstoning
            State["currentIndex"] = currentIndex;
            State["url"] = webBrowser1.Source;

            base.OnNavigatedFrom(e);
        }

        private void buttonEmail_Click(object sender, EventArgs e)
        {
            try
            {
                var context = (FeedItemViewModel)this.DataContext;
                Uri uri = webBrowser1.Source;
                string url = uri != null ? uri.AbsoluteUri : context.Url.AbsolutePath;

                EmailComposeTask task = new EmailComposeTask();
                task.Subject = context.Title;
                task.Body = string.Format("{0}\n\n{1}({2})", Strings.InterestingArticle, context.Title, url);
                task.To = string.Empty;
                task.Show();
            }
            catch
            {
                // prevent exceptions from double-click
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(App.ViewModel.AllFeedItems[currentIndex].Url);
        }

        private void buttonPrevious_Click(object sender, System.EventArgs e)
        {
            WebBrowserNavigate(currentIndex - 1);
        }

        private void buttonNext_Click(object sender, System.EventArgs e)
        {
            WebBrowserNavigate(currentIndex + 1);
        }

        private void WebBrowserNavigate(int index)
        {
            if (IsIndexValid(index))
            {
                currentIndex = index;
                DataContext = App.ViewModel.AllFeedItems[currentIndex];
                webBrowser1.Source = ((FeedItemViewModel)DataContext).Url;
                EnableOrDisableNavigation(currentIndex);

                // item should not be highlighted anymore
                App.ViewModel.AllFeedItems[currentIndex].IsNew = false;
            }
        }

        private void EnableOrDisableNavigation(int index)
        {
            var buttonPrevious = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            buttonPrevious.IsEnabled = index > 0 && App.ViewModel.AllFeedItems.Count != 0;

            var buttonNext = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
            buttonNext.IsEnabled = index < App.ViewModel.AllFeedItems.Count - 1;
        }

        private static bool IsIndexValid(int newIndex)
        {
            return newIndex >= 0 && newIndex < App.ViewModel.AllFeedItems.Count;
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            OrientationHelper.HideSystemTrayWhenInLandscapeMode(e.Orientation);
        }

        private void PhoneApplicationPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            OrientationHelper.HideSystemTrayWhenInLandscapeMode(this.Orientation);
        }
    }
}