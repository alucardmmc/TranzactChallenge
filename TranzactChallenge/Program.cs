using System;
using System.Collections.Generic;
using TranzactChallenge.Model;
using TranzactChallenge.Application;

namespace TranzactChallenge
{
    public class Program
    {

        static void Main(string[] args)
        {
            // -------------------------- Variables -----------------------------

            // Data Access Variables:
            DataAccess db = new DataAccess();

            // Initialiazing Variable for WikiDump:
            WikiDump wikiDump = new WikiDump();

            // Initializing List of Time:
            List<Time> listTime = Helper.GetListTime();

            // -------------------------- Variables -----------------------------

            // -------------------------- Main Code -----------------------------

            ConsolePrint.PrintTile();

            // Adding the sub foler if it doesnt exists:
            FileDirectory.AddFolderDirectory();

            // Download All Files
            FileDownload.ExecuteAsyncDownload(listTime).Wait();

            // Decompressing All Files
            FileDecompress.ExecuteDecompress(listTime);

            // Accessing Data Files and Saving it to our database
            db.ExecuteDataSave(listTime, wikiDump);

            // First Query:
            ConsolePrint.PrintFirstQuery(db.GetFirstQuery());

            // SecondQuery:
            ConsolePrint.PrintSecondQuery(db.GetSecondQuery());

            Console.WriteLine("\nEverything Done!");
            Console.ReadLine();
        }
    }
}
