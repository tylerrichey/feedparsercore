# feedparsercore
RSS/Atom/RDF feed parser in .NET Standard 2.0

[![Build status](https://ci.appveyor.com/api/projects/status/bcmdea24nwidpm5u?svg=true)](https://ci.appveyor.com/project/tylerrichey/feedparsercore) [![NuGet](https://img.shields.io/nuget/v/FeedParserCore.svg)](https://www.nuget.org/packages/FeedParserCore/)

Since System.ServiceModel.Syndication is not available in dotnet core yet, I re-factored the example here: http://www.anotherchris.net/csharp/simplified-csharp-atom-and-rss-feed-parser/

Usage:
```c#
var itemList = FeedParser.Parse(Url, FeedType.Atom, true);
```
Install:
```
Install-Package FeedParserCore
```
or
```
dotnet add package FeedParserCore
````
