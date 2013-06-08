using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Code.Mongo;
using Web.Code.DataExportProvider;
using Web.Code.Mongo.Entities;

namespace Web.Controllers
{
    public class DataMartController : Controller
    {
        public ActionResult Index()
        {
            return View();
            //return RedirectToAction("Login", "DataMart");
        }

        public ActionResult Login()
        {
            return RedirectToAction("Index", "DataMart");
            //return View();
        }

        [HttpPost]
        public ActionResult Login(string login, string password)
        {
            return RedirectToAction("Index", "DataMart");
            //return View("Login");
        }

        //private bool _validateUser(string id)
        //{
        //    return Database.Instance.IsUser(id);
        //}

        public void SuperAction()
        {
            DataExportProvider.Instance.Export();
        }

        public JsonResult GetTransactions()
        {
            return Json(Database.Instance.GetCollection<TransactionStats>(Database.CollectionNames.Transactions), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAmounts()
        {
            return Json(Database.Instance.GetCollection<EntityCount>(Database.CollectionNames.Counts), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAggregations()
        {
            return Json(Database.Instance.GetCollection<EntityAggregation>(Database.CollectionNames.Aggregations), JsonRequestBehavior.AllowGet);
        }
    }
}
