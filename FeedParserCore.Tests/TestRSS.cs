using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeedParserCore.Tests
{
	[TestClass]
    [TestCategory("RSS")]
    public class TestRSS : TestFeed
	{
		public TestRSS()
		{
            expectedItems = 9;
			_feed = FeedParser.ParseAsync("Samples/testrss.rss", FeedType.RSS)
				.Result
				.ToList();
		}

		[TestMethod]
		public void TestBadNoPubDate()
        {
			var feed = FeedParser.ParseAsync("Samples/badrss.rss", FeedType.RSS)
				.Result
				.ToList();
			Assert.IsTrue(feed.All(f => f.PublishDate == DateTime.MinValue));
        }
	}
}
