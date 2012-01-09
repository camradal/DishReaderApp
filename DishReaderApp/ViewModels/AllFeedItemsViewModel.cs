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
        public bool IsDataLoaded { get; private set; }
        
        public DateTime LastUpdated
        {
            get
            {
                return feedRepository.LastUpdated;
            }
            set
            {
                feedRepository.LastUpdated = value;
            }
        }

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

                // make sure items do not show up as new if there are any updates
                if (items.Count > 0)
                {
                    foreach (var item in AllFeedItems)
                    {
                        item.IsNew = false;
                    }
                }

                // insert new items
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
