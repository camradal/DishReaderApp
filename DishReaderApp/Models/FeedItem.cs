using System;

namespace DishReaderApp.Models
{
    public sealed class FeedItem
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public Uri Url { get; set; }
        public DateTime PublishedDate { get; set; }
        public bool IsNew { get; set; }
    }
}
