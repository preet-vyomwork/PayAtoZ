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
    public class ProviderController : Controller
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private EasyPayContext db = new EasyPayContext();

        //
        // GET: /Provider/

        public ActionResult Index()
        {
            return View(db.Providers.ToList());
        }

        //
        // GET: /Provider/Details/5
        /// <summary>
        /// This mehod will give details for seleced Provider.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id = 0)
        {
            logger.Info("Details Method Start" + " at " + DateTime.UtcNow);
            Provider provider = db.Providers.Find(id);
            if (provider == null)
            {
                logger.Info("Details Method Provider not found" + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Details Method End" + " at " + DateTime.UtcNow);
            return View(provider);
        }

        //
        // GET: /Provider/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Provider/Create
        /// <summary>
        /// This method will create new Provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Provider provider)
        {
            logger.Info("Create Post Method Start" + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.Providers.Add(provider);
                db.SaveChanges();
                logger.Info("Create Post Method Provider is added " + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            logger.Info("Create Post Method End" + " at " + DateTime.UtcNow);
            return View(provider);
        }

        //
        // GET: /Provider/Edit/5

        public ActionResult Edit(int id = 0)
        {
            logger.Info("Edit Get Method Start" + " at " + DateTime.UtcNow);
            Provider provider = db.Providers.Find(id);
            if (provider == null)
            {
                logger.Info("Edit Get Method Provider not found" + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Edit Get Method End" + " at " + DateTime.UtcNow);
            return View(provider);
        }

        //
        // POST: /Provider/Edit/5
        /// <summary>
        /// This method will update data. 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Provider provider)
        {
            logger.Info("Edit HttpPost Method Start" + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.Entry(provider).State = EntityState.Modified;
                db.SaveChanges();
                logger.Info("Edit HttpPost Method provider details updated" + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            logger.Info("Edit HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(provider);
        }

        //
        // GET: /Provider/Delete/5
        /// <summary>
        /// This method will delete selected Provider.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id = 0)
        {
            logger.Info("Delete Get Method Start" + " at " + DateTime.UtcNow);
            Provider provider = db.Providers.Find(id);
            if (provider == null)
            {
                logger.Info("Delete Get Method Provider not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Delete Get Method End" + " at " + DateTime.UtcNow);
            return View(provider);
        }

        //
        // POST: /Provider/Delete/5
        /// <summary>
        /// This method will delete selected Provider.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            logger.Info("Delete HttpPost Method Start" + " at " + DateTime.UtcNow);
            Provider provider = db.Providers.Find(id);
            db.Providers.Remove(provider);
            db.SaveChanges();
            logger.Info("Delete HttpPost Method provider " +id+ " removed at " + DateTime.UtcNow);
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