﻿using System.Data.Odbc;
using System.Web.Mvc;

namespace TabScore.Controllers
{
    public class StartScreenController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Header = "";
            ViewData["CancelButton"] = "FALSE";
            return View();
        }

        public ActionResult OKButtonClick()
        {
            // Check if DB connection has been set up
            string pathToTabScoreDB = System.Environment.ExpandEnvironmentVariables(@"%Public%\TabScore\TabScoreDB.txt");
            string pathToDB = "";
            if (System.IO.File.Exists(pathToTabScoreDB))
            {
                pathToDB = System.IO.File.ReadAllText(pathToTabScoreDB);
            }
            if (pathToDB == "")
            {
                TempData["alertMessage"] = "Scoring database not yet started";
                return RedirectToAction("Index", "StartScreen");
            }
            else
            {
                // Check that we can open the DB
                OdbcConnectionStringBuilder cs = new OdbcConnectionStringBuilder();
                cs.Driver = "Microsoft Access Driver (*.mdb)";
                cs.Add("Dbq", pathToDB);
                cs.Add("Uid", "Admin");
                using (OdbcConnection connection = new OdbcConnection(cs.ToString()))
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (OdbcException e)
                    {
                        TempData["alertMessage"] = e.Message;
                        return RedirectToAction("Index", "StartScreen");
                    }
                }
                Session["DBConnectionString"] = cs.ToString();

                return RedirectToAction("Index", "EnterSection");
            }
        }

        public ActionResult ControlButtonClick()
        {
            Session["ControlReturnScreen"] = "StartScreen";
            return RedirectToAction("Index", "ControlMenu");
        }
    }
}