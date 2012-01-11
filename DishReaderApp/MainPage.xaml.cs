﻿using System;
using System.Windows;
using System.Windows.Controls;
using DishReaderApp.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DishReaderApp
{
    public partial class MainPage : PhoneApplicationPage
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
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.ViewModel.LoadData();
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

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            // use dispatcher to prevent jumping elements on the screen
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
            });
        }
    }
}