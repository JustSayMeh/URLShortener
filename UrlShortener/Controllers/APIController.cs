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
    public class URLRequest
    {
        public string url { get; set; }
    }
    [Route("[controller]/[action]")]
    public class APIController : Controller
    {
        private readonly int hash_size = 7;
        private readonly string prefix = "X";
        private readonly string salt = "HkdfdjAler\\dsf";
        // base58 алфавит без символов в верхнем регистре
        private readonly string alphabet58 = "123456789abcdefghijkmnopqrstuvwxyz";
        private Regex regex = new Regex(@"^(?:http(s)?:\/\/)?([а-яА-ЯA-Za-z0-9_.-]+(?:\.[а-яА-ЯA-Za-z0-9_\.-]+))+[а-яА-ЯA-Za-z0-9_\-\._~:\/?#[\]@!\$&'\(\)\*\+,;=.%]+$");

        private readonly SQLiteDbContext db;
        public APIController(SQLiteDbContext db) => this.db = db;
        [HttpPost]
        // FromBody - чтобы обработать аргументы тела запроса
        public JsonResult Create([FromBody] URLRequest q1) 
        {
            if (q1 == null)
            {
                this.HttpContext.Response.StatusCode = 400;
                return Json(new Response("invalid request", ""));
            }
            string q = q1.url;
            // валидация переданного url
            Hashids hashids = new Hashids(salt, hash_size, alphabet58);
            Match m = regex.Match(q);
            // получить хост
            string domain = m.Groups[2].Value;
            if (!m.Success)
            {
                this.HttpContext.Response.StatusCode = 400;
                return Json(new Response("invalid link", ""));
            }
            // так как ссылки без http допустимы, то проверяем его наличие и добавляем в случае отсутствия
            if (!q.Contains("http"))
                q = "https://" + q;
            // удаляем www для однородности ссылок
            q = q.Replace(domain, domain.ToLower()).Replace("://www.", "://");
            // Получаем доменное имя с протоколом
            string domainName = Utils.GetRequestURLHead(HttpContext.Request);
            // получаем ссылку из бд
            Link flink = db.Links.FirstOrDefault(it => it.Original.Equals(q));
            // Если ссылка не найдена, то создаем её и добавляем в бд
            if (flink == null)
            {
                string hash = hashids.EncodeLong(db.Links.Count() + 1);
                Link url = new Link(q, hash);
                db.Add(url);
                db.SaveChanges();
                return Json(new Response(domainName + "/" + prefix + hash, q));
            }
            return Json(new Response(domainName + "/" + prefix + flink.Short, flink.Original));
        }
    }
}
