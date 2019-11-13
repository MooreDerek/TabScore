﻿using System.Collections.Generic;
using System.Web.Mvc;
using TabScore.Models;

namespace TabScore.Controllers
{
    public class EnterSectionController : Controller
    {
        public ActionResult Index()
        {
            string DBConnectionString = Session["DBConnectionString"].ToString();
            if (DBConnectionString == "") return RedirectToAction("Index", "ErrorScreen");

            List<Section> sectionsList = SectionsList.GetSections(DBConnectionString);
            if (sectionsList == null) return RedirectToAction("Index", "ErrorScreen");

            // Check if only one section - if so use it
            if (sectionsList.Count == 1)
            {
                Session["SectionLetter"] = sectionsList[0].Letter;
                Session["SectionID"] = sectionsList[0].ID;
                Session["NumTables"] = sectionsList[0].Tables;
                Session["MissingPair"] = sectionsList[0].MissingPair;
                return RedirectToAction("Index", "EnterTableNumber");
            }
            else
            // Get Section
            {
                ViewData["BackButton"] = "FALSE";
                return View(sectionsList);
            }
        }

        public ActionResult OKButtonClick(string sectionLetter)
        {
            string DBConnectionString = Session["DBConnectionString"].ToString();
            if (DBConnectionString == "") return RedirectToAction("Index", "ErrorScreen");

            List<Section> sectionsList = SectionsList.GetSections(DBConnectionString);
            if (sectionsList == null) return RedirectToAction("Index", "ErrorScreen");

            Session["SectionLetter"] = sectionLetter;
            Section section = sectionsList.Find(x => x.Letter == sectionLetter);
            Session["SectionID"] = section.ID;
            Session["NumTables"] = section.Tables;
            Session["MissingPair"] = section.MissingPair;
            return RedirectToAction("Index", "EnterTableNumber");
        }
    }
}