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
    public class CategoryController : Controller
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private EasyPayContext db = new EasyPayContext();

        //
        // GET: /Category/

        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        //
        // GET: /Category/Details/5
        /// <summary>
        /// This mehod will give details for seleced caegoty.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id = 0)
        {
            logger.Info("Details Method Start" + " at " + DateTime.UtcNow);
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                logger.Info("Details Method Category is null" + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Details Method End" + " at " + DateTime.UtcNow);
            return View(category);
        }

        //
        // GET: /Category/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Category/Create
        /// <summary>
        /// This method will create new category.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Category category)
        {
            logger.Info("Create HttpPost Method Start" + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                logger.Info("Create HttpPost Method Category " + category.CategoryName + " at " + DateTime.UtcNow);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            logger.Info("Create HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(category);
        }

        //
        // GET: /Category/Edit/5
        /// <summary>
        /// This method will edit selected category.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            logger.Info("Edit Get Method Start" + " at " + DateTime.UtcNow);
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                logger.Info("Edit Get Method Category is null" + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Edit Get Method End" + " at " + DateTime.UtcNow);
            return View(category);
        }

        //
        // POST: /Category/Edit/5
        /// <summary>
        /// This method will update data. 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            logger.Info("Edit HttpPost Method Start" + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                logger.Info("Edit HttpPost Method Category " + category.CategoryName + " at " + DateTime.UtcNow);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            logger.Info("Edit HttpPost Method Start" + " at " + DateTime.UtcNow);
            return View(category);
        }

        //
        // GET: /Category/Delete/5
        /// <summary>
        /// This method will delete selected category.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id = 0)
        {
            logger.Info("Delete Get Method Start" + " at " + DateTime.UtcNow);
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                logger.Info("Delete Get Method Category is null" + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Delete Get Method End" + " at " + DateTime.UtcNow);
            return View(category);
        }

        //
        // POST: /Category/Delete/5
        /// <summary>
        /// This method will delete selected category.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            logger.Info("Delete HttpPost Method Start" + " at " + DateTime.UtcNow);
            Category category = db.Categories.Find(id);
            logger.Info("Delete HttpPost Method Category " + category.CategoryName + " at " + DateTime.UtcNow);
            db.Categories.Remove(category);
            db.SaveChanges();
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