using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyPay.Models;
using System.Collections.Specialized;
using log4net;

namespace EasyPay.Controllers
{
    public class UserWalletLogController : Controller
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private EasyPayContext db = new EasyPayContext();

        //
        // GET: /UserWalletLog/
        [Authorize(Roles = "Admin")]
        public ActionResult Index(int id)
        {
            var userwalletlog = db.UserWalletLog.Include(u => u.UserProfile).Where(x => x.UserProfileId == id);
            return View(userwalletlog.ToList());
        }

        //
        // GET: /UserWalletLog/Details/5
        /// <summary>
        /// This mehod will give details for seleced Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id = 0)
        {
            logger.Info("Details Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Details Method UserWalletLog Id" + id + " at " + DateTime.UtcNow);
            UserWalletLog userwalletlog = db.UserWalletLog.Find(id);
            if (userwalletlog == null)
            {
                logger.Info("Details Method UserWalletLog not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Details Method End" + " at " + DateTime.UtcNow);
            return View(userwalletlog);
        }

        //
        // GET: /UserWalletLog/Create
        /// <summary>
        /// This methos will be used to create new wallet.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Create(int Id = 0)
        {
            logger.Info("Create GET Method Start" + " at " + DateTime.UtcNow);
            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", Id);
            UserWalletViewModel viewModel = new UserWalletViewModel();
            viewModel.UserProfileId = Id;
            viewModel.UserWallet = db.UserWallets.Where(t => t.UserProfileId == Id).FirstOrDefault();
            ViewData.Model = viewModel;
            logger.Info("Create GET Method End" + " at " + DateTime.UtcNow);
            return View();
        }

        //
        // POST: /UserWalletLog/Create
        /// <summary>
        /// This methos will be used to create new wallet.
        /// </summary>
        /// <param name="userwalletViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(UserWalletViewModel userwalletViewModel)
        {
            logger.Info("Create HttpPost Method Start" + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                UserWallet wallet = db.UserWallets.Where(t => t.UserProfileId == userwalletViewModel.UserProfileId).FirstOrDefault();
                if (wallet == null)
                {
                    wallet = new UserWallet();
                    wallet.UserProfileId = userwalletViewModel.UserProfileId;
                    wallet.Balance += userwalletViewModel.UserWalletLog.Amount;
                    logger.Info("Details Method user " + wallet.UserProfileId + " at " + DateTime.UtcNow);
                    logger.Info("Details Method Balance " + wallet.Balance + " at " + DateTime.UtcNow);
                    db.UserWallets.Add(wallet);
                    logger.Info("Details Method User wallet details added " + " at " + DateTime.UtcNow);
                }
                else
                {
                    wallet.Balance += userwalletViewModel.UserWalletLog.Amount;
                    logger.Info("Details Method Balance " + wallet.Balance + " at " + DateTime.UtcNow);

                    db.Entry(wallet).State = EntityState.Modified;
                    logger.Info("Details Method User wallet details updated " + " at " + DateTime.UtcNow);
                }
                userwalletViewModel.UserWalletLog.UserProfileId = userwalletViewModel.UserProfileId;
                userwalletViewModel.UserWalletLog.TransactionDate = DateTime.Now;
                db.UserWalletLog.Add(userwalletViewModel.UserWalletLog);
                db.SaveChanges();
            }
            else
            {
                ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", userwalletViewModel.UserProfileId);
                logger.Info("Details Method User " + ViewBag.UserProfileId + " at " + DateTime.UtcNow);
                ModelState.AddModelError("Select", "Select User");
                return View(userwalletViewModel);
            }
            logger.Info("Create HttpPost Method End" + " at " + DateTime.UtcNow);
            return RedirectToAction("Index", "UserWallet");
        }

        private void GenerateRequest(UserProfile user, string amount, Order order = null)
        {
            logger.Info("GenerateRequest Method Start" + " at " + DateTime.UtcNow);
            RequestParams requestParams = new RequestParams();
            NameValueCollection formFields = new NameValueCollection();

            logger.Info("GenerateRequest Method User" + user.UserProfileId + " at " + DateTime.UtcNow);
            logger.Info("GenerateRequest Method Amount " + amount + " at " + DateTime.UtcNow);
            formFields = requestParams.GenerateResponse(user, amount, "http://www.payatoz.com/UserWalletLog/response?DR={DR}");
            requestParams.Return_Url = "http://www.payatoz.com/UserWalletLog/response?DR={DR}";

            Response.Clear();
            Response.Write("<html><head>");
            Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", requestParams.FormName));
            Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", requestParams.FormName, requestParams.Method, requestParams.Url));
            for (int i = 0; i < formFields.Keys.Count; i++)
            {
                Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", formFields.Keys[i], formFields[formFields.Keys[i]]));
            }
            Response.Write("</form>");
            Response.Write("</body></html>");
            Response.End();
            logger.Info("GenerateRequest Method End" + " at " + DateTime.UtcNow);
        }

        [ActionName("Response")]
        public ActionResult GetResponse(string DR)
        {
            logger.Info("GetResponse Method Start" + " at " + DateTime.UtcNow);
            UserWalletViewModel userWalletViewModal = (UserWalletViewModel)TempData["UserWallet"];
            RequestParams parameters = new RequestParams();
            UserWalletLog userWalletLog = userWalletViewModal.UserWalletLog;

            db.Entry(userWalletLog).State = EntityState.Modified;
            parameters.GetResponse(DR, userWalletViewModal.UserProfileId, userWalletViewModal.UserWalletLog.Amount, ref userWalletLog);
            
            userWalletViewModal.UserWalletLog = userWalletLog;
            logger.Info("GetResponse Method userWalletLog Status" + userWalletLog.Status1 + " at " + DateTime.UtcNow);
            
            if (userWalletLog.Status1 == (int)OrderStatus.PaymentSuceess)
            {
                UserWallet wallet = db.UserWallets.Where(p => p.UserProfileId == userWalletViewModal.UserProfileId).FirstOrDefault();
                if (wallet == null)
                {
                    wallet = new UserWallet();
                    wallet.UserProfileId = userWalletViewModal.UserProfileId;
                    wallet.Balance += userWalletLog.Amount;
                    logger.Info("GetResponse Method user " + wallet.UserProfileId + " at " + DateTime.UtcNow);
                    logger.Info("GetResponse Method userWallet Balance " + wallet.Balance + " at " + DateTime.UtcNow);
                    db.UserWallets.Add(wallet);
                    logger.Info("GetResponse Method User wallet details added at " + DateTime.UtcNow);
                }
                else
                {
                    wallet.Balance += userWalletLog.Amount;
                    logger.Info("GetResponse Method userWallet Balance " + wallet.Balance + " at " + DateTime.UtcNow);
                    db.Entry(wallet).State = EntityState.Modified;
                    logger.Info("GetResponse Method User wallet details updated at " + DateTime.UtcNow);
                }
                userWalletViewModal.UserWallet = wallet;
            }
            db.SaveChanges();
            ViewData.Model = userWalletViewModal;
            logger.Info("GetResponse Method End" + " at " + DateTime.UtcNow);
            return View();
        }

        //
        // GET: /UserWalletLog/Edit/5
        /// <summary>
        /// This method will edit selected UserWalletLog.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            logger.Info("Edit Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit Get Method UserWalletLog Id " + id + " at " + DateTime.UtcNow);
            UserWalletLog userwalletlog = db.UserWalletLog.Find(id);
            if (userwalletlog == null)
            {
                logger.Info("Edit Get Method User Wallet Log is null " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", userwalletlog.UserProfileId);
            logger.Info("Edit Get Method Start" + " at " + DateTime.UtcNow);
            return View(userwalletlog);
        }

        //
        // POST: /UserWalletLog/Edit/5
        /// <summary>
        ///  This method will update data.        
        /// </summary>
        /// <param name="userwalletlog"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(UserWalletLog userwalletlog)
        {
            logger.Info("Edit HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit HttpPost Method UserWalletLog Id " + userwalletlog.UserWalletLogId + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.Entry(userwalletlog).State = EntityState.Modified;
                db.SaveChanges();
                logger.Info("Edit HttpPost Method User Wallet Log updated " + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", userwalletlog.UserProfileId);
            logger.Info("Edit HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(userwalletlog);
        }

        //
        // GET: /UserWalletLog/Delete/5
        /// <summary>
        /// This method will delete selected UserWalletLog.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id = 0)
        {
            logger.Info("Delete Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete Get Method Start User Wallet Log Id " + id + " at " + DateTime.UtcNow);
            UserWalletLog userwalletlog = db.UserWalletLog.Find(id);
            if (userwalletlog == null)
            {
                logger.Info("Delete Get Method User Wallet Log is null " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Delete Get Method End" + " at " + DateTime.UtcNow);
            return View(userwalletlog);
        }

        //
        // POST: /UserWalletLog/Delete/5
        /// <summary>
        /// This method will delete selected UserWalletLog.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            logger.Info("Delete HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete HttpPost Method UserWalletLog Id " + id + " at " + DateTime.UtcNow);
            UserWalletLog userwalletlog = db.UserWalletLog.Find(id);
            db.UserWalletLog.Remove(userwalletlog);
            db.SaveChanges();
            logger.Info("Delete HttpPost Method User Wallet Log details removed " + " at " + DateTime.UtcNow);
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