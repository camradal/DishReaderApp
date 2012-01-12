using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Windows;
using DishReaderApp.DataAccess;
using DishReaderApp.Models;
using DishReaderApp.Resources;
using System.Collections.Generic;
using System.Threading;

namespace DishReaderApp.ViewModels
{
    public sealed class AllFeedItemsViewModel : ViewModelBase
    {
        private readonly FeedRepository feedRepository = new FeedRepository(new Uri(@"http://feeds.feedburner.com/andrewsullivan/rApM"));

        public ObservableCollection<FeedItemViewModel> AllFeedItems { get; private set; }
        public bool IsDataLoaded { get; set; }
        public DateTime LastUpdated { get; set; }

        public AllFeedItemsViewModel()
        {
            AllFeedItems = new ObservableCollection<FeedItemViewModel>();
            feedRepository.FeedUpdated += new EventHandler<FeedUpdatedEventArgs>(feedRepository_FeedUpdated);
        }

        /// <summary>
        /// Load data asynchronously
        /// </summary>
        public void LoadData()
        {
            // only load data for the network if connection is available
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                GlobalLoading.Instance.LoadingWithText = Strings.Loading;
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
                int newItems = 0;
                foreach (var item in AllFeedItems)
                {
                    if (item.PublishedDate > LastUpdated)
                    {
                        item.IsNew = true;
                        newItems++;
                    }
                    else
                    {
                        item.IsNew = false;
                    }
                }

                if (newItems == 1)
                {
                    GlobalLoading.Instance.LoadingWithText = Strings.LoadedSingleNewPost;
                }
                else if (newItems > 0)
                {
                    GlobalLoading.Instance.LoadingWithText = string.Format(Strings.LoadedManyNewPostsTemplate, newItems);
                }
                else
                {
                    GlobalLoading.Instance.LoadingWithText = Strings.LoadedNoNewPosts;
                }

                // save last updated time from the actual repository
                LastUpdated = feedRepository.LastUpdated;
            });

            IsDataLoaded = true;
            Thread.CurrentThread.Join(1500);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                GlobalLoading.Instance.IsLoading = false;
                GlobalLoading.Instance.IsLoading = false;
            });
        }
    }
}
