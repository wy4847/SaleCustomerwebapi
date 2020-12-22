using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace webapi.Models
{
    public class Address : ApiController
    {
        public string IPAddress { get; set; }
        public string Province { get; set; }
        public string City { get; set; }

    }
   
}
