using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using TranzactChallenge.Model;

namespace TranzactChallenge.Application
{
    public class FileDecompress
    {
        private static void Decompress(FileInfo fileToDecompress, string unzFile)
        {
            try
            {
                if (File.Exists(unzFile))
                {
                    File.Delete(unzFile);
                }

                using (FileStream originalFileStream = fileToDecompress.OpenRead())
                {
                    string currentFileName = fileToDecompress.FullName;
                    string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                    using (FileStream decompressedFileStream = File.Create(newFileName))
                    {
                        using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(decompressedFileStream);
                            Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        public static void ExecuteDecompress(List<Time> time)
        {
            Console.WriteLine("\n=======================");
            Console.WriteLine("Decompressing gz Files: ");
            Console.WriteLine("=======================");

            foreach (Time t in time)
            {
                Console.Write($"Decompressing WikiDump pageviews-{t.Year}{t.Month}{t.Day}-{t.Hour}0000.gz... ");
                FileInfo file = new FileInfo(FileDirectory.GetZippedFile(t.Year, t.Month, t.Day, t.Hour));
                string unzFile = FileDirectory.GetUnzippedFile(t.Year, t.Month, t.Day, t.Hour);

                Decompress(file, unzFile);
                Console.Write("Done!\n");
            }

            Console.WriteLine("Decompressing Completed!");
        }
    }
}
