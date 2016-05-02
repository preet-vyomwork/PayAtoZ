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
    public class ServiceOperatorPlanController : Controller
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private EasyPayContext db = new EasyPayContext();

        //
        // GET: /ServiceOperatorPlan/

        public ActionResult Index()
        {
            logger.Info("Method Start" + " at " + DateTime.UtcNow);
            var serviceoperatorplans = db.ServiceOperatorPlans.Include(s => s.ServiceOperator).Include(s => s.State);
            logger.Info("Method End " + " at " + DateTime.UtcNow);
            return View(serviceoperatorplans.ToList());
        }

        //
        // GET: /ServiceOperatorPlan/Details/5

        public ActionResult Details(int id = 0)
        {
            logger.Info("Method Start" + " at " + DateTime.UtcNow);
            logger.Info("ServiceOperator Plan Id" + id + " at " + DateTime.UtcNow);
            ServiceOperatorPlan serviceoperatorplan = db.ServiceOperatorPlans.Find(id);
            if (serviceoperatorplan == null)
            {
                logger.Info("Service Operator Plan not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Method End " + " at " + DateTime.UtcNow);
            return View(serviceoperatorplan);
        }

        //
        // GET: /ServiceOperatorPlan/Create

        public ActionResult Create()
        {
            logger.Info("Create Get Method Start" + " at " + DateTime.UtcNow);
            ViewBag.ServiceOperatorId = new SelectList(db.ServiceOperators, "ServiceOperatorId", "OperatorName");
            ViewBag.StateId = new SelectList(db.States, "StateID", "StateName");
            logger.Info("Create Get Method End " + " at " + DateTime.UtcNow);
            return View();
        }

        //
        // POST: /ServiceOperatorPlan/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ServiceOperatorPlan serviceoperatorplan)
        {
            logger.Info("Create Post Method Start" + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.ServiceOperatorPlans.Add(serviceoperatorplan);
                db.SaveChanges();
                logger.Info("Create Post Method new serviceoperatorplan created " + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }

            ViewBag.ServiceOperatorId = new SelectList(db.ServiceOperators, "ServiceOperatorId", "OperatorName", serviceoperatorplan.ServiceOperatorId);
            ViewBag.StateId = new SelectList(db.States, "StateID", "StateName", serviceoperatorplan.StateId);
            logger.Info("Create Post Method End " + " at " + DateTime.UtcNow);
            return View(serviceoperatorplan);
        }

        //
        // GET: /ServiceOperatorPlan/Edit/5

        public ActionResult Edit(int id = 0)
        {
            logger.Info("Edit Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit Get Method ServiceOperatorPlan Id " + id + " at " + DateTime.UtcNow);
            ServiceOperatorPlan serviceoperatorplan = db.ServiceOperatorPlans.Find(id);
            if (serviceoperatorplan == null)
            {
                logger.Info("Edit Get Method serviceoperatorplan not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            ViewBag.ServiceOperatorId = new SelectList(db.ServiceOperators, "ServiceOperatorId", "OperatorName", serviceoperatorplan.ServiceOperatorId);
            ViewBag.StateId = new SelectList(db.States, "StateID", "StateName", serviceoperatorplan.StateId);
            logger.Info("Edit Get Method End " + " at " + DateTime.UtcNow);
            return View(serviceoperatorplan);
        }

        //
        // POST: /ServiceOperatorPlan/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ServiceOperatorPlan serviceoperatorplan)
        {
            logger.Info("Edit Post Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit Post Method serviceoperatorplan Id" + serviceoperatorplan.ServiceOperatorId + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.Entry(serviceoperatorplan).State = EntityState.Modified;
                db.SaveChanges();
                logger.Info("Edit Post Method serviceoperatorplan details updated " + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            ViewBag.ServiceOperatorId = new SelectList(db.ServiceOperators, "ServiceOperatorId", "OperatorName", serviceoperatorplan.ServiceOperatorId);
            ViewBag.StateId = new SelectList(db.States, "StateID", "StateName", serviceoperatorplan.StateId);
            logger.Info("Edit Post Method End " + " at " + DateTime.UtcNow);
            return View(serviceoperatorplan);
        }

        //
        // GET: /ServiceOperatorPlan/Delete/5

        public ActionResult Delete(int id = 0)
        {
            logger.Info("Delete Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete Get Method serviceoperatorplan Id" + id + " at " + DateTime.UtcNow);
            ServiceOperatorPlan serviceoperatorplan = db.ServiceOperatorPlans.Find(id);
            if (serviceoperatorplan == null)
            {
                logger.Info("Delete Get Method serviceoperatorplan not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Delete Get Method End " + " at " + DateTime.UtcNow);
            return View(serviceoperatorplan);
        }

        //
        // POST: /ServiceOperatorPlan/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            logger.Info("DeleteConfirmed Post Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete Get Method serviceoperatorplan Id" + id + " at " + DateTime.UtcNow);
            ServiceOperatorPlan serviceoperatorplan = db.ServiceOperatorPlans.Find(id);
            db.ServiceOperatorPlans.Remove(serviceoperatorplan);
            db.SaveChanges();
            logger.Info("DeleteConfirmed Post serviceoperatorplan removed " + " at " + DateTime.UtcNow);
            logger.Info("DeleteConfirmed Post Method End " + " at " + DateTime.UtcNow);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}