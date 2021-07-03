using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener.Controllers
{
    [Route("/")]
    public class HomeController : Controller
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
            string cookies = "";
            HttpContext.Request.Cookies.TryGetValue("links", out cookies);
            string domainName = Utils.GetRequestURLHead(HttpContext.Request);
            List<Link> links = db.Links.Where(p => cookies.Contains(p.Short)).ToList();
            ViewData["Active"] = "links";
            ViewData["Head"] = domainName;
            return View("Views/Links/LinksPage.cshtml", links);
        }

        [HttpGet("/X{shorturl}")]
        public RedirectResult generatedLinks(string shorturl)
        {
            Link link = db.Links.FirstOrDefault(i => i.Short.Equals(shorturl));
            if (link == null)
                return Redirect("/error?code=404");
            link.LastRedirectTime = DateTime.Now;
            db.Links.Update(link);
            db.SaveChanges();
            return Redirect(link.Original);
        }

        [HttpGet("/error")]
        public ActionResult error(string code)
        {
            return View("Views/Error/Error.cshtml", code);
        }
    }
}
