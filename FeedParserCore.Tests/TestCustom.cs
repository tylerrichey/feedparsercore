using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeedParserCore.Tests
{
    [TestClass]
    [TestCategory("Custom")]
    public class TestCustom
    {
        private const string testString = "test";

        [TestMethod]
        public async Task ItemHandler()
        {
            var feed = await FeedParser.ParseAsync("Samples/testrss.rss", FeedType.RSS, _ => new
            {
                Summary = testString
            });
            Assert.IsTrue(feed.All(f => f.Summary == testString));
        }

        [TestMethod]
        public async Task FeedHanlder()
        {
            var feed = await FeedParser.ParseAsync("Samples/testrss.rss", x => x.Root
                    .Descendants()
                    .Where(i => i.Name.LocalName == "channel")
                    .Elements()
                    .Where(i => i.Name.LocalName == "item"), _ => new FeedItem
                    {
                        Content = testString
                    });
            Assert.IsTrue(feed.All(f => f.Content == testString));
            Assert.IsTrue(feed.Any());
        }
    }
}
