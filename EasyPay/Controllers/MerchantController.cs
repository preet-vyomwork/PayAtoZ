using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyPay.Models;
using log4net;
using System.IO;
using ServiceEngine;
using System.Web.Security;
using WebMatrix.WebData;

namespace EasyPay.Controllers
{
    public class MerchantController : Controller
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        Utils utils = new Utils();
        private EasyPayContext db = new EasyPayContext();

        //
        // GET: /Merchant/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            logger.Info("Method Start" + " at " + DateTime.UtcNow);
            var merchants = db.Merchants.Include(m => m.Category);
            logger.Info("Method Start" + " at " + DateTime.UtcNow);
            return View(merchants.ToList());
            //return View();
        }
        public ActionResult DashBoard(int id = 0)
        {
            logger.Info("Method Start" + " at " + DateTime.UtcNow);
            logger.Info("User Profile Id " + id + " at " + DateTime.UtcNow);
            Merchant merchant = new Merchant();
            if (id != 0)
            {
                merchant = db.Merchants.Where(m => m.UserProfileId == id).FirstOrDefault();
                if (merchant == null)
                {
                    logger.Info("Merchant not found " + " at " + DateTime.UtcNow);
                    return HttpNotFound();
                }
            }
            logger.Info("Method End" + " at " + DateTime.UtcNow);
            return View(merchant);
            //return View();
        }

        //
        // GET: /Merchant/Details/5
        /// <summary>
        /// This mehod will give details for seleced Merchant.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id = 0)
        {
            logger.Info("Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Merchant Id " + id + " at " + DateTime.UtcNow);
            Merchant merchant = db.Merchants.Find(id);
            if (merchant == null)
            {
                logger.Info("Merchant not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Method End" + " at " + DateTime.UtcNow);
            return View(merchant);
        }

        //
        // GET: /Merchant/Create

        public ActionResult Create()
        {
            logger.Info("Create Get Method Start" + " at " + DateTime.UtcNow);
            ViewBag.State = new SelectList(db.States, "StateID", "StateName");
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");

            UserMerchantViewModel userMerchantViewModel = new UserMerchantViewModel();
            RegisterModel registerModel = new RegisterModel();
            Merchant merchant = new Merchant();
            userMerchantViewModel.Merchant = merchant;
            userMerchantViewModel.RegisterModel = registerModel;
            logger.Info("Create Get Method end" + " at " + DateTime.UtcNow);
            return View(userMerchantViewModel);
        }
        //
        // POST: /Merchant/Create
        /// <summary>
        /// This method will create new Merchant.
        /// </summary>
        /// <param name="merchant"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(UserMerchantViewModel userMerchantViewModel, HttpPostedFileBase file)
        {
            logger.Info("Create Post Method Start" + " at " + DateTime.UtcNow);
            EasyPayContext dbContext = new EasyPayContext();
            string path = utils.SaveFile(file);
            Merchant thisMerchant = new Merchant();
            RegisterModel registerModel = new RegisterModel();

            if (userMerchantViewModel != null)
            {
                thisMerchant.Address = registerModel.Address = userMerchantViewModel.Merchant.Address;
                thisMerchant.CategoryId = userMerchantViewModel.Merchant.CategoryId;
                thisMerchant.City = registerModel.City = userMerchantViewModel.Merchant.City;
                thisMerchant.ContactNo = userMerchantViewModel.Merchant.ContactNo;
                thisMerchant.EmailId = registerModel.Email = userMerchantViewModel.Merchant.EmailId;
                thisMerchant.LogoImagePath = path;
                thisMerchant.MerchantName = userMerchantViewModel.Merchant.MerchantName;
                thisMerchant.PostalCode = registerModel.PostalCode = userMerchantViewModel.Merchant.PostalCode;
                thisMerchant.State_StateID = Convert.ToInt32(userMerchantViewModel.RegisterModel.StateID);
                thisMerchant.Terms = userMerchantViewModel.Merchant.Terms;
                thisMerchant.URL = userMerchantViewModel.Merchant.URL;


                registerModel.UserName = userMerchantViewModel.RegisterModel.UserName;
                registerModel.StateID = userMerchantViewModel.RegisterModel.StateID;
                registerModel.Phone = userMerchantViewModel.Merchant.ContactNo;
                registerModel.Password = userMerchantViewModel.RegisterModel.Password;
                registerModel.ConfirmPassword = userMerchantViewModel.RegisterModel.Password;

                AccountController AccountController = new Controllers.AccountController();

                string dbToken = AccountController.RegisterUser(registerModel);
                var userId = dbContext.UserProfiles.Where(p => p.UserName == registerModel.UserName).Select(u => u.UserProfileId).FirstOrDefault();
                thisMerchant.UserProfileId = userId;
                dbContext.Merchants.Add(thisMerchant);
                WebSecurity.ConfirmAccount(userMerchantViewModel.RegisterModel.UserName, dbToken);

                dbContext.SaveChanges();
                logger.Info("Create Post Method New Merchant Created " + " at " + DateTime.UtcNow);
                //EasyPayContext dbContext1 = new EasyPayContext();
                //EasyPay.Models.Membership membership = dbContext1.Memberships.Where(m => m.UserId == userId).FirstOrDefault();
                ////var confirm = dbContext1.Memberships.Where(m => m.UserId == userId).FirstOrDefault();
                //if (membership.IsConfirmed == false)
                //{

                //    membership.IsConfirmed = true;
                //    dbContext1.SaveChanges();
                //}
                EasyPayContext dbContext2 = new EasyPayContext();
                string newRoleName = "Merchant";
                if (!Roles.RoleExists(newRoleName))
                {
                    logger.Info("Create new Role Merchant" + " at " + DateTime.UtcNow);
                    Roles.CreateRole(newRoleName);
                }
                var checkUserRole = dbContext2.UsersInRoles.ToList().FirstOrDefault(r => r.UserId == userId);
                if (checkUserRole == null)
                {
                    logger.Info("Assign role Merchant to User " + userId + " at " + DateTime.UtcNow);
                    dbContext2.UsersInRoles.Add(new UsersInRole()
                    {
                        RoleId = dbContext2.Roles.Where(r => r.RoleName == newRoleName).Select(r => r.RoleId).FirstOrDefault(),
                        UserId = userId
                        //RoleId=Roles.
                    });
                    dbContext2.SaveChanges();
                }
                logger.Info("Method End" + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(dbContext.Categories, "CategoryId", "CategoryName", userMerchantViewModel.Merchant.CategoryId);
            logger.Info("Create Method End" + " at " + DateTime.UtcNow);
            return View(userMerchantViewModel);
            //return View();
        }

        //
        // GET: /Merchant/Edit/5
        /// <summary>
        /// This method will edit selected coupon.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            logger.Info("Edit Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit Get Method Mrechant Id " + id + " at " + DateTime.UtcNow);
            Merchant merchant = db.Merchants.Find(id);
            if (merchant == null)
            {
                return HttpNotFound();
            }
            UserMerchantViewModel userMerchantViewModel = new UserMerchantViewModel();
            RegisterModel registerModel = new RegisterModel();
            //Merchant merchant = new Merchant();
            userMerchantViewModel.Merchant = merchant;
            userMerchantViewModel.RegisterModel = registerModel;

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", merchant.CategoryId);

            ViewBag.States = db.States.Select(r => r.StateName).ToList();
            logger.Info("Edit Get Method End" + " at " + DateTime.UtcNow);
            return View(userMerchantViewModel);
        }

        //
        // POST: /Merchant/Edit/5
        /// <summary>
        /// This method will update data.
        /// </summary>
        /// <param name="merchant"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(UserMerchantViewModel userMerchantViewModel, HttpPostedFileBase file)
        {
            logger.Info("Edit HttpPost Method Start" + " at " + DateTime.UtcNow);
            Merchant merchant = db.Merchants.Where(m => m.MerchantId == userMerchantViewModel.Merchant.MerchantId).FirstOrDefault();

            try
            {
                //Merchant merchant = new Merchant();

                if (file != null)
                {
                    string path = utils.SaveFile(file);
                    merchant.LogoImagePath = path;
                }
                merchant.Address = userMerchantViewModel.Merchant.Address;
                merchant.CategoryId = userMerchantViewModel.Merchant.CategoryId;
                merchant.City = userMerchantViewModel.Merchant.City;
                merchant.ContactNo = userMerchantViewModel.Merchant.ContactNo;
                merchant.EmailId = userMerchantViewModel.Merchant.EmailId;
                merchant.PostalCode = userMerchantViewModel.Merchant.PostalCode;
                merchant.State_StateID = Convert.ToInt32(userMerchantViewModel.RegisterModel.StateID);
                merchant.Terms = userMerchantViewModel.Merchant.Terms;
                merchant.URL = userMerchantViewModel.Merchant.URL;
                // merchant.UserProfileId = db.Merchants.Where(m => m.MerchantId == userMerchantViewModel.Merchant.MerchantId).Select(n => n.UserProfileId).FirstOrDefault();

                db.SaveChanges();
                logger.Info("Edit HttpPost Method Merchant Id " + merchant.MerchantId + " Updated at " + DateTime.UtcNow);
                ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", merchant.CategoryId);
                logger.Info("Edit HttpPost Method End" + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.Error("In HomeController - Index Method Error: " + ex.InnerException + " " + ex.StackTrace + " " + ex.Message + " at " + DateTime.UtcNow);
                throw ex; 
            }
            return View(userMerchantViewModel);
        }

        //
        // GET: /Merchant/Delete/5
        /// <summary>
        /// This method will delete selected Merchant.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id = 0)
        {
            logger.Info("Delete Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete Get Method Merchant Id " + id + " at " + DateTime.UtcNow);
            Merchant merchant = db.Merchants.Find(id);
            if (merchant == null)
            {
                logger.Info("Delete Get Method Merchant not found" + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            else
            {
                db.Merchants.Remove(merchant);
                db.SaveChanges();
                logger.Info("Delete Get Method Merchant Id" + id + " is removed at " + DateTime.UtcNow);
            }
            logger.Info("Delete Get Method End" + " at " + DateTime.UtcNow);
            return RedirectToAction("Index");
        }

        //
        // POST: /Merchant/Delete/5
        /// <summary>
        /// This method will delete selected Merchant.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            logger.Info("Delete HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete HttpPost Method Merchant Id " + id + " at " + DateTime.UtcNow);
            Merchant merchant = db.Merchants.Find(id);
            db.Merchants.Remove(merchant);
            db.SaveChanges();
            logger.Info("Delete Get Method Merchant Id" + id + " is removed at " + DateTime.UtcNow);
            logger.Info("Delete HttpPost Method End" + " at " + DateTime.UtcNow);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        //public string SaveFile(HttpPostedFileBase file)
        //{
        //    string path = "";
        //    if (file != null && file.ContentLength > 0)
        //        try
        //        {
        //            path = Path.Combine(Server.MapPath("~/Images"),
        //                                       Path.GetFileName(file.FileName));
        //            file.SaveAs(path);
        //            ViewBag.Message = "File uploaded successfully";
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = "ERROR:" + ex.Message.ToString();
        //        }
        //    else
        //    {
        //        ViewBag.Message = "You have not specified a file.";
        //    }
        //    return path.Substring(path.IndexOf("\\Images"));
        //}
    }
}