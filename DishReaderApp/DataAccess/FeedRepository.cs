using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;
using DishReaderApp.Models;

namespace DishReaderApp.DataAccess
{
    /// <summary>
    /// Feed repository is responsible for managing internal feed items collection
    /// </summary>
    public sealed class FeedRepository
    {
        private readonly List<FeedItem> feedItems;
        private readonly Uri sourceUri;

        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Register for this event to be modified when feed is updated
        /// </summary>
        public event EventHandler<FeedUpdatedEventArgs> FeedUpdated;

        public FeedRepository(Uri uri)
        {
            feedItems = new List<FeedItem>();
            sourceUri = uri;
        }

        /// <summary>
        /// Load feed items async, results will returned in FeedUpdated event
        /// </summary>
        public void LoadFeedsAsync()
        {
            LoadFromWeb();
        }

        private void LoadFromWeb()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(sourceUri);
                request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
            }
            catch
            {
                // handle failure to submit request
            }
        }

        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)callbackResult.AsyncState;
                using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(callbackResult))
                using (StreamReader httpWebStreamReader = new StreamReader(response.GetResponseStream()))
                {
                    string results = httpWebStreamReader.ReadToEnd();
                    IEnumerable<FeedItem> itemsToInsert = ExtractFeedItemsFromSyndicationString(results);

                    // extract unique items and insert them into beginning of storage collection
                    List<FeedItem> uniqueItems = itemsToInsert.Where(item => item.PublishedDate > LastUpdated).ToList();
                    feedItems.InsertRange(0, uniqueItems);

                    // set last updated time to max date
                    if (uniqueItems.Count > 0)
                    {
                        LastUpdated = uniqueItems.Max(item => item.PublishedDate);
                    }

                    // this is async operation, raise event that collection has been updated and pass unique items
                    if (FeedUpdated != null)
                    {
                        FeedUpdated(this, new FeedUpdatedEventArgs(uniqueItems));
                    }
                }
            }
            catch
            {
                // handle failure to read response
            }
        }

        private IEnumerable<FeedItem> ExtractFeedItemsFromSyndicationString(string value)
        {
            using (StringReader stringReader = new StringReader(value))
            using (XmlReader reader = XmlReader.Create(stringReader))
            {
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                return from item in feed.Items
                       select new FeedItem()
                       {
                           Title = item.Title.Text,
                           Summary = item.Summary.Text,
                           Url = item.Links[0].Uri,
                           PublishedDate = item.PublishDate.DateTime,
                           IsNew = true
                       };
            }
        }
    }
}
