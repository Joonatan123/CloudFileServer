using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudFileServer.Functions;
namespace CloudFileServer.Models
{
    public class MvcFile
    {
        public string Path { get; set; }
        public string Name { get; set; }
        [Display(Name = "Ext.")]
        public string Extension { get; set; }
        //[Display(Name = "Upload Date")]
        //[DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public int Size { get; set; }
        public string SizeString { get; set; }
        public string FullName { get; set; }
        public bool UseThumbnail { get; set; }
        public MvcFile(string path, string rootDirectory)
        {
            string file = path;
            int finalSlash = 0;
            for (int i = 0; i < file.Length; i++)
                if (file[i] == '/')
                    finalSlash = i;
            string fileName = file.Substring(finalSlash + 1);
            string filePath = file.Substring(0, finalSlash + 1);
            filePath = filePath.Substring(rootDirectory.Length);
            int dotIndex = -1;
            for (int i = 0; i < fileName.Length; i++)
                if (fileName[i] == '.')
                    dotIndex = i;
            string fileExtension = "";
            if (dotIndex != -1)
                fileExtension = fileName.Substring(dotIndex);

            Path = filePath;
            Date = File.GetCreationTime(file);
            Name = fileName;
            Extension = fileExtension;
            Size = (int)new System.IO.FileInfo(file).Length;
            FullName = Path + Name;
            SizeString = "";
            MakeSizeString();
            DateString = HelperFunctions.MakeDateString(Date);
            string[] extensions = new string[5]{".jpg",".png",".webp","bmp","tiff"};
            if(extensions.Contains(Extension.ToLower()))
                UseThumbnail = true;
        }
        public void MakeSizeString()
        {
            int unit = 0;
            decimal tempSize = Size;
            for (; tempSize >= 1000; unit++)
            {
                tempSize = tempSize / 1000;
            }
            int decimals = 0;
            if (tempSize >= 100)
                decimals = 0;
            else if (tempSize >= 10)
                decimals = 1;
            else
                decimals = 2;
            tempSize = Math.Round(tempSize, decimals);

            string[] unitString = new string[4]{"B","kB","MB","GB"};

            SizeString = tempSize.ToString();
            SizeString += " " + unitString[unit];
        }
    }
}