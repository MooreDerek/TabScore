﻿// TabScore - TabScore, a wireless bridge scoring program.  Copyright(C) 2020 by Peter Flippant
// Licensed under the Apache License, Version 2.0; you may not use this file except in compliance with the License

using System.Web.Mvc;
using TabScore.Models;

namespace TabScore.Controllers
{
    public class ErrorScreenController : Controller
    {
        public ActionResult Index()
        {
            Session["Header"] = "";
            ViewData["BackButton"] = "FALSE";
            return View();
        }

        public ActionResult OKButtonClick()
        {
            AppData.Refresh();
            if (AppData.DBConnectionString == "")
            {
                TempData["warningMessage"] = "Scoring database not yet started";
                return RedirectToAction("Index", "StartScreen");
            }
            else
            {
                Settings.Refresh();
                return RedirectToAction("Index", "EnterSection");
            }
        }
    }
}