using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyPay.Models;
using log4net;

namespace EasyPay.Controllers
{
    public class ServiceOperatorController : Controller
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private EasyPayContext db = new EasyPayContext() ;

        //
        // GET: /ServiceOperator/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var serviceoperators = db.ServiceOperators.Include(s => s.ServiceType);
            return View(serviceoperators.ToList());
        }

        //
        // GET: /ServiceOperator/Details/5
        /// <summary>
        /// This mehod will give details for seleced Service Operator.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id = 0)
        {
            logger.Info("Details Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Details Method ServiceOperators Id" + id + " at " + DateTime.UtcNow);
            ServiceOperator serviceoperator = db.ServiceOperators.Find(id);
            if (serviceoperator == null)
            {
                logger.Info("Details Method ServiceOperators not found at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Details Method End" + " at " + DateTime.UtcNow);
            return View(serviceoperator);
        }

        //
        // GET: /ServiceOperator/Create

        public ActionResult Create()
        {
            ViewBag.ServiceTypeId = new SelectList(db.ServiceTypes, "ServiceTypeId", "ServiceTypeName");
            return View();
        }

        //
        // POST: /ServiceOperator/Create
        /// <summary>
        /// This method will create new Service Operator.
        /// </summary>
        /// <param name="serviceoperator"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ServiceOperator serviceoperator)
        {
            logger.Info("Create HttpPost Method Start" + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.ServiceOperators.Add(serviceoperator);
                db.SaveChanges();
                logger.Info("Create HttpPost Method Service Operator added" + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }

            ViewBag.ServiceTypeId = new SelectList(db.ServiceTypes, "ServiceTypeId", "ServiceTypeName", serviceoperator.ServiceTypeId);
            logger.Info("Create HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(serviceoperator);
        }

        //
        // GET: /ServiceOperator/Edit/5
        /// <summary>
        /// This method will edit selected Service Operator.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            logger.Info("Edit Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit Get Method Serviceoperator Id " + id + " at " + DateTime.UtcNow);
            ServiceOperator serviceoperator = db.ServiceOperators.Find(id);
            if (serviceoperator == null)
            {
                logger.Info("Edit Get Method ServiceOperator not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            ViewBag.ServiceTypeId = new SelectList(db.ServiceTypes, "ServiceTypeId", "ServiceTypeName", serviceoperator.ServiceTypeId);
            logger.Info("Edit Get Method End" + " at " + DateTime.UtcNow);
            return View(serviceoperator);
        }

        //
        // POST: /ServiceOperator/Edit/5
        /// <summary>
        /// This method will update data.
        /// </summary>
        /// <param name="serviceoperator"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(ServiceOperator serviceoperator)
        {
            logger.Info("Edit HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit HttpPost Method Serviceoperator Id "+serviceoperator.ServiceOperatorId + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.Entry(serviceoperator).State = EntityState.Modified;
                db.SaveChanges();
                logger.Info("Edit HttpPost Method ServiceOperator details updated " + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            ViewBag.ServiceTypeId = new SelectList(db.ServiceTypes, "ServiceTypeId", "ServiceTypeName", serviceoperator.ServiceTypeId);
            logger.Info("Edit HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(serviceoperator);
        }

        //
        // GET: /ServiceOperator/Delete/5
        /// <summary>
        /// This method will delete selected Service Operator.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id = 0)
        {
            logger.Info("Delete Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete Get Method ServiceOperator Id" + id + " at " + DateTime.UtcNow);
            ServiceOperator serviceoperator = db.ServiceOperators.Find(id);
            if (serviceoperator == null)
            {
                logger.Info("Delete Get Method ServiceOperator not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Delete Get Method End" + " at " + DateTime.UtcNow);
            return View(serviceoperator);
        }

        //
        // POST: /ServiceOperator/Delete/5
        /// <summary>
        /// This method will delete selected Service Operator.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            logger.Info("Delete HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete HttpPost Method ServiceOperator Id" + id + " at " + DateTime.UtcNow);
            ServiceOperator serviceoperator = db.ServiceOperators.Find(id);
            db.ServiceOperators.Remove(serviceoperator);
            db.SaveChanges();
            logger.Info("Delete HttpPost Method ServiceOperator removed at " + DateTime.UtcNow);
            logger.Info("Delete HttpPost Method End" + " at " + DateTime.UtcNow);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}