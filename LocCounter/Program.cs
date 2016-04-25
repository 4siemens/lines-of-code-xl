using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Http;
using Gma.CodeCloud.Base.TextAnalyses.Blacklist;
using Gma.CodeCloud.Base.TextAnalyses.Extractors.Code;
using Gma.CodeCloud.Base.TextAnalyses.Processing;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.StaticFiles.ContentTypes;
using Newtonsoft.Json;
using Owin;

namespace LocCounter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var rootDir = args.Length > 0 ? args[0] : @"D:\WS\WM5_WinCC_MU_Work\src\PE\";

            //var sizeTree = CountLines(rootDir);
            //var wordTree = CountWords(rootDir);

            //File.WriteAllText(@"html\flare.json", JsonConvert.SerializeObject(sizeTree));
            //File.WriteAllText(@"html\words.json", JsonConvert.SerializeObject(wordTree));

            var url = StartHttpHost();

            Process.Start(url);
            Console.ReadKey();
        }

        private static string StartHttpHost()
        {
            var url = "http://localhost:8080";
            var fileSystem = new PhysicalFileSystem("./html");
            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                FileSystem = fileSystem
            };

            options.StaticFileOptions.ContentTypeProvider = new JsonContentTypeProvider();

            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            WebApp.Start(url, builder =>
            {
                builder.UseFileServer(options).UseWebApi(config);
            });
            Console.WriteLine("Listening at " + url);
            return url;
        }

        private static WordsTree CountWords(string rootDir)
        {
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Counting words.");
            var tree = new WordsTree() { Name = rootDir };

            var fileCount = 0;
            GetRelevantFiles(rootDir)
                .AsParallel()
                .ForAll(file =>
                {
                    var shorname = file.Substring(rootDir.Length);
                    WordDo[] words = 
                        new CSharpExtractor(file)
                            .CountOccurences()
                            .Select(w=>w.ToDo())
                            .ToArray();
                    tree.Add(shorname, words);

                    fileCount++;
                    if (fileCount % 100 == 0)
                    {
                        Console.WriteLine($"Processing {shorname}, #{fileCount}");
                    }
                });
            return tree;
        }

        private static SizeTree CountLines(string rootDir)
        {
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Counting lines.");

            var tree = new SizeTree(500) {Name = rootDir};

            var fileCount = 0;
            GetRelevantFiles(rootDir)
                .AsParallel()
                .ForAll(file =>
                {
                    var shorname = file.Substring(rootDir.Length);
                    var locs = GetLocs(file);
                    tree.Add(shorname, locs);
                    fileCount++;
                    if (fileCount%100 == 0)
                    {
                        Console.WriteLine($"Processing {shorname}, #{fileCount}");
                    }
                });
            return tree;
        }

        private static IEnumerable<string> GetRelevantFiles(string rootDir)
        {
            return Directory
                .EnumerateFiles(rootDir, "*.*", SearchOption.AllDirectories)
                .Where(IsSource)
                .Where(f => !IsTest(f))
                .Where(f => !MustSkip(f));
        }

        private static bool MustSkip(string shorname)
        {
            if (shorname.Contains(@"\3rdParty\")) return true;
            if (shorname.Contains(@"\ARCH\EA\")) return true;
            if (shorname.Contains(@"\obj\")) return true;

            return false;
        }

        private static bool IsTest(string file)
        {
            return
                file.ToLowerInvariant().Contains("#_test");
        }

        private static bool IsSource(string file)
        {
            var extension = Path.GetExtension(file)?.ToLowerInvariant();
            switch (extension)
            {
                case ".c":
                case ".cpp":
                //case ".xml":
                //case ".resx":
                //case ".xsd":
                //case ".xls":
                //case ".xlsx":
                case ".cs":
                    return true;
                default:
                    return false;
            }
        }

        private static int GetLocs(string file)
        {
            return File.ReadLines(file).Count();
        }

        public class JsonContentTypeProvider : FileExtensionContentTypeProvider
        {
            public JsonContentTypeProvider()
            {
                Mappings.Add(".json", "application/json");
            }
        }
    }
}