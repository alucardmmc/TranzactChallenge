using ConsoleTables;
using System;

namespace TranzactChallenge.Application
{
    public static class ConsolePrint
    {
        public static void PrintTile()
        {
            Console.WriteLine("==================");
            Console.WriteLine("Tranzact Challenge");
            Console.WriteLine("==================");
        }

        public static void PrintFirstQuery(ConsoleTable table)
        {
            Console.WriteLine("\n==================");
            Console.WriteLine("Query 1:");
            Console.WriteLine("==================");

            table.Write();
        }

        public static void PrintSecondQuery(ConsoleTable table)
        {
            Console.WriteLine("\n==================");
            Console.WriteLine("Query 2:");
            Console.WriteLine("==================");

            table.Write();
        }
    }
}
