﻿// TabScore - TabScore, a wireless bridge scoring program.  Copyright(C) 2020 by Peter Flippant
// Licensed under the Apache License, Version 2.0; you may not use this file except in compliance with the License

using System.Collections.Generic;
using System.Data.Odbc;

namespace TabScore.Models
{
    public class ResultsList : List<Result>
    {
        public bool GotAllResults { get; private set; }
        public bool ShowViewButton { get; private set; }

        public ResultsList(SessionData sessionData, Round round)
        {
            GotAllResults = true;
            ShowViewButton = Settings.ShowResults;

            using (OdbcConnection connection = new OdbcConnection(AppData.DBConnectionString))
            {
                connection.Open();
                string SQLString;
                OdbcCommand cmd = null;
                OdbcDataReader reader = null;
                try
                {
                    SQLString = $"SELECT Board, [NS/EW], Contract, Result, Remarks FROM ReceivedData WHERE Section={sessionData.SectionID} AND [Table]={sessionData.TableNumber} AND Round={round.RoundNumber} AND Board>={round.LowBoard} AND Board<={round.HighBoard}";
                    cmd = new OdbcCommand(SQLString, connection);
                    ODBCRetryHelper.ODBCRetry(() =>
                    {
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Result result = new Result()
                            {
                                SectionID = sessionData.SectionID,
                                TableNumber = sessionData.TableNumber,
                                RoundNumber = round.RoundNumber,
                                BoardNumber = reader.GetInt32(0)
                            };
                            if (reader.GetString(4) == "Not played")
                            {
                                result.ContractLevel = -1;
                                result.ContractSuit = "";
                                result.ContractX = "NONE";
                                result.NSEW = "";
                                result.TricksTakenNumber = -1;
                            }
                            else
                            {
                                string temp = reader.GetString(2);
                                result.Contract = temp;   // Sets ContractLevel, etc
                                if (result.ContractLevel == 0)  // Passed out
                                {
                                    result.NSEW = "";
                                    result.TricksTakenNumber = -1;
                                }
                                else
                                {
                                    result.NSEW = reader.GetString(1);
                                    result.TricksTakenSymbol = reader.GetString(3);
                                }
                            }
                            Add(result);
                        }
                    }); 
                }
                finally
                {
                    reader.Close();
                    cmd.Dispose();
                }

                // Check to see if any boards don't have a result, and add dummies to the list
                for (int iBoard = round.LowBoard; iBoard <= round.HighBoard; iBoard++)
                {
                    if (Find(x => x.BoardNumber == iBoard) == null)
                    {
                        GotAllResults = false;
                        Result result = new Result
                        {
                            SectionID = sessionData.SectionID,
                            TableNumber = sessionData.TableNumber,
                            RoundNumber = round.RoundNumber,
                            BoardNumber = iBoard,
                            ContractLevel = -999,
                            ContractSuit = "",
                            ContractX = "NONE",
                            NSEW = "",
                            TricksTakenNumber = -1,
                            LeadCard = ""
                        };
                        Add(result);
                    }
                }
                Sort((x, y) => x.BoardNumber.CompareTo(y.BoardNumber));
            }
        }
    }
}