using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DishReaderApp.Resources;
using Microsoft.Phone.Controls;
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
            // TODO: implement navigation service
            // NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + MainListBox.SelectedIndex, UriKind.Relative));
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = App.ViewModel.AllFeedItems[MainListBox.SelectedIndex].Url;
            webBrowserTask.Show();

            // reset selected index to -1 (no selection)
            MainListBox.SelectedIndex = -1;
        }
    }
}