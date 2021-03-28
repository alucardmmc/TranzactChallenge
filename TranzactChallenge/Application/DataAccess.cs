using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
using TranzactChallenge.Model;
using ConsoleTables;

namespace TranzactChallenge.Application
{
    public class DataAccess
    {
        public ConsoleTable GetFirstQuery()
        {
            var table = new ConsoleTable("Period", "Language", "Domain", "ViewCount");

            using (var connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("WikiDumpDB")))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("dbo.spWikiPage_GetFirstQuery", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    cmd.CommandTimeout = 300;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var periodHour = Convert.ToInt32(reader["PeriodHour"]);
                            var pageLanguage = reader["PageLanguage"].ToString();
                            var pageDomain = reader["PageDomain"].ToString();
                            var countViews = Convert.ToInt32(reader["CountViews"]);

                            table.AddRow(periodHour, pageLanguage, pageDomain, countViews);
                        }
                    }
                }
            }
            return table;
        }

        public ConsoleTable GetSecondQuery()
        {
            var table = new ConsoleTable("Period", "PageTitle", "ViewCount");

            using (var connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("WikiDumpDB")))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("dbo.spWikiPage_GetSecondQuery", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {

                    cmd.CommandTimeout = 300;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var periodHour = Convert.ToInt32(reader["PeriodHour"]);
                            var pageTitle = reader["PageTitle"].ToString();
                            var countViews = Convert.ToInt32(reader["CountViews"]);

                            table.AddRow(periodHour, pageTitle, countViews);
                        }
                    }
                }
            }

            return table;
        }

        private void InsertWikiPages(DataTable table)
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("WikiDumpDB")))
            {

                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                    {

                        try
                        {
                            bulkCopy.DestinationTableName = "WikiPage";
                            bulkCopy.WriteToServer(table);
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Commit Exception Type: {0}", e.GetType());
                            Console.WriteLine("  Message: {0}", e.Message);
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Rollback Exception Type", ex.ToString());
                                Console.WriteLine("  Message: {0}", ex.Message);
                            }
                        }
                    }
                }
            }
        }

        public void TruncateTable()
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("WikiDumpDB")))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("dbo.spWikiPage_Truncate", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    cmd.CommandTimeout = 30;

                    using SqlDataReader reader = cmd.ExecuteReader();
                }
            }
        }

        public void ExecuteDataSave(List<Time> time, WikiDump wikiDump)
        {
            Console.WriteLine("\n==========================");
            Console.WriteLine("Accessing and Saving Data: ");
            Console.WriteLine("==========================");

            Console.Write("Truncating Table... ");
            TruncateTable();
            Console.Write("Done\n");

            foreach (Time t in time)
            {

                var table = new DataTable();

                table.Columns.Add("ID", typeof(int));
                table.Columns.Add("PeriodHour", typeof(int));
                table.Columns.Add("PageLanguage", typeof(string));
                table.Columns.Add("PageDomain", typeof(string));
                table.Columns.Add("PageDesign", typeof(string));
                table.Columns.Add("PageTitle", typeof(string));
                table.Columns.Add("CountViews", typeof(int));
                table.Columns.Add("TotalResponseSize", typeof(string));

                Console.Write($"Saving WikiDump pageviews-{t.Year}{t.Month}{t.Day}-{t.Hour}0000... ");

                List<string> lines = File.ReadAllLines(FileDirectory.GetUnzippedFile(t.Year, t.Month, t.Day, t.Hour)).ToList();

                foreach (var line in lines)
                {
                    string[] entries = line.Split(' ');

                    WikiPage wp = wikiDump.CreateWikiPage(Int32.Parse(t.Hour), entries);

                    table.Rows.Add(new object[]
                    {
                        wp.ID,
                        wp.PeriodHour,
                        wp.PageLanguage,
                        wp.PageDomain,
                        wp.PageDesign,
                        wp.PageTitle,
                        wp.CountViews,
                        wp.TotalResponseSize
                    });
                }

                InsertWikiPages(table);

                Console.Write("Done!\n");
            }

            Console.WriteLine("Saving Completed!");
        }


    }
}
