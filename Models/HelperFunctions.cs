using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
            if(today.Year == date.Year){
                return date.ToShortDateString().Substring(0,5) + " " + date.ToShortTimeString();
            }
            return date.ToShortDateString() ;
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
    }
}