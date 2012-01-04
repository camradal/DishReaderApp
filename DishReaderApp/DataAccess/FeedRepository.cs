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
    public sealed class FeedRepository
    {
        private readonly List<FeedItem> feedItems;
        private readonly Uri sourceUri;

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
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(sourceUri);
            request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
        }

        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest request = (HttpWebRequest)callbackResult.AsyncState;
            using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(callbackResult))
            using (StreamReader httpWebStreamReader = new StreamReader(response.GetResponseStream()))
            {
                string results = httpWebStreamReader.ReadToEnd();
                IEnumerable<FeedItem> itemsToInsert = ExtractFeedItems(results);
                IEnumerable<FeedItem> uniqueItems = itemsToInsert.TakeWhile(item => item != feedItems.FirstOrDefault());
                feedItems.InsertRange(0, uniqueItems);
                
                // this is async operation, raise event that collection has been updated
                if (FeedUpdated != null)
                {
                    FeedUpdated(this, new FeedUpdatedEventArgs(uniqueItems));
                }
            }
        }

        private IEnumerable<FeedItem> ExtractFeedItems(string result)
        {
            using (StringReader stringReader = new StringReader(result))
            using (XmlReader reader = XmlReader.Create(stringReader))
            {
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                return from item in feed.Items
                       select new FeedItem()
                       {
                           Title = item.Title.Text,
                           Summary = item.Summary.Text,
                           Url = item.Links[0].Uri,
                           PublishedDate = item.PublishDate.DateTime
                       };
            }
        }
    }
}
