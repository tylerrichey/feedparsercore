# FeedParserCore
RSS/Atom/RDF/Custom feed parser in .NET Standard 2.0

[![Build status](https://ci.appveyor.com/api/projects/status/bcmdea24nwidpm5u?svg=true)](https://ci.appveyor.com/project/tylerrichey/feedparsercore) [![NuGet](https://img.shields.io/nuget/v/FeedParserCore.svg)](https://www.nuget.org/packages/FeedParserCore/)

Initially, this was done as System.ServiceModel.Syndication was not available in dotnet core yet, so I re-factored the example here: http://www.anotherchris.net/csharp/simplified-csharp-atom-and-rss-feed-parser/

Today, it's been re-factored further to be asynchronous and more customizable for non-standard feeds. I think, ideally, this would be a .NET Standard 2.1 library as the Xml libraries are more advanced, but I'm leaving this as 2.0 for the backwards compatibility across the framework.

Basic Usage:
```c#
var items = await FeedParser.ParseAsync("https://github.com/security-advisories", FeedType.Atom);
```
Advanced Usage:
```c#
var stream = await GetRawFeedMemoryStream();
var items = await FeedParser.ParseAsync(stream,
    xDocument => xDocument.Root
        .Descendants()
        .Where(i => i.Name.LocalName == "channel")
        .Elements()
        .Where(i => i.Name.LocalName == "item"),
    item => new
    {
        Summary = item.GetElementValue<string>("summary"),
        Date = item.GetElementValue<DateTime>("date")
    });
```
Install:
```
Install-Package FeedParserCore
```
or
```
dotnet add package FeedParserCore
````
