using System;
using DishReaderApp.DataAccess;
using DishReaderApp.Models;

namespace DishReaderApp.ViewModels
{
    public sealed class FeedItemViewModel : ViewModelBase
    {
        private FeedItem feedItem;
        private FeedRepository feedRepository;

        public FeedItemViewModel(FeedItem feedItem, FeedRepository feedRepository)
        {
            this.feedItem = feedItem;
            this.feedRepository = feedRepository;
        }

        public string Title
        {
            get
            {
                return feedItem.Title;
            }
            set
            {
                feedItem.Title = value;
                base.NotifyPropertyChanged("Title");
            }
        }

        public string Summary
        {
            get
            {
                return feedItem.Summary;
            }
            set
            {
                feedItem.Summary = value;
                base.NotifyPropertyChanged("Summary");
            }
        }

        public Uri Url
        {
            get
            {
                return feedItem.Url;
            }
            set
            {
                feedItem.Url = value;
                base.NotifyPropertyChanged("Url");
            }
        }

        public DateTime PublishedDate
        {
            get
            {
                return feedItem.PublishedDate;
            }
            set
            {
                feedItem.PublishedDate = value;
                base.NotifyPropertyChanged("PublishedDate");
            }
        }
    }
}
