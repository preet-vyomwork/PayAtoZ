using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyPay.Models;
using ServiceEngine;
using System.Web.Security;
using log4net;

namespace EasyPay.Controllers
{
    public class OrderListController : Controller
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private EasyPayContext db = new EasyPayContext();

        //
        // GET: /OrderList/
        //[Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.UserProfile).Include(o => o.ProviderService).Where(x => x.OrderDate.Year == DateTime.Now.Year && x.OrderDate.Month == DateTime.Now.Month).Where(x => x.Status != 4);
            return View(orders.ToList());


            //foreach (Order order in orders)
            //{
            //    db.Entry(order).State = EntityState.Modified;
            //    ServiceHelper serviceHelper = new ServiceHelper();
            //    string[] paramString = order.Remarks.Split(',');
            //    string transactionNumber = string.Empty;
            //    if (paramString.Count() > 1)
            //    {
            //        transactionNumber = paramString[1];
            //    }
            //    else
            //    {
            //        transactionNumber = paramString[0];
            //    }
            //    db.Entry(order).State = EntityState.Modified;
            //    string result = serviceHelper.GetTransactionResult(transactionNumber);
            //    ProcessResult(result, order);
            //}
            //db.SaveChanges();
            
        }


        //
        // GET: /OrderList/Details/5
        /// <summary>
        /// This mehod will give details for seleced Order List.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id = 0)
        {
            logger.Info("Details Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Details Method Order Id "+id + " at " + DateTime.UtcNow);
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                logger.Info("Details Method Order not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Details Method End" + " at " + DateTime.UtcNow);
            return View(order);
        }

        //
        // GET: /OrderList/Status/5

        public ActionResult Status(int id = 0)
        {
            logger.Info("Status Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Status Method Order Id "+id + " at " + DateTime.UtcNow);

            Order order = db.Orders.Find(id);
            if (order == null)
            {
                logger.Info("Status Method Orer not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", order.UserProfileId);
            ViewBag.ProviderServiceId = new SelectList(db.ProviderServices, "ProviderServiceId", "Format", order.ProviderServiceId);
            logger.Info("Status Method End" + " at " + DateTime.UtcNow);
            return View(order);
        }

        //
        // POST: /OrderList/Status/5

        [HttpPost]
        public ActionResult Status(Order order)
        {
            logger.Info("Status HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Status HttpPost Method Order Id "+order.OrderId + " at " + DateTime.UtcNow);
            if (ModelState.IsValid == false)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                logger.Info("Status HttpPost Method Status updated " + " at " + DateTime.UtcNow);
                logger.Info("Status HttpPost Method End " + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", order.UserProfileId);
            ViewBag.ProviderServiceId = new SelectList(db.ProviderServices, "ProviderServiceId", "Format", order.ProviderServiceId);
            logger.Info("Status HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(order);

            //int str = order.Status;
            //var rec = (from i in db.Orders
            //           where order.Status == str
            //           select i).FirstOrDefault();

            //if (rec != null)
            //{
            //    rec.Status = str;
                
            //    //db.Entry(order).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            //if (ModelState.IsValidField("Status"))
            //{
            //    db.Entry(order).State = EntityState.Modified;
            //    var query = (from str in db.Orders
            //                 where str.Status == st
            //                 select str).SingleOrDefault();
            //    ec.Orders.Add(query);
            //    ec.SaveChanges();

            //}
            //if (ModelState.IsValid)
            //{
            //    db.Entry(order).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            //ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", order.UserProfileId);
            //ViewBag.ProviderServiceId = new SelectList(db.ProviderServices, "ProviderServiceId", "Format", order.ProviderServiceId);
            
        }

        //
        // GET: /OrderList/Create

        public ActionResult Create()
        {
            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName");
            ViewBag.ProviderServiceId = new SelectList(db.ProviderServices, "ProviderServiceId", "Format");
            return View();
        }

        //
        // POST: /OrderList/Create
        /// <summary>
        /// This method will create new Order List.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Order order)
        {
            logger.Info("Create Method Start" + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                logger.Info("Create Method Order Created " + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }

            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", order.UserProfileId);
            ViewBag.ProviderServiceId = new SelectList(db.ProviderServices, "ProviderServiceId", "Format", order.ProviderServiceId);
            logger.Info("Create Method End" + " at " + DateTime.UtcNow);
            return View(order);
        }

        //
        // GET: /OrderList/Edit/5
        /// <summary>
        /// This method will edit selected Order List.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            logger.Info("Edit Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit Get Method Order id "+id  + " at " + DateTime.UtcNow);
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                logger.Info("Edit Get Method Order not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", order.UserProfileId);
            ViewBag.ProviderServiceId = new SelectList(db.ProviderServices, "ProviderServiceId", "Format", order.ProviderServiceId);
            logger.Info("Edit Get Method End" + " at " + DateTime.UtcNow);
            return View(order);
        }

        //
        // POST: /OrderList/Edit/5

        [HttpPost]
        public ActionResult Edit(Order order)
        {
            logger.Info("Edit HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit HttpPost Method Order "+order.OrderId + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                logger.Info("Edit HttpPost Method Order updated " + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", order.UserProfileId);
            ViewBag.ProviderServiceId = new SelectList(db.ProviderServices, "ProviderServiceId", "Format", order.ProviderServiceId);
            logger.Info("Edit HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(order);
        }

        //
        // GET: /OrderList/Delete/5
        /// <summary>
        /// This method will delete selected Order List.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id = 0)
        {
            logger.Info("Delete Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete Get Method Order Id "+id + " at " + DateTime.UtcNow);
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                logger.Info("Delete Get Method Order not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Delete Get Method Start" + " at " + DateTime.UtcNow);
            return View(order);
        }

        //
        // POST: /OrderList/Delete/5
        /// <summary>
        /// This method will delete selected Order List.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            logger.Info("Delete HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete HttpPost Method Order Id "+id + " at " + DateTime.UtcNow);
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            logger.Info("Delete HttpPost Method Order details removed " + " at " + DateTime.UtcNow);
            logger.Info("Delete HttpPost Method End" + " at " + DateTime.UtcNow);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private void ProcessResult(string result, Order order)
        {
            logger.Info("ProcessResult Method Start" + " at " + DateTime.UtcNow);
            string[] resultParams = result.Split(',');
            if (resultParams.Count() > 1)
            {
                logger.Info("Result " + resultParams[1].ToString() + " at " + DateTime.UtcNow);
                //MembershipUser membershipUser = Membership.GetUser();
                //order.UserProfileId = Convert.ToInt32(membershipUser.ProviderUserKey);
                //var userWallet = db.UserWallets.ToList();
                UserWallet userwallet = db.UserWallets.FirstOrDefault(x => x.UserProfileId == order.UserProfileId);
                //order.Remarks = resultParams[0];// "AZ" + order.OrderId.ToString() + GetUniqueKey();
                if (resultParams[1].ToString().ToLower().Equals("success"))
                {
                    order.Status = (int)OrderStatus.Success;
                }
                else if (resultParams[1].ToString().ToLower().Equals("failed"))
                {
                    order.Status = (int)OrderStatus.Failed;
                    userwallet.Balance += order.Amount;
                }
                else if (resultParams[1].ToString().ToLower().Equals("pending"))
                {
                    order.Status = (int)OrderStatus.Pending;
                }
                else if (resultParams[1].ToString().ToLower().Equals("refund"))
                {
                    order.Status = (int)OrderStatus.Refunded;
                    userwallet.Balance += order.Amount;
                }
            }
            logger.Info("ProcessResult Method End " + " at " + DateTime.UtcNow);
        }
    }
}