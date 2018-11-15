﻿using TabScore.Models;
using System.Web.Mvc;

namespace TabScore.Controllers
{
    public class EnterLeadController : Controller
    {
        public ActionResult Index(string secondPass)
        {
            ResultClass res = new ResultClass()
            {
                ContractLevel = Session["ContractLevel"].ToString(),
                ContractSuit = Session["ContractSuit"].ToString(),
                ContractX = Session["ContractX"].ToString(),
                NSEW = Session["NSEW"].ToString()
            };
            ViewData["DisplayContract"] = res.DisplayContract(1);

            ViewData["Board"] = Session["Board"];
            ViewBag.Header = $"Table {Session["SectionLetter"]}{Session["Table"]} - Round {Session["Round"]} - {Vulnerability.SetPairString("NS", Session["Board"].ToString(), Session["PairNS"].ToString())} v {Vulnerability.SetPairString("EW", Session["Board"].ToString(), Session["PairEW"].ToString())}";
            ViewData["CancelButton"] = "TRUE";
            ViewData["SecondPass"] = secondPass;
            return View(); 
        }

        public ActionResult OKButtonClick(string card, string secondPass)
        {
            if (secondPass == "FALSE")
            {
                if (HandRecord.ValidateLead(Session["DBConnectionString"].ToString(), Session["SectionID"].ToString(), Session["Board"].ToString(), card, Session["NSEW"].ToString()))
                {
                    Session["LeadCard"] = card;
                    return RedirectToAction("Index", "EnterTricksTaken");
                }
                else
                {
                    return RedirectToAction("Index", "EnterLead", new { secondPass = "TRUE" });
                }
            }
            else
            {
                Session["LeadCard"] = card;
                return RedirectToAction("Index", "EnterTricksTaken");
            }
        }

        public ActionResult CancelButtonClick()
        {
            return RedirectToAction("Index", "EnterContract", new { board = Session["Board"].ToString() } );
        }

        public ActionResult ControlButtonClick()
        {
            Session["ControlReturnScreen"] = "ShowBoards";
            return RedirectToAction("Index", "ControlMenu");
        }

    }
}