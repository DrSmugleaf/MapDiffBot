using CommandLine;

namespace MapDiffBot
{
    public class Arguments
    {
        [Option("uploaded", Required = true, HelpText = "An array of image links.")]
        public string Uploaded { get; set; } = default!;
        
        [Option("maps", Required = true, HelpText = "An array of map names.")]
        public string Maps { get; set; } = default!;
        
        // Example: https://spacestation14.io/map-diff-viewer/
        [Option("url", Required = true, HelpText = "Url to append ?old and ?new query parameters on.")]
        public string DifferUrl { get; set; } = default!;

        [Option("baseCommit", Required = true, HelpText = "Commit hash of the base branch.")]
        public string BaseCommit { get; set; } = default!;
        
        [Option("repoName", Required = true, HelpText = "Repository's full name in the format {owner}/{repo}.")]
        public string RepoName { get; set; } = default!;
        
        [Option("mapImageDirectory", Required = true, HelpText = "Path to the directory that contains old map images.")]
        public string MapImageDirectory { get; set; } = default!;
    }
}