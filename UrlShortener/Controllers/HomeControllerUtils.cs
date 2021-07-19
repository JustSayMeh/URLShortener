using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener.Controllers
{
    public partial class HomeController
    {
        private List<Link> links;

        private void UpdateDatabase(Link link)
        {
            db.Links.Update(link);
            db.SaveChanges();
        }
        private void UpdateLinkRedirectTime(Link link) => link.LastRedirectTime = DateTime.Now;

        private string GetRequestLinks()
        {
            string cookies = "";
            HttpContext.Request.Cookies.TryGetValue("links", out cookies);
            return cookies;
        }

        private void SetViewData()
        {
            string cookies_links = GetRequestLinks();
            string domainName = Utils.GetRequestURLHead(HttpContext.Request);
            links = db.Links.Where(p => cookies_links.Contains(p.Short)).ToList();
            ViewData["Active"] = "links";
            ViewData["Head"] = domainName;
        }
    }
}
