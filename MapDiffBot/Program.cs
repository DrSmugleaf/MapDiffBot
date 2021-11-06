using System;
using System.IO;
using System.Linq;
using System.Net;
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
                .Remove(str.Length - 2, 1)
                .Split(',')
                .ToArray();
        }

        private static void Comment(Arguments args)
        {
            var images = ParseArray(args.Uploaded);
            var maps = ParseArray(args.Maps);
            args.DifferUrl += args.DifferUrl.EndsWith('/') ? string.Empty : '/';

            var message = new StringBuilder($"{maps.Length} maps were modified.\n");
            
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
                oldImageLink ??= $"{GithubContentUrl}{args.RepoName}/{args.BaseCommit}/{args.MapImageDirectory}/{mapName}.png";
                
                try
                {
                    var request = WebRequest.Create(oldImageLink);
                    using var response = request.GetResponse();
                }
                catch (WebException e)
                {
                    if (e.Status != WebExceptionStatus.ProtocolError || e.Response == null)
                    {
                        throw;
                    }
                    
                    var resp = (HttpWebResponse) e.Response;
                    if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        oldImageLink = null;
                    }
                }
                
                var url = $"{args.DifferUrl}?new={newImageLink}";

                if (oldImageLink != null)
                {
                    url += $"?old={oldImageLink}";
                }

                message.AppendLine($"[{mapName}]({url})");
            }
        }
    }
}