﻿using System;
using System.Data.Odbc;

namespace TabScore.Models
{
    public static class ODBCRetryHelper
    {
        public static void ODBCRetry(Action cmd)
        {
            int attempts = 3;
            do
            {
                try
                {
                    attempts--;
                    cmd();
                    break;
                }
                catch (OdbcException e)
                {
                    if (e.Errors.Count == 1 && (e.Errors[0].SQLState != "42S02" || e.Errors[0].SQLState != "42S22"))  throw e;   // Table or column does not exist
                    if (attempts <= 0) throw e;
                    System.Threading.Thread.Sleep(700);
                }
            } while (true);
        }
    }

}