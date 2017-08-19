using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FeedParserCore;

namespace FeedParserCore.Tests
{
	[TestClass]
    public class TestRSS : TestFeed
	{
		public TestRSS()
		{
            expectedItems = 9;
			_feed = FeedParser.Parse(@"Samples/testrss.rss", FeedType.RSS, true)
							  .ToList();
		}
	}
}
