using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Code.Mongo;
using Web.Code.DataExportProvider;
using Web.Code.Mongo.Entities;
using Web.Models;
using System.Threading;

namespace Web.Controllers
{
    public class DataMartController : Controller
    {
        public ActionResult Index()
        {            
            return View();
        }

        public JsonResult UpdateData(DataMartModel model)
        {
            try
            {
                model.UpdateDatabase();
                return Json(new
                {
                    Success = true
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    Success = false
                });
            }
        }

        public JsonResult GetTransactions(DataMartModel model)
        {
            return Json(model.GetTransactions(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAmounts(DataMartModel model)
        {
            return Json(model.GetAmounts(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAggregations(DataMartModel model)
        {
            return Json(model.GetAggregations(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRatings(DataMartModel model)
        {
            return Json(model.GetRatings(), JsonRequestBehavior.AllowGet);
        }
    }
}
