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
    public class PassengerFlowController : ApiController
    {
        private readonly BLL.HN_Customer bll = new BLL.HN_Customer();
        [System.Web.Http.HttpGet]
        public DataTable GetSumPassenger(string endTime)
        {
            endTime = DateTime.Now.ToString();
            //endTime = "2019-12-24 22:00";
            return bll.GetSumPassenger(endTime);
        }
        /// <summary>
        /// 去年同期客流
        /// </summary>
        [System.Web.Http.HttpGet]
        public DataTable GetLastYearTodayCustomer()
        {
            DateTime dt = DateTime.Now;
            string endTime = dt.AddYears(-1).ToString();
            //endTime = "2019-12-24 22:00";
            return bll.GetSumPassenger(endTime);
        }
        [System.Web.Http.HttpGet]
        public DataTable YesterdayImmediateCustomer()
        {
            return bll.YesterdayImmediateCustomer();
        }
        [System.Web.Http.HttpGet]
        public DataTable Before7DayAndTodayImmediateCustomer()
        {
            return bll.Before7DayAndTodayImmediateCustomer();
        }
        [System.Web.Http.HttpGet]
        public DataTable TwoMonthCustomer()
        {
            return bll.TwoMonthCustomer();
        }
        [System.Web.Http.HttpGet]
        public DataTable TodayCustomer()
        {
            return bll.TodayCustomer();
        }
        [System.Web.Http.HttpGet]
        public DataTable TodayCustomer_fuxin()
        {
            return bll.TodayCustomer_fuxin();
        }
        [System.Web.Http.HttpGet]
        public DataTable TodayCustomer_chaoyang()
        {
            return bll.TodayCustomer_chaoyang();
        }
        /// 今天7点及以后每30分钟客流情况

        [System.Web.Http.HttpGet]
        public DataTable TodayAm7and30mImmediateCustomer()
        {
            return bll.TodayAm7and30mImmediateCustomer();
        }
        /// <summary>
        /// 指定日期早7点及以后每隔30分钟客流情况
        /// </summary>
        /// <param name="searchDate"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayAm7and30mImmediateCustomer(string searchDate)
        {
            return bll.AnyDayAm7and30mImmediateCustomer(searchDate);
        }

        /// <summary>
        /// 指定日期早7点及以后每隔30分钟客流及合计情况
        /// </summary>
        /// <param name="searchDate"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayAm7and30mImmediateCustomerAndTotal(string searchDate)
        {
            return bll.AnyDayAm7and30mImmediateCustomerAndTotal(searchDate);
        }

        //1.两个结构一样的DT合并
        private DataTable MergeDataTable(DataTable dt1, DataTable dt2, string DTName)
        {
            
            DataTable newDataTable = dt1.Clone();

            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);
            }

            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                dt2.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);

            }
            newDataTable.TableName = DTName; //设置DT的名字
            return newDataTable;

        }

        //1.两个结构一样的DT合并
        private DataTable MergeDataTableRow(DataTable dt1, DataTable dt2, string DTName)
        {
            dt1.Columns.Add("tongqi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongbi", System.Type.GetType("System.String"));
            DataTable newDataTable = dt1.Clone();

            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i]["tongqi"] = dt2.Rows[i]["InSum"].ToString();
               
                double mynum = Convert.ToInt32(dt1.Rows[i]["InSum"]) - Convert.ToInt32(dt2.Rows[i]["InSum"]);
                double mynum2 = Convert.ToInt32(dt2.Rows[i]["InSum"]);
                double mynum3 = mynum / mynum2;
                dt1.Rows[i]["tongbi"] = Math.Round(mynum3*100, 2)+"%";
                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
               
                
               
                newDataTable.Rows.Add(obj);
            }

          
            newDataTable.TableName = DTName; //设置DT的名字
            return newDataTable;

        }

        /// <summary>
        /// 查询广场日级同比
        /// </summary>
        /// <returns></returns>
        private DataTable MergeDataTable_Plaza(DataTable dt1, DataTable dt2, string DTName)
        {
            dt1.Columns.Add("tongqi_date", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_shop_keliu", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_shop_keliu_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_shop_keliu_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_plaza_shopaccess", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_plaza_datetype", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_plaza_datetype2", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_plaza_important", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_plaza_marketing", System.Type.GetType("System.String"));
            DataTable newDataTable = dt1.Clone();

            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i]["tongqi_date"] = dt2.Rows[i]["DateKey"].ToString();
                dt1.Rows[i]["tongqi_keliu"] = dt2.Rows[i]["InSum"].ToString();
                int mynum = Convert.ToInt32(dt1.Rows[i]["InSum"]) - Convert.ToInt32(dt2.Rows[i]["InSum"]);
                //dt1.Rows[i]["tongqi_keliu_chae"] = Convert.ToInt32(dt1.Rows[i]["InSum"]);
                dt1.Rows[i]["tongqi_keliu_chae"] = mynum.ToString ();
                double mynum2 = Convert.ToInt32(dt2.Rows[i]["InSum"]);
                double mynum3 = mynum / mynum2;
                dt1.Rows[i]["tongqi_keliu_tongbi"] = Math.Round(mynum3 * 100, 2) + "%";

                dt1.Rows[i]["tongqi_sale"] = dt2.Rows[i]["plaza_sale"].ToString();
                double tongqi_sale_chae = Convert.ToDouble(dt1.Rows[i]["plaza_sale"]) - Convert.ToDouble(dt2.Rows[i]["plaza_sale"]);
                dt1.Rows[i]["tongqi_sale_chae"] = tongqi_sale_chae;
                double plaza_sale2 = Convert.ToDouble(dt2.Rows[i]["plaza_sale"]);
                double tongqi_keliu_tongbi = tongqi_sale_chae / plaza_sale2;
                dt1.Rows[i]["tongqi_sale_tongbi"] = Math.Round(tongqi_keliu_tongbi * 100, 2) + "%";
                

                dt1.Rows[i]["tongqi_shop_keliu"] = dt2.Rows[i]["plaza_shopsum"].ToString();
                double tongqi_shop_keliu_chae = Convert.ToDouble(dt1.Rows[i]["plaza_shopsum"]) - Convert.ToDouble(dt2.Rows[i]["plaza_shopsum"]);
                dt1.Rows[i]["tongqi_shop_keliu_chae"] = tongqi_shop_keliu_chae;
                double plaza_shopsum2 = Convert.ToDouble(dt2.Rows[i]["plaza_shopsum"]);
                double tongqi_shop_keliu_tongbi = tongqi_shop_keliu_chae / plaza_shopsum2;
                dt1.Rows[i]["tongqi_shop_keliu_tongbi"] = Math.Round(tongqi_shop_keliu_tongbi * 100, 2) + "%";

                dt1.Rows[i]["tongqi_plaza_shopaccess"] = dt2.Rows[i]["plaza_shopaccess"].ToString();
                dt1.Rows[i]["tongqi_plaza_datetype"] = dt2.Rows[i]["plaza_datetype"].ToString();
                dt1.Rows[i]["tongqi_plaza_datetype2"] = dt2.Rows[i]["plaza_datetype2"].ToString();
                dt1.Rows[i]["tongqi_plaza_important"] = dt2.Rows[i]["plaza_important"].ToString();
                dt1.Rows[i]["tongqi_plaza_marketing"] = dt2.Rows[i]["plaza_marketing"].ToString();

                dt1.Rows[i].ItemArray.CopyTo(obj, 0);



                newDataTable.Rows.Add(obj);
            }


            newDataTable.TableName = DTName; //设置DT的名字
            return newDataTable;

        }

        /// <summary>
        /// 查询广场区间同比
        /// </summary>
        /// <returns></returns>
        private DataTable MergeDataTable_Plaza_SumAvg(DataTable dt1, DataTable dt2, string DTName)
        {
            dt1.Columns.Add("tongqi_days", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_rijun", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_rijun", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_shop_keliu", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_shop_keliu_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_shop_keliu_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_shop_keliu_rijun", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_ShopaccessAvg", System.Type.GetType("System.String"));
            DataTable newDataTable = dt1.Clone();

            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i]["tongqi_days"] = dt2.Rows[i]["Row"].ToString();

                dt1.Rows[i]["tongqi_keliu"] = dt2.Rows[i]["InSum"].ToString();
                int mynum = Convert.ToInt32(dt1.Rows[i]["InSum"]) - Convert.ToInt32(dt2.Rows[i]["InSum"]);
                //dt1.Rows[i]["tongqi_keliu_chae"] = Convert.ToInt32(dt1.Rows[i]["InSum"]);
                dt1.Rows[i]["tongqi_keliu_chae"] = mynum.ToString();
                double mynum2 = Convert.ToInt32(dt2.Rows[i]["InSum"]);
                double mynum3 = mynum / mynum2;
                dt1.Rows[i]["tongqi_keliu_tongbi"] = Math.Round(mynum3 * 100, 2) + "%";
                dt1.Rows[i]["tongqi_keliu_rijun"] = dt2.Rows[i]["InAvg"].ToString();

                dt1.Rows[i]["tongqi_sale"] = dt2.Rows[i]["SaleSum"].ToString();
                double tongqi_sale_chae = Convert.ToDouble(dt1.Rows[i]["SaleSum"]) - Convert.ToDouble(dt2.Rows[i]["SaleSum"]);
                dt1.Rows[i]["tongqi_sale_chae"] = tongqi_sale_chae;
                double plaza_sale2 = Convert.ToDouble(dt2.Rows[i]["SaleSum"]);
                double tongqi_sale_tongbi = tongqi_sale_chae / plaza_sale2;
                dt1.Rows[i]["tongqi_sale_tongbi"] = Math.Round(tongqi_sale_tongbi * 100, 2) + "%";
                dt1.Rows[i]["tongqi_sale_rijun"] = dt2.Rows[i]["SaleAvg"].ToString();


                dt1.Rows[i]["tongqi_shop_keliu"] = dt2.Rows[i]["ShopsumSum"].ToString();
                double tongqi_shop_keliu_chae = Convert.ToDouble(dt1.Rows[i]["ShopsumSum"]) - Convert.ToDouble(dt2.Rows[i]["ShopsumSum"]);
                dt1.Rows[i]["tongqi_shop_keliu_chae"] = tongqi_shop_keliu_chae;
                double plaza_shopsum2 = Convert.ToDouble(dt2.Rows[i]["ShopsumSum"]);
                double tongqi_shop_keliu_tongbi = tongqi_shop_keliu_chae / plaza_shopsum2;
                dt1.Rows[i]["tongqi_shop_keliu_tongbi"] = Math.Round(tongqi_shop_keliu_tongbi * 100, 2) + "%";
                dt1.Rows[i]["tongqi_shop_keliu_rijun"] = dt2.Rows[i]["shopsumAvg"].ToString();

                dt1.Rows[i]["tongqi_ShopaccessAvg"] = dt2.Rows[i]["ShopaccessAvg"].ToString();


                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);
            }


            newDataTable.TableName = DTName; //设置DT的名字
            return newDataTable;

        }

        /// <summary>
        /// 查询总抽成区间同比
        /// </summary>
        /// <returns></returns>
        private DataTable MergeDataTable_chouchengtotal_SumAvg(DataTable dt1, DataTable dt2, string DTName)
        {
            
            dt1.Columns.Add("tongqi_keliu", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_rijun", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_rijun", System.Type.GetType("System.String"));
           
            DataTable newDataTable = dt1.Clone();

            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
               

                dt1.Rows[i]["tongqi_keliu"] = dt2.Rows[i]["insumsum"].ToString();
                int mynum = Convert.ToInt32(dt1.Rows[i]["insumsum"]) - Convert.ToInt32(dt2.Rows[i]["insumsum"]);
                
                dt1.Rows[i]["tongqi_keliu_chae"] = mynum.ToString();
                double mynum2 = Convert.ToInt32(dt2.Rows[i]["insumsum"]);
                double mynum3 = mynum / mynum2;
                dt1.Rows[i]["tongqi_keliu_tongbi"] = Math.Round(mynum3 * 100, 2) + "%";
                dt1.Rows[i]["tongqi_keliu_rijun"] = dt2.Rows[i]["insumavg"].ToString();

                dt1.Rows[i]["tongqi_sale"] = dt2.Rows[i]["salesum"].ToString();
                double tongqi_sale_chae = Convert.ToDouble(dt1.Rows[i]["salesum"]) - Convert.ToDouble(dt2.Rows[i]["salesum"]);
                dt1.Rows[i]["tongqi_sale_chae"] = tongqi_sale_chae;
                double plaza_sale2 = Convert.ToDouble(dt2.Rows[i]["salesum"]);
                double tongqi_sale_tongbi = tongqi_sale_chae / plaza_sale2;
                dt1.Rows[i]["tongqi_sale_tongbi"] = Math.Round(tongqi_sale_tongbi * 100, 2) + "%";
                dt1.Rows[i]["tongqi_sale_rijun"] = dt2.Rows[i]["saleavg"].ToString();


                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);
            }


            newDataTable.TableName = DTName; //设置DT的名字
            return newDataTable;

        }

        /// <summary>
        /// 查询商户抽成区间同比
        /// </summary>
        /// <returns></returns>
        private DataTable MergeDataTable_choucheng_shop(DataTable dt1, DataTable dt2, string DTName)
        {
            dt1.Columns.Add("tongqi_rowscount", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_rijun", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_rijun", System.Type.GetType("System.String"));

            DataTable newDataTable = dt1.Clone();

            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                //k 为状态值，如符合为1，不符合仍为0
                int k = 0;
                
                string dt1_shop = dt1.Rows[i]["shop_brand"].ToString();
                for (int j = 0; j < dt2.Rows.Count; j++)
                {
                    
                    string dt2_shop = dt2.Rows[j]["shop_brand"].ToString();

                    if (dt1_shop == dt2_shop)
                    {
                        dt1.Rows[i]["tongqi_rowscount"] = dt2.Rows[j]["rowscount"].ToString();
                        dt1.Rows[i]["tongqi_keliu"] = dt2.Rows[j]["insumsum"].ToString();
                        int mynum = Convert.ToInt32(dt1.Rows[i]["insumsum"]) - Convert.ToInt32(dt2.Rows[j]["insumsum"]);

                        dt1.Rows[i]["tongqi_keliu_chae"] = mynum.ToString();
                        double mynum2 = Convert.ToInt32(dt2.Rows[j]["insumsum"]);
                        double mynum3 = mynum / mynum2;
                        dt1.Rows[i]["tongqi_keliu_tongbi"] = Math.Round(mynum3 * 100, 2) + "%";
                        dt1.Rows[i]["tongqi_keliu_rijun"] = dt2.Rows[j]["insumavg"].ToString();

                        dt1.Rows[i]["tongqi_sale"] = dt2.Rows[j]["salesum"].ToString();
                        double tongqi_sale_chae = Convert.ToDouble(dt1.Rows[i]["salesum"]) - Convert.ToDouble(dt2.Rows[j]["salesum"]);
                        dt1.Rows[i]["tongqi_sale_chae"] = tongqi_sale_chae;
                        double plaza_sale2 = Convert.ToDouble(dt2.Rows[j]["salesum"]);
                        double tongqi_sale_tongbi = tongqi_sale_chae / plaza_sale2;
                        dt1.Rows[i]["tongqi_sale_tongbi"] = Math.Round(tongqi_sale_tongbi * 100, 2) + "%";
                        dt1.Rows[i]["tongqi_sale_rijun"] = dt2.Rows[j]["saleavg"].ToString();


                        dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                        newDataTable.Rows.Add(obj);
                        k = 1;

                    }
         

                }
                if (k == 0)
                {
                    dt1.Rows[i]["tongqi_rowscount"] = "无";
                    dt1.Rows[i]["tongqi_keliu"] = "无";
                    dt1.Rows[i]["tongqi_keliu_chae"] = "无";
                    dt1.Rows[i]["tongqi_keliu_tongbi"] = "无";
                    dt1.Rows[i]["tongqi_keliu_rijun"] = "无";
                    dt1.Rows[i]["tongqi_sale"] = "无";
                    dt1.Rows[i]["tongqi_sale_chae"] = "无";
                    dt1.Rows[i]["tongqi_sale_tongbi"] = "无";
                    dt1.Rows[i]["tongqi_sale_rijun"] = "无";

                    dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                    newDataTable.Rows.Add(obj);
                }



                
            }


            newDataTable.TableName = DTName; //设置DT的名字
            return newDataTable;

        }

        /// <summary>
        /// 查询业种区间同比
        /// </summary>
        /// <returns></returns>
        private DataTable MergeDataTable_brand_format(DataTable dt1, DataTable dt2, string DTName)
        {
            
            dt1.Columns.Add("tongqi_keliu", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_rijun", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_rijun", System.Type.GetType("System.String"));

            DataTable newDataTable = dt1.Clone();

            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                //k 为状态值，如符合为1，不符合仍为0
                int k = 0;

                string dt1_shop = dt1.Rows[i]["brand_format"].ToString();
                for (int j = 0; j < dt2.Rows.Count; j++)
                {

                    string dt2_shop = dt2.Rows[j]["brand_format"].ToString();

                    if (dt1_shop == dt2_shop)
                    {
                        
                        dt1.Rows[i]["tongqi_keliu"] = dt2.Rows[j]["insumsum"].ToString();
                        int mynum = Convert.ToInt32(dt1.Rows[i]["insumsum"]) - Convert.ToInt32(dt2.Rows[j]["insumsum"]);

                        dt1.Rows[i]["tongqi_keliu_chae"] = mynum.ToString();
                        double mynum2 = Convert.ToInt32(dt2.Rows[j]["insumsum"]);
                        double mynum3 = mynum / mynum2;
                        dt1.Rows[i]["tongqi_keliu_tongbi"] = Math.Round(mynum3 * 100, 2) + "%";
                        dt1.Rows[i]["tongqi_keliu_rijun"] = dt2.Rows[j]["insumavg"].ToString();

                        dt1.Rows[i]["tongqi_sale"] = dt2.Rows[j]["salesum"].ToString();
                        double tongqi_sale_chae = Convert.ToDouble(dt1.Rows[i]["salesum"]) - Convert.ToDouble(dt2.Rows[j]["salesum"]);
                        dt1.Rows[i]["tongqi_sale_chae"] = tongqi_sale_chae;
                        double plaza_sale2 = Convert.ToDouble(dt2.Rows[j]["salesum"]);
                        double tongqi_sale_tongbi = tongqi_sale_chae / plaza_sale2;
                        dt1.Rows[i]["tongqi_sale_tongbi"] = Math.Round(tongqi_sale_tongbi * 100, 2) + "%";
                        dt1.Rows[i]["tongqi_sale_rijun"] = dt2.Rows[j]["saleavg"].ToString();


                        dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                        newDataTable.Rows.Add(obj);
                        k = 1;

                    }


                }
                if (k == 0)
                {
                    
                    dt1.Rows[i]["tongqi_keliu"] = "无";
                    dt1.Rows[i]["tongqi_keliu_chae"] = "无";
                    dt1.Rows[i]["tongqi_keliu_tongbi"] = "无";
                    dt1.Rows[i]["tongqi_keliu_rijun"] = "无";
                    dt1.Rows[i]["tongqi_sale"] = "无";
                    dt1.Rows[i]["tongqi_sale_chae"] = "无";
                    dt1.Rows[i]["tongqi_sale_tongbi"] = "无";
                    dt1.Rows[i]["tongqi_sale_rijun"] = "无";

                    dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                    newDataTable.Rows.Add(obj);
                }


            }


            newDataTable.TableName = DTName; //设置DT的名字
            return newDataTable;

        }

        /// <summary>
        /// 查询业态商户区间同比
        /// </summary>
        /// <returns></returns>
        private DataTable MergeDataTable_brand_format_shop(DataTable dt1, DataTable dt2, string DTName)
        {
            dt1.Columns.Add("tongqi_rowscount", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_rijun", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_rijun", System.Type.GetType("System.String"));

            DataTable newDataTable = dt1.Clone();

            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                //k 为状态值，如符合为1，不符合仍为0
                int k = 0;

                string dt1_shop = dt1.Rows[i]["shop_brand"].ToString();
                for (int j = 0; j < dt2.Rows.Count; j++)
                {

                    string dt2_shop = dt2.Rows[j]["shop_brand"].ToString();

                    if (dt1_shop == dt2_shop)
                    {
                        dt1.Rows[i]["tongqi_rowscount"] = dt2.Rows[j]["rowscount"].ToString();
                        dt1.Rows[i]["tongqi_keliu"] = dt2.Rows[j]["insumsum"].ToString();
                        int mynum = Convert.ToInt32(dt1.Rows[i]["insumsum"]) - Convert.ToInt32(dt2.Rows[j]["insumsum"]);

                        dt1.Rows[i]["tongqi_keliu_chae"] = mynum.ToString();
                        double mynum2 = Convert.ToInt32(dt2.Rows[j]["insumsum"]);
                        double mynum3 = mynum / mynum2;
                        dt1.Rows[i]["tongqi_keliu_tongbi"] = Math.Round(mynum3 * 100, 2) + "%";
                        dt1.Rows[i]["tongqi_keliu_rijun"] = dt2.Rows[j]["insumavg"].ToString();

                        dt1.Rows[i]["tongqi_sale"] = dt2.Rows[j]["salesum"].ToString();
                        double tongqi_sale_chae = Convert.ToDouble(dt1.Rows[i]["salesum"]) - Convert.ToDouble(dt2.Rows[j]["salesum"]);
                        dt1.Rows[i]["tongqi_sale_chae"] = tongqi_sale_chae;
                        double plaza_sale2 = Convert.ToDouble(dt2.Rows[j]["salesum"]);
                        double tongqi_sale_tongbi = tongqi_sale_chae / plaza_sale2;
                        dt1.Rows[i]["tongqi_sale_tongbi"] = Math.Round(tongqi_sale_tongbi * 100, 2) + "%";
                        dt1.Rows[i]["tongqi_sale_rijun"] = dt2.Rows[j]["saleavg"].ToString();


                        dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                        newDataTable.Rows.Add(obj);
                        k = 1;

                    }


                }
                if (k == 0)
                {
                    dt1.Rows[i]["tongqi_rowscount"] = "无";
                    dt1.Rows[i]["tongqi_keliu"] = "无";
                    dt1.Rows[i]["tongqi_keliu_chae"] = "无";
                    dt1.Rows[i]["tongqi_keliu_tongbi"] = "无";
                    dt1.Rows[i]["tongqi_keliu_rijun"] = "无";
                    dt1.Rows[i]["tongqi_sale"] = "无";
                    dt1.Rows[i]["tongqi_sale_chae"] = "无";
                    dt1.Rows[i]["tongqi_sale_tongbi"] = "无";
                    dt1.Rows[i]["tongqi_sale_rijun"] = "无";

                    dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                    newDataTable.Rows.Add(obj);
                }


            }


            newDataTable.TableName = DTName; //设置DT的名字
            return newDataTable;

        }

        /// <summary>
        /// 查询楼层区间同比
        /// </summary>
        /// <returns></returns>
        private DataTable MergeDataTable_floor(DataTable dt1, DataTable dt2, string DTName)
        {

            dt1.Columns.Add("tongqi_keliu", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_rijun", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_rijun", System.Type.GetType("System.String"));

            DataTable newDataTable = dt1.Clone();

            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                //k 为状态值，如符合为1，不符合仍为0
                int k = 0;

                string dt1_shop = dt1.Rows[i]["floor"].ToString();
                for (int j = 0; j < dt2.Rows.Count; j++)
                {

                    string dt2_shop = dt2.Rows[j]["floor"].ToString();

                    if (dt1_shop == dt2_shop)
                    {

                        dt1.Rows[i]["tongqi_keliu"] = dt2.Rows[j]["insumsum"].ToString();
                        int mynum = Convert.ToInt32(dt1.Rows[i]["insumsum"]) - Convert.ToInt32(dt2.Rows[j]["insumsum"]);

                        dt1.Rows[i]["tongqi_keliu_chae"] = mynum.ToString();
                        double mynum2 = Convert.ToInt32(dt2.Rows[j]["insumsum"]);
                        double mynum3 = mynum / mynum2;
                        dt1.Rows[i]["tongqi_keliu_tongbi"] = Math.Round(mynum3 * 100, 2) + "%";
                        dt1.Rows[i]["tongqi_keliu_rijun"] = dt2.Rows[j]["insumavg"].ToString();

                        dt1.Rows[i]["tongqi_sale"] = dt2.Rows[j]["salesum"].ToString();
                        double tongqi_sale_chae = Convert.ToDouble(dt1.Rows[i]["salesum"]) - Convert.ToDouble(dt2.Rows[j]["salesum"]);
                        dt1.Rows[i]["tongqi_sale_chae"] = tongqi_sale_chae;
                        double plaza_sale2 = Convert.ToDouble(dt2.Rows[j]["salesum"]);
                        double tongqi_sale_tongbi = tongqi_sale_chae / plaza_sale2;
                        dt1.Rows[i]["tongqi_sale_tongbi"] = Math.Round(tongqi_sale_tongbi * 100, 2) + "%";
                        dt1.Rows[i]["tongqi_sale_rijun"] = dt2.Rows[j]["saleavg"].ToString();


                        dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                        newDataTable.Rows.Add(obj);
                        k = 1;

                    }


                }
                if (k == 0)
                {

                    dt1.Rows[i]["tongqi_keliu"] = "无";
                    dt1.Rows[i]["tongqi_keliu_chae"] = "无";
                    dt1.Rows[i]["tongqi_keliu_tongbi"] = "无";
                    dt1.Rows[i]["tongqi_keliu_rijun"] = "无";
                    dt1.Rows[i]["tongqi_sale"] = "无";
                    dt1.Rows[i]["tongqi_sale_chae"] = "无";
                    dt1.Rows[i]["tongqi_sale_tongbi"] = "无";
                    dt1.Rows[i]["tongqi_sale_rijun"] = "无";

                    dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                    newDataTable.Rows.Add(obj);
                }


            }


            newDataTable.TableName = DTName; //设置DT的名字
            return newDataTable;

        }

        /// <summary>
        /// 查询楼层商户区间同比
        /// </summary>
        /// <returns></returns>
        private DataTable MergeDataTable_floor_shop(DataTable dt1, DataTable dt2, string DTName)
        {
            dt1.Columns.Add("tongqi_rowscount", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_rijun", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_rijun", System.Type.GetType("System.String"));

            DataTable newDataTable = dt1.Clone();

            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                //k 为状态值，如符合为1，不符合仍为0
                int k = 0;

                string dt1_shop = dt1.Rows[i]["shop_brand"].ToString();
                for (int j = 0; j < dt2.Rows.Count; j++)
                {

                    string dt2_shop = dt2.Rows[j]["shop_brand"].ToString();

                    if (dt1_shop == dt2_shop)
                    {
                        dt1.Rows[i]["tongqi_rowscount"] = dt2.Rows[j]["rowscount"].ToString();
                        dt1.Rows[i]["tongqi_keliu"] = dt2.Rows[j]["insumsum"].ToString();
                        int mynum = Convert.ToInt32(dt1.Rows[i]["insumsum"]) - Convert.ToInt32(dt2.Rows[j]["insumsum"]);

                        dt1.Rows[i]["tongqi_keliu_chae"] = mynum.ToString();
                        double mynum2 = Convert.ToInt32(dt2.Rows[j]["insumsum"]);
                        double mynum3 = mynum / mynum2;
                        dt1.Rows[i]["tongqi_keliu_tongbi"] = Math.Round(mynum3 * 100, 2) + "%";
                        dt1.Rows[i]["tongqi_keliu_rijun"] = dt2.Rows[j]["insumavg"].ToString();

                        dt1.Rows[i]["tongqi_sale"] = dt2.Rows[j]["salesum"].ToString();
                        double tongqi_sale_chae = Convert.ToDouble(dt1.Rows[i]["salesum"]) - Convert.ToDouble(dt2.Rows[j]["salesum"]);
                        dt1.Rows[i]["tongqi_sale_chae"] = tongqi_sale_chae;
                        double plaza_sale2 = Convert.ToDouble(dt2.Rows[j]["salesum"]);
                        double tongqi_sale_tongbi = tongqi_sale_chae / plaza_sale2;
                        dt1.Rows[i]["tongqi_sale_tongbi"] = Math.Round(tongqi_sale_tongbi * 100, 2) + "%";
                        dt1.Rows[i]["tongqi_sale_rijun"] = dt2.Rows[j]["saleavg"].ToString();


                        dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                        newDataTable.Rows.Add(obj);
                        k = 1;

                    }


                }
                if (k == 0)
                {
                    dt1.Rows[i]["tongqi_rowscount"] = "无";
                    dt1.Rows[i]["tongqi_keliu"] = "无";
                    dt1.Rows[i]["tongqi_keliu_chae"] = "无";
                    dt1.Rows[i]["tongqi_keliu_tongbi"] = "无";
                    dt1.Rows[i]["tongqi_keliu_rijun"] = "无";
                    dt1.Rows[i]["tongqi_sale"] = "无";
                    dt1.Rows[i]["tongqi_sale_chae"] = "无";
                    dt1.Rows[i]["tongqi_sale_tongbi"] = "无";
                    dt1.Rows[i]["tongqi_sale_rijun"] = "无";

                    dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                    newDataTable.Rows.Add(obj);
                }


            }


            newDataTable.TableName = DTName; //设置DT的名字
            return newDataTable;

        }

        /// <summary>
        /// 查询主力店、步行街区间同比
        /// </summary>
        /// <returns></returns>
        private DataTable MergeDataTable_zhuli(DataTable dt1, DataTable dt2, string DTName)
        {

            dt1.Columns.Add("tongqi_keliu", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_rijun", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_rijun", System.Type.GetType("System.String"));

            DataTable newDataTable = dt1.Clone();

            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                //k 为状态值，如符合为1，不符合仍为0
                int k = 0;

                string dt1_shop = dt1.Rows[i]["brand_type"].ToString();
                for (int j = 0; j < dt2.Rows.Count; j++)
                {

                    string dt2_shop = dt2.Rows[j]["brand_type"].ToString();

                    if (dt1_shop == dt2_shop)
                    {

                        dt1.Rows[i]["tongqi_keliu"] = dt2.Rows[j]["insumsum"].ToString();
                        int mynum = Convert.ToInt32(dt1.Rows[i]["insumsum"]) - Convert.ToInt32(dt2.Rows[j]["insumsum"]);

                        dt1.Rows[i]["tongqi_keliu_chae"] = mynum.ToString();
                        double mynum2 = Convert.ToInt32(dt2.Rows[j]["insumsum"]);
                        double mynum3 = mynum / mynum2;
                        dt1.Rows[i]["tongqi_keliu_tongbi"] = Math.Round(mynum3 * 100, 2) + "%";
                        dt1.Rows[i]["tongqi_keliu_rijun"] = dt2.Rows[j]["insumavg"].ToString();

                        dt1.Rows[i]["tongqi_sale"] = dt2.Rows[j]["salesum"].ToString();
                        double tongqi_sale_chae = Convert.ToDouble(dt1.Rows[i]["salesum"]) - Convert.ToDouble(dt2.Rows[j]["salesum"]);
                        dt1.Rows[i]["tongqi_sale_chae"] = tongqi_sale_chae;
                        double plaza_sale2 = Convert.ToDouble(dt2.Rows[j]["salesum"]);
                        double tongqi_sale_tongbi = tongqi_sale_chae / plaza_sale2;
                        dt1.Rows[i]["tongqi_sale_tongbi"] = Math.Round(tongqi_sale_tongbi * 100, 2) + "%";
                        dt1.Rows[i]["tongqi_sale_rijun"] = dt2.Rows[j]["saleavg"].ToString();


                        dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                        newDataTable.Rows.Add(obj);
                        k = 1;

                    }


                }
                if (k == 0)
                {

                    dt1.Rows[i]["tongqi_keliu"] = "无";
                    dt1.Rows[i]["tongqi_keliu_chae"] = "无";
                    dt1.Rows[i]["tongqi_keliu_tongbi"] = "无";
                    dt1.Rows[i]["tongqi_keliu_rijun"] = "无";
                    dt1.Rows[i]["tongqi_sale"] = "无";
                    dt1.Rows[i]["tongqi_sale_chae"] = "无";
                    dt1.Rows[i]["tongqi_sale_tongbi"] = "无";
                    dt1.Rows[i]["tongqi_sale_rijun"] = "无";

                    dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                    newDataTable.Rows.Add(obj);
                }


            }


            newDataTable.TableName = DTName; //设置DT的名字
            return newDataTable;

        }

        /// <summary>
        /// 查询楼层商户区间同比
        /// </summary>
        /// <returns></returns>
        private DataTable MergeDataTable_zhuli_shop(DataTable dt1, DataTable dt2, string DTName)
        {
            dt1.Columns.Add("tongqi_rowscount", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_keliu_rijun", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_chae", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_tongbi", System.Type.GetType("System.String"));
            dt1.Columns.Add("tongqi_sale_rijun", System.Type.GetType("System.String"));

            DataTable newDataTable = dt1.Clone();

            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                //k 为状态值，如符合为1，不符合仍为0
                int k = 0;

                string dt1_shop = dt1.Rows[i]["shop_brand"].ToString();
                for (int j = 0; j < dt2.Rows.Count; j++)
                {

                    string dt2_shop = dt2.Rows[j]["shop_brand"].ToString();

                    if (dt1_shop == dt2_shop)
                    {
                        dt1.Rows[i]["tongqi_rowscount"] = dt2.Rows[j]["rowscount"].ToString();
                        dt1.Rows[i]["tongqi_keliu"] = dt2.Rows[j]["insumsum"].ToString();
                        int mynum = Convert.ToInt32(dt1.Rows[i]["insumsum"]) - Convert.ToInt32(dt2.Rows[j]["insumsum"]);

                        dt1.Rows[i]["tongqi_keliu_chae"] = mynum.ToString();
                        double mynum2 = Convert.ToInt32(dt2.Rows[j]["insumsum"]);
                        double mynum3 = mynum / mynum2;
                        dt1.Rows[i]["tongqi_keliu_tongbi"] = Math.Round(mynum3 * 100, 2) + "%";
                        dt1.Rows[i]["tongqi_keliu_rijun"] = dt2.Rows[j]["insumavg"].ToString();

                        dt1.Rows[i]["tongqi_sale"] = dt2.Rows[j]["salesum"].ToString();
                        double tongqi_sale_chae = Convert.ToDouble(dt1.Rows[i]["salesum"]) - Convert.ToDouble(dt2.Rows[j]["salesum"]);
                        dt1.Rows[i]["tongqi_sale_chae"] = tongqi_sale_chae;
                        double plaza_sale2 = Convert.ToDouble(dt2.Rows[j]["salesum"]);
                        double tongqi_sale_tongbi = tongqi_sale_chae / plaza_sale2;
                        dt1.Rows[i]["tongqi_sale_tongbi"] = Math.Round(tongqi_sale_tongbi * 100, 2) + "%";
                        dt1.Rows[i]["tongqi_sale_rijun"] = dt2.Rows[j]["saleavg"].ToString();


                        dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                        newDataTable.Rows.Add(obj);
                        k = 1;

                    }


                }
                if (k == 0)
                {
                    dt1.Rows[i]["tongqi_rowscount"] = "无";
                    dt1.Rows[i]["tongqi_keliu"] = "无";
                    dt1.Rows[i]["tongqi_keliu_chae"] = "无";
                    dt1.Rows[i]["tongqi_keliu_tongbi"] = "无";
                    dt1.Rows[i]["tongqi_keliu_rijun"] = "无";
                    dt1.Rows[i]["tongqi_sale"] = "无";
                    dt1.Rows[i]["tongqi_sale_chae"] = "无";
                    dt1.Rows[i]["tongqi_sale_tongbi"] = "无";
                    dt1.Rows[i]["tongqi_sale_rijun"] = "无";

                    dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                    newDataTable.Rows.Add(obj);
                }


            }


            newDataTable.TableName = DTName; //设置DT的名字
            return newDataTable;

        }

        [System.Web.Http.HttpGet]
        /// <summary>
        /// 查询广场时间区间同比
        /// </summary>
        /// <returns></returns>
        public DataTable getTongbiPlazaSumAvg(string startdate1, string enddate1, string startdate2, string enddate2)

        {
           
            DataTable mytable1 = AnyDayToDayCustomerSumAvg(startdate1, enddate1);
            DataTable mytable2 = AnyDayToDayCustomerSumAvg(startdate2, enddate2);
            DataTable mytable3 = MergeDataTable_Plaza_SumAvg(mytable1, mytable2, "table3");
            
            return mytable3;

        }

        [System.Web.Http.HttpGet]
        /// <summary>
        /// 查询广场日级同比
        /// </summary>
        /// <returns></returns>
        public DataTable getTongbiPlaza(string startdate1, string enddate1, string startdate2, string enddate2)

        {

            DataTable mytable1 = AnyDayToDayCustomer(startdate1, enddate1);
            DataTable mytable2 = AnyDayToDayCustomer(startdate2, enddate2);
            DataTable mytable3 = MergeDataTable_Plaza(mytable1, mytable2, "table3");

            return mytable3;

        }

        [System.Web.Http.HttpGet]
        /// <summary>
        /// 查询广场抽成同比
        /// </summary>
        /// <returns></returns>
        public DataTable getTongbiChouchengPlaza(string startdate1, string enddate1, string startdate2, string enddate2)

        {

            DataTable mytable1 = AnyDayToDaychouchengtotalSumAvg(startdate1, enddate1);
            DataTable mytable2 = AnyDayToDaychouchengtotalSumAvg(startdate2, enddate2);
            DataTable mytable3 = MergeDataTable_chouchengtotal_SumAvg(mytable1, mytable2, "table3");

            return mytable3;

        }

        [System.Web.Http.HttpGet]
        /// <summary>
        /// 查询商户抽成同比
        /// </summary>
        /// <returns></returns>
        public DataTable getTongbiChouchengShop(string startdate1, string enddate1, string startdate2, string enddate2)

        {

            DataTable mytable1 = AnyDayToDaychouchengshopSumAvg(startdate1, enddate1);
            DataTable mytable2 = AnyDayToDaychouchengshopSumAvg(startdate2, enddate2);
            DataTable mytable3 = MergeDataTable_choucheng_shop(mytable1, mytable2, "table3");

            return mytable3;

        }

        [System.Web.Http.HttpGet]
        /// <summary>
        /// 查询业态同比
        /// </summary>
        /// <returns></returns>
        public DataTable getTongbiBrand_format(string startdate1, string enddate1, string startdate2, string enddate2)

        {

            DataTable mytable1 = AnyDayToDaybrand_formattotalSumAvg(startdate1, enddate1);
            DataTable mytable2 = AnyDayToDaybrand_formattotalSumAvg(startdate2, enddate2);
            DataTable mytable3 = MergeDataTable_brand_format(mytable1, mytable2, "table3");

            return mytable3;

        }

        [System.Web.Http.HttpGet]
        /// <summary>
        /// 查询业态商户同比
        /// </summary>
        /// <returns></returns>
        public DataTable getTongbiBrand_format_shop(string brand_format,string startdate1, string enddate1, string startdate2, string enddate2)

        {

            DataTable mytable1 = AnyDayToDaybrand_formatshopSumAvg(brand_format,startdate1, enddate1);
            DataTable mytable2 = AnyDayToDaybrand_formatshopSumAvg(brand_format,startdate2, enddate2);
            DataTable mytable3 = MergeDataTable_brand_format_shop(mytable1, mytable2, "table3");

            return mytable3;

        }

        [System.Web.Http.HttpGet]
        /// <summary>
        /// 查询楼层同比
        /// </summary>
        /// <returns></returns>
        public DataTable getTongbiFloor( string startdate1, string enddate1, string startdate2, string enddate2)

        {

            DataTable mytable1 = AnyDayToDayfloortotalSumAvg(startdate1, enddate1);
            DataTable mytable2 = AnyDayToDayfloortotalSumAvg( startdate2, enddate2);
            DataTable mytable3 = MergeDataTable_floor(mytable1, mytable2, "table3");

            return mytable3;

        }

        [System.Web.Http.HttpGet]
        /// <summary>
        /// 查询楼层商户同比
        /// </summary>
        /// <returns></returns>
        public DataTable getTongbiFloor_shop(string floornum, string startdate1, string enddate1, string startdate2, string enddate2)

        {

            DataTable mytable1 = AnyDayToDayfloorshopSumAvg(floornum, startdate1, enddate1);
            DataTable mytable2 = AnyDayToDayfloorshopSumAvg(floornum, startdate2, enddate2);
            DataTable mytable3 = MergeDataTable_floor_shop(mytable1, mytable2, "table3");

            return mytable3;

        }

        [System.Web.Http.HttpGet]
        /// <summary>
        /// 查询主力店步行街同比
        /// </summary>
        /// <returns></returns>
        public DataTable getTongbiZhuli( string startdate1, string enddate1, string startdate2, string enddate2)

        {

            DataTable mytable1 = AnyDayToDayzhulitotalSumAvg(startdate1, enddate1);
            DataTable mytable2 = AnyDayToDayzhulitotalSumAvg(startdate2, enddate2);
            DataTable mytable3 = MergeDataTable_zhuli(mytable1, mytable2, "table3");

            return mytable3;

        }

        [System.Web.Http.HttpGet]
        /// <summary>
        /// 查询主力店步行街商户同比
        /// </summary>
        /// <returns></returns>
        public DataTable getTongbiZhuli_shop(string brand_type_str, string startdate1, string enddate1, string startdate2, string enddate2)

        {

            DataTable mytable1 = AnyDayToDayzhulishopSumAvg(brand_type_str, startdate1, enddate1);
            DataTable mytable2 = AnyDayToDayzhulishopSumAvg(brand_type_str, startdate2, enddate2);
            DataTable mytable3 = MergeDataTable_zhuli_shop(mytable1, mytable2, "table3");

            return mytable3;

        }



        [System.Web.Http.HttpPost]
        /// <summary>
        /// 查询重点商铺同比情况
        /// </summary>
        /// <returns></returns>
        public IList<class_PointShop_data> getTongbiPointCustomer(class_PointShop my_pointshop)
        
        {
            string startdate1 = my_pointshop.startdate1;
            string enddate1 = my_pointshop.enddate1;
            string startdate2 = my_pointshop.startdate2;
            string enddate2 = my_pointshop.enddate2;
            string sitetype = my_pointshop.sitetype;
            DataTable mytable1 = AnyDaytoDayPointShopCustomer(startdate1, enddate1, sitetype);
            DataTable mytable2 = AnyDaytoDayPointShopCustomer(startdate2, enddate2, sitetype);
            DataTable mytable3 = MergeDataTableRow(mytable1, mytable2, "table3");
            IList<class_PointShop_data> mylist = ModelConvertHelper.DataTableToList<class_PointShop_data>(mytable3);
            return  mylist;

        }

        
        public class class_PointShop
        {
            public string startdate1 { get; set; }   //开始时间1
            public string enddate1 { get; set; }   //结束时间1
            public string startdate2 { get; set; }   //开始时间2
            public string enddate2 { get; set; }   // 结束时间2
            public string sitetype { get; set; }  // 点位代码包括 商铺600，通道700

        }

       
        public class class_PointShop_data
        {
            public string SiteName { get; set; }   //商铺名称
            public string InSum { get; set; }     //进客流
            public string tongqi { get; set; }   //同期
            public string tongbi { get; set; }  // 同比



        }



        [System.Web.Http.HttpGet]
        /// <summary>
        /// 查询重点商铺客流
        /// </summary>
        /// <returns></returns>
        public DataTable AnyDaytoDayPointShopCustomer(string StartDate, string EndDate, string SiteType)
        {

            return bll.AnyDaytoDayPointShopCustomer(StartDate, EndDate, SiteType);
        }

        //两个结构不同的DT合并
        /// <summary>
        /// 将两个列不同的DataTable合并成一个新的DataTable
        /// </summary>
        /// <param name="dt1">表1</param>
        /// <param name="dt2">表2</param>
        /// <param name="DTName">合并后新的表名</param>
        /// <returns></returns>
        private DataTable UniteDataTable(DataTable dt1, DataTable dt2, string DTName)
        {
            DataTable dt3 = dt1.Clone();
            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                dt3.Columns.Add(dt2.Columns[i].ColumnName);
            }
            object[] obj = new object[dt3.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                dt3.Rows.Add(obj);
            }
            if (dt1.Rows.Count >= dt2.Rows.Count)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                    }
                }
            }
            else
            {
                DataRow dr3;
                for (int i = 0; i < dt2.Rows.Count - dt1.Rows.Count; i++)
                {
                    dr3 = dt3.NewRow();
                    dt3.Rows.Add(dr3);
                }
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                    }
                }
            }
            dt3.TableName = DTName; //设置DT的名字
            return dt3;
        }

        [System.Web.Http.HttpGet]
        public DataTable TodayCustomer_all()
        {
            DataTable benxi_table = bll.TodayCustomer();
            DataTable fuxin_table = bll.TodayCustomer_fuxin();
            DataTable caoyang_table = bll.TodayCustomer_chaoyang();
            DataTable fuxin_chaoyang = UniteDataTable(fuxin_table, caoyang_table, "table0");
            return UniteDataTable(benxi_table, fuxin_chaoyang, "table0");

        }
        [System.Web.Http.HttpGet]
        public DataTable TodayCustomer_all_Merge()
        {
            DataTable benxi_table = bll.TodayCustomer();
            DataTable fuxin_table = bll.TodayCustomer_fuxin();
            DataTable caoyang_table = bll.TodayCustomer_chaoyang();
            DataTable benxi_fuxin = MergeDataTable(benxi_table, fuxin_table, "table0");
            return MergeDataTable(benxi_fuxin, caoyang_table, "table0");

        }

        /// <summary>
        /// 查询近两个月外围通道
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable TwoMonthPassageway(string searchDate)
        {

            return bll.TwoMonthPassageway(searchDate);
        }

        /// <summary>
        /// 查询近两个月广场及主力店及步行街客流
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable TwoMonthPlaza(string searchDate)
        {

            return bll.TwoMonthPlaza(searchDate);
        }

        /// <summary>
        /// 查询近两个月商铺客流
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable TwoMonthShop(string searchDate)
        {

            return bll.TwoMonthShop(searchDate);
        }

        /// <summary>
        /// 查询当日主力店客流
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable TodayBigShop()
        {

            return bll.TodayBigShop();
        }

        /// <summary>
        /// 查询开业到现在任意一天客流
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayCustomer(string searchDate)
        {

            return bll.AnyDayCustomer(searchDate);
        }

        /// <summary>
        /// 查询开业到现在任意区间客流
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayToDayCustomer(string searchDate1, string searchDate2)
        {

            return bll.AnyDayToDayCustomer(searchDate1, searchDate2);
        }

        /// <summary>
        /// 查询开业到现在任意区间客流和及平均值
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayToDayCustomerSumAvg(string searchDate1, string searchDate2)
        {

            return bll.AnyDayToDayCustomerSumAvg(searchDate1, searchDate2);
        }

        /// <summary>
        /// 查询指定日期商铺销售客流
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayPlazaShopCustomer(string searchDate)

        {
            return bll.AnyDayPlazaShopCustomer(searchDate);

        }

        /// <summary>
        /// 查询指定时间段抽成商户销售客流
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayToDaychouchengshopSumAvg(string searchDate1, string searchDate2)

        {
            return bll.AnyDayToDaychouchengshopSumAvg(searchDate1, searchDate2);

        }

        /// <summary>
        /// 查询指定时间段抽成商户合计销售客流
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayToDaychouchengtotalSumAvg(string searchDate1, string searchDate2)

        {
            return bll.AnyDayToDaychouchengtotalSumAvg(searchDate1, searchDate2);

        }

        /// <summary>
        /// 查询指定时间段业种合计销售客流
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayToDaybrand_formattotalSumAvg(string searchDate1, string searchDate2)

        {

            return bll.AnyDayToDaybrand_formattotalSumAvg(searchDate1, searchDate2);

        }

        /// <summary>
        /// 查询指定时间段业种商户销售客流
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayToDaybrand_formatshopSumAvg(string brand_format, string searchDate1, string searchDate2)

        {
            return bll.AnyDayToDaybrand_formatshopSumAvg(brand_format, searchDate1, searchDate2);
        }


        /// <summary>
        /// 查询指定时间段楼层合计销售客流
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayToDayfloortotalSumAvg(string searchDate1, string searchDate2)

        {

            return bll.AnyDayToDayfloortotalSumAvg(searchDate1, searchDate2);

        }

        /// <summary>
        /// 查询指定时间段楼层商铺销售客流
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayToDayfloorshopSumAvg(string floornum, string searchDate1, string searchDate2)

        {

            return bll.AnyDayToDayfloorshopSumAvg(floornum, searchDate1, searchDate2);

        }

        /// <summary>
        /// 查询指定时间段主力店合计销售客流
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayToDayzhulitotalSumAvg(string searchDate1, string searchDate2)

        {
            return bll.AnyDayToDayzhulitotalSumAvg(searchDate1, searchDate2);

        }

        /// <summary>
        /// 查询指定时间段主力店商铺销售客流
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayToDayzhulishopSumAvg(string brand_type_str, string searchDate1, string searchDate2)

        {
            return bll.AnyDayToDayzhulishopSumAvg(brand_type_str, searchDate1, searchDate2);
        }


        /// <summary>
        /// 查询指定时间段商铺每日信息
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable AnyDayToDayshopList(string shop_brand, string searchDate1, string searchDate2)

        {

            return bll.AnyDayToDayshopList(shop_brand, searchDate1, searchDate2);
        }




        #region 获取小程序openid 和session_key
        [System.Web.Http.HttpGet]  //网页中显示这个方法
        public string GetCode(string json_code)
        {
            

            string serviceAddress = "https://api.weixin.qq.com/sns/jscode2session?appid=wxd7526ad7c10909fa&secret=5895721c889c64f8dc7d46eae6298dbe&js_code=" + json_code + "&grant_type=authorization_code";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);
            request.Method = "GET";
            request.ContentType = "text/html;charset=utf-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, System.Text.Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            
            
            var obj = new
            {
                data = retString,
                Success = true
            };
            
           
            Formatting microsoftDataFormatSetting = default(Formatting);
            string result = JsonConvert.SerializeObject(obj, microsoftDataFormatSetting);



            string key = "openid";
 
            int Index = result.IndexOf(key);

            if (Index != -1)
            {
                int startIndex = Index + 11;
                //int endIndex = startIndex + 29;
                string openid = result.Substring(startIndex,28);                //MyOpenId.Value=openid;

                
                
            
                return openid;


            }
            else
            {
                //Response.Write("找不到OpenidID");
                return "找不到OpenidID";
                //List<string> list = new List<string>() { "找不到OpenidID或已失效" };
                //return list;
          
            }

        }

        [System.Web.Http.HttpGet]
        public webapi.Models.MesOpenid GetOpenid(string json_code)
        {
            MesOpenid mymes = new MesOpenid();
            try
            {
               
                string _posdata = "appid=wxd7526ad7c10909fa&secret=5895721c889c64f8dc7d46eae6298dbe&js_code=" + json_code + "&grant_type=authorization_code";
                string _url = "https://api.weixin.qq.com/sns/jscode2session";//获取openid
                string _data = request_url(_url, _posdata);
                if (_data.Contains("\"openid\""))
                {

                    string _ip = HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString().Trim();
                    dynamic _modal = Newtonsoft.Json.Linq.JToken.Parse(_data) as dynamic;
                    string _openid = _modal.openid;
                    string _session_key = _modal.session_key;

                    mymes.OpenId = _openid;
                    mymes.Session_Key = _session_key;
                    mymes.MsgCode = "ok";
                    return mymes;

                    
                   

                }
                else
                {
                    mymes.OpenId = "找不到OpenidID或已失效";
                    mymes.Session_Key = "找不到session_key或已失效";
                    mymes.MsgCode = "400";
                    return mymes;
                }

            }

            catch (Exception ex)
            {
                //this.Json(new { succeed = false, data = ex.Message }, JsonRequestBehavior.AllowGet);
                mymes.OpenId = "严重错误";
                mymes.Session_Key = ex.ToString ();
                mymes.MsgCode = "400";
                return mymes;
            }

        }

        private string Json(object p, JsonRequestBehavior allowGet)
        {
            throw new NotImplementedException();
        }

        #region 请求api
        /// <summary>
                /// 请求api
                /// </summary>
                /// <param name="_url"></param>
                /// <param name="post_data"></param>
                /// <returns></returns>
        private string request_url(string _url, string post_data)
        {
            string result = ""; string url = _url;// "https://api.weixin.qq.com/sns/jscode2session";

            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            //request.Headers=""
            request.Method = "POST";

            byte[] buffer = encoding.GetBytes(post_data.Trim());
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        #endregion

        #endregion
        #region 解密电话号码
        [System.Web.Http.HttpGet]  //网页中显示这个方法
        public string getPhoneNumber(string encryptedData, string IV, string Session_key)
        {
            try
            {

                
                byte[] encryData = Convert.FromBase64String(encryptedData);  // strToToHexByte(text);
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Key = Convert.FromBase64String(Session_key); // Encoding.UTF8.GetBytes(AesKey);
                rijndaelCipher.IV = Convert.FromBase64String(IV);// Encoding.UTF8.GetBytes(AesIV);
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] plainText = transform.TransformFinalBlock(encryData, 0, encryData.Length);
                string result = Encoding.Default.GetString(plainText);
                //动态解析result 成对象
                dynamic model = Newtonsoft.Json.Linq.JToken.Parse(result) as dynamic;
                return model.phoneNumber;
               
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return ex.ToString ();

            }
            
        }
        #endregion

        /// <summary>
        /// 测试GET返回列表对象
        /// </summary>
        /// <param name="token">接口访问令牌</param>
        /// <returns>返回列表对象</returns>
        [System.Web.Http.HttpGet]
        public List<string> TestAction(string token)
        {
            List<string> list = new List<string>() { "123", "234", "345", token };
            return list;
        }

        /// <summary>
        /// 简单的GET方式获取数据
        /// </summary>
        /// <param name="id">字符串ID</param>
        /// <param name="token">接口访问令牌</param>
        /// <returns>返回字符串值</returns>
        [System.Web.Http.HttpGet]
        public string Test(string id, string token)
        {
            return string.Format("返回结果, id:{0}", id);
        }

        /// <summary>
        /// 多个参数的GET方式获取数据
        /// </summary>
        /// <param name="id">字符串ID</param>
        /// <param name="name">名称</param>
        /// <param name="token">接口访问令牌</param>
        /// <returns>返回字符串值</returns>
        [System.Web.Http.HttpGet]
        public string TestMulti(string id, string name, string token)
        {
            return string.Format("返回结果, id:{0} name:{1}", id, name);
        }

        /// <summary>
        /// 查询openid及相关信息
        /// </summary>
        /// <param name="strOpenid"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public DataTable GetUserOpenId(string strOpenid)
        {

            return bll.GetUserOpenId(strOpenid);
        }



        





        /// <summary>
        /// 招聘信息系统
        /// </summary>
        [System.Web.Http.HttpPost]
        public int add_zhaopin_user([FromBody]Dal.zhaopin_person newperson)
        {
            return bll.add_zhaopin_user(newperson);
        }






    }
}
