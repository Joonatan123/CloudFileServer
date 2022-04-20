using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CloudFileServer.Models
{
    public class DownloadViewModel
    {
        public List<MvcFile> files { get; set; }
        public List<MvcFolder> folders { get; set; }
        public string path { get; set; }
        //[Display(Name = "Creation Date")]
        //[DataType(DataType.Date)]
        public SortInfo sortInfo { get; set; }
        public string upperFolder { get; set; }
        public DownloadViewModel(string path, string rootDirectory)
        {
            sortInfo = new SortInfo();
            this.path = path;
            string[] fileNames = Directory.GetFiles(rootDirectory + path);
            files = new List<MvcFile>(fileNames.Length);

            for (int i = 0; i < fileNames.Length; i++)
            {
                files.Add(new MvcFile(fileNames[i], rootDirectory));
            }
            bool backFolder = !path.Equals("/");

            string[] folderNames = Directory.GetDirectories(rootDirectory + path);
            folders = new List<MvcFolder>(folderNames.Length);
            if (backFolder){
                MvcFolder mvcFolder = new MvcFolder(path);
                upperFolder = mvcFolder.Path;
            }

            for (int i = 0; i < folderNames.Length; i++)
            {
                folders.Add(new MvcFolder(folderNames[i], rootDirectory));
            }
        }
        public void Sort()
        {
            if (sortInfo.SortBy.Equals(""))
                return;
            switch (sortInfo.SortBy)
            {
                case "Name":
                    if (!sortInfo.Reverse)
                    {
                        folders = folders.OrderBy(m => m.Name.ToUpper()).ToList();
                        files = files.OrderBy(m => m.Name.ToUpper()).ToList();
                    }
                    else
                    {
                        folders = folders.OrderByDescending(m => m.Name.ToUpper()).ToList();
                        files = files.OrderByDescending(m => m.Name.ToUpper()).ToList();
                    }
                    break;
                case "Date":
                    if (!sortInfo.Reverse)
                    {
                        folders = folders.OrderBy(m => m.Date).ToList();
                        files = files.OrderBy(m => m.Date).ToList();
                    }
                    else
                    {
                        folders = folders.OrderByDescending(m => m.Date).ToList();
                        files = files.OrderByDescending(m => m.Date).ToList();
                    }
                    break;
                case "Extension":
                    if (!sortInfo.Reverse)
                        files = files.OrderBy(m => m.Extension).ToList();
                    else
                        files = files.OrderByDescending(m => m.Extension).ToList();
                    break;
                case "Size":
                    if (!sortInfo.Reverse)
                        files = files.OrderBy(m => m.Size).ToList();
                    else
                        files = files.OrderByDescending(m => m.Size).ToList();
                    break;
            }
        }
    }
}