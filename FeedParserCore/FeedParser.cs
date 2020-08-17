using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;

namespace FeedParserCore
{
    /// <summary>
    /// A simple RSS, RDF and ATOM feed parser.
    /// </summary>
    public static class FeedParser
    {
        private static HttpClient httpClient = new HttpClient();

        /// <summary>
        /// Parses the given <see cref="FeedType"/> and returns a <see cref="IEnumerable<FeedItem>"/>.
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<FeedItem>> ParseAsync(string url, FeedType feedType, bool throwExceptions = false) =>
            feedType switch
            {
                FeedType.RSS => await ParseRssAsync(url, throwExceptions),
                FeedType.RDF => await ParseRdfAsync(url, throwExceptions),
                FeedType.Atom => await ParseAtomAsync(url, throwExceptions),
                _ => throw new NotSupportedException(string.Format("{0} is not supported", feedType.ToString()))
            };

        /// <summary>
        /// Parses the given <see cref="FeedType"/> and returns a <see cref="IEnumerable<FeedItem>"/>.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FeedItem> Parse(string url, FeedType feedType, bool throwExceptions = false) => ParseAsync(url, feedType, throwExceptions).Result;

        /// <summary>
        /// Parses an Atom feed and returns a <see cref="IEnumerable<FeedItem>"/>.
        /// </summary>
        private static async Task<IEnumerable<FeedItem>> ParseAtomAsync(string url, bool throwExceptions)
        {
            try
            {
                var xDocument = await GetXDocument(url);
                return xDocument
                    .Root
                    .Elements()
                    .Where(i => i.Name.LocalName == "entry")
                    .Select(item => new FeedItem
                    {
                        FeedType = FeedType.Atom,
                        Content = item.GetElementValue<string>("content"),
                        Link = item.GetElementValue<string>("link", "href"),
                        PublishDate = item.GetElementValue<DateTime>("updated"),
                        Title = item.GetElementValue<string>("title")
                    });
            }
            catch (Exception e)
            {
                return HandleException(throwExceptions, e);
            }
        }

		/// <summary>
		/// Parses an RSS feed and returns a <see cref="IEnumerable<FeedItem>"/>.
		/// </summary>
		private static async Task<IEnumerable<FeedItem>> ParseRssAsync(string url, bool throwExceptions)
        {
            try
            {
                var xDocument = await GetXDocument(url);
                return xDocument
                    .Root
                    .Descendants()
                    .Where(i => i.Name.LocalName == "channel")
                    .Elements()
                    .Where(i => i.Name.LocalName == "item")
                    .Select(item => new FeedItem
                    {
                        FeedType = FeedType.RSS,
                        Content = item.GetElementValue<string>("description"),
                        Link = item.GetElementValue<string>("link"),
                        PublishDate = item.GetElementValue<DateTime>("pubDate"),
                        Title = item.GetElementValue<string>("title")
                    });
            }
            catch (Exception e)
            {
                return HandleException(throwExceptions, e);
            }
        }

		/// <summary>
		/// Parses an RDF feed and returns a <see cref="IEnumerable<FeedItem>"/>.
		/// </summary>
		private static async Task<IEnumerable<FeedItem>> ParseRdfAsync(string url, bool throwExceptions)
        {
            try
            {
                var xDocument = await GetXDocument(url);
                return xDocument
                    .Root
                    .Descendants()
                    .Where(i => i.Name.LocalName == "item")
                    .Select(item => new FeedItem
                    {
                        FeedType = FeedType.RDF,
                        Content = item.GetElementValue<string>("description"),
                        Link = item.GetElementValue<string>("link"),
                        PublishDate = item.GetElementValue<DateTime>("date"),
                        Title = item.GetElementValue<string>("title")
                    });
            }
            catch (Exception e)
            {
                return HandleException(throwExceptions, e);
            }
        }

        private static async Task<XDocument> GetXDocument(string url)
        {
            Stream data;
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri result))
            {
                data = await httpClient.GetStreamAsync(url);
            }
            else
            {
                data = File.OpenRead(url);
            }
            //LoadAsync is only in .net standard 2.1
            return await Task.Run(() => XDocument.Load(data));
        }

        private static T GetElementValue<T>(this XElement element, string fieldName, string attribute = null)
        {
            var item = element.Elements()
                .FirstOrDefault(i => i.Name.LocalName == fieldName);

            if (item == null)
            {
                return (T)Convert.ChangeType(new object(), typeof(T));
            }
            else
            {
                if (attribute != null)
                {
                    var attr = item.Attribute(attribute);
                    if (attr == null)
                    {
                        return (T)Convert.ChangeType(new object(), typeof(T));
                    }
                    else
                    {
                        return (T)Convert.ChangeType(attr.Value, typeof(T));
                    }
                }
                return (T)Convert.ChangeType(item.Value, typeof(T));
            }
        }

        private static IEnumerable<FeedItem> HandleException(bool throwExceptions, Exception e) => throwExceptions ? throw e : new List<FeedItem>();

        /// <summary>
        /// Pass in the HttpClient instance your application is already using.
        /// </summary>
        /// <param name="newHttpClient"></param>
        public static void SetHttpClient(HttpClient newHttpClient) => httpClient = newHttpClient;
    }
}
