using HashidsNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UrlShortener.Controllers
{

    [Route("[controller]/[action]")]
    public class APIController : Controller
    {
        private readonly string prefix = "X";
        private readonly string salt = "HkdfdjAler\\dsf";
        private readonly int hash_size = 7;
        private readonly string alphabet58 = "123456789abcdefghijkmnopqrstuvwxyz";
        private readonly SQLiteDbContext db = new SQLiteDbContext();
        private Regex regex = new Regex(@"^((https:)|(http:)\/\/)?[а-яa-z0-9_\/%\-]+(\.[а-яa-z%\-]+)+(\/[а-яa-z0-9_\/%\-\.]*)?(\?[а-яa-z0-9_\/%\&=\-\.\!]*)?$");
        public JsonResult Create(string q) 
        {
            Hashids hashids = new Hashids(salt, hash_size, alphabet58);
            Match m = regex.Match(q);
            if (!m.Success)
            {
                this.HttpContext.Response.StatusCode = 400;
                return Json(new Response("invalid link", ""));
            }

            if (m.Groups[1].Length == 0)
                q = "https://" + q;
            
            Link flink = db.Links.FirstOrDefault(it => it.Original.Equals(q));
            if (flink == null)
            {
                string hash = hashids.EncodeLong(db.Links.Count() + 1);
                Link url = new Link(q, hash);
                db.Add(url);
                db.SaveChanges();
                return Json(new Response(prefix + hash, q));
            }
            return Json(new Response(prefix + flink.Short, flink.Original));
        }
    }
}
