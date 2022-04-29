using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;
using System.Text;

namespace CloudFileServer.Functions
{
    class HelperFunctions
    {
        public static void CloneDirectory(string root, string dest)
        {
            foreach (var directory in Directory.GetDirectories(root))
            {
                string dirName = Path.GetFileName(directory);
                if (!Directory.Exists(Path.Combine(dest, dirName)))
                {
                    Directory.CreateDirectory(Path.Combine(dest, dirName));
                }
                CloneDirectory(directory, Path.Combine(dest, dirName));
            }

            foreach (var file in Directory.GetFiles(root))
            {
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
            }
        }
        public static string MakeDateString(DateTime date)
        {
            string[] weekdays = new string[7] { "Monday   ", "Tuesday  ", "Wednesday", "Thursday ", "Friday   ", "Saturday ", "Sunday   " };
            DateTime today = DateTime.Today;
            if (today.Year == date.Year)
            {
                return date.ToShortDateString().Substring(0, 5) + " " + date.ToShortTimeString();
            }
            return date.ToShortDateString();
            /*
            if (today.Year == date.Year && today.DayOfYear == date.DayOfYear)
            {
                return "Today     " + date.ToShortTimeString();
            }
            TimeSpan elapsed = today.Subtract(date);
            int daysElapsed = (int)Math.Ceiling(elapsed.TotalDays);
            if (daysElapsed == 1)
            {
                return "Yesterday " + date.ToShortTimeString();
            }
            if (daysElapsed < 6)
            {
                return weekdays[((int)date.DayOfWeek)] + " " + date.ToShortTimeString();
            }
            return "older";*/
        }
        public static void MakeThumbnail(string file, string thumbnailName)
        {

            int thumbnailHeight = 50;
            using (var image = SixLabors.ImageSharp.Image.Load(file))
            {
                // Resize the image in place and return it for chaining.
                // 'x' signifies the current image processing context.
                using (var ms = new MemoryStream())
                {
                    float aspect = (float)image.Width / image.Height;
                    image.Mutate(x => x.Resize((int)(aspect * thumbnailHeight), thumbnailHeight));
                    //if (image.Width > 160)
                    //image.Mutate(x => x.Resize(200, (int)(200 / aspect)));
                    //Console.WriteLine("thumbnails/" + thumbnailName + ".jpg");
                    image.SaveAsJpegAsync("thumbnails/" + thumbnailName + ".jpg");

                }
            }
        }
        public static void MakeThumbnail(string file)
        {

            MakeThumbnail(file,GetStringHashString(file));
        }
        public static void MakeThumbnails(string path)
        {
            
            List<string> files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();
            List<int> remove = new List<int>();
            string[] extensions = new string[5] { ".jpg", ".png", ".webp", "bmp", "tiff" };
            int size = files.Count;
            for (int i = 0; i < files.Count; i++)
            {
                if (!extensions.Contains(Path.GetExtension(files[i]).ToLower()))
                    remove.Add(i);
            }
            for (; remove.Count != 0;)
            {
                files.RemoveAt(remove.Last());
                remove.RemoveAt(remove.Count - 1);
            }
            if (!Directory.Exists("thumbnails"))
            {
                Directory.CreateDirectory("thumbnails");
            }
            foreach (string file in files)
            {
                //Console.WriteLine(file);
                string hash = GetStringHashString(file);
                if (File.Exists("thumbnails/" + hash + ".jpg"))
                    continue;
                Task.Run(() => MakeThumbnail(file, hash));
            }
            Console.WriteLine("Thumbnails ready!");
        }
        public static bool Exists(string filename)
        {
            //Console.WriteLine(filename);
            return File.Exists(filename);
        }
        static byte[] HashString(string value){
            return SHA1.HashData(Encoding.ASCII.GetBytes(value));
        }
        static string ByteArrayToHexString(byte[] bytes)
        {
            return string.Concat(Array.ConvertAll(bytes, b => b.ToString("X2")));
        }
        public static string GetStringHashString(string value){
            return ByteArrayToHexString(HashString(value));
        }
    }
}