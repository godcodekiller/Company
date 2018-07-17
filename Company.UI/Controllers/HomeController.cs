﻿using Company.BLLContainer;
using Company.Common;
using Company.Common.Staff;
using Company.IBLL;
using Company.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Company.UI.Controllers
{
    public class HomeController : Controller
    {
        private IStaffService StaffService = Container.Resolve<IStaffService>();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetList(GridPager pager, StaffSearch staffSearch)
        {
            List<Staff> list = new List<Staff>();
            DateTime defaultDateTime = new DateTime(1, 1, 1, 0, 0, 0);
            //staffSearch.name = "";
            /*
             * 初始化页面:只有pager接收到浏览器传来的参数，staffSearch中的属性均为默认值。
             */
            if (staffSearch.name == null && staffSearch.date_from.Value == defaultDateTime && staffSearch.date_to == defaultDateTime)
            {
                pager.totalRows = StaffService.GetModels(c => true).ToList().Count();
                //初始化页面根据Id排序
                list = StaffService.GetModelsByPage(pager.rows, pager.page, pager.isAsc, c => c.Id, c => true).ToList();
            }
            /*
             * 查询页面：staffSearch接收到浏览器传来的值，staffSearch中的属性均变为自定义的默认值。
             */
            else
            {
                //先想一想SQL怎么写，再写Lambda表达式
                Expression<Func<Staff, bool>> whereLambda = u =>
               (u.CreateTime >= staffSearch.date_from && u.CreateTime <= staffSearch.date_to)
               && u.Name.Contains(staffSearch.name);
                //查询页面根据名称排序
                Expression<Func<Staff, string>> orderByLambda = c => c.Name;

                pager.totalRows = StaffService.GetModels(whereLambda).ToList().Count();
                list = StaffService.GetModelsByPage(pager.rows, pager.page, pager.isAsc, orderByLambda, whereLambda).ToList();
                
            }

            var json = new
            {
                total = pager.totalRows,
                rows = (from r in list
                        select new Staff()
                        {
                            Id = r.Id,
                            Name = r.Name,
                            Age = r.Age,
                            Sex = r.Sex,
                            CreateTime = r.CreateTime,
                        }).ToArray()
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}