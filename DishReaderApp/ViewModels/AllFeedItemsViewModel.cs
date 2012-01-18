using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using DishReaderApp.DataAccess;
using DishReaderApp.Models;
using DishReaderApp.Resources;
using Utilities;

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
                ShowLoadingMessage();
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
                UpdateViewModelWithNewItems(e.Items);
                int newItems = MarkNewItemsInViewModel();
                DisplayStatusMessage(newItems);

                // save last updated time from the actual repository
                LastUpdated = feedRepository.LastUpdated;
            });
            IsDataLoaded = true;

            // wait a bit to dismiss the message
            Thread.CurrentThread.Join(2500);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                RemoveLoadingMessage();
            });
        }

        private static void ShowLoadingMessage()
        {
            GlobalLoading.Instance.IsLoading = true;
            GlobalLoading.Instance.LoadingText = Strings.Loading;
        }

        private static void RemoveLoadingMessage()
        {
            GlobalLoading.Instance.IsLoading = false;
            GlobalLoading.Instance.LoadingText = null;
        }

        private static void DisplayStatusMessage(int newItems)
        {
            if (newItems == 1)
            {
                GlobalLoading.Instance.LoadingText = Strings.LoadedSingleNewPost;
            }
            else if (newItems > 0)
            {
                GlobalLoading.Instance.LoadingText = string.Format(Strings.LoadedManyNewPostsTemplate, newItems);
            }
            else
            {
                GlobalLoading.Instance.LoadingText = Strings.LoadedNoNewPosts;
            }
        }

        private int MarkNewItemsInViewModel()
        {
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
            return newItems;
        }

        private void UpdateViewModelWithNewItems(List<FeedItem> items)
        {
            int i = 0;
            foreach (FeedItem item in items)
            {
                AllFeedItems.Insert(i, new FeedItemViewModel(item, feedRepository));
                i++;
            }
        }
    }
}
