using System;
using System.Collections.Generic;
using DishReaderApp.Models;

namespace DishReaderApp.DataAccess
{
    public sealed class FeedUpdatedEventArgs : EventArgs
    {
        public FeedUpdatedEventArgs(IEnumerable<FeedItem> items)
        {
            this.Items = items;
        }

        public IEnumerable<FeedItem> Items { get; private set; }
    }
}
