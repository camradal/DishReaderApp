using System;
using System.Collections.Generic;
using DishReaderApp.Models;

namespace DishReaderApp.DataAccess
{
    /// <summary>
    /// Arguments for the the event when feed is updated
    /// </summary>
    public sealed class FeedUpdatedEventArgs : EventArgs
    {
        public FeedUpdatedEventArgs(IEnumerable<FeedItem> items)
        {
            this.Items = items;
        }

        public IEnumerable<FeedItem> Items { get; private set; }
    }
}
