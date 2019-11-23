﻿using System.Collections.Generic;
using System.Data.Odbc;

namespace TabScore.Models
{
    public class BoardMove
    {
        public int Table { get; }

        public BoardMove(string DB, int sectionID, int round, int table, int lowBoard)
        {
            using (OdbcConnection connection = new OdbcConnection(DB))
            {
                // Get a list of all possible tables to which boards could move
                List<int> tableList = new List<int>();
                string SQLString = $"SELECT [Table] FROM RoundData WHERE Section={sectionID} AND Round={round} AND LowBoard={lowBoard}";
                connection.Open();
                OdbcCommand cmd = new OdbcCommand(SQLString, connection);
                OdbcDataReader reader = null;
                try
                {
                    ODBCRetryHelper.ODBCRetry(() =>
                    {
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int t = reader.GetInt32(0);
                            tableList.Add(t);
                        }
                    });
                }
                catch (OdbcException)
                {
                    Table = -1;
                    return;
                }
                finally
                {
                    reader.Close();
                    cmd.Dispose();
                }

                if (tableList.Count == 0)
                {
                    // No table, so move to relay table
                    Table = 0;
                }
                else if (tableList.Count == 1)
                {
                    // Just one table, so use it
                    Table = tableList[0];
                }
                else
                {
                    // Find the next table down to which the boards could move
                    for (int t = table; t > 0; t--)
                    {
                        if (tableList.Contains(t))
                        {
                            Table = t;
                            return;
                        }
                    }
                    Table = 0;
                    foreach (int t in tableList)  // Next table down must be highest table number in the list
                    {
                        if (t > Table) Table = t;
                    }
                }
            }
        }
    }
}