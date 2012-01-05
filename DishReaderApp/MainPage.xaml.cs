using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using DishReaderApp.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace DishReaderApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // application title need to be upper case
            ApplicationTitle.Text = Strings.AppTitle.ToUpper(CultureInfo.CurrentCulture);

            // specify the text explicitly on the app bar using our resource string
            var buttonRefresh = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            buttonRefresh.Text = Strings.ButtonRefresh;

            var buttonSettings = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            buttonSettings.Text = Strings.ButtonSettings;

            // initialize view model after the page is loaded
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
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
    }
}