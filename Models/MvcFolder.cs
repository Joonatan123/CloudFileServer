using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudFileServer.Models
{
    public class MvcFolder
    {
        public string Path { get; set; }
        public string Name { get; set; }
        //[Display(Name = "Creation Date")]
        //[DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string ServerPath { get; set; }
        public MvcFolder(string path, string rootDirectory)
        {
            Date = Directory.GetCreationTime(path);
            Path = path.Substring(rootDirectory.Length);
            int lastSlash = -1;
            for (int i = 0; i < Path.Length; i++)
                if (Path[i] == '/')
                    lastSlash = i;
            Name = Path.Substring(lastSlash + 1) + '/';
            Path = Path.Substring(0, lastSlash + 1);
            ServerPath = Path + Name;
        }
        public MvcFolder(string path)
        {
            path = path.Substring(0, path.Length - 1);
            int lastSlash = -1;
            for (int i = 0; i < path.Length; i++)
                if (path[i] == '/')
                    lastSlash = i;
            Path = path.Substring(0, lastSlash + 1);
            Name = "..";
            Date = new DateTime();
            ServerPath = Path;
        }
    }
}