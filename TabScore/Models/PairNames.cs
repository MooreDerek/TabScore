﻿using System.Data.Odbc;

namespace TabScore.Models
{
    public static class PairNames
    {
        public static NamesClass GetNamesForPairNo(string DB, string SectionID, string PairNo, string Direction)
        {
            string SQLString;
            object queryResult;
            string Table;
            string StartDirection;

            using (OdbcConnection connection = new OdbcConnection(DB))
            {
                // First get table at which this pair started
                connection.Open();
                if (Direction == "NS")
                {
                    SQLString = $"SELECT Table FROM RoundData WHERE Section={SectionID} AND NSPair={PairNo} AND Round=1";
                }
                else
                {
                    SQLString = $"SELECT Table FROM RoundData WHERE Section={SectionID} AND EWPair={PairNo} AND Round=1";
                }
                OdbcCommand cmd = new OdbcCommand(SQLString, connection);
                queryResult = cmd.ExecuteScalar();
                if (queryResult != null)
                {
                    Table = queryResult.ToString();
                    StartDirection = Direction;
                }
                else
                {
                    if (Direction == "NS")
                    {
                        SQLString = $"SELECT Table FROM RoundData WHERE Section={SectionID} AND EWPair={PairNo} AND Round=1";
                    }
                    else
                    {
                        SQLString = $"SELECT Table FROM RoundData WHERE Section={SectionID} AND NSPair={PairNo} AND Round=1";
                    }
                    cmd = new OdbcCommand(SQLString, connection);
                    queryResult = cmd.ExecuteScalar();
                    Table = queryResult.ToString();
                    if (Direction == "NS")
                    {
                        StartDirection = "EW";
                    }
                    else
                    {
                        StartDirection = "NS";
                    }
                }
                cmd.Dispose();
            }
            // Now get names from that starting table
            return GetNamesForStartTableNo(DB, SectionID, Table, StartDirection);
        }

        public static NamesClass GetNamesForStartTableNo(string DB, string SectionID, string Table, string StartDirection)
        {
            NamesClass names = new NamesClass();

            string SQLString;
            object queryResult;

            using (OdbcConnection connection = new OdbcConnection(DB))
            {
                connection.Open();
                if (StartDirection == "NS")
                {
                    SQLString = $"SELECT Number FROM PlayerNumbers WHERE Section={SectionID} AND Direction='N' AND Table={Table}";
                }
                else
                {
                    SQLString = $"SELECT Number FROM PlayerNumbers WHERE Section={SectionID} AND Direction='E' AND Table={Table}";
                }
                OdbcCommand cmd = new OdbcCommand(SQLString, connection);
                queryResult = cmd.ExecuteScalar();
                if (queryResult == null  || queryResult.ToString() == "")
                {
                    names.NameNE = "";
                }
                else
                {
                    SQLString = $"SELECT Name FROM PlayerNames WHERE ID={queryResult.ToString()}";
                    cmd = new OdbcCommand(SQLString, connection);
                    queryResult = cmd.ExecuteScalar();
                    if (queryResult == null || queryResult.ToString() == "")
                    {
                        names.NameNE = "";
                    }
                    else
                    {
                        names.NameNE = queryResult.ToString();
                    }
                }
                if (StartDirection == "NS")
                {
                    SQLString = $"SELECT Number FROM PlayerNumbers WHERE Section={SectionID} AND Direction='S' AND Table={Table}";
                }
                else
                {
                    SQLString = $"SELECT Number FROM PlayerNumbers WHERE Section={SectionID} AND Direction='W' AND Table={Table}";
                }
                cmd = new OdbcCommand(SQLString, connection);
                queryResult = cmd.ExecuteScalar();
                if (queryResult == null || queryResult.ToString() == "")
                {
                    names.NameSW = "";
                }
                else
                {
                    SQLString = $"SELECT Name FROM PlayerNames WHERE ID={queryResult.ToString()}";
                    cmd = new OdbcCommand(SQLString, connection);
                    queryResult = cmd.ExecuteScalar();
                    if (queryResult == null || queryResult.ToString() == "")
                    {
                        names.NameSW = "";
                    }
                    else
                    {
                        names.NameSW = queryResult.ToString();
                    }
                }
                cmd.Dispose();
            }
            return names;
        }
    }
}