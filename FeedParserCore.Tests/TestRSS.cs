using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
		public async Task TestBadNoPubDate()
        {
			var feed = await FeedParser.ParseAsync("Samples/badrss.rss", FeedType.RSS);
			Assert.IsTrue(feed.All(f => f.PublishDate == DateTime.MinValue));
        }

		[TestMethod]
		public async Task TestStream()
		{
			var file = File.OpenRead("Samples/testrss.rss");
			var feed = await FeedParser.ParseAsync(file, FeedType.RSS);
			Assert.AreEqual(expectedItems, feed.Count());
		}
	}
}
