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
    public class ServiceTypeController : Controller
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private EasyPayContext db = new EasyPayContext() ;

        //
        // GET: /ServiceType/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.ServiceTypes.ToList());
        }

        //
        // GET: /ServiceType/Details/5
        /// <summary>
        /// This mehod will give details for selected Service Type.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id = 0)
        {
            logger.Info("Details Method Start" + " at " + DateTime.UtcNow);
            ServiceType servicetype = db.ServiceTypes.Find(id);
            if (servicetype == null)
            {
                return HttpNotFound();
            }
            logger.Info("Details Method End" + " at " + DateTime.UtcNow);
            return View(servicetype);
        }

        //
        // GET: /ServiceType/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ServiceType/Create
        /// <summary>
        /// This method will create new Service type.
        /// </summary>
        /// <param name="servicetype"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ServiceType servicetype)
        {
            logger.Info("Create HttpPost Method Start" + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.ServiceTypes.Add(servicetype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            logger.Info("Create HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(servicetype);
        }

        //
        // GET: /ServiceType/Edit/5
        /// <summary>
        /// This method will edit selected Service Type.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            logger.Info("Edit Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit Get Method Service type id" + id + " at " + DateTime.UtcNow);
            ServiceType servicetype = db.ServiceTypes.Find(id);
            if (servicetype == null)
            {
                logger.Info("Edit Get Method Service type not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Edit Get Method End" + " at " + DateTime.UtcNow);
            return View(servicetype);
        }

        //
        // POST: /ServiceType/Edit/5
        /// <summary>
        /// This method will update data.
        /// </summary>
        /// <param name="servicetype"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(ServiceType servicetype)
        {
            logger.Info("Edit HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit HttpPost Method ServiceType Id"+servicetype.ServiceTypeId + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.Entry(servicetype).State = EntityState.Modified;
                db.SaveChanges();
                logger.Info("Edit HttpPost Method Service Type details updated " + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            logger.Info("Edit HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(servicetype);
        }

        //
        // GET: /ServiceType/Delete/5
        /// <summary>
        /// This method will delete selected Service Type.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id = 0)
        {
            logger.Info("Delete Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete Get Method ServiceType Id"+id + " at " + DateTime.UtcNow);
            ServiceType servicetype = db.ServiceTypes.Find(id);
            if (servicetype == null)
            {
                logger.Info("Delete Get Method Service Type not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Delete Get Method End" + " at " + DateTime.UtcNow);
            return View(servicetype);
        }

        //
        // POST: /ServiceType/Delete/5
        /// <summary>
        /// This method will delete selected Service Type.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            logger.Info("DeleteConfirmed Method Start" + " at " + DateTime.UtcNow);
            logger.Info("DeleteConfirmed Method ServiceType Id"+id + " at " + DateTime.UtcNow);
            ServiceType servicetype = db.ServiceTypes.Find(id);
            db.ServiceTypes.Remove(servicetype);
            db.SaveChanges();
            logger.Info("DeleteConfirmed Method Service Type removed " + " at " + DateTime.UtcNow);
            logger.Info("DeleteConfirmed Method End" + " at " + DateTime.UtcNow);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}