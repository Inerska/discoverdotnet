﻿using System;
using System.Collections.Generic;
using System.Text;
using DiscoverDotnet.Modules;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;
using Statiq.Yaml;

namespace DiscoverDotnet.Pipelines.Broadcasts
{
    public class Broadcasts : Pipeline
    {
        public Broadcasts()
        {
            InputModules = new ModuleList
            {
                new ReadFiles(Config.FromContext(x => x.FileSystem.RootPath.Combine("../../data/broadcasts/*.yml").FullPath)),
                new ExecuteIf(Config.FromContext(x => x.ApplicationState.IsCommand("preview")))
                {
                    new OrderDocuments(Config.FromDocument(x => x.Source)),
                    new TakeDocuments(5)
                }
            };

            ProcessModules = new ModuleList
            {
                new ParseYaml(),
                new ReplaceContent(string.Empty),
                new GetFeedData(),
                new AddMetadata("Key", Config.FromDocument(x => x.Source.FileNameWithoutExtension.FullPath)),
                new AddMetadata("Link", Config.FromDocument((d, c) => c.GetLink(d))),
                new AddMetadata("Language", Config.FromDocument(x => x.GetString("Language", "English"))),
                new AddMetadata("SearchData", Config.FromDocument(x => x.GetMetadata(
                    "Key",
                    "Title",
                    "Description"))),
                new AddMetadata("CardData", Config.FromDocument(x => x.GetMetadata(
                    "Key",
                    "Title",
                    "Image",
                    "Link",
                    "Description",
                    "Author",
                    "Website",
                    "Feed",
                    "Comment",
                    "Language",
                    "LastPublished",
                    "NewestFeedItem",
                    "DiscoveryDate"))),
                new SetDestination(Config.FromDocument(x => (FilePath)$"broadcasts/{x.Source.FileName.ChangeExtension("html")}"))
            };

            TransformModules = new ModuleList
            {
                new RenderRazor().WithLayout((FilePath)"/broadcasts/_Layout.cshtml")
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
