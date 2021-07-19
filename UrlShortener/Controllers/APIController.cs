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
    public partial class APIController : Controller
    {
        private readonly SQLiteDbContext db;
        public APIController(SQLiteDbContext db) => this.db = db;
        private string original_url;

        [HttpPost]
        public JsonResult Create([FromBody] URLRequest request_url_argument) 
        {
            if (IsArgumentEmpty(request_url_argument))
                ProvideErrorResponseWithCode400("invalid request");
            SetOriginalUrlFromRequest(request_url_argument);
            CheckAndAddHttpsPrefix();
            try
            {
                return ShortenUrlAndProvideOkResponse();
            }
            catch(UriFormatException e)
            {
                return ProvideErrorResponseWithCode400("invalid link");
            }
        }
    }
}
