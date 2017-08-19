using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace FeedParserCore
{
    /// <summary>
    /// A simple RSS, RDF and ATOM feed parser.
    /// </summary>
    public static class FeedParser
    {
        /// <summary>
        /// Parses the given <see cref="FeedType"/> and returns a <see cref="IEnumerable<FeedItem>"/>.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FeedItem> Parse(string url, FeedType feedType, bool throwExceptions = false)
        {
            switch (feedType)
            {
                case FeedType.RSS:
                    return ParseRss(url, throwExceptions);
                case FeedType.RDF:
                    return ParseRdf(url, throwExceptions);
                case FeedType.Atom:
                    return ParseAtom(url, throwExceptions);
                default:
                    throw new NotSupportedException(string.Format("{0} is not supported", feedType.ToString()));
            }
        }

		/// <summary>
		/// Parses an Atom feed and returns a <see cref="IEnumerable<FeedItem>"/>.
		/// </summary>
		private static IEnumerable<FeedItem> ParseAtom(string url, bool throwExceptions)
        {
            try
            {
                return XDocument.Load(url)
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
		private static IEnumerable<FeedItem> ParseRss(string url, bool throwExceptions)
        {
            try
            {
                return XDocument.Load(url)
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
		private static IEnumerable<FeedItem> ParseRdf(string url, bool throwExceptions)
        {
            try
            {
                return XDocument.Load(url)
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

        private static T GetElementValue<T>(this XElement element, string fieldName, string attribute = null)
        {
            var item = element.Elements().FirstOrDefault(i => i.Name.LocalName == fieldName);

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

        private static IEnumerable<FeedItem> HandleException(bool throwExceptions, Exception e)
        {
            if (throwExceptions)
            {
                throw e;
            }
            else
            {
                return new List<FeedItem>();
            }
        }
    }
    /// <summary>
    /// Represents the XML format of a feed.
    /// </summary>
    public enum FeedType
    {
        /// <summary>
        /// Really Simple Syndication format.
        /// </summary>
        RSS,
        /// <summary>
        /// RDF site summary format.
        /// </summary>
        RDF,
        /// <summary>
        /// Atom Syndication format.
        /// </summary>
        Atom
    }
}
