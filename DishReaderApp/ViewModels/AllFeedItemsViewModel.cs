using System;
using System.Collections.ObjectModel;
using System.Windows;
using DishReaderApp.DataAccess;
using DishReaderApp.Models;

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
            feedRepository.LoadFeedsAsync();
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
            });

            IsDataLoaded = true;
        }
    }
}
