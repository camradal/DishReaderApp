using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Windows;
using DishReaderApp.DataAccess;
using DishReaderApp.Models;
using DishReaderApp.Resources;
using System.Collections.Generic;

namespace DishReaderApp.ViewModels
{
    public sealed class AllFeedItemsViewModel : ViewModelBase
    {
        private readonly FeedRepository feedRepository = new FeedRepository(new Uri(@"http://feeds.feedburner.com/andrewsullivan/rApM"));

        public ObservableCollection<FeedItemViewModel> AllFeedItems { get; private set; }
        public DateTime LastUpdated { get; set; }

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
                List<FeedItem> items = new List<FeedItem>(e.Items);

                // insert new items
                int i = 0;
                foreach (FeedItem item in e.Items)
                {
                    AllFeedItems.Insert(i, new FeedItemViewModel(item, feedRepository));
                    i++;
                }

                // make sure items do not show up as new if there are any updates
                foreach (var item in AllFeedItems)
                {
                    item.IsNew = item.PublishedDate > LastUpdated;
                }

                // save last updated time from the actual repository
                LastUpdated = feedRepository.LastUpdated;

                GlobalLoading.Instance.IsLoading = false;
            });
        }
    }
}
