﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;
using DishReaderApp.Models;
using Utilities;

namespace DishReaderApp.DataAccess
{
    /// <summary>
    /// Feed repository is responsible for managing internal feed items collection
    /// </summary>
    public sealed class FeedRepository
    {
        private readonly HtmlToTextConverter htmlConverter = new HtmlToTextConverter();
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
                        LastUpdated = uniqueItems.First().PublishedDate;
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
            var items = new List<FeedItem>();
            using (StringReader stringReader = new StringReader(value))
            {
                string content = stringReader.ReadToEnd();
                content = content.Replace("-0001 00:00:00 +0000", string.Format("{0} 00:00:00 +0000", DateTime.Now.Year));
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
                using (XmlReader reader = XmlReader.Create(new MemoryStream(bytes)))
                {
                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                    foreach (SyndicationItem item in feed.Items)
                    {
                        try
                        {
                            items.Add(new FeedItem()
                            {
                                Title = htmlConverter.Convert(item.Title.Text),
                                Summary = htmlConverter.Convert(item.Summary.Text),
                                Url = item.Links[0].Uri,
                                PublishedDate = item.PublishDate.DateTime.AddHours(5).ToLocalTime(), // adjust for EST
                                IsNew = true
                            });
                        }
                        catch (Exception)
                        {
                            // ignore individual errors
                        }
                    }
                }
            }
            return items;
        }
    }
}
