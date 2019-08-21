﻿using System;
using System.Data.Odbc;
using System.Text;

namespace TabScore.Models
{
    public static class HandRecord
    {
        public static HandRecordClass GetHandRecord(string DB, string sectionID, string board)
        {
            HandRecordClass hr = new HandRecordClass()
            {
                NorthSpades = "###",
                EvalNorthSpades = "###"
            };

            using (OdbcConnection connection = new OdbcConnection(DB))
            {
                string SQLString = $"SELECT NorthSpades, NorthHearts, NorthDiamonds, NorthClubs, EastSpades, EastHearts, EastDiamonds, EastClubs, SouthSpades, SouthHearts, SouthDiamonds, SouthClubs, WestSpades, WestHearts, WestDiamonds, WestClubs FROM HandRecord WHERE Section={sectionID} AND Board={board}";
                OdbcCommand cmd = new OdbcCommand(SQLString, connection);
                connection.Open();
                try
                {
                    ODBCRetryHelper.ODBCRetry(() =>
                    {
                        OdbcDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            hr.NorthSpades = reader.GetString(0);
                            hr.NorthHearts = reader.GetString(1);
                            hr.NorthDiamonds = reader.GetString(2);
                            hr.NorthClubs = reader.GetString(3);
                            hr.EastSpades = reader.GetString(4);
                            hr.EastHearts = reader.GetString(5);
                            hr.EastDiamonds = reader.GetString(6);
                            hr.EastClubs = reader.GetString(7);
                            hr.SouthSpades = reader.GetString(8);
                            hr.SouthHearts = reader.GetString(9);
                            hr.SouthDiamonds = reader.GetString(10);
                            hr.SouthClubs = reader.GetString(11);
                            hr.WestSpades = reader.GetString(12);
                            hr.WestHearts = reader.GetString(13);
                            hr.WestDiamonds = reader.GetString(14);
                            hr.WestClubs = reader.GetString(15);
                        }
                        reader.Close();
                    });
                }
                catch (OdbcException e)
                {
                    if (e.Errors.Count == 1 && e.Errors[0].SQLState != "42S02")  // HandRecord table does not exist
                    {
                        return hr;
                    }
                    else
                    {
                        hr.NorthSpades = "Error";
                        return hr;
                    }
                }
                finally
                {
                    cmd.Dispose();
                }

                SQLString = $"SELECT NorthSpades, NorthHearts, NorthDiamonds, NorthClubs, NorthNotrump, EastSpades, EastHearts, EastDiamonds, EastClubs, EastNotrump, SouthSpades, SouthHearts, SouthDiamonds, SouthClubs, SouthNotrump, WestSpades, WestHearts, WestDiamonds, WestClubs, WestNoTrump, NorthHcp, EastHcp, SouthHcp, WestHcp FROM HandEvaluation WHERE Section={sectionID} AND Board={board}";
                cmd = new OdbcCommand(SQLString, connection);
                try
                {
                    ODBCRetryHelper.ODBCRetry(() =>
                    {
                        OdbcDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            if (reader.GetInt16(0) > 6) hr.EvalNorthSpades = (reader.GetInt16(0) - 6).ToString(); else hr.EvalNorthSpades = "";
                            if (reader.GetInt16(1) > 6) hr.EvalNorthHearts = (reader.GetInt16(1) - 6).ToString(); else hr.EvalNorthHearts = "";
                            if (reader.GetInt16(2) > 6) hr.EvalNorthDiamonds = (reader.GetInt16(2) - 6).ToString(); else hr.EvalNorthDiamonds = "";
                            if (reader.GetInt16(3) > 6) hr.EvalNorthClubs = (reader.GetInt16(3) - 6).ToString(); else hr.EvalNorthClubs = "";
                            if (reader.GetInt16(4) > 6) hr.EvalNorthNT = (reader.GetInt16(4) - 6).ToString(); else hr.EvalNorthNT = "";
                            if (reader.GetInt16(5) > 6) hr.EvalEastSpades = (reader.GetInt16(5) - 6).ToString(); else hr.EvalEastSpades = "";
                            if (reader.GetInt16(6) > 6) hr.EvalEastHearts = (reader.GetInt16(6) - 6).ToString(); else hr.EvalEastHearts = "";
                            if (reader.GetInt16(7) > 6) hr.EvalEastDiamonds = (reader.GetInt16(7) - 6).ToString(); else hr.EvalEastDiamonds = "";
                            if (reader.GetInt16(8) > 6) hr.EvalEastClubs = (reader.GetInt16(8) - 6).ToString(); else hr.EvalEastClubs = "";
                            if (reader.GetInt16(9) > 6) hr.EvalEastNT = (reader.GetInt16(9) - 6).ToString(); else hr.EvalEastNT = "";
                            if (reader.GetInt16(10) > 6) hr.EvalSouthSpades = (reader.GetInt16(10) - 6).ToString(); else hr.EvalSouthSpades = "";
                            if (reader.GetInt16(11) > 6) hr.EvalSouthHearts = (reader.GetInt16(11) - 6).ToString(); else hr.EvalSouthHearts = "";
                            if (reader.GetInt16(12) > 6) hr.EvalSouthDiamonds = (reader.GetInt16(12) - 6).ToString(); else hr.EvalSouthDiamonds = "";
                            if (reader.GetInt16(13) > 6) hr.EvalSouthClubs = (reader.GetInt16(13) - 6).ToString(); else hr.EvalSouthClubs = "";
                            if (reader.GetInt16(14) > 6) hr.EvalSouthNT = (reader.GetInt16(14) - 6).ToString(); else hr.EvalSouthNT = "";
                            if (reader.GetInt16(15) > 6) hr.EvalWestSpades = (reader.GetInt16(15) - 6).ToString(); else hr.EvalWestSpades = "";
                            if (reader.GetInt16(16) > 6) hr.EvalWestHearts = (reader.GetInt16(16) - 6).ToString(); else hr.EvalWestHearts = "";
                            if (reader.GetInt16(17) > 6) hr.EvalWestDiamonds = (reader.GetInt16(17) - 6).ToString(); else hr.EvalWestDiamonds = "";
                            if (reader.GetInt16(18) > 6) hr.EvalWestClubs = (reader.GetInt16(18) - 6).ToString(); else hr.EvalWestClubs = "";
                            if (reader.GetInt16(19) > 6) hr.EvalWestNT = (reader.GetInt16(19) - 6).ToString(); else hr.EvalWestNT = "";

                            hr.HCPNorth = reader.GetInt16(20).ToString();
                            hr.HCPEast = reader.GetInt16(21).ToString();
                            hr.HCPSouth = reader.GetInt16(22).ToString();
                            hr.HCPWest = reader.GetInt16(23).ToString();
                        }
                        reader.Close();
                    });
                }
                catch (OdbcException e)
                {
                    if (e.Errors.Count > 1 || e.Errors[0].SQLState != "42S02")  // Error other than HandEvaluation table does not exist
                    {
                        hr.NorthSpades = "Error";
                        return hr;
                    }
                }
                finally
                {
                    cmd.Dispose();
                }
            }
            return hr;
        }

        public static bool ValidateLead(string DB, string sectionID, string board, string card, string NSEW)
        {
            if (card == "SKIP") return true;
            if (card.Substring(1,1) == "1")    // Account for different representations of '10'
            {
                card = card.Substring(0, 1) + "T";
            }
            StringBuilder SQLString = new StringBuilder();
            SQLString.Append("SELECT ");
            switch (NSEW)
            {
                case "N":
                    SQLString.Append("East");
                    break;
                case "S":
                    SQLString.Append("West");
                    break;
                case "E":
                    SQLString.Append("South");
                    break;
                case "W":
                    SQLString.Append("North");
                    break;
            }
            switch (card.Substring(0, 1))
            {
                case "S":
                    SQLString.Append("Spades");
                    break;
                case "H":
                    SQLString.Append("Hearts");
                    break;
                case "D":
                    SQLString.Append("Diamonds");
                    break;
                case "C":
                    SQLString.Append("Clubs");
                    break;
            }
            SQLString.Append($" FROM HandRecord WHERE Section={sectionID} AND Board={board}");

            bool validateOk = true;
            using (OdbcConnection connection = new OdbcConnection(DB))
            {
                OdbcCommand cmd = new OdbcCommand(SQLString.ToString(), connection);
                connection.Open();
                try
                {
                    ODBCRetryHelper.ODBCRetry(() =>
                    {
                        object queryResult = cmd.ExecuteScalar();
                        if (queryResult == null)
                        {
                            validateOk = true;
                        }
                        else
                        {
                            string suitString = queryResult.ToString();
                            if (suitString.Contains(card.Substring(1, 1)))
                            {
                                validateOk = true;
                            }
                            else
                            {
                                validateOk = false;
                            }
                        }
                    });
                }
                catch (OdbcException)  // An error occurred, even after retries, so no validation possible 
                {
                    return true;
                }
                finally
                {
                    cmd.Dispose();
                }
            }
            return validateOk;
        }
    }
}