using System;
namespace FeedParserCore
{
	/// <summary>
	/// Represents a feed item.
	/// </summary>
	public class FeedItem
	{
		/// <summary>
		/// String representation of the feed item's URL.
		/// </summary>
        public string Link { get; set; }
		/// <summary>
		/// The feed item's title.
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// The summary/description/data/etc of the feed item.
		/// </summary>
		public string Content { get; set; }
		/// <summary>
		/// Published/created date for the feed item.
		/// </summary>
		public DateTime PublishDate { get; set; } = DateTime.MinValue;
	}
}
