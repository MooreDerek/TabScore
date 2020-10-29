﻿// TabScore - TabScore, a wireless bridge scoring program.  Copyright(C) 2020 by Peter Flippant
// Licensed under the Apache License, Version 2.0; you may not use this file except in compliance with the License

using System.Web.Mvc;
using TabScore.Models;

namespace TabScore.Controllers
{
    public class ShowFinalRankingListController : Controller
    {
        public ActionResult Index(int sectionID, int tableNumber)
        {
            TableStatus tableStatus = AppData.TableStatusList.Find(x => x.SectionID == sectionID && x.TableNumber == tableNumber);
            RankingList rankingList = new RankingList(tableStatus);

            // Only show the ranking list if it contains something meaningful
            if (rankingList == null || rankingList.Count == 0 || rankingList[0].ScoreDecimal == 0 || rankingList[0].ScoreDecimal == 50)
            {
                return RedirectToAction("Index", "EndScreen", new { sectionID, tableNumber });
            }
            else
            {
                rankingList.FinalRankingList = true;
                ViewData["Header"] = $"Table {tableStatus.SectionTableString} - Round {tableStatus.RoundData.RoundNumber}";
                ViewData["Title"] = $"Final Ranking List - {tableStatus.SectionTableString}";
                ViewData["ButtonOptions"] = ButtonOptions.OKEnabled;
                if (AppData.IsIndividual)
                {
                    return View("IndividualRankingList", rankingList);
                }
                else if (rankingList.Exists(x => x.Orientation == "E"))
                {
                    return View("TwoWinnersRankingList", rankingList);
                }
                else
                {
                    return View("OneWinnerRankingList", rankingList);
                }
            }
        }
    }
}