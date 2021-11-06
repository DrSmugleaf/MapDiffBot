using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using CommandLine;

namespace MapDiffBot
{
    internal static class Program
    {
        private const string GithubContentUrl = "https://raw.githubusercontent.com/";
        
        private static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Arguments>(args)
                .WithParsed(Comment)
                .WithNotParsed(_ => Environment.Exit(2));
        }

        private static string[] ParseArray(string str)
        {
            return str
                .Remove(0, 1)
                .Remove(str.Length - 1, 1)
                .Split(',')
                .Select(s => s.Remove(0, 1).Remove(s.Length - 1, 1))
                .ToArray();
        }

        private static void Comment(Arguments args)
        {
            var images = ParseArray(args.Uploaded);
            var maps = ParseArray(args.Maps);
            args.DifferUrl += args.DifferUrl.EndsWith('/') ? string.Empty : '/';

            var message = new StringBuilder($"{maps.Length} maps modified.");
            
            for (var i = 0; i < maps.Length; i++)
            {
                var mapPath = maps[i];
                var mapName = Path.GetFileNameWithoutExtension(mapPath);
                var mapDirectory = new FileInfo(mapPath).Directory!.FullName;
                var jsonPath = $"{mapDirectory}{Path.DirectorySeparatorChar}{mapName}.json";

                string? oldImageLink = null;
                if (File.Exists(jsonPath))
                {
                    var json = JsonDocument.Parse(jsonPath).RootElement;

                    if (json.GetProperty("oldLink").GetString() is { } oldLink)
                    {
                        oldImageLink =  Uri.EscapeDataString(oldLink);
                    }
                }

                var newImageLink = Uri.EscapeDataString(images[i]);
                oldImageLink ??= $"{GithubContentUrl}{args.RepoName}/{args.BaseCommit}/{args.MapImageDirectory}";
                var url = $"{args.DifferUrl}?old={oldImageLink}&new={newImageLink}";

                message.AppendLine($"[{mapName}]({url})");
            }
        }
    }
}