﻿// TabScore - TabScore, a wireless bridge scoring program.  Copyright(C) 2020 by Peter Flippant
// Licensed under the Apache License, Version 2.0; you may not use this file except in compliance with the License

using System.Web.Mvc;
using TabScore.Models;

namespace TabScore.Controllers
{
    public class ShowFinalRankingListController : Controller
    {
        public ActionResult Index()
        {
            Round round = Session["Round"] as Round;

            RankingList rankingList = new RankingList((Session["SessionData"] as SessionData).SectionID);
            if (rankingList != null && rankingList.Count != 0 && rankingList[0].Score != "     0" && rankingList[0].Score != "50")
            {
                rankingList.PairNS = round.PairNS;
                rankingList.PairEW = round.PairEW;
                ViewData["BackButton"] = "REFRESH";
                if (AppData.IsIndividual)
                {
                    rankingList.South = round.South;
                    rankingList.West = round.West;
                    return View("Individual", rankingList);
                }
                else if (rankingList.Exists(x => x.Orientation == "E"))
                {
                    return View("TwoWinners", rankingList);
                }
                else
                {
                    return View("OneWinner", rankingList);
                }
            }

            return RedirectToAction("Index", "EndScreen");
        }

        public ActionResult OKButtonClick()
        {
            return RedirectToAction("Index", "EndScreen");
        }

        public ActionResult RefreshButtonClick()
        {
            return RedirectToAction("Index", "ShowFinalRankingList");
        }
    }
}