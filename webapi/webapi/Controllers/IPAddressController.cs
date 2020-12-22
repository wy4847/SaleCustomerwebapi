using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using webapi.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;

namespace webapi.Controllers
{
    public class IPAddressController : ApiController
    {
        private readonly BLL.HN_Customer bll = new BLL.HN_Customer();
        private static IList<Address> addresses = new List<Address>
        {
            new Address(){ IPAddress="1.91.38.31", Province="北京市", City="北京市" },
            new Address(){ IPAddress = "210.75.225.254", Province = "上海市", City = "上海市"  },
        };
        [HttpGet]
        public IEnumerable<Address> GetIPAddresses()
        {
            return addresses;
        }
        [HttpGet]
        public Address GetIPAddressByIP(string IP)
        {
            return addresses.FirstOrDefault(x => x.IPAddress == IP);
        }
        [HttpGet]
        public DataTable GetSumPassenger(string endTime)
        {
            endTime = DateTime.Now.ToString();
            //endTime = "2019-12-24 22:00";
            return bll.GetSumPassenger(endTime);
        }
        /// <summary>
        /// 去年同期客流
        /// </summary>
        [HttpGet]
        public DataTable GetLastYearTodayCustomer()
        {
            DateTime dt = DateTime.Now;
            string endTime = dt.AddYears(-1).ToString();
            //endTime = "2019-12-24 22:00";
            return bll.GetSumPassenger(endTime);
        }
        [HttpGet]
        public DataTable YesterdayImmediateCustomer()
        {
            return bll.YesterdayImmediateCustomer();
        }
        [HttpGet]
        public DataTable Before7DayAndTodayImmediateCustomer()
        {
            return bll.Before7DayAndTodayImmediateCustomer();
        }
        [HttpGet]
        public DataTable TwoMonthCustomer()
        {
            return bll.TwoMonthCustomer();
        }
        [HttpGet]
        public DataTable TodayCustomer()
        {
            return bll.TodayCustomer();
        }
        [HttpGet]
        public DataTable TodayCustomer_fuxin()
        {
            return bll.TodayCustomer_fuxin();
        }
        [HttpGet]
        public DataTable TodayCustomer_chaoyang()
        {
            return bll.TodayCustomer_chaoyang();
        }
    }
}
