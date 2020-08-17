using System;
namespace FeedParserCore
{
	/// <summary>
	/// Represents a feed item.
	/// </summary>
	public class FeedItem
	{
        public string Link { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public DateTime PublishDate { get; set; }

        public FeedItem()
		{
			Link = "";
			Title = "";
			Content = "";
			PublishDate = DateTime.MinValue;
		}
	}
}
