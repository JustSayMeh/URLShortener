using HashidsNet;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener.Controllers
{
    public partial class APIController
    {
        private readonly int hash_size = 7;
        private readonly string short_url_prefix = "X";
        private readonly string salt = "HkdfdjAler\\dsf";
        // base58 алфавит без символов в верхнем регистре
        private readonly string alphabet58 = "123456789abcdefghijkmnopqrstuvwxyz";

        private bool IsArgumentEmpty(object argument) => argument == null;
        private bool IsHttpPrefixContains(string url) => url.Contains("http");
        private void SetOriginalUrlFromRequest(URLRequest request_url_argument) => original_url = request_url_argument.url;
        private JsonResult ProvideResponsewithShortenUrl(string server_domain, string short_url, string local_original_url) => Json(new Response(server_domain + "/" + short_url_prefix + short_url, local_original_url));

        private void CheckAndAddHttpsPrefix()
        {
            if (!IsHttpPrefixContains(original_url))
                original_url = "https://" + original_url;

        }

        private JsonResult ProvideErrorResponseWithCode400(string error_response_message)
        {
            this.HttpContext.Response.StatusCode = 400;
            return Json(new Response(error_response_message, ""));
        }


        private void RemoveWWWIfExists()
        {
            Uri uri = new Uri(original_url);
            string original_url_domain = uri.Host;
            original_url = original_url.Replace(original_url_domain, original_url_domain.ToLower()).Replace("://www.", "://");
        }

        private string GetShortenUrl()
        {
            Hashids hashids = new Hashids(salt, hash_size, alphabet58);
            return hashids.EncodeLong(db.Links.Count() + 1);
        }

        private void AddUrlToDatabase(string hash)
        {
            Link url = new Link(original_url, hash);
            db.Add(url);
            db.SaveChanges();
        }

        private JsonResult ShortenUrlAndProvideOkResponse()
        {

            RemoveWWWIfExists();
            string server_domain = Utils.GetRequestURLHead(HttpContext.Request);
            Link flink = db.Links.FirstOrDefault(it => it.Original.Equals(original_url));
            if (flink == null)
            {
                string hash = GetShortenUrl();
                AddUrlToDatabase(hash);
                return ProvideResponsewithShortenUrl(server_domain, hash, original_url);
            }
            return ProvideResponsewithShortenUrl(server_domain, flink.Short, flink.Original);
        }
    }
}
