using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FeedParserCore;

namespace FeedParserCore.Tests
{
	public class TestFeed
	{
		protected int expectedItems = 0;
        protected List<FeedItem> _feed = new List<FeedItem>();

		[TestMethod]
		public void BasicTest()
		{
			Assert.AreEqual(expectedItems, _feed.Count());
		}

		[TestMethod]
		public void TestTitleNotBlank()
		{
			var titles = _feed.Count(i => !string.IsNullOrEmpty(i.Title));
			Assert.AreEqual(expectedItems, titles);
		}

		[TestMethod]
		public void TestPublishedNoMin()
		{
			var dates = _feed.Count(i => i.PublishDate != DateTime.MinValue);
			Assert.AreEqual(expectedItems, dates);
		}

		[TestMethod]
		public void TestContentNoBlank()
		{
			var dates = _feed.Count(i => !string.IsNullOrEmpty(i.Link));
			Assert.AreEqual(expectedItems, dates);
		}

		[TestMethod]
		public void TestLinkNoBlank()
		{
			var dates = _feed.Count(i => !string.IsNullOrEmpty(i.Content));
			Assert.AreEqual(expectedItems, dates);
		}
	}
}
