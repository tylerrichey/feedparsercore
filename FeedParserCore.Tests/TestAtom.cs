using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeedParserCore.Tests
{
    [TestClass]
    [TestCategory("Atom")]
    public class TestAtom : TestFeed
    {
        public TestAtom()
        {
            expectedItems = 25;
            _feed = FeedParser.ParseAsync("Samples/testatom.rss", FeedType.Atom)
                .Result
                .ToList();
        }

        [TestMethod]
        public async Task TestRemoteFeed()
        {
            var feed = await FeedParser.ParseAsync("https://github.com/security-advisories", FeedType.Atom);
            Assert.IsTrue(feed.Any());
        }
    }
}
