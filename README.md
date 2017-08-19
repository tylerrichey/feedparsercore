# feedparsercore
RSS/Atom/RDF feed parser in .NET Standard 2.0

[![Build status](https://ci.appveyor.com/api/projects/status/bcmdea24nwidpm5u?svg=true)](https://ci.appveyor.com/project/tylerrichey/feedparsercore)

Since System.ServiceModel.Syndication is not available in dotnet core yet, I re-factored the example here: http://www.anotherchris.net/csharp/simplified-csharp-atom-and-rss-feed-parser/

Usage:
```c#
var itemList = FeedParser.Parse(Url, FeedType.Atom, true);
```
NuGet: Coming soon.
