using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace webapi.Models
{
    public class MesOpenid : ApiController
    {
        public string OpenId { get; set; }
        public string Session_Key { get; set; }
        public string MsgCode { get; set; }
    }
}