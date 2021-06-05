using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener
{
    public class Response
    {
        public string M1 { get; set; }
        public string M2 { get; set; }
        public Response(string m1, string m2)
        {
            this.M1 = m1;
            this.M2 = m2;
        }
    }
}
