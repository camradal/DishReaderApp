using System;
using System.Windows;
using System.Windows.Controls;
using DishReaderApp.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Utilities;
using DishReaderApp.ViewModels;

namespace DishReaderApp
{
    public partial class MainPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // specify the text explicitly on the app bar using our resource string
            var buttonRefresh = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            buttonRefresh.Text = Strings.ButtonRefresh;

            var menuItemAbout = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
            menuItemAbout.Text = Strings.MenuItemAbout;

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

            // item should not be highlighted anymore
            var item = (FeedItemViewModel)MainListBox.Items[MainListBox.SelectedIndex];
            item.IsNew = false;

            // navigate to the new page
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + MainListBox.SelectedIndex, UriKind.Relative));

            // reset selected index to -1 (no selection)
            MainListBox.SelectedIndex = -1;
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            App.ViewModel.LoadData();
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