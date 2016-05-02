using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyPay.Models;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.IO;
using System.Globalization;
using log4net;
namespace EasyPay.Controllers
{
    public class HomeController : Controller
    {
        EasyPayContext db = new EasyPayContext();
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //public string userPreference;

        public ActionResult Index(int? orderId)
        {
            try
            {
                logger.Info("Method Start" + " at " + DateTime.UtcNow);

                EasyPayContext db = new EasyPayContext();

                //ServiceEngine.ServiceHelper serviceHelper = new ServiceEngine.ServiceHelper();
                //ServiceEngine.ServiceOrder serviceOrder = new ServiceEngine.ServiceOrder();

                //serviceOrder.Amount = 100;
                //serviceOrder.ItemCode = "8734962007";
                //serviceOrder.OperatorCode = "UN";
                //serviceOrder.OrderNumber = "123";
                //serviceOrder.ProviderType = ServiceEngine.ProviderType.JOLO;
                //serviceOrder.ServiceType = ServiceEngine.ServiceType.Mobile;
                //string resultString = serviceHelper.DoRecharge(serviceOrder);

                object selectedOperator = null;
                Order currentOrder = null;
                if (orderId != null)
                {
                    logger.Info("Order Id " + orderId + " at " + DateTime.UtcNow);
                    currentOrder = db.Orders.Where(x => x.OrderId == orderId).First();
                    selectedOperator = db.ProviderServices.Where(x => x.ProviderServiceId == currentOrder.ProviderServiceId).Select(x => x.ServiceOperator.ServiceOperatorId).First();
                }
                string unique = GetUniqueKey();
                string s = Guid.NewGuid().ToString("N").Substring(0, 12);
                s = Guid.NewGuid().ToString("N").Substring(0, 12);
                ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
                var prepaidOperators = db.ServiceOperators.Where(x => x.ServiceTypeId.Value == 1);
                ViewBag.prepaidOperators = new SelectList(prepaidOperators, "ServiceOperatorId", "OperatorName", selectedOperator);
                var dthOperators = db.ServiceOperators.Where(x => x.ServiceTypeId.Value == 3);
                ViewBag.dthOperators = new SelectList(dthOperators, "ServiceOperatorId", "OperatorName", selectedOperator);
                var postpaidOperators = db.ServiceOperators.Where(x => x.ServiceTypeId.Value == 2);
                ViewBag.postpaidOperators = new SelectList(postpaidOperators, "ServiceOperatorId", "OperatorName", selectedOperator);
                var dataCardOperators = db.ServiceOperators.Where(x => x.ServiceTypeId.Value == 4);
                ViewBag.dataCardOperators = new SelectList(dataCardOperators, "ServiceOperatorId", "OperatorName", selectedOperator);
                ViewBag.TotalOrders = db.Orders.Count(x => x.Status == 4);
                if (orderId != null)
                {
                    logger.Info("Provider Service " + currentOrder.ProviderService + " at " + DateTime.UtcNow);
                    logger.Info("Current Order - Order Amount " + currentOrder.Amount + " at " + DateTime.UtcNow);
                    logger.Info("Current Order - Order Status " + currentOrder.Status + " at " + DateTime.UtcNow);
                    ViewData.Model = currentOrder;
                }
                logger.Info("Method end at " + DateTime.UtcNow);
                return View();
            }
            catch (Exception ex)
            {
                logger.Error("Error: " + ex.InnerException + " " + ex.StackTrace + " " + ex.Message + " at " + DateTime.UtcNow);
                return View();
            }

        }

        //public object ServiceOperatorPlans(int id = 0)
        //{
        //    var operatorPlans = db.ServiceOperatorPlans.Where(c => c.ServiceOperatorId == id).ToList();

        //    return PartialView("OperatorPlans", operatorPlans);
        //}

        //public object OperatorPlans(int id)
        //{
        //    var operatorPlans = db.ServiceOperatorPlans.Where(c => c.ServiceOperatorId == id).ToList();
        //    ServiceOperatorPlan plan = new ServiceOperatorPlan();
        //    return PartialView("OperatorPlans", plan);
        //}

        private string GetUniqueKey()
        {
            logger.Info("GetUniqueKey Method Start" + " at " + DateTime.UtcNow);
            int maxSize = 8;
            int minSize = 5;
            char[] chars = new char[62];
            string a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }
            logger.Info("GetUniqueKey Method End" + " at " + DateTime.UtcNow);
            return result.ToString();
        }
        public string GetJoloRecharge()
        {
            logger.Info("GetJoloRecharge Method Start" + " at " + DateTime.UtcNow);
            string result = string.Empty;
            WebRequest request = WebRequest.Create("http://jolo.in/api/recharge.php?mode=1&key=cinnovative_solutions581&operator=BS&service=9428292017&amount=50&orderid=a14");
            //WebRequest request = WebRequest.Create("http://jolo.in/api/recharge_advance.php?mode=1&key=cinnovative_solutions581&operator=UN&service=7383048530&amount=50&orderid=a13");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content. 
            result = reader.ReadToEnd();
            logger.Info("GetJoloRecharge Method End" + " at " + DateTime.UtcNow);
            return result;
        }
        public void SavePreference(string preference)
        {
            Session["_ssnUserPreference"] = preference;
            //userPreference = preference;
        }
        public ActionResult Pay()
        {

            logger.Info("Method Start" + " at " + DateTime.UtcNow);
            ServiceEngine.ServiceHelper serviceHelper = new ServiceEngine.ServiceHelper();
            Order order = (Order)Session["order"];
            ServiceEngine.ServiceOrder serviceOrder = (ServiceEngine.ServiceOrder)Session["serviceOrder"];
            try
            {
                if (Session["order"] != null)
                {
                    logger.Info("Order Session Found" + " at " + DateTime.UtcNow);
                    if (ModelState.IsValid)
                    {
                        EasyPayContext db = new EasyPayContext();
                        //MembershipUser membershipUser = Membership.GetUser();
                        //order.UserProfileId = Convert.ToInt32(membershipUser.ProviderUserKey);
                        //UserWallet userwallet = db.UserWallets.FirstOrDefault(x => x.UserProfileId == order.UserProfileId);
                        //if (userwallet != null)
                        //{
                        //    if (userwallet.Balance > order.Amount)
                        //    {
                        //        userwallet.Balance -= order.Amount;
                        //        db.Entry(userwallet).State = EntityState.Modified;
                        //    }
                        //    else
                        //    {
                        //        return Redirect("~/Order/Balance");
                        //    }
                        //}
                        MembershipUser membershipUser = System.Web.Security.Membership.GetUser();
                        order.UserProfileId = Convert.ToInt32(membershipUser.ProviderUserKey);
                        logger.Info("Order UserProfileId " + order.UserProfileId + " at " + DateTime.UtcNow);

                        db.Orders.Add(order);
                        db.SaveChanges();
                        order.Remarks = "AZ" + order.OrderId.ToString() + GetUniqueKey();
                        db.Entry(order).State = EntityState.Modified;
                        order.Status = (int)OrderStatus.RegargeInProgress;
                        logger.Info("Order Status " + order.Status + " at " + DateTime.UtcNow);
                        db.SaveChanges();
                        serviceOrder.OrderNumber = order.Remarks;
                        Session["order"] = order;
                        Session["serviceOrder"] = serviceOrder;
                        //string resultString = serviceHelper.DoRecharge(serviceOrder);
                        //ProcessResult(resultString, order);
                        //db.SaveChanges();
                        //Session.Remove("order");
                        //Session.Remove("serviceOrder");
                        //return Redirect("~/Order/Index");

                        return Redirect("~/ConfirmOrder/Index");
                    }
                    return Redirect("~/ConfirmOrder/Index");
                    //return Redirect("~/Order/Index");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error: " + ex.InnerException + " " + ex.StackTrace + " " + ex.Message + " at " + DateTime.UtcNow);
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Pay(Order order)
        {
            logger.Info("Method Start" + " at " + DateTime.UtcNow);
            EasyPayContext db = new EasyPayContext();
            int servicetypeId = 0;
            int serviceOperatorId = 0;
            ServiceEngine.ServiceHelper serviceHelper = new ServiceEngine.ServiceHelper();
            ServiceEngine.ServiceOrder serviceOrder = new ServiceEngine.ServiceOrder();
            logger.Info("Type is " + Request.Form["type"] + " at " + DateTime.UtcNow);
            try
            {
                if (Request.Form["type"] == "Mobile")
                {
                    if (Request.Form["mobiletype"] == "Mobile")
                    {
                        servicetypeId = 1;
                        serviceOperatorId = Convert.ToInt32(Request.Form["prepaidOperators"]);
                    }
                    else
                    {
                        servicetypeId = 2;
                        serviceOperatorId = Convert.ToInt32(Request.Form["postpaidOperators"]);
                    }
                    serviceOrder.ServiceType = ServiceEngine.ServiceType.Mobile;
                }
                else if (Request.Form["type"] == "DTH")
                {
                    servicetypeId = 3;
                    serviceOperatorId = Convert.ToInt32(Request.Form["dthOperators"]);
                    serviceOrder.ServiceType = ServiceEngine.ServiceType.DTH;
                }
                else if (Request.Form["type"] == "DataCard")
                {
                    servicetypeId = 4;
                    serviceOperatorId = Convert.ToInt32(Request.Form["dataCardOperators"]);
                    serviceOrder.ServiceType = ServiceEngine.ServiceType.DataCard;
                }
                var providerService = db.ProviderServices.FirstOrDefault(x => x.ProviderId == 2 && x.ServiceOperatorId == serviceOperatorId);
                order.ProviderServiceId = providerService.ProviderServiceId;
                string utcDateString = System.DateTime.UtcNow.ToString();
                DateTime localDate = DateTime.Parse(utcDateString,
                                      CultureInfo.CurrentCulture,
                                      DateTimeStyles.AssumeUniversal);
                order.OrderDate = localDate.AddMinutes(30).AddHours(12);
                order.comments = "Pending";
                order.Status = (int)OrderStatus.RegargeInProgress;
                order.commission = providerService.Comission;
                serviceOrder.OperatorCode = providerService.Format;
                serviceOrder.ProviderType = ServiceEngine.ProviderType.JOLO;
                serviceOrder.Amount = order.Amount;
                serviceOrder.ItemCode = order.ItemCode;
                if (User.Identity.IsAuthenticated)
                {
                    // if (ModelState.IsValid)
                    // {
                    MembershipUser membershipUser = System.Web.Security.Membership.GetUser();
                    //UserProfile userProfile = db.UserProfiles.Where(u => u.UserName == membershipUser.UserName).FirstOrDefault();
                    //userProfile.IsSkip = Session["_ssnUserPreference"].ToString();
                    //db.Entry(userProfile).State = EntityState.Modified;
                    //db.SaveChanges();

                    order.UserProfileId = Convert.ToInt32(membershipUser.ProviderUserKey);
                    order.UserProfile = db.UserProfiles.First(t => t.UserProfileId == order.UserProfileId);
                    db.Orders.Add(order);
                    db.SaveChanges();
                    order.Remarks = "AZ" + order.OrderId.ToString() + GetUniqueKey();
                    db.Entry(order).State = EntityState.Modified;
                    order.Status = (int)OrderStatus.RegargeInProgress;
                    db.SaveChanges();
                    serviceOrder.OrderNumber = order.Remarks;
                    //string resultString = serviceHelper.DoRecharge(serviceOrder);
                    //ProcessResult(resultString, order);
                    //db.SaveChanges();

                    //If user agrees to set preference, then save details in Preference table.

                    //if (order.UserProfile.IsSkip == "No")
                    //{
                    //    if (Session["_ssnUserPreference"].ToString() == "No")
                    //    {
                    //        Preference preference = new Models.Preference();
                    //        var userName = User.Identity.Name;
                    //        var thisUser = db.UserProfiles.Where(u => u.UserName.Equals(userName)).FirstOrDefault();
                    //        if (thisUser != null)
                    //        {
                    //            preference.UserProfileId = thisUser.UserProfileId;
                    //            preference.Amount = order.Amount.ToString();
                    //            preference.ServiceOperatorId = serviceOperatorId;
                    //            preference.RechargeNumber = order.ItemCode;
                    //            preference.ServiceTypeId = servicetypeId;
                    //            db.Preferences.Add(preference);
                    //            db.SaveChanges();
                    //        }
                    //    }
                    //}
                    Session["order"] = order;
                    Session["serviceOrder"] = serviceOrder;
                    //return Redirect("~/Order/Index");
                    return Redirect("~/ConfirmOrder/Index");
                    // }
                    //return RedirectToAction("Index");
                }
                else
                {
                    Session["order"] = order;
                    Session["serviceOrder"] = serviceOrder;

                    return RedirectToAction("Login", "Account", new { returnUrl = "/Home/Pay" });
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error: " + ex.InnerException + " " + ex.StackTrace + " " + ex.Message + " at " + DateTime.UtcNow);
            }
            return RedirectToAction("Index");
        }

        private void ProcessResult(string result, Order order)
        {
            logger.Info("ProcessResult Method Start" + " at " + DateTime.UtcNow);
            string[] resultParams = result.Split(',');
            logger.Info("ProcessResult Method result parameters" +resultParams.Count()+ " at " + DateTime.UtcNow);
            if (resultParams.Count() > 2)
            {
                order.Remarks = order.Remarks + "," + resultParams[0];// "AZ" + order.OrderId.ToString() + GetUniqueKey();
                if (resultParams[1].ToString().ToLower().Equals("success"))
                {
                    order.Status = (int)OrderStatus.Success;
                    logger.Info("ProcessResult Method Order status" + order.Status + " at " + DateTime.UtcNow);
                }
            }
            logger.Info("ProcessResult Method End" + " at " + DateTime.UtcNow);
        }

        //[Authorize(Roles = "Administrator")]
        public ActionResult About()
        {
            ViewBag.Message = "";

            return View();
        }
        public ActionResult Refund()
        {
            ViewBag.Message = "";

            return View();
        }

        public ActionResult FAQ()
        {
            ViewBag.Message = "";

            return View();
        }
        public ActionResult TermsCondition()
        {
            ViewBag.Message = "";

            return View();
        }
        public ActionResult Privacy()
        {
            ViewBag.Message = "";

            return View();
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "";

            return View();
        }

        public ActionResult Administration()
        {
            logger.Info("Method Start" + " at " + DateTime.UtcNow);

            EasyPayContext db = new EasyPayContext();
            var prepaidOrders = db.Orders.Include(o => o.ProviderService).Where(x => x.UserProfileId == 6 && x.Status == (int)OrderStatus.Success).Where(y => y.ProviderService.ServiceTypeId != 2);
            var postpaidOrders = db.Orders.Include(o => o.ProviderService).Where(x => x.UserProfileId == 6 && x.Status == (int)OrderStatus.Success).Where(y => y.ProviderService.ServiceTypeId == 2);
            ServiceEngine.ServiceHelper serviceHelper = new ServiceEngine.ServiceHelper();
            ViewBag.Balance = serviceHelper.ChecktWallet();
            logger.Info("Wallet Balance" + ViewBag.Balance + " at " + DateTime.UtcNow);
            decimal totalPrepaidAmount = prepaidOrders.Sum(x => x.Amount);
            decimal totalPostpaidAmount = postpaidOrders.Sum(y => y.Amount);
            ViewBag.UserPrepaidCommision = (totalPrepaidAmount * 2) / 100;
            ViewBag.UserPostpaidCommision = (totalPostpaidAmount * 3) / 10000;

            logger.Info("TotalPrepaidAmount " + totalPrepaidAmount + " at " + DateTime.UtcNow);
            logger.Info("TotalPostpaidAmount " + totalPostpaidAmount + " at " + DateTime.UtcNow);

            return View();
        }

    }
}
