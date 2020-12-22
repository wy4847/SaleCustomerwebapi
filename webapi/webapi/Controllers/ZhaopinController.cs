using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using webapi.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;

using Newtonsoft.Json;

using Newtonsoft.Json.Linq;

using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Web.Mvc;
using Dal;

namespace webapi.Controllers
{
    public class ZhaopinController : ApiController
    {
        
        private readonly BLL.HN_Customer bll = new BLL.HN_Customer();
        /// <summary>
        /// 招聘信息系统
        /// </summary>
        [System.Web.Http.HttpPost]
        public int add_zhaopin_user([FromBody]zhaopin_person newperson)
        {
            return bll.add_zhaopin_user(newperson);
        }

     

    }

   

}