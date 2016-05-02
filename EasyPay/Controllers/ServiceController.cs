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
    public class ServiceController : Controller
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private EasyPayContext db = new EasyPayContext();

        //
        // GET: /Service/

        public ActionResult Index()
        {
            var providerservices = db.ProviderServices.Include(p => p.ServiceType).Include(p => p.ServiceOperator).Include(p => p.Provider);
            return View(providerservices.ToList());
        }

        //
        // GET: /Service/Details/5
        /// <summary>
        /// This mehod will give details for seleced Service.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id = 0)
        {
            logger.Info("Details Method Start" + " at " + DateTime.UtcNow);
            ProviderService providerservice = db.ProviderServices.Find(id);
            if (providerservice == null)
            {
                logger.Info("Details Method Provider service not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Details Method End" + " at " + DateTime.UtcNow);
            return View(providerservice);
        }

        //
        // GET: /Service/Create

        public ActionResult Create()
        {
            logger.Info("Create Get Method Start" + " at " + DateTime.UtcNow);
            ViewBag.ServiceTypeId = new SelectList(db.ServiceTypes, "ServiceTypeId", "ServiceTypeName");
            ViewBag.ServiceOperatorId = new SelectList(db.ServiceOperators, "ServiceOperatorId", "OperatorName");
            ViewBag.ProviderId = new SelectList(db.Providers, "ProviderId", "ProviderName");
            logger.Info("Create Get Method End" + " at " + DateTime.UtcNow);
            return View();
        }

        //
        // POST: /Service/Create
        /// <summary>
        /// This method will create new Service.
        /// </summary>
        /// <param name="providerservice"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ProviderService providerservice)
        {
            logger.Info("Create Post Method Start" + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.ProviderServices.Add(providerservice);
                db.SaveChanges();
                logger.Info("Create Post Method Provider service added" + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }

            ViewBag.ServiceTypeId = new SelectList(db.ServiceTypes, "ServiceTypeId", "ServiceTypeName", providerservice.ServiceTypeId);
            ViewBag.ServiceOperatorId = new SelectList(db.ServiceOperators, "ServiceOperatorId", "OperatorName", providerservice.ServiceOperatorId);
            ViewBag.ProviderId = new SelectList(db.Providers, "ProviderId", "ProviderName", providerservice.ProviderId);
            logger.Info("Create Post Method End" + " at " + DateTime.UtcNow);
            return View(providerservice);
        }

        //
        // GET: /Service/Edit/5
        /// <summary>
        /// This method will edit selected Service.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            logger.Info("Edit Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit Get Method Provider Service id " + id + " at " + DateTime.UtcNow);
            ProviderService providerservice = db.ProviderServices.Find(id);
            if (providerservice == null)
            {
                logger.Info("Edit Get Method Provider service not found" + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            ViewBag.ServiceTypeId = new SelectList(db.ServiceTypes, "ServiceTypeId", "ServiceTypeName", providerservice.ServiceTypeId);
            ViewBag.ServiceOperatorId = new SelectList(db.ServiceOperators, "ServiceOperatorId", "OperatorName", providerservice.ServiceOperatorId);
            ViewBag.ProviderId = new SelectList(db.Providers, "ProviderId", "ProviderName", providerservice.ProviderId);
            logger.Info("Edit Get Method End" + " at " + DateTime.UtcNow);
            return View(providerservice);
        }

        //
        // POST: /Service/Edit/5
        /// <summary>
        /// This method will update data.
        /// </summary>
        /// <param name="providerservice"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(ProviderService providerservice)
        {
            logger.Info("Edit HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit HttpPost Method Provider Service id " + providerservice.ProviderServiceId + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.Entry(providerservice).State = EntityState.Modified;
                db.SaveChanges();
                logger.Info("Edit HttpPost Method Provider service details updated " + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            ViewBag.ServiceTypeId = new SelectList(db.ServiceTypes, "ServiceTypeId", "ServiceTypeName", providerservice.ServiceTypeId);
            ViewBag.ServiceOperatorId = new SelectList(db.ServiceOperators, "ServiceOperatorId", "OperatorName", providerservice.ServiceOperatorId);
            ViewBag.ProviderId = new SelectList(db.Providers, "ProviderId", "ProviderName", providerservice.ProviderId);
            logger.Info("Edit HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(providerservice);
        }

        //
        // GET: /Service/Delete/5
        /// <summary>
        /// This method will delete selected Service.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id = 0)
        {
            logger.Info("Delete Get Method Start" + " at " + DateTime.UtcNow);
            ProviderService providerservice = db.ProviderServices.Find(id);
            if (providerservice == null)
            {
                logger.Info("Delete Get Method providerservice not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Delete Get Method End" + " at " + DateTime.UtcNow);
            return View(providerservice);
        }

        //
        // POST: /Service/Delete/5
        /// <summary>
        /// This method will delete selected Service.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            logger.Info("Delete HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete HttpPost Method Provider service id " + id + " at " + DateTime.UtcNow);
            ProviderService providerservice = db.ProviderServices.Find(id);
            db.ProviderServices.Remove(providerservice);
            db.SaveChanges();
            logger.Info("Delete HttpPost Method provider service removed " + " at " + DateTime.UtcNow);
            logger.Info("Delete HttpPost Method End" + " at " + DateTime.UtcNow);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

		[Authorize]
		public ActionResult Buy(int id)
		{
			//var album = GetAlbums().Single(a => a.AlbumId == id);
			//Charge the user and ship the album!!!
			return View();
		}
    }
}