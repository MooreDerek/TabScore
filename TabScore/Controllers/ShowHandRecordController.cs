﻿using System.Web.Mvc;
using TabScore.Models;

namespace TabScore.Controllers
{
    public class ShowHandRecordController : Controller
    {
        public ActionResult Index()
        {
            int boardNumber = (Session["Result"] as Result).BoardNumber;
            int sectionID = (Session["SessionData"] as SessionData).SectionID;

            HandRecord handRecord = new HandRecord(sectionID, boardNumber);
            if (handRecord.NorthSpades == "###" && sectionID != 1)    // Use default Section 1 hand records)
            {
                handRecord = new HandRecord(1, boardNumber);
            }

            ViewData["BackButton"] = "FALSE";
            return View(handRecord);
        }

        public ActionResult OKButtonClick()
        {
            return RedirectToAction("Index", "ShowTraveller");
        }
    }
}