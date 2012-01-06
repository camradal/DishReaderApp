using System;
using System.Collections.ObjectModel;
using System.Windows;
using DishReaderApp.DataAccess;
using DishReaderApp.Models;
using System.Net.NetworkInformation;
using DishReaderApp.Resources;

namespace DishReaderApp.ViewModels
{
    public sealed class AllFeedItemsViewModel : ViewModelBase
    {
        private readonly FeedRepository feedRepository = new FeedRepository(new Uri(@"http://feeds.feedburner.com/andrewsullivan/rApM"));

        public ObservableCollection<FeedItemViewModel> AllFeedItems { get; private set; }
        public bool IsDataLoaded { get; private set; }

        public AllFeedItemsViewModel()
        {
            AllFeedItems = new ObservableCollection<FeedItemViewModel>();
            feedRepository.FeedUpdated += new EventHandler<FeedUpdatedEventArgs>(feedRepository_FeedUpdated);
        }

        /// <summary>
        /// Load data asynchronously, to get the result, need to register for FeedUpdated event
        /// </summary>
        public void LoadData()
        {
            // only load data for the network if connection is available
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                GlobalLoading.Instance.IsLoading = true;
                feedRepository.LoadFeedsAsync();
            }
            else
            {
                MessageBox.Show(Strings.ErrorInternetConnection);
            }
        }

        private void feedRepository_FeedUpdated(object sender, FeedUpdatedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                int i = 0;
                foreach (FeedItem item in e.Items)
                {
                    AllFeedItems.Insert(i, new FeedItemViewModel(item, feedRepository));
                    i++;
                }
                GlobalLoading.Instance.IsLoading = false;
            });

            IsDataLoaded = true;
        }
    }
}
