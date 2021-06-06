﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly SQLiteDbContext db = new SQLiteDbContext();
        [HttpGet("/")]
        public ActionResult Index()
        {
            ViewData["Active"] = "main";
            return View("Views/Home/Index.cshtml");
        }

        [HttpGet("/links")]
        public ActionResult Links()
        {
            string cookies = "";
            HttpContext.Request.Cookies.TryGetValue("links", out cookies);
            List<Link> links = db.Links.Where(p => cookies.Contains(p.Short)).ToList();
            ViewData["Active"] = "links";
            return View("Views/Links/LinksPage.cshtml", links); 
        }

        [HttpGet("/X{shorturl}")]
        public RedirectResult generatedLinks(string shorturl)
        {
            Link link = db.Links.FirstOrDefault(i => i.Short.Equals(shorturl));
            if (link == null)
                return Redirect("/");
            return Redirect(link.Original);
        }
    }
}
