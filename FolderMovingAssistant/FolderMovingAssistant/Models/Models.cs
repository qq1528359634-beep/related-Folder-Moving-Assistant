using System;
using System.Collections.Generic;
using System.Text;

namespace relatedFolderMovingAssistant.Models
{
    class FolderPath
    {
        public static string? destiPath;
        public static string? sourcePath;

        public static List<string>? destiFolders;
        public static List<string>? sourceFolders;
    }
    class SortedFolder
    {
        public string? destiFolder { get; set; }
        public string? SourceFolder { get; set; }
    }

}
