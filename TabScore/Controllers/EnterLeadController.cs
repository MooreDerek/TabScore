﻿using TabScore.Models;
using System.Web.Mvc;

namespace TabScore.Controllers
{
    public class EnterLeadController : Controller
    {
        public ActionResult Index(string secondPass)
        {
            string DBConnectionString = Session["DBConnectionString"].ToString();
            if (DBConnectionString == "") return RedirectToAction("Index", "ErrorScreen");

            if (!Settings.GetSetting<bool>(DBConnectionString, SettingName.EnterLeadCard))
            {
                Session["LeadCard"] = "SKIP";
                return RedirectToAction("Index", "EnterTricksTaken");
            }

            ResultClass res = new ResultClass()
            {
                ContractLevel = Session["ContractLevel"].ToString(),
                ContractSuit = Session["ContractSuit"].ToString(),
                ContractX = Session["ContractX"].ToString(),
                NSEW = Session["NSEW"].ToString()
            };
            ViewData["DisplayContract"] = res.DisplayContract(2);

            if (Session["LeadCard"].ToString() == "")
            {
                ViewData["Suit"] = "";
                ViewData["Card"] = "";
            }
            else
            {
                ViewData["Suit"] = Session["LeadCard"].ToString().Substring(0, 1);
                ViewData["Card"] = Session["LeadCard"].ToString().Substring(1, 1);
            }

            ViewBag.Header = $"Table {Session["SectionLetter"]}{Session["Table"]} - Round {Session["Round"]} - {Vulnerability.SetPairString("NS", Session["Board"].ToString(), Session["PairNS"].ToString())} v {Vulnerability.SetPairString("EW", Session["Board"].ToString(), Session["PairEW"].ToString())}";
            ViewData["BackButton"] = "TRUE";
            ViewData["SecondPass"] = secondPass;
            return View();
        }

        public ActionResult OKButtonClick(string card, string secondPass)
        {
            string DBConnectionString = Session["DBConnectionString"].ToString();
            if (DBConnectionString == "") return RedirectToAction("Index", "ErrorScreen");

            if (!Settings.GetSetting<bool>(DBConnectionString, SettingName.ValidateLeadCard) || secondPass == "TRUE")
            {
                Session["LeadCard"] = card;
                return RedirectToAction("Index", "EnterTricksTaken");
            }
            else
            {
                if (HandRecord.ValidateLead(DBConnectionString, Session["SectionID"].ToString(), Session["Board"].ToString(), card, Session["NSEW"].ToString()))
                {
                    Session["LeadCard"] = card;
                    return RedirectToAction("Index", "EnterTricksTaken");
                }
                else
                {
                    return RedirectToAction("Index", "EnterLead", new { secondPass = "TRUE" });
                }
            }
        }

        public ActionResult BackButtonClick()
        {
            return RedirectToAction("Index", "EnterContract", new { board = Session["Board"].ToString() } );
        }
    }
}