using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Utilities;

namespace DishReaderApp
{
    public partial class MainPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // initialize view model after the page is loaded
            DataContext = App.ViewModel;

            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ReviewThisAppTask rate = new ReviewThisAppTask();
            rate.NumberOfStarts++;
            rate.ShowAfterThreshold();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // if selected index is -1 (no selection) do nothing
            if (MainListBox.SelectedIndex == -1)
            {
                return;
            }

            // navigate to the new page
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + MainListBox.SelectedIndex, UriKind.Relative));

            // reset selected index to -1 (no selection)
            MainListBox.SelectedIndex = -1;
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            App.ViewModel.LoadData();
        }

        private void RateThisAppMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var task = new MarketplaceReviewTask();
                task.Show();
            }
            catch
            {
                // prevent exceptions from double-click
            }
        }

        private void MoreAppsMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var task = new MarketplaceSearchTask();
                task.ContentType = MarketplaceContentType.Applications;
                task.SearchTerms = "Dapper Panda";
                task.Show();
            }
            catch
            {
                // prevent exceptions from double-click
            }
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            // use dispatcher to prevent jumping elements on the screen
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
            });
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            OrientationHelper.HideSystemTrayWhenInLandscapeMode(e.Orientation);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            OrientationHelper.HideSystemTrayWhenInLandscapeMode(this.Orientation);
        }
    }
}