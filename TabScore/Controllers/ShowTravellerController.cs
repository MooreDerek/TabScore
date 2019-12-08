﻿using System;
using System.Web.Mvc;
using TabScore.Models;

namespace TabScore.Controllers
{
    public class ShowTravellerController : Controller
    {
        public ActionResult Index()
        {
            string DBConnectionString = Session["DBConnectionString"].ToString();
            if (DBConnectionString == "") return RedirectToAction("Index", "ErrorScreen");

            if (!new Settings(DBConnectionString).ShowResults)
            {
                return RedirectToAction("Index", "ShowBoards");
            }

            Result result = Session["Result"] as Result;
            Section section = Session["Section"] as Section;
            Traveller traveller = new Traveller(DBConnectionString, section.ID, result.BoardNumber, result.PairNS, Convert.ToBoolean(Session["IndividualEvent"]));
            ViewData["BackButton"] = "TRUE";
            return View(traveller);
        }

        public ActionResult OKButtonClick()
        {
            return RedirectToAction("Index", "ShowBoards");
        }

        public ActionResult BackButtonClick()
        {
            return RedirectToAction("Index", "ConfirmResult");
        }

        public ActionResult HandRecordButtonClick()
        {
            return RedirectToAction("Index", "ShowHandRecord");
        }
    }
}
