using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

namespace FeedParserCore
{
    /// <summary>
    /// A simple RSS, RDF and ATOM feed parser.
    /// </summary>
    public static class FeedParser
    {
        private static HttpClient httpClient = SetupHttpClient();
        private static HttpClient SetupHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new("FeedParserCore", "2.0.0"));
            return httpClient;
        }

        private static readonly Dictionary<FeedType, Func<XElement, FeedItem>> feedTypeItemMaps = new Dictionary<FeedType, Func<XElement, FeedItem>>
        {
            {
                FeedType.RSS, item => new FeedItem
                {
                    Content = item.GetElementValue<string>("description"),
                    Link = item.GetElementValue<string>("link"),
                    PublishDate = item.GetElementValue<DateTime>("pubDate"),
                    Title = item.GetElementValue<string>("title")
                }
            },
            {
                FeedType.RDF, item => new FeedItem
                {
                    Content = item.GetElementValue<string>("description"),
                    Link = item.GetElementValue<string>("link"),
                    PublishDate = item.GetElementValue<DateTime>("date"),
                    Title = item.GetElementValue<string>("title")
                }
            },
            {
                FeedType.Atom, item => new FeedItem
                {
                    Content = item.GetElementValue<string>("content"),
                    Link = item.GetElementValue<string>("link", "href"),
                    PublishDate = item.GetElementValue<DateTime>("updated"),
                    Title = item.GetElementValue<string>("title")
                }
            }
        };

        private static readonly Dictionary<FeedType, Func<XDocument, IEnumerable<XElement>>> feedTypeHandlers = new Dictionary<FeedType, Func<XDocument, IEnumerable<XElement>>>
        {
            {
                FeedType.RSS, (x) => x.Root
                    .Descendants()
                    .Where(i => i.Name.LocalName == "channel")
                    .Elements()
                    .Where(i => i.Name.LocalName == "item")
            },
            {
                FeedType.RDF, (x) => x.Root
                    .Descendants()
                    .Where(i => i.Name.LocalName == "item")
            },
            {
                FeedType.Atom, (x) => x.Root
                    .Elements()
                    .Where(i => i.Name.LocalName == "entry")
            }
        };

        /// <summary>
        /// Async method to parse the given url.
        /// </summary>
        /// <param name="url">URL of feed</param>
        /// <param name="feedType">Type of feed to parse</param>
        public static async Task<IEnumerable<FeedItem>> ParseAsync(string url, FeedType feedType)
        {
            var data = await GetXDocumentFromUrl(url);
            return await ParseAsync(data, feedType.GetFeedHandler(), feedType.GetItemHandler());
        }

        /// <summary>
        /// Async method to parse the given stream.
        /// </summary>
        /// <param name="stream">Stream containing raw feed data</param>
        /// <param name="feedType">Type of feed to parse</param>
        public static async Task<IEnumerable<FeedItem>> ParseAsync(Stream stream, FeedType feedType)
        {
            var xDocument = await GetXDocumentFromStream(stream);
            return await ParseAsync(xDocument, feedType.GetFeedHandler(), feedType.GetItemHandler());
        }

        /// <summary>
        /// Async method to parse the given url with a custom item handler
        /// </summary>
        /// <param name="url">URL of feed</param>
        /// <param name="feedType">Type of feed to parse</param>
        /// <param name="itemHandler">Custom handler to translate element to T</param>
        public static async Task<IEnumerable<T>> ParseAsync<T>(string url, FeedType feedType, Func<XElement, T> itemHandler)
        {
            var data = await GetXDocumentFromUrl(url);
            return await ParseAsync(data, feedType.GetFeedHandler(), itemHandler);
        }

        /// <summary>
        /// Async method to parse the given url with custom handlers
        /// </summary>
        /// <param name="url">URL of feed</param>
        /// <param name="feedHandler">Custom handler to select items from XML document</param>
        /// <param name="itemHandler">Custom handler to translate element to T</param>
        public static async Task<IEnumerable<T>> ParseAsync<T>(string url, Func<XDocument, IEnumerable<XElement>> feedHandler, Func<XElement, T> itemHandler)
        {
            var data = await GetXDocumentFromUrl(url);
            return await ParseAsync(data, feedHandler, itemHandler);
        }

        /// <summary>
        /// Async method to parse the given stream with a custom item handler
        /// </summary>
        /// <param name="stream">Stream containing raw feed data</param>
        /// <param name="feedType">Type of feed to parse</param>
        /// <param name="itemHandler">Custom handler to translate element to T</param>
        public static async Task<IEnumerable<T>> ParseAsync<T>(Stream stream, FeedType feedType, Func<XElement, T> itemHandler)
        {
            var data = await GetXDocumentFromStream(stream);
            return await ParseAsync(data, feedType.GetFeedHandler(), itemHandler);
        }

        /// <summary>
        /// Async method to parse the given stream with a custom item handler
        /// </summary>
        /// <param name="stream">Stream containing raw feed data</param>
        /// <param name="feedHandler">Custom handler to select items from XML document</param>
        /// <param name="itemHandler">Custom handler to translate element to T</param>
        public static async Task<IEnumerable<T>> ParseAsync<T>(Stream stream, Func<XDocument, IEnumerable<XElement>> feedHandler, Func<XElement, T> itemHandler)
        {
            var data = await GetXDocumentFromStream(stream);
            return await ParseAsync(data, feedHandler, itemHandler);
        }

        /// <summary>
        /// Pass in the static HttpClient instance your application is already using.
        /// </summary>
        /// <param name="newHttpClient">Existing static HttpClient instance</param>
        public static void SetHttpClient(HttpClient newHttpClient) => httpClient = newHttpClient;

        /// <summary>
        /// Helper to translate an XML value to a typed object, useful for custom item handlers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="fieldName"></param>
        /// <param name="attribute"></param>
        public static T GetElementValue<T>(this XElement element, string fieldName, string attribute = null)
        {
            var item = element.Elements()
                .FirstOrDefault(i => i.Name.LocalName == fieldName);

            if (item == null)
            {
                return ConvertToType<T>();
            }
            else
            {
                if (attribute != null)
                {
                    var attr = item.Attribute(attribute);
                    return attr == null ? ConvertToType<T>() : ConvertToType<T>(attr.Value);
                }
                return ConvertToType<T>(item.Value);
            }
        }

        private static async Task<IEnumerable<T>> ParseAsync<T>(XDocument xDocument, Func<XDocument, IEnumerable<XElement>> feedHandler, Func<XElement, T> itemHandler) =>
            await Task.Run(() => feedHandler(xDocument).Select(itemHandler));

        private static Func<XElement, FeedItem> GetItemHandler(this FeedType feedType) => feedTypeItemMaps[feedType];

        private static Func<XDocument, IEnumerable<XElement>> GetFeedHandler(this FeedType feedType) => feedTypeHandlers[feedType];

        private static async Task<XDocument> GetXDocumentFromUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri result))
            {
                var data = await httpClient.GetStreamAsync(result);
                return await GetXDocumentFromStream(data);
            }
            using var file = new FileStream(url, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
            return await GetXDocumentFromStream(file);
        }

        //LoadAsync is only in .net standard 2.1
        private static async Task<XDocument> GetXDocumentFromStream(Stream stream) => await Task.Run(() => XDocument.Load(stream));

        private static T ConvertToType<T>(string value = "")
        {
            if (typeof(T) == typeof(DateTime) && string.IsNullOrWhiteSpace(value))
            {
                return (T)Convert.ChangeType(DateTime.MinValue, typeof(T));
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            else if (string.IsNullOrWhiteSpace(value))
            {
                return (T)Convert.ChangeType(new object(), typeof(T));
            }
            else
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }
    }
}
