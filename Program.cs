using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace DabDownload
{
    class Program
    {
        static void Main(string[] args)
        {
            var sYear = DateTime.Now.Year.ToString();
            int year = DateTime.Now.Year;

            while(!int.TryParse(sYear, out year))
            {
                Console.Write("Enter a year (ex: 2019): ");
                sYear = Console.ReadLine();
            }

            //if (year > 2000)
            //  DownloadYear(year);

            if (year > 2000)
            {
                //DownloadYear(year, 1, 366);
                /*
                Console.WriteLine("Downloaded day 1-50. Hit enter to continue.");
                Console.ReadLine();

                DownloadYear(year, 51, 100);

                Console.WriteLine("Downloaded day 51-10. Hit enter to continue.");
                Console.ReadLine();

                DownloadYear(year, 101, 150);

                Console.WriteLine("Downloaded day 101-150. Hit enter to continue.");
                Console.ReadLine();

                DownloadYear(year, 151, 200);

                Console.WriteLine("Downloaded day 151-200. Hit enter to continue.");
                Console.ReadLine();

                DownloadYear(year, 201, 250);

                Console.WriteLine("Downloaded day 201-250. Hit enter to continue.");
                Console.ReadLine();

                DownloadYear(year, 251, 300);

                Console.WriteLine("Downloaded day 251-300. Hit enter to continue.");
                Console.ReadLine();

                DownloadYear(year, 301, 366);

                Console.WriteLine("Downloaded day 301-366. Hit enter to continue.");
                */
                TagEdit(year, 1, 366);

                Console.ReadLine();
            }
        }

        static void DownloadYear(int year, int startDay, int endDate)
        {
            var cnt = 0;
            var dir = @"C:\Users\jmoon\Downloads";

            var date = new DateTime(year, 1, 1);
            //if (startDay > 1)
            //    date = date.AddDays(startDay - 1);

            for (int day = startDay; day <= endDate; day++)
            {                
                //https://player.dailyaudiobible.com/file.php?file=https://media.dailyaudiobible.com/2019/Daily%20Audio%20Bible%20-%20349%20-%20December%2015%2C%202019.m4a
                /*
                  https://player.dailyaudiobible.com/file.php?
                    file=https://media.dailyaudiobible.com/
                            2019/Daily %20 Audio %20 Bible %20 - %20 349 %20 - %20 December %20 15 %2C %20 2019.m4a

                %20 = ` `
                %2C = `,`

                */

                var link = "";
                var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month);
                var sDay = date.Day < 10 ? $"0{date.Day}" : date.Day.ToString();
                var sDayOfYear = date.DayOfYear < 100 ? $"0{date.DayOfYear}" : date.DayOfYear.ToString();

                /*
                 * don't download if file exists
                 */
#if DEBUG
                if (date.Month == 4)
                {
                    var x = 0;
                    x++;
                }
                if (date.Month == 4 && date.Day < 10)
                {
                    var x = 0;
                    x++;
                }
#endif

                var filename = $@"{dir}\Daily Audio Bible - {date.DayOfYear} - {month} {sDay}, {year}.m4a";
                if (System.IO.File.Exists(filename))
                {
                    var fileLength = new System.IO.FileInfo(filename).Length;
                    if (fileLength > 5000000)
                    {
                        date = date.AddDays(1);
                        cnt++;
                        continue;
                    }
                    else
                        System.IO.File.Delete(filename);
                }

                if (date.Month < 4)
                    link = $"https://player.dailyaudiobible.com/file.php?file=http://podcast.dailyaudiobible.com/mp3/{month}{sDay}-{year}.m4a";
                else
                    link = $"https://player.dailyaudiobible.com/file.php?file=https://media.dailyaudiobible.com/2019/Daily%20Audio%20Bible%20-%20{sDayOfYear}%20-%20{month}%20{sDay}%2C%20{year}.m4a";

                Console.WriteLine($"Downloading DAB file for {month} {sDay}, {year}");
                Console.WriteLine($"\tlink: {link}");

                Process.Start(link);

                date = date.AddDays(1);
                cnt++;
            }
        }

        static void TagEdit(int year, int startDay, int endDate)
        {
            var date = new DateTime(year, 1, 1);

            var dir = @"C:\Users\jmoon\Downloads";

            for (int day = startDay; day <= endDate; day++)
            {
                try
                {
                    /*
                    * January01-2019.m4a
                    * Daily Audio Bible - 272 - September 29, 2018.m4a
                    * 
                    */
                    var filename = "";
                    var finalFilename = "";

                    var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month);
                    var sDay = date.Day < 10 ? $"0{date.Day}" : date.Day.ToString();


                    if (date.Month < 4)
                    {
                        filename = $@"{dir}\{month}{sDay}-{year}.m4a";
                        finalFilename = $@"{dir}\Daily Audio Bible - {date.DayOfYear} - {month} {sDay}, {year}.m4a";

                        if(System.IO.File.Exists(filename))
                            System.IO.File.Move(filename, finalFilename);

                        filename = finalFilename;
                    }
                    else
                    {
                        filename = $@"{dir}\Daily Audio Bible - {date.DayOfYear} - {month} {sDay}, {year}.m4a";
                        finalFilename = filename;
                    }

                    if (!System.IO.File.Exists(filename))
                    {
                        Console.WriteLine($"Failed to find file: {filename} - {month} {sDay}, {year}");
                        continue;
                    }

                    Console.WriteLine($"Setting ID3 Tags for {filename} - {month} {sDay}, {year}");

                    var file = File.Create(filename);
                    file.Tag.AlbumArtists = new List<string>() { "Daily Audio Bible" }.ToArray();
                    file.Tag.Year = uint.Parse(year.ToString());
                    file.Tag.Title = $"DAB - {day} - {month} {sDay}, {year}";
                    file.Tag.Album = $"Daily Audio Bible {date.Year}";

                    var tag = (TagLib.Mpeg4.AppleTag)file.GetTag(TagTypes.Apple);
                    tag.Track = uint.Parse(day.ToString());
                    tag.TrackCount = uint.Parse(new DateTime(year, 12, 31).DayOfYear.ToString());
                    file.Save();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed on date {date.ToLongDateString()}. Ex: {ex}");
                }
                finally
                {
                    date = date.AddDays(1);
                }
            }
        }
    }
}
