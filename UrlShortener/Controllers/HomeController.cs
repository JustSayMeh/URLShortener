using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener.Controllers
{
    [Route("/")]
    public partial class HomeController : Controller
    {

        private readonly SQLiteDbContext db;
        public HomeController(SQLiteDbContext db) => this.db = db;
       

        [HttpGet("/")]
        public ActionResult Index()
        {
            ViewData["Active"] = "main";
            return View("Views/Home/Index.cshtml");
        }

        [HttpGet("/links")]
        public ActionResult Links()
        {
            SetViewData();
            return View("Views/Links/LinksPage.cshtml", links);
        }

        [HttpGet("/X{shorturl}")]
        public RedirectResult generatedLinks(string shorturl)
        {
            Link link = db.Links.FirstOrDefault(i => i.Short.Equals(shorturl));
            if (link == null)
                return Redirect("/error?code=404");
            UpdateLinkRedirectTime(link);
            UpdateDatabase(link);
            return Redirect(link.Original);
        }

        [HttpGet("/error")]
        public ActionResult error(string code) => View("Views/Error/Error.cshtml", code);
    }
}
