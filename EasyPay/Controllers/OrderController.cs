using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyPay.Models;
using System.Web.Security;
using PagedList;
using log4net;
using ServiceEngine;
using System.Data.Entity.Core.Objects;
namespace EasyPay.Controllers
{
    public class OrderController : Controller
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private EasyPayContext db = new EasyPayContext();

        //
        // GET: /Order/

        public ActionResult Index(int? page)
        {
            logger.Info("Index Method Start" + " at " + DateTime.UtcNow);
            try
            {
                int userId = Convert.ToInt32(System.Web.Security.Membership.GetUser().ProviderUserKey);
                logger.Info("Index Method User " + userId + " at " + DateTime.UtcNow);
                UserProfile userProfile = db.UserProfiles.Where(u => u.UserProfileId == userId).FirstOrDefault();
                ViewBag.UserProfileData = userProfile;

                var thisRole = Roles.GetRolesForUser(System.Web.Security.Membership.GetUser().UserName).FirstOrDefault();
                var coupon = new Coupon();
                int coupAmt;

                List<OrderCoupon> orderCoupons = new List<OrderCoupon>();
                orderCoupons = (List<OrderCoupon>)Session["_ssnOrderCoupon"];

                if (orderCoupons != null)
                {
                    foreach (var orderCoupon in orderCoupons)
                    {
                        coupon = db.Coupons.Where(c => c.CouponId == orderCoupon.CouponId).FirstOrDefault();

                        coupon.CouponsUsed++;
                        coupon.AvailableCoupons = coupon.TotalCoupons - coupon.CouponsUsed;
                        db.Entry(coupon).State = EntityState.Modified;
                        db.SaveChanges();

                        db.OrderCoupons.Add(orderCoupon);
                        db.SaveChanges();
                    }
                    //foreach (var orderCoupon in orderCoupons)
                    //{
                    //    coupon = db.Coupons.Where(c => c.CouponId == orderCoupon.CouponId).FirstOrDefault();

                    //    coupon.CouponsUsed++;
                    //    coupon.AvailableCoupons -= coupon.CouponsUsed;
                    //    db.Entry(coupon).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //}
                }

                //DateTime dayBefore = DateTime.Now.AddDays(-10);
                //var updateOrders = db.Orders.Where(x => (x.Status == 3 || x.Status == 4) && x.UserProfileId == userId && x.OrderDate > dayBefore);

                var updateOrders = db.Orders.Include(o => o.UserProfile).Include(y => y.ProviderService).Where(x => x.UserProfileId == userId && (x.Status == 7));
                if (userId != null)
                {
                    var thisUser = (from u in db.UserProfiles
                                    where u.UserProfileId == userId
                                    select u).FirstOrDefault();
                    if (thisUser.IsSkip == "No")
                    {

                        List<UserPreferenceViewModel> thisUserPreference = (from pref in db.Preferences
                                                                            join so in db.ServiceOperators on pref.ServiceOperatorId equals so.ServiceOperatorId
                                                                            join st in db.ServiceTypes on pref.ServiceTypeId equals st.ServiceTypeId
                                                                            where pref.UserProfileId == thisUser.UserProfileId

                                                                            select new UserPreferenceViewModel
                                                                            {
                                                                                RechargeNumber = pref.RechargeNumber,
                                                                                Amount = pref.Amount,
                                                                                ServiceTypeName = st.ServiceTypeName,
                                                                                ServiceTypeId = st.ServiceTypeId,
                                                                                OperatorName = so.OperatorName
                                                                            }).ToList();
                        ViewBag.UserPreference = thisUserPreference;



                        //query = (from user in db.AspNetUsers
                        //         join userRoles in db.AspNetUserRoles on user.Id equals userRoles.UserId into rolesOfUser

                        //         from rou in rolesOfUser.DefaultIfEmpty()
                        //         join roles in db.AspNetRoles on rou.RoleId equals roles.Id into getRoles

                        //         from gr in getRoles.DefaultIfEmpty()
                        //         join userInfo in db.UserInfoes on user.UserInfo_Id equals userInfo.Id into users
                        //         from u in users.DefaultIfEmpty()

                        //         select new UserRoleViewModel
                        //         {
                        //             Role = (gr.Name == null) ? "User" : gr.Name,
                        //             UserName = (u.UserName == null) ? "None" : u.UserName,
                        //             Name = (u.UserName == null) ? "None" : u.Name,
                        //             EmailId = (u.EmailId == null) ? "None" : u.EmailId,
                        //             UserId = (u.Id == null) ? 0 : u.Id,
                        //             RoleId = (gr.Id == null) ? "None" : gr.Id

                        //         }).OrderBy(o => o.UserName).ToList();


                        //List<Preference> preferenceData = (from preference in db.Preferences
                        //                      where preference.UserProfileId == userId
                        //                      select preference).ToList();
                        //ViewBag.preferenceData = preferenceData;
                    }
                }
                logger.Info("Index Method Orders " + updateOrders.Count() + " at " + DateTime.UtcNow);
                if (updateOrders.Count() > 0)
                {
                    foreach (var order in updateOrders)
                    {
                        if (!string.IsNullOrEmpty(order.Remarks))
                        {
                            ServiceEngine.ServiceHelper serviceHelper = new ServiceEngine.ServiceHelper();
                            string[] paramString = order.Remarks.Split(',');
                            string transactionNumber = string.Empty;
                            if (paramString.Count() > 1)
                            {
                                transactionNumber = paramString[1];
                            }
                            else
                            {
                                transactionNumber = paramString[0];
                            }
                            db.Entry(order).State = EntityState.Modified;
                            int serviceTypeId = 1;
                            if (order.ProviderService.ServiceTypeId == 1)
                                serviceTypeId = 1;
                            else if (order.ProviderService.ServiceTypeId == 2)
                                serviceTypeId = 4;
                            else if (order.ProviderService.ServiceTypeId == 3)
                                serviceTypeId = 2;
                            else if (order.ProviderService.ServiceTypeId == 4)
                                serviceTypeId = 3;

                            string result = serviceHelper.GetTransactionResult(transactionNumber, serviceTypeId);
                            ProcessResult(result, order);

                        }
                    }
                    db.SaveChanges();

                }
                int pagenumber = page ?? 1;

                var orders = (thisRole == "Admin") ? db.Orders.Include(o => o.UserProfile).Include(o => o.ProviderService).OrderByDescending(x => x.OrderDate) : db.Orders.Include(o => o.UserProfile).Include(o => o.ProviderService).Where(x => x.UserProfileId == userId).OrderByDescending(x => x.OrderDate);

                //if (thisRole == "Admin")
                //{
                //    var orders = db.Orders.Include(o => o.UserProfile).Include(o => o.ProviderService).OrderByDescending(x => x.OrderDate);
                //}
                //else {
                // var   orders = db.Orders.Include(o => o.UserProfile).Include(o => o.ProviderService).Where(x => x.UserProfileId == userId).OrderByDescending(x => x.OrderDate);
                //}

                logger.Info("Index Method End" + " at " + DateTime.UtcNow);
                return View(orders.ToPagedList(pagenumber, 10));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult RefundAmount(string orderId)
        {
            logger.Info("Method Start" + " at " + DateTime.UtcNow);
            try
            {
                if (orderId != null)
                {
                    logger.Info("Order Id " + orderId + " at " + DateTime.UtcNow);
                    int id = Convert.ToInt32(orderId);
                    var order = db.Orders.Find(id);
                    var wallet = db.UserWallets.Where(w => w.UserProfileId == order.UserProfileId).FirstOrDefault();
                    wallet.Balance += order.Amount;
                    logger.Info("Wallet Balance " + wallet.Balance + " at " + DateTime.UtcNow);
                    db.Entry(wallet).State = EntityState.Modified;
                    db.SaveChanges();
                }
                logger.Info("Amount Refunded " + " at " + DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                logger.Error("Error: " + ex.InnerException + " " + ex.StackTrace + " " + ex.Message + " at " + DateTime.UtcNow);
                throw ex;
            }
            logger.Info("Index Method End " + " at " + DateTime.UtcNow);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult DeleteOrder(string orderId)
        {
            logger.Info("DeleteOrder Post Method Start " + " at " + DateTime.UtcNow);
            try
            {
                if (orderId != null)
                {
                    logger.Info("DeleteOrder Post Method Order Id "+orderId + " at " + DateTime.UtcNow);
                    int id = Convert.ToInt32(orderId);
                    DeleteConfirmed(id);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error: " + ex.InnerException + " " + ex.StackTrace + " " + ex.Message + " at " + DateTime.UtcNow);
                throw ex;
            }
            logger.Info("DeleteOrder Post Method End " + " at " + DateTime.UtcNow);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult SendMail(string user)
        {
            logger.Info("Send Mail Post Method Start " + " at " + DateTime.UtcNow);
            Utils utils = new Utils();
            try
            {
                if (user != null)
                {
                    logger.Info("Send Mail Post Method user " + user + " at " + DateTime.UtcNow);
                    int id = Convert.ToInt32(user);
                    var thisUser = db.UserProfiles.Where(u => u.UserProfileId == id).FirstOrDefault();
                    string subject = "Pending Payment";
                    string body = "<div><div>Dear {0},</div><p><span>Thank you for visiting PayAtoZ. This is to remind you that your order is incomplete, please visit PAyAtoZ to complete your order</span></p><p>Thank you for choosing us as your recharge service provider.";
                    utils.mailUser(subject, body, thisUser.Email, thisUser.UserName);
                    logger.Info("Send Mail Post Method Mail Sent at " + DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error: " + ex.InnerException + " " + ex.StackTrace + " " + ex.Message + " at " + DateTime.UtcNow);
                throw ex; 
            }
            logger.Info("Send Mail Post Method End at " + DateTime.UtcNow);
            return RedirectToAction("Index");
        }
        private void ProcessResult(string result, Order order)
        {
            logger.Info("ProcessResult Method Start" + " at " + DateTime.UtcNow);
            string[] resultParams = result.Split(',');

            if (resultParams.Count() > 1)
            {
                MembershipUser membershipUser = System.Web.Security.Membership.GetUser();
                order.UserProfileId = Convert.ToInt32(membershipUser.ProviderUserKey);
                var userWallet = db.UserWallets.ToList();
                UserWallet userwallet = db.UserWallets.FirstOrDefault(x => x.UserProfileId == order.UserProfileId);
                //order.Remarks = resultParams[0];// "AZ" + order.OrderId.ToString() + GetUniqueKey();
                logger.Info("ProcessResult Method result " + resultParams[1].ToString() + " at " + DateTime.UtcNow);
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
            logger.Info("ProcessResult Method End" + " at " + DateTime.UtcNow);
        }

        //
        // GET: /Order/Details/5

        public ActionResult Details(int id = 0)
        {
            logger.Info("Details Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Details Method Order Id" + id + " at " + DateTime.UtcNow);
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
        // GET: /Order/Create

        public ActionResult Create()
        {
            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName");
            ViewBag.ProviderServiceId = new SelectList(db.ProviderServices, "ProviderServiceId", "Format");
            return View();
        }

        //
        // POST: /Order/Create

        [HttpPost]
        public ActionResult Create(Order order)
        {
            logger.Info("Create HttpPost Method Start" + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                logger.Info("Create HttpPost Method Order details added " + " at " + DateTime.UtcNow);
                logger.Info("Create HttpPost Method End" + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }

            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", order.UserProfileId);
            ViewBag.ProviderServiceId = new SelectList(db.ProviderServices, "ProviderServiceId", "Format", order.ProviderServiceId);
            logger.Info("Create HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(order);
        }

        //
        // GET: /Order/Edit/5

        public ActionResult Edit(int id = 0)
        {
            logger.Info("Edit Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit Method Order Id " + id + " at " + DateTime.UtcNow);
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                logger.Info("Edit Method Order not found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", order.UserProfileId);
            ViewBag.ProviderServiceId = new SelectList(db.ProviderServices, "ProviderServiceId", "Format", order.ProviderServiceId);
            logger.Info("Edit Method End" + " at " + DateTime.UtcNow);
            return View(order);
        }

        //
        // POST: /Order/Edit/5

        [HttpPost]
        public ActionResult Edit(Order order)
        {
            logger.Info("Edit HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Edit HttpPost Method Order Id " + order.OrderId + " at " + DateTime.UtcNow);
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                logger.Info("Edit HttpPost Method Order Details Updated " + " at " + DateTime.UtcNow);
                logger.Info("Edit HttpPost Method End" + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            ViewBag.UserProfileId = new SelectList(db.UserProfiles, "UserProfileId", "UserName", order.UserProfileId);
            ViewBag.ProviderServiceId = new SelectList(db.ProviderServices, "ProviderServiceId", "Format", order.ProviderServiceId);
            logger.Info("Edit HttpPost Method End" + " at " + DateTime.UtcNow);
            return View(order);
        }

        //
        // GET: /Order/Delete/5

        public ActionResult Delete(int id = 0)
        {
            logger.Info("Delete Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Delete Method Order Id "+id + " at " + DateTime.UtcNow);
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                logger.Info("Delete Method Order Not Found " + " at " + DateTime.UtcNow);
                return HttpNotFound();
            }
            logger.Info("Delete Method End" + " at " + DateTime.UtcNow);
            return View(order);
        }

        public ActionResult Balance()
        {
            return View();
        }
        //
        // POST: /Order/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            logger.Info("DeleteConfirmed HttpPost Method Start" + " at " + DateTime.UtcNow);
            logger.Info("DeleteConfirmed HttpPost Method Order Id "+id  + " at " + DateTime.UtcNow);
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            logger.Info("DeleteConfirmed HttpPost Method Order Details removed " + " at " + DateTime.UtcNow);
            logger.Info("DeleteConfirmed HttpPost Method End" + " at " + DateTime.UtcNow);
            return RedirectToAction("Index");
        }

        public ActionResult RedirectToHome(int? orderId)
        {
            return RedirectToAction("Index", "Home", new { });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}