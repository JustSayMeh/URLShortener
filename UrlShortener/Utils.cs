using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener
{
    public class Utils
    {
        /// <summary>
        /// Метод возвращает протокол с хостом
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetRequestURLHead(HttpRequest request)
        {
            string domainName = request.Host.Value;
            if (request.IsHttps)
                domainName = "https://" + domainName;
            else
                domainName = "http://" + domainName;
            return domainName;
        }
    }
}
