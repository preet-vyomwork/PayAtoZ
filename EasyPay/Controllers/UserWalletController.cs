using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyPay.Models;
using System.Web.Security;
using System.Collections.Specialized;
using log4net;

namespace EasyPay.Controllers
{
    public class UserWalletController : Controller
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private EasyPayContext db = new EasyPayContext();

        //
        // GET: /UserWallet/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var userwallets = db.UserWallets.Include(u => u.UserProfile);
            return View(userwallets.ToList());
        }

        //
        // GET: /UserWallet/Details/5
        /// <summary>
        /// This mehod will give details for seleced User Wallet.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id = 0)
        {
            logger.Info("Details Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Details Method UserWallet Id" + id + " at " + DateTime.UtcNow);
            UserWallet userwallet = db.UserWallets.Find(id);
            if (userwallet == null)
            {
                logger.Info("Details Method UserWallet Id not found" + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Details Method End" + " at " + DateTime.UtcNow);
            return View(userwallet);
        }

        //
        // GET: /UserWallet/Create

        public ActionResult Create()
        {
            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName");
            return View();
        }

        //
        // POST: /UserWallet/Create
        /// <summary>
        /// This method will create new category.
        /// </summary>
        /// <param name="userwallet"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(UserWallet userwallet)
        {
            logger.Info("Create HttpPost Method Start" + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.UserWallets.Add(userwallet);
                db.SaveChanges();
                logger.Info("Create HttpPost Method User wallet details added " + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }

            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", userwallet.UserProfileId);
            logger.Info("Create HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(userwallet);
        }

        //
        // GET: /UserWallet/Edit/5
        /// <summary>
        /// This method will edit selected User Wallet.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            logger.Info("Edit Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit Get Method User Wallet Id" + id + " at " + DateTime.UtcNow);
            UserWallet userwallet = db.UserWallets.Find(id);
            if (userwallet == null)
            {
                logger.Info("Edit Get Method User Wallet is null" + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", userwallet.UserProfileId);
            logger.Info("Edit Get Method End" + " at " + DateTime.UtcNow);
            return View(userwallet);
        }

        //
        // POST: /UserWallet/Edit/5
        /// <summary>
        /// This method will update data. 
        /// </summary>
        /// <param name="userwallet"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(UserWallet userwallet)
        {
            logger.Info("Edit HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit HttpPost Method UserWallet Id" + userwallet.UserWalletId + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.Entry(userwallet).State = EntityState.Modified;
                db.SaveChanges();
                logger.Info("Edit HttpPost Method User Wallet details updated " + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", userwallet.UserProfileId);
            logger.Info("Edit HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(userwallet);
        }

        //
        // GET: /UserWallet/Delete/5
        /// <summary>
        /// This method will delete selected User Wallet.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id = 0)
        {
            logger.Info("Delete Get Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete Get Method User Wallet Id" + id + " at " + DateTime.UtcNow);
            UserWallet userwallet = db.UserWallets.Find(id);
            if (userwallet == null)
            {
                logger.Info("Delete Get Method UserWallet is null " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Delete Get Method End" + " at " + DateTime.UtcNow);
            return View(userwallet);
        }


        //
        // POST: /UserWallet/Delete/5
        /// <summary>
        /// This method will delete selected User Wallet.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            logger.Info("Delete HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete HttpPost Method User Wallet Id" + id + " at " + DateTime.UtcNow);
            UserWallet userwallet = db.UserWallets.Find(id);
            IEnumerable<UserWalletLog> userWalletLogs;
            if (userwallet != null)
            {
                userWalletLogs = db.UserWalletLog.Where(x => x.UserProfileId == userwallet.UserProfileId);
                foreach (var item in userWalletLogs)
                {
                    db.UserWalletLog.Remove(item);
                }
                logger.Info("Delete HttpPost Method " + userWalletLogs.Count() + " wallet logs removed " + " at " + DateTime.UtcNow);
            }
            db.UserWallets.Remove(userwallet);
            db.SaveChanges();
            logger.Info("Delete HttpPost Method User Wallet Details removed " + " at " + DateTime.UtcNow);
            logger.Info("Delete HttpPost Method End" + " at " + DateTime.UtcNow);
            return RedirectToAction("Index");
        }

        public ActionResult Wallet()
        {
            logger.Info("Wallet Method Start" + " at " + DateTime.UtcNow);
            if (TempData["WalletStatus"] != null)
            {
                int statusId = (int)TempData["WalletStatus"];
                logger.Info("Wallet Method Wallet Status " + statusId + " at " + DateTime.UtcNow);
                switch(statusId)
                {
                    case 0:
                        ViewBag.WalletStatus="Your recharge amount is in Pending stage.";
                          break;  
                    case 1:
                          ViewBag.WalletStatus = "Your recharge amount is in InProgress stage.";
                          break;
                    case 2:
                          ViewBag.WalletStatus = "Payment Failed, Try again.";
                          break;
                    case 7:
                          ViewBag.WalletStatus = "Payment Succeed.";
                          break;
                    default:
                          break;
                }
            }
            int userId = Convert.ToInt32(System.Web.Security.Membership.GetUser().ProviderUserKey);
            UserWallet userwallet = db.UserWallets.FirstOrDefault(x => x.UserProfileId == userId);

            MembershipUser membershipUser = System.Web.Security.Membership.GetUser();
            var walletLogs = db.UserWalletLog.Include(o => o.UserProfile).Where(x => x.UserProfileId == userId).OrderByDescending(x => x.TransactionDate);
            ViewBag.walletLogList = walletLogs;
            if (userwallet == null)
            {
                return HttpNotFound();
            }
            TempData["WalletStatus"] = null;
            logger.Info("Wallet Method End" + " at " + DateTime.UtcNow);
            return View(userwallet);
        }

        [HttpPost]
        public void AddToWallet(FormCollection _formControls)
        {
            logger.Info("AddToWallet Method Start" + " at " + DateTime.UtcNow);
            UserWalletLog walletLog = new UserWalletLog();
            walletLog.TransactionDate = DateTime.Now;
            logger.Info("AddToWallet Method FormControls" + _formControls.Count + " at " + DateTime.UtcNow);
            foreach (string _formData in _formControls)
            {
                if (_formData == "UserProfileId")
                {
                    walletLog.UserProfileId = Convert.ToInt32(_formControls[_formData]);
                }
                else
                {
                    walletLog.Amount = Convert.ToDecimal(_formControls[_formData]);
                }
            }
            walletLog.IsRefund = false;
            
            //walletLog.Status = (int)OrderStatus.Pending;
            //walletLog.Comment = "Generate Request";
            db.UserWalletLog.Add(walletLog);
            db.SaveChanges();

            UserProfile user = db.UserProfiles.Where(p => p.UserProfileId == walletLog.UserProfileId).First();
            TempData["UserWalletLog"] = walletLog;
            
            GenerateRequest(user, walletLog.Amount.ToString());
            logger.Info("AddToWallet Method End" + " at " + DateTime.UtcNow);
        }

        private void GenerateRequest(UserProfile user, string amount, Order order = null)
        {
            logger.Info("GenerateRequest Method Start" + " at " + DateTime.UtcNow);
            logger.Info("GenerateRequest Method user" + user.UserProfileId+" at " + DateTime.UtcNow);
            logger.Info("GenerateRequest Method amount" + amount + " at " + DateTime.UtcNow);
            RequestParams requestParams = new RequestParams();
            NameValueCollection formFields = new NameValueCollection();
            formFields = requestParams.GenerateResponse(user, amount,"http://www.payatoz.com/UserWallet/response?DR={DR}");
            requestParams.Return_Url = "http://www.payatoz.com/UserWallet/response?DR={DR}";

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
            UserWalletLog userWalletLog = (UserWalletLog)TempData["UserWalletLog"];
            RequestParams parameters = new RequestParams();
            db.Entry(userWalletLog).State = EntityState.Modified;
            parameters.GetResponse(DR, userWalletLog.UserProfileId, userWalletLog.Amount, ref userWalletLog);

            logger.Info("GetResponse Method userWallet log status "+userWalletLog.Status1 + " at " + DateTime.UtcNow);
            if (userWalletLog.Status1 == (int)OrderStatus.PaymentSuceess)
            {
                UserWallet wallet = db.UserWallets.Where(p => p.UserProfileId == userWalletLog.UserProfileId).FirstOrDefault();
                logger.Info("GetResponse Method wallet "+wallet.UserWalletId + " at " + DateTime.UtcNow);
                if (wallet == null)
                {
                    wallet = new UserWallet();
                    wallet.UserProfileId = userWalletLog.UserProfileId;
                    wallet.Balance += userWalletLog.Amount;
                    logger.Info("GetResponse Method user "+wallet.UserProfileId + " at " + DateTime.UtcNow);
                    logger.Info("GetResponse Method wallet balance "+wallet.Balance + " at " + DateTime.UtcNow);
                    db.UserWallets.Add(wallet);
                }
                else
                {
                    wallet.Balance += userWalletLog.Amount;
                    logger.Info("GetResponse Method wallet balance " + wallet.Balance + " at " + DateTime.UtcNow);
                    db.Entry(wallet).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            TempData["WalletStatus"] = userWalletLog.Status1;
            logger.Info("GetResponse Method End" + " at " + DateTime.UtcNow);
            return RedirectToAction("Wallet", "UserWallet"); ;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}