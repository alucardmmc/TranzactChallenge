using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TranzactChallenge.Model;

namespace TranzactChallenge.Application
{
    public class FileDownload{

        private static Uri GetUri(string year, string month, string day, string hour)
        {
            Uri uri = new Uri($"https://dumps.wikimedia.org/other/pageviews/{year}/{year}-{month}/" +
                         $"pageviews-{year}{month}{day}-{hour}0000.gz");
            return uri;
        }

        private static async Task DownloadAsyncUrl(Uri uri, string fileDir)
        {
            try
            {
                if (File.Exists(fileDir))
                {
                    File.Delete(fileDir);
                }

                WebClient wc = new WebClient();
                await wc.DownloadFileTaskAsync(uri, fileDir);
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        public static async Task ExecuteAsyncDownload(List<Time> time)
        {
            Console.WriteLine("\n===================");
            Console.WriteLine("Downloading Files: ");
            Console.WriteLine("===================");

            var watch = System.Diagnostics.Stopwatch.StartNew();

            foreach (Time t in time)
            {
                Console.Write($"Downloading WikiDump of {t.Year}-{t.Month}-{t.Day}-{t.Hour}... ");

                await DownloadAsyncUrl(GetUri(t.Year, t.Month, t.Day, t.Hour), FileDirectory.GetZippedFile(t.Year, t.Month, t.Day, t.Hour));
                Console.Write("Done!\n");
            }

            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;

            Console.WriteLine("Download Completed!");
            Console.WriteLine($"Total Execution Time: { elapsedMs }");
        }

    }
}
