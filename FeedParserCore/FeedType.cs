using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace FeedParserCore
{
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
        Atom,
        /// <summary>
        /// Type for custom feed/item handlers
        /// </summary>
        Custom
    }
}
