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
        private readonly SQLiteDbContext db = new SQLiteDbContext();
        [HttpGet("/")]
        public ActionResult Index()
        {
            return View("Views/Home/Index.cshtml");
        }

        [HttpGet("/{shorturl}")]
        public RedirectResult generatedLinks(string shorturl)
        {
            Link link = db.Links.FirstOrDefault(i => i.Short.Equals(shorturl));
            if (link == null)
                return Redirect("/");
            return Redirect(link.Original);
        }
    }
}
