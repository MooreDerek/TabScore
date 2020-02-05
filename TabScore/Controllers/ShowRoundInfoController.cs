﻿using System.Web.Mvc;
using TabScore.Models;

namespace TabScore.Controllers
{
    public class ShowRoundInfoController : Controller
    {
        public ActionResult Index()
        {
            string DBConnectionString = Session["DBConnectionString"].ToString();
            if (DBConnectionString == "") return RedirectToAction("Index", "ErrorScreen");

            Round round = Session["Round"] as Round;
            if (round.RoundNumber == 1)
            {
                ViewData["BackButton"] = "FALSE";
            }
            else 
            {
                ViewData["BackButton"] = "TRUE";
            }

            SessionData sessionData = Session["SessionData"] as SessionData;
            Session["Header"] = $"Table {sessionData.SectionTableString}";

            if (round.PairNS == 0 || round.PairNS == sessionData.MissingPair)
            {
                if (sessionData.IsIndividual)
                {
                   return View("NSMissingIndividual", round);
                }
                else
                {
                    return View("NSMissing", round);
                }
            }
            else if (round.PairEW == 0 || round.PairEW == sessionData.MissingPair)
            {
                if (sessionData.IsIndividual)
                {
                   return View("EWMissingIndividual", round);
                }
                else
                {
                    return View("EWMissing", round);
                }
            }
            else
            {
                if (sessionData.IsIndividual)
                {
                    return View("Individual", round);
                }
                else
                {
                   return View("Pair", round);
                }
            }
        }

        public ActionResult OKButtonClick()
        {
            return RedirectToAction("Index", "ShowBoards");
        }

        public ActionResult OKSitOutButtonClick()
        {
            return RedirectToAction("Index", "ShowRankingList");
        }

        public ActionResult BackButtonClick()
        {
            string DBConnectionString = Session["DBConnectionString"].ToString();
            if (DBConnectionString == "") return RedirectToAction("Index", "ErrorScreen");

            // Reset to the previous round; RoundNumber > 1 else no Back button and cannot get here
            int roundNumber =(Session["Round"] as Round).RoundNumber;
            SessionData sessionData = Session["SessionData"] as SessionData;
            Session["Round"] = new Round(DBConnectionString, sessionData.SectionID, sessionData.TableNumber, roundNumber - 1, sessionData.IsIndividual);
            return RedirectToAction("Index", "ShowMove", new { newRoundNumber = roundNumber });
        }
    }
}