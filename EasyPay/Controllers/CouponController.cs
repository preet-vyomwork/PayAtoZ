using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyPay.Models;
using log4net;
using ServiceEngine;

namespace EasyPay.Controllers
{
    public class CouponController : Controller
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        Utils utils = new Utils();
        

        //
        // GET: /Coupon/

        public ActionResult Index(int id = 0)
        {
            EasyPayContext context = new EasyPayContext();
            if (id == 0)
            {
                var coupons = from c in context.Coupons select c;
                return View(coupons.ToList());
            }
            else
            {
                var query = context.Coupons.Where(c => c.MerchantId == id);
                return View(query.ToList());
            }
        }

        public ActionResult CopyCoupon(int id = 0)
        {
            EasyPayContext context = new EasyPayContext();
            ViewBag.CategoryId = new SelectList(context.Categories, "CategoryId", "CategoryName");
            Coupon coupon = context.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }

            return View(coupon);
        }
        [HttpPost]
        public ActionResult CopyCoupon(Coupon thisCoupon, int id = 0)
        {
            int paramId = 0;
            EasyPayContext context = new EasyPayContext();
            Coupon coupon = context.Coupons.Find(id);

            Merchant merchant = new Merchant { MerchantId = coupon.MerchantId };
            context.Merchants.Attach(merchant);

            Category category = new Category { CategoryId = coupon.Category_CategoryId };
            context.Categories.Attach(category);
            Coupon newCoupon = new Coupon()
            {
                AdditionalFees = coupon.AdditionalFees,
                AvailableCoupons = thisCoupon.TotalCoupons,
                Category = category,
                CouponCode = coupon.CouponCode,
                CouponImagepath = coupon.CouponImagepath,
                CouponsUsed = coupon.CouponsUsed,
                CouponTitle = coupon.CouponTitle,
                CouponValue = coupon.CouponValue,
                Description = coupon.Description,
                IsValid = coupon.IsValid,
                Merchant = merchant,
                MerchantId=merchant.MerchantId,
                Published = coupon.Published,
                Store = coupon.Store,
                Terms = coupon.Terms,
                TotalCoupons = thisCoupon.TotalCoupons,
                Type = coupon.Type,
                Url = coupon.Url,
                ValidInCities = coupon.ValidInCities,
                ValidityEnd = coupon.ValidityEnd,
                ValidityStart = coupon.ValidityStart
            };
            CouponHistory couponHistory = new CouponHistory()
            {
                AdditionalFees = coupon.AdditionalFees,

                Category = category,
                CouponCode = coupon.CouponCode,
                CouponImagepath = coupon.CouponImagepath,
                CouponId = newCoupon.CouponId,
                CouponsAddedDate = DateTime.Today,
                CouponTitle = coupon.CouponTitle,
                CouponValue = coupon.CouponValue,
                Description = coupon.Description,
                IsValid = coupon.IsValid,
                Merchant = merchant,
                Published = coupon.Published,
                Store = coupon.Store,
                Terms = coupon.Terms,
                TotalCouponsAdded = newCoupon.TotalCoupons,
                Type = coupon.Type,
                Url = coupon.Url,
                ValidInCities = coupon.ValidInCities,
                ValidityEnd = coupon.ValidityEnd,
                ValidityStart = coupon.ValidityStart
            };
            context.CouponHistories.Add(couponHistory);
            context.Coupons.Add(newCoupon);
            context.SaveChanges();
            paramId = (id == 0) ? 0 : newCoupon.MerchantId;
            return RedirectToAction("Index", new {id = paramId });
        }

        //
        // GET: /Coupon/Details/5
        /// <summary>
        /// This mehod will give details for seleced coupon.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id = 0)
        {
            EasyPayContext context = new EasyPayContext();
            logger.Info("Details Method Start" + " at " + DateTime.UtcNow);
            Coupon coupon = context.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }
            logger.Info("Details Method End" + " at " + DateTime.UtcNow);
            return View(coupon);
        }

        //
        // GET: /Coupon/Create

        public ActionResult Create(int id = 0)
        {
            EasyPayContext context = new EasyPayContext();
            ViewBag.Merchant = ViewBag.MerchantName = null;

            ViewBag.CategoryId = new SelectList(context.Categories, "CategoryId", "CategoryName");
            if (id == 0)
            {
                ViewBag.Merchant = new SelectList(context.Merchants, "MerchantId", "MerchantName");
            }
            else
            {
                ViewBag.MerchantName = context.Merchants.Where(m => m.MerchantId == id).Select(m => m.MerchantName).FirstOrDefault();
            }
            return View();
        }

        //
        // POST: /Coupon/Create
        /// <summary>
        /// This method will create new coupon.
        /// </summary>
        /// <param name="coupon"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Coupon coupon, HttpPostedFileBase file, int id = 0)
        {
            EasyPayContext context = new EasyPayContext();
            int mId = 0;
            int paramId = 0;
            logger.Info("Create Method Start" + " at " + DateTime.UtcNow);

            string path = utils.SaveFile(file);
            if (coupon != null)
            {
                if (path != null)
                {
                    coupon.CouponImagepath = path;
                }
               
                var a = ViewBag.Merchant;
                mId = (id != 0) ? id : coupon.MerchantId;

                Merchant merchant = new Merchant { MerchantId = mId };
                context.Merchants.Attach(merchant);

                Category category = new Category { CategoryId = coupon.Category_CategoryId };
                context.Categories.Attach(category);
                Coupon newCoupon = new Coupon()
                {
                    AdditionalFees = coupon.AdditionalFees,
                    AvailableCoupons = coupon.TotalCoupons,
                    Category = category,
                    CouponCode = coupon.CouponCode,
                    CouponImagepath = path,
                    CouponsUsed = coupon.CouponsUsed,
                    CouponTitle = coupon.CouponTitle,
                    CouponValue = coupon.CouponValue,
                    Description = coupon.Description,
                    IsValid = coupon.IsValid,
                    Merchant = merchant,
                    MerchantId=merchant.MerchantId,
                    Published = coupon.Published,
                    Store = coupon.Store,
                    Terms = coupon.Terms,
                    TotalCoupons = coupon.TotalCoupons,
                    Type = coupon.Type,
                    Url = coupon.Url,
                    ValidInCities = coupon.ValidInCities,
                    ValidityEnd = coupon.ValidityEnd,
                    ValidityStart = coupon.ValidityStart
                };
                CouponHistory couponHistory = new CouponHistory()
                {
                    AdditionalFees = coupon.AdditionalFees,

                    Category = category,
                    CouponCode = coupon.CouponCode,
                    CouponImagepath = coupon.CouponImagepath,
                    CouponId = newCoupon.CouponId,
                    CouponsAddedDate = DateTime.Today,
                    CouponTitle = coupon.CouponTitle,
                    CouponValue = coupon.CouponValue,
                    Description = coupon.Description,
                    IsValid = coupon.IsValid,
                    Merchant = merchant,
                    Published = coupon.Published,
                    Store = coupon.Store,
                    Terms = coupon.Terms,
                    TotalCouponsAdded = newCoupon.TotalCoupons,
                    Type = coupon.Type,
                    Url = coupon.Url,
                    ValidInCities = coupon.ValidInCities,
                    ValidityEnd = coupon.ValidityEnd,
                    ValidityStart = coupon.ValidityStart
                };
                context.CouponHistories.Add(couponHistory);
                context.Coupons.Add(newCoupon);
                context.SaveChanges();
                //State state = new State();
                //var thisMerchant = context.Merchants.Where(m => m.MerchantId == id).FirstOrDefault();
                //var thisUserState = context.UserProfiles.Where(u => u.UserProfileId == thisMerchant.UserProfileId).Select(y => y.StateID).FirstOrDefault();
                //using (EasyPayContext context = new EasyPayContext())
                //{
                //    coupon.MerchantId = id;
                //    context.Coupons.Add(coupon);
                //    context.SaveChanges();
                //}

                //coupon.Merchant.State_StateID = thisUserState;
                //coupon.Merchant.CategoryId = thisMerchant.CategoryId;
                //coupon.Merchant.UserProfileId = thisMerchant.UserProfileId;
                //coupon.MerchantId = id;
                //coupon.Category_CategoryId = 3;
                paramId = (id == 0) ? 0 : newCoupon.MerchantId;

                return RedirectToAction("Index", new { id = paramId });
            }

            ViewBag.MerchantId = new SelectList(context.Merchants, "MerchantId", "MerchantName", coupon.MerchantId);
            logger.Info("Create Method End" + " at " + DateTime.UtcNow);
            return View(coupon);
        }

        //
        // GET: /Coupon/Edit/5
        /// <summary>
        /// This method will edit selected coupon.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            EasyPayContext context = new EasyPayContext();
            logger.Info("Edit Get Method Start" + " at " + DateTime.UtcNow);
            Coupon coupon = context.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }
            ViewBag.MerchantId = new SelectList(context.Merchants, "MerchantId", "MerchantName", coupon.MerchantId);
            logger.Info("Edit Get Method End" + " at " + DateTime.UtcNow);
            return View(coupon);
        }

        //
        // POST: /Coupon/Edit/5
        /// <summary>
        /// This method will update data.
        /// </summary>
        /// <param name="coupon"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Coupon coupon, HttpPostedFileBase file, int id=0)
        {
            int paramId = 0;
            EasyPayContext context = new EasyPayContext();
            logger.Info("Edit HttpPost Method Start" + " at " + DateTime.UtcNow);
            Merchant merchant = new Merchant { MerchantId = id };
            context.Merchants.Attach(merchant);

            Category category = new Category { CategoryId = coupon.Category_CategoryId };
            context.Categories.Attach(category);
            Coupon coup = context.Coupons.Where(c => c.CouponId == coupon.CouponId).FirstOrDefault();
            if (coupon.TotalCoupons != coup.TotalCoupons)
            {
                CouponHistory couponHistory = new CouponHistory()
                {
                    AdditionalFees = coupon.AdditionalFees,

                    Category = category,
                    CouponCode = coupon.CouponCode,
                    CouponImagepath = coupon.CouponImagepath,
                    CouponId = coupon.CouponId,
                    CouponsAddedDate = DateTime.Today,
                    CouponTitle = coupon.CouponTitle,
                    CouponValue = coupon.CouponValue,
                    Description = coupon.Description,
                    IsValid = coupon.IsValid,
                    Merchant = merchant,
                    Published = coupon.Published,
                    Store = coupon.Store,
                    Terms = coupon.Terms,
                    TotalCouponsAdded = coupon.TotalCoupons,
                    Type = coupon.Type,
                    Url = coupon.Url,
                    ValidInCities = coupon.ValidInCities,
                    ValidityEnd = coupon.ValidityEnd,
                    ValidityStart = coupon.ValidityStart
                };
                context.CouponHistories.Add(couponHistory);
            }
            coupon.TotalCoupons += coup.TotalCoupons;
            coupon.AvailableCoupons = coupon.TotalCoupons - coup.CouponsUsed;
            string path = utils.SaveFile(file);
            if (ModelState.IsValid)
            {
                if (path != null)
                {
                    coupon.CouponImagepath = path;
                }
                context.Entry(coupon).State = EntityState.Modified;

                context.SaveChanges();
                paramId = (id == 0) ? 0 : coupon.MerchantId;
                return RedirectToAction("Index", new { id = paramId });
            }
            ViewBag.MerchantId = new SelectList(context.Merchants, "MerchantId", "MerchantName", coupon.MerchantId);
            logger.Info("Edit HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(coupon);
        }

        //
        // GET: /Coupon/Delete/5
        /// <summary>
        /// This method will delete selected category.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id = 0)
        {
            EasyPayContext context = new EasyPayContext();
            logger.Info("Delete Get Method Start" + " at " + DateTime.UtcNow);
            Coupon coupon = context.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }
            else
            {
                context.Coupons.Remove(coupon);
                context.SaveChanges();
            }
            logger.Info("Delete Get Method End" + " at " + DateTime.UtcNow);
            return RedirectToAction("Index");
        }

        //
        // POST: /Coupon/Delete/5
        /// <summary>
        /// This method will delete selected category.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    logger.Info("Delete HttpPost Method Start" + " at " + DateTime.UtcNow);
        //    Coupon coupon = db.Coupons.Find(id);
        //    db.Coupons.Remove(coupon);
        //    db.SaveChanges();
        //    logger.Info("Delete HttpPost Method End" + " at " + DateTime.UtcNow);
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            EasyPayContext context = new EasyPayContext();
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}