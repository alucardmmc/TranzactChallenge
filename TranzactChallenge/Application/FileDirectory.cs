using System;
using System.IO;

namespace TranzactChallenge.Application
{
    public static class FileDirectory
    {
        public static string subDir = Helper.GetDataDumpFolder("DataDumpFolder");

        public static string GetZippedFile(string year, string month, string day, string hour)
        {
            string file = $"pageviews-{year}{month}{day}-{hour}0000.gz";
            string fileDir = subDir + @"\" + file;
            return fileDir;
        }

        public static string GetUnzippedFile(string year, string month, string day, string hour)
        {
            string file = $"pageviews-{year}{month}{day}-{hour}0000";
            string fileDir = subDir + @"\" + file;
            return fileDir;
        }

        public static void AddFolderDirectory()
        {
            Console.WriteLine("\n===========================");
            Console.WriteLine("Adding Subfolder for Dumps: ");
            Console.WriteLine("===========================");

            try
            {
                if (Directory.Exists(subDir))
                {
                    Console.WriteLine("The path already exists!");
                    return;
                }

                DirectoryInfo di = Directory.CreateDirectory(subDir);
                Console.WriteLine("The directory was created successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

    }
}
