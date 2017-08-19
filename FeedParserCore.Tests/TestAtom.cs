using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FeedParserCore;

namespace FeedParserCore.Tests
{
    [TestClass]
    public class TestAtom : TestFeed
    {
        public TestAtom()
        {
            expectedItems = 25;
            _feed = FeedParser.Parse(@"Samples/testatom.rss", FeedType.Atom, true)
                              .ToList();
        }
    }
}
