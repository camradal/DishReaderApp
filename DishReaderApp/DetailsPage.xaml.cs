using System;
using System.Windows.Navigation;
using DishReaderApp.Resources;
using DishReaderApp.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

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

            webBrowser1.Navigated += new EventHandler<NavigationEventArgs>(webBrowser1_Navigated);
            webBrowser1.LoadCompleted += new LoadCompletedEventHandler(webBrowser1_LoadCompleted);
        }

        void webBrowser1_Navigated(object sender, NavigationEventArgs e)
        {
            if (!navigating)
            {
                GlobalLoading.Instance.IsLoading = true;
            }
            navigating = true;
        }

        private void webBrowser1_LoadCompleted(object sender, NavigationEventArgs e)
        {
            navigating = false;
            GlobalLoading.Instance.IsLoading = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                WebBrowserNavigate(int.Parse(selectedIndex));
            }
        }

        private void buttonEmail_Click(object sender, EventArgs e)
        {

        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(webBrowser1.Source);
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
            }
        }

        private void EnableOrDisableNavigation(int index)
        {
            var buttonPrevious = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            buttonPrevious.IsEnabled = index > 0;

            var buttonNext = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
            buttonNext.IsEnabled = index < App.ViewModel.AllFeedItems.Count - 1;
        }

        private static bool IsIndexValid(int newIndex)
        {
            return newIndex >= 0 && newIndex < App.ViewModel.AllFeedItems.Count;
        }
    }
}