using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EasyPay.Models;
using System.Web.Security;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Specialized;
using System.Web.Configuration;
using log4net;
using PagedList;
using System.Data.Entity;
using System.Web.Mail;
using System.Net.Mail;
using System.Collections;
using ServiceEngine;


namespace EasyPay.Controllers
{
    public class ConfirmOrderController : Controller
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /ConfirmOrder/

        EasyPayContext db = new EasyPayContext();

        [HttpPost]
        public ActionResult Pay(string paymentMode, List<string> categoryList)
        {
            logger.Info("Pay Method Start" + " at " + DateTime.UtcNow);
            Utils utils = new Utils();
            OrderCoupon orderCoupon;
            List<OrderCoupon> orderCoupons = new List<OrderCoupon>();
            List<Merchant> merchants = new List<Merchant>();
            try
            {
                string subject = "";
                string body = "";
                //var paymentMode = Request.Form["payment"];
                Order oldOrder = (Order)Session["order"];

                ServiceEngine.ServiceOrder serviceOrder = (ServiceEngine.ServiceOrder)Session["serviceOrder"];
                Order order = db.Orders.Include("UserProfile").First(t => t.OrderId == oldOrder.OrderId);
                int orderId = order.OrderId;
                int couponTotalAmount = 0;

                //if (categoryList != null)
                //{
                //    string[] array = categoryList.ToArray();
                //    string category = array[0].ToString();
                //    int[] categoryId = (category.Split(',').Select(int.Parse).ToArray());
                //    string coupCode = "";


                //    subject = "Coupons";
                //    for (int i = 0; i < categoryId.Length; i++)
                //    {
                //        orderCoupon = new OrderCoupon();
                //        var id = categoryId[i];
                //        var couponAmount = (from li in db.Coupons
                //                            where li.CouponId == id
                //                            select li).FirstOrDefault();
                //        couponTotalAmount += Convert.ToInt32(couponAmount.CouponValue);
                //        orderCoupon.OrderId = orderId;
                //        orderCoupon.CouponId = categoryId[i];
                //        orderCoupon.CouponAmount = couponAmount.CouponValue;

                //        orderCoupons.Add(orderCoupon);
                //        coupCode = couponAmount.CouponCode;
                //        var merchant = db.Merchants.Where(m => m.MerchantId == couponAmount.MerchantId).FirstOrDefault();

                //        body = "<div><div>Dear {0},</div><p><span>Your coupon has been sold having Couponcode - " + coupCode + " in Amount Rs." + orderCoupon.CouponAmount + " </span></p><p>Thank you.";
                //       utils.mailUser(subject, body, merchant.EmailId, merchant.MerchantName);
                //       // merchants.Add(merchant);
                //        //db.OrderCoupons.Add(orderCoupon);
                //        //db.SaveChanges();
                //    }
                //    //send mail to all the merchants whose coupon(s) have been sold.
                //    //subject = "Coupons";
                //    //body = "<div><div>Dear {0},</div><p><span>Your coupon has been sold having Couponcode = "+coupCode+" in Amount Rs."+orderCoupon.CouponAmount+" </span></p><p>Thank you.";
                //    //foreach (var merchant in merchants)
                //    //{

                //    //}
                //    Session["_ssnOrderCoupon"] = orderCoupons;
                //    order.CouponAmount = couponTotalAmount;
                //}
                //if (couponTotalAmount != null)
                //order.TotalAmount = (order.Amount) + couponTotalAmount;
                //else
                //order.TotalAmount = order.Amount;
                ////if(finalOrder.CouponAmount != null)
                ////order.CouponAmount = finalOrder.CouponAmount;
                ////if (finalOrder.TotalAmount != null)
                ////order.TotalAmount = finalOrder.TotalAmount;

                //string userName = User.Identity.Name;
                //var email = (from userProfile in db.UserProfiles
                //             where userProfile.UserName == userName
                //             select userProfile.Email).FirstOrDefault();
                //subject = "Email confirmation";
                //body = "<div><div>Dear {0},</div><p><span>Your Recharge has been successfuly done</span></p><p>Thank you for choosing us as your recharge service provider.";
                order.TotalAmount = order.Amount;
                logger.Info("Pay Method Order Id" + order.OrderId + " at " + DateTime.UtcNow);
                logger.Info("Pay Method Order Amount" + order.Amount + " at " + DateTime.UtcNow);
                ServiceEngine.ServiceHelper serviceHelper = new ServiceEngine.ServiceHelper();
                MembershipUser membershipUser = System.Web.Security.Membership.GetUser();
                order.UserProfileId = Convert.ToInt32(membershipUser.ProviderUserKey);
                if (paymentMode == "Wallet")
                {
                    logger.Info("Pay Method Payment mode is wallet" + " at " + DateTime.UtcNow);
                    UserWallet userwallet = db.UserWallets.FirstOrDefault(x => x.UserProfileId == order.UserProfileId);
                    if (userwallet != null)
                    {
                        logger.Info("Pay Method Wallet is not null - Balance " + userwallet.Balance + " at " + DateTime.UtcNow);

                        if (userwallet.Balance > order.TotalAmount)
                        {
                            logger.Info("Pay Method Payment mode is wallet Balance" + userwallet.Balance + " and order amount " + order.TotalAmount + " at " + DateTime.UtcNow);
                            userwallet.Balance -= Convert.ToDecimal(order.TotalAmount);
                            db.Entry(userwallet).State = EntityState.Modified;
                            db.Entry(order).State = EntityState.Modified;
                            order.Status = (int)OrderStatus.PaymentSuceess;
                            logger.Info("Pay Method Order Status " + order.Status + " at " + DateTime.UtcNow);
                            string resultString = serviceHelper.DoRecharge(serviceOrder);
                            ProcessResult(resultString, order);
                            db.SaveChanges();
                        }
                        else
                        {
                            logger.Info("Pay Method Payment mode is wallet Balance" + userwallet.Balance + " and order amount " + order.TotalAmount + " at " + DateTime.UtcNow);
                            db.Entry(order).State = EntityState.Modified;
                            order.Status = (int)OrderStatus.InSufficientBalance;
                            logger.Info("Pay Method Order Status " + order.Status + " at " + DateTime.UtcNow);
                            db.SaveChanges();
                            logger.Info("Pay Method End" + " at " + DateTime.UtcNow);
                            return Redirect("~/Order/Balance");
                        }
                    }
                    else
                    {
                        logger.Info("Pay Method Wallet is null - Balance " + userwallet.Balance + " at " + DateTime.UtcNow);
                        db.Entry(order).State = EntityState.Modified;
                        order.Status = (int)OrderStatus.InSufficientBalance;
                        logger.Info("Pay Method Order Status " + order.Status + " at " + DateTime.UtcNow);
                        db.SaveChanges();
                        logger.Info("Pay Method End" + " at " + DateTime.UtcNow);
                        return Redirect("~/Order/Balance");
                    }
                    //db.Orders.Add(order);
                    //db.SaveChanges();
                    //order.Remarks = "AZ" + order.OrderId.ToString() + GetUniqueKey();
                    //serviceOrder.OrderNumber = order.Remarks;

                    Session.Remove("order");
                    Session.Remove("serviceOrder");
                    logger.Info("Pay Method End" + " at " + DateTime.UtcNow);

                    //Call Email Method
                    //utils.mailUser(subject, body, email, userName);
                    //return RedirectToAction("Index","Order");
                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    Session.Remove("order");
                    //    Session.Remove("serviceOrder");
                    //    return RedirectToAction("Index");
                    //}

                }
                else
                {
                    logger.Info("Pay Method Payment mode is Online Payment" + " at " + DateTime.UtcNow);
                    //db.Entry(order).State = EntityState.Modified;
                    //order.Status = (int)OrderStatus.PaymentInProgress;
                    //db.SaveChanges();
                    GenerateRequest(order);
                    logger.Info("Pay Method End" + " at " + DateTime.UtcNow);

                    //Call Email Method
                    // utils.mailUser(subject, body, email, userName);
                    return Redirect("/");
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                var outputLines = new List<string>();
                foreach (var eve in e.EntityValidationErrors)
                {
                    outputLines.Add(string.Format(
                        "{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:",
                        DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        outputLines.Add(string.Format(
                            "- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }
                System.IO.File.AppendAllLines(@"c:\temp\errors.txt", outputLines);
                logger.Error("Error: " + e.InnerException + " " + e.StackTrace + " " + e.Message + " at " + DateTime.UtcNow);
                throw;
            }
        }

        public ActionResult GetCouponsList(int pageSize, int page)
        {
            //XDocument feedXML = XDocument.Load(_feedURI);

            //var feeds =
            //  from feed in feedXML.Descendants("item")
            //  select new
            //  {
            //      Date = DateTime.Parse(feed.Element("pubDate").Value)
            //                     .ToShortDateString(),
            //      Title = feed.Element("title").Value,
            //      Link = feed.Element("link").Value,
            //      Description = feed.Element("description").Value,
            //  };

            //return feeds.Skip((Page - 1) * PageSize).Take(PageSize);
            EasyPayContext context = new EasyPayContext();
            var coupons = (from c in context.Coupons select c).OrderBy(x => x.ValidityEnd).ToList();
            //ViewBag.Coupons = coupons;
            //ViewBag.Coupons = coupons.Skip((page - 1) * pageSize).Take(pageSize);
            List<Coupon> couponModel = coupons.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            //ViewBag.Coupons = couponModel;
            return PartialView("_Coupon", couponModel);

            //// EasyPayContext context = new EasyPayContext();
            ////int? page = 1;
            //int pageNumber = page ?? 1;
            //var coupons = (from c in db.Coupons select c).OrderByDescending(x => x.ValidityEnd);
            //var couponModel = coupons.ToPagedList(pageNumber, 9);
            ////List<Coupon> coupens = (from c in db.Coupons select c).ToList();
            //ViewBag.Coupons = couponModel;

        }

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
        private void ProcessResult(string result, Order order)
        {
            logger.Info("ProcessResult Method Start" + " at " + DateTime.UtcNow);
            logger.Info("Result parameter" + result + " at " + DateTime.UtcNow);
            MembershipUser membershipUser = System.Web.Security.Membership.GetUser();
            order.UserProfileId = Convert.ToInt32(membershipUser.ProviderUserKey);
            var userWallet = db.UserWallets.ToList();
            UserWallet userwallet = db.UserWallets.FirstOrDefault(x => x.UserProfileId == order.UserProfileId);
            string[] resultParams = result.Split(',');
            logger.Info("Result parameter" + resultParams.Count() + " at " + DateTime.UtcNow);
            if (resultParams.Count() > 1)
            {
                order.Remarks = order.Remarks + "," + resultParams[0];// "AZ" + order.OrderId.ToString() + GetUniqueKey();
                if (resultParams[1].ToString().ToLower().Equals("success"))
                {
                    order.Status = (int)OrderStatus.Success;
                    logger.Info("Order status" + order.Status + " at " + DateTime.UtcNow);
                }
                else if (resultParams[1].ToString().ToLower().Equals("failed"))
                {
                    order.Status = (int)OrderStatus.Failed;
                    logger.Info("Order status" + order.Status + " at " + DateTime.UtcNow);
                    if (userwallet != null)
                    {
                        userwallet.Balance += order.Amount;
                        logger.Info("Order Failed, Update User Wallet Balance" + userwallet.Balance + " at " + DateTime.UtcNow);
                    }
                }

            }
            logger.Info("ProcessResult Method End" + " at " + DateTime.UtcNow);
        }
        /// <summary>
        /// This method will give list of orders.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int? page, int orderId = 0)
        {
            logger.Info("Index Method Start" + " at " + DateTime.UtcNow);

            if (orderId != null)
            {
                var Query = (from orders in db.Orders
                             where orders.OrderId == orderId
                             join userRoles in db.ProviderServices on orders.ProviderServiceId equals userRoles.ProviderServiceId into rolesOfUser
                             from rou in rolesOfUser.DefaultIfEmpty()

                             join roles in db.ServiceOperators on rou.ServiceOperatorId equals roles.ServiceOperatorId into getRoles
                             from gr in getRoles.DefaultIfEmpty()

                             orderby orders.OrderDate descending
                             select new { orders.ItemCode, orders.Amount, gr.OperatorName }).FirstOrDefault();

                if (Query != null)
                {

                    ViewData["Amount"] = Query.Amount;
                    ViewData["MobileNumber"] = Query.ItemCode;
                    ViewData["Operator"] = Query.OperatorName;
                }
            }

            Order order = (EasyPay.Models.Order)Session["order"];
            MembershipUser membershipUser = System.Web.Security.Membership.GetUser();
            // oldOrder.UserProfileId = Convert.ToInt32(membershipUser.ProviderUserKey);
            if (order != null)
            {
                UserWallet userwallet = db.UserWallets.FirstOrDefault(x => x.UserProfileId == order.UserProfileId);

                if (userwallet != null)
                {
                    logger.Info("User Wallet is not null" + " at " + DateTime.UtcNow);
                    if (userwallet.Balance > order.Amount)
                    {
                        logger.Info("User Wallet is " + userwallet.Balance + " Order Amount " + order.Amount + " at " + DateTime.UtcNow);
                        //1= show
                        ViewBag.WalletHide = 1;
                    }
                    else
                    {
                        logger.Info("User Wallet is " + userwallet.Balance + " Order Amount " + order.Amount + " at " + DateTime.UtcNow);
                        //2=Hide
                        ViewBag.WalletHide = 2;
                    }
                }
            }
            else
            {
                logger.Info("Order is null at " + DateTime.UtcNow);
                //2=Hide
                ViewBag.WalletHide = 2;
            }
            //get categories
            //ViewBag.Category = new SelectList(db.Categories, "CategoryId", "CategoryName");
            //var result = (from li in db.Categories
            //              select new
            //              {
            //                  CategoryId = li.CategoryId.ToString(),
            //                  CategoryName = li.CategoryName
            //              });

            //ViewBag.Category = result.Select(l => l.CategoryId + "_" + l.CategoryName).ToList();
            List<CategoryViewModel> caegoryList = new List<CategoryViewModel>();
            CategoryViewModel category = new CategoryViewModel();
            List<CategoryViewModel> categoryView = (from li in db.Categories
                                                    select new CategoryViewModel
                                                    {
                                                        CategoryId = li.CategoryId,
                                                        CategoryName = li.CategoryName
                                                    }).ToList();



            ViewBag.Category = categoryView;
            // EasyPayContext context = new EasyPayContext();
            //int? page = 1;
            //int pageNumber = page ?? 1;
            //List<Coupon> coupons = (from c in db.Coupons select c).OrderByDescending(x => x.ValidityEnd).Take(9).ToList();
            //var couponModel = coupons.ToPagedList(pageNumber, 9);
            ////List<Coupon> coupens = (from c in db.Coupons select c).ToList();
            // ViewBag.Coupons = coupons;

            logger.Info("Index Method End" + " at " + DateTime.UtcNow);
            return View(order);
        }

        public ActionResult getCoupons(int currentCategoryId, string categoryType)
        {
            logger.Info("Method Start" + " at " + DateTime.UtcNow);
            List<Coupon> couponsList;
            if (currentCategoryId == 0 && categoryType == "All")
            {
                couponsList = (from c in db.Coupons select c).ToList();
            }
            else if (currentCategoryId != 0 && categoryType == "All")
            {
                couponsList = (from coupon in db.Coupons
                               where coupon.Category.CategoryId == currentCategoryId
                               select coupon).ToList();
            }
            else if (currentCategoryId == 0 && (categoryType == "Free" || categoryType == "Paid"))
            {
                couponsList = (from coupon in db.Coupons
                               where coupon.Type == categoryType
                               select coupon).ToList();
            }
            else
            {
                couponsList = (from coupon in db.Coupons
                               where coupon.Category.CategoryId == currentCategoryId && coupon.Type == categoryType
                               select coupon).ToList();
            }
            logger.Info("Method End" + " at " + DateTime.UtcNow);
            return PartialView("_Coupon", couponsList.Take(9));
        }

        //
        // GET: /ConfirmOrder/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /ConfirmOrder/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ConfirmOrder/Create
        /// <summary>
        /// This method will create new order.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            logger.Info("Create HttpPost Method Start" + " at " + DateTime.UtcNow);
            try
            {
                // TODO: Add insert logic here
                logger.Info("Create HttpPost Method End" + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            catch
            {
                logger.Info("Create HttpPost Method End" + " at " + DateTime.UtcNow);
                return View();
            }
        }

        //
        // GET: /ConfirmOrder/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /ConfirmOrder/Edit/5
        /// <summary>
        /// This method will upadte selected data.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            logger.Info("Edit HttpPost Method Start" + " at " + DateTime.UtcNow);
            try
            {
                // TODO: Add update logic here
                logger.Info("Edit HttpPost Method End" + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            catch
            {
                logger.Info("Edit HttpPost Method End" + " at " + DateTime.UtcNow);
                return View();
            }
        }

        //
        // GET: /ConfirmOrder/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ConfirmOrder/Delete/5
        /// <summary>
        /// This method will delete selected order.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            logger.Info("Delete HttpPost Method Start" + " at " + DateTime.UtcNow);
            try
            {
                // TODO: Add delete logic here
                logger.Info("Delete HttpPost Method End" + " at " + DateTime.UtcNow);
                return RedirectToAction("Index");
            }
            catch
            {
                logger.Info("Delete HttpPost Method End" + " at " + DateTime.UtcNow);
                return View();
            }
        }


        private void GenerateRequest(Order order)
        {
            logger.Info("GenerateRequest Method Start" + " at " + DateTime.UtcNow);
            string Url = "https://secure.ebs.in/pg/ma/sale/pay/";
            string Method = "post";
            string FormName = "form1";

            string secret_key = WebConfigurationManager.AppSettings["SecretKey"].ToString();
            string account_id = WebConfigurationManager.AppSettings["AccountId"].ToString();
            string amount = order.AmountString;
            string reference_no = Convert.ToString(order.OrderId);
            string mode = WebConfigurationManager.AppSettings["Mode"].ToString();
            string return_url = "http://www.payatoz.com/ConfirmOrder/response?DR={DR}";

            string input = secret_key + "|" + account_id + "|" + amount + "|" + reference_no + "|" + return_url + "|" + mode;

            MD5 md5 = MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }

            string secure_hash = sb.ToString();

            NameValueCollection FormFields = new NameValueCollection();
            FormFields.Add("account_id", account_id);
            FormFields.Add("reference_no", reference_no);
            FormFields.Add("amount", amount);
            FormFields.Add("description", "Testing");
            FormFields.Add("name", order.UserProfile.UserName);
            FormFields.Add("address", order.UserProfile.Address);
            FormFields.Add("city", order.UserProfile.City);
            FormFields.Add("state", order.UserProfile.State.StateName);
            FormFields.Add("postal_code", order.UserProfile.PostalCode);
            FormFields.Add("country", "IND");
            FormFields.Add("email", order.UserProfile.Email);
            FormFields.Add("phone", order.UserProfile.Phone);
            FormFields.Add("ship_name", order.UserProfile.UserName);
            FormFields.Add("ship_address", order.UserProfile.Address);
            FormFields.Add("ship_city", order.UserProfile.City);
            FormFields.Add("ship_state", order.UserProfile.State.StateName);
            FormFields.Add("ship_postal_code", order.UserProfile.PostalCode);
            FormFields.Add("ship_country", "IND");
            FormFields.Add("ship_phone", order.UserProfile.Phone);
            FormFields.Add("return_url", return_url);
            FormFields.Add("mode", mode);
            FormFields.Add("secure_hash", secure_hash);

            Response.Clear();
            Response.Write("<html><head>");
            Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));
            Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
            for (int i = 0; i < FormFields.Keys.Count; i++)
            {
                Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", FormFields.Keys[i], FormFields[FormFields.Keys[i]]));
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
            Order order = (Order)Session["order"];

            //Order order = db.Orders.Include("UserProfile").First(t => t.OrderId == oldOrder.OrderId);

            ResponseText responseString = new ResponseText();
            responseString.OrderId = order.OrderId;
            //esponseString.OrderId = 1;
            responseString.ResponseString = DR;
            db.ResponseText.Add(responseString);
            db.SaveChanges();

            string sQS;
            string[] aQS;
            string pwd = WebConfigurationManager.AppSettings["SecretKey"].ToString();
            DR = DR.Replace(' ', '+');
            sQS = Base64Decode(DR);
            DR = EasyPay.Models.RC4.Decrypt(pwd, sQS, false);
            aQS = DR.Split('&');

            TransactionResponse response = new TransactionResponse();
            foreach (string param in aQS)
            {
                string[] aParam = param.Split('=');
                switch (aParam[0])
                {
                    case "ResponseCode":
                        response.ResponseCode = aParam[1];
                        break;
                    case "ResponseMessage":
                        response.ResponseMessage = aParam[1];
                        break;
                    case "DateCreated":
                        response.DateCreated = Convert.ToDateTime(aParam[1]);
                        break;
                    case "PaymentID":
                        response.PaymentID = Convert.ToInt32(aParam[1]);
                        break;
                    case "MerchantRefNo":
                        response.MerchantRefNo = Convert.ToInt32(aParam[1]);
                        break;
                    case "Amount":
                        response.Amount = Convert.ToDecimal(aParam[1]);
                        break;
                    case "Mode":
                        response.Mode = aParam[1];
                        break;
                    case "BillingName":
                        response.BillingName = aParam[1];
                        break;
                    case "TransactionID":
                        response.TransactionID = Convert.ToInt32(aParam[1]);
                        break;
                    case "IsFlagged":
                        response.IsFlagged = aParam[1].ToString();
                        break;
                }
            }

            response.OrderId = response.MerchantRefNo;
            db.Responses.Add(response);
            order = db.Orders.FirstOrDefault(x => x.OrderId == response.OrderId);
            db.Entry(order).State = EntityState.Modified;
            logger.Info("Response Code" + response.ResponseCode + " at " + DateTime.UtcNow);
            if (response.ResponseCode != null || response.ResponseCode != string.Empty)
            {
                if (response.ResponseCode == "0")
                {
                    logger.Info("Response Code is 0" + " at " + DateTime.UtcNow);
                    //successful transaction
                    if (response.IsFlagged.ToLower().Equals("no"))
                    {
                        logger.Info("Response IsFlagged" + response.IsFlagged + " at " + DateTime.UtcNow);
                        //successful transaction
                        if (response.MerchantRefNo == order.OrderId && response.Amount == order.Amount)
                        {
                            logger.Info("Merchant Reference No " + response.MerchantRefNo + " Merchant Amount " + response.Amount + " at " + DateTime.UtcNow);
                            order.Status = (int)OrderStatus.PaymentSuceess;
                            logger.Info("Order Status " + order.Status + " at " + DateTime.UtcNow);
                            //order.Remarks = "Payment Successfull";
                            order.comments = response.ResponseMessage;
                        }
                        else
                        {
                            order.Status = (int)OrderStatus.WrongOrderId;
                            logger.Info("Order Status " + order.Status + " at " + DateTime.UtcNow);
                            // response with wrong orderid
                            //order.Status = (int)OrderStatus.Pending;
                            //order.Remarks = "Payment Pending";
                            order.comments = response.ResponseMessage;
                        }
                    }
                    else
                    {
                        logger.Info("Response IsFlagged" + response.IsFlagged + " at " + DateTime.UtcNow);
                        if (response.MerchantRefNo == order.OrderId && response.Amount == order.Amount)
                        {
                            order.Status = (int)OrderStatus.PaymentSuceess;
                            logger.Info("Order Status " + order.Status + " at " + DateTime.UtcNow);
                            //order.Remarks = "Payment Successfull";
                            order.comments = response.ResponseMessage;
                        }
                        //Temporary
                        ////pending trasaction
                        //order.Status = (int)OrderStatus.IsFlaggedPayment;
                        ////order.Remarks = "Payment Pending";
                        //order.comments = response.ResponseMessage;
                    }
                }
                else
                {
                    //declined trasaction or erroroccured
                    order.Status = (int)OrderStatus.PaymentFailed;
                    logger.Info("Order Status " + order.Status + " at " + DateTime.UtcNow);
                    //order.Remarks = "Payment Failed";
                    order.comments = response.ResponseMessage;
                }
            }
            else
            {
                logger.Info("Response code is null or empty at " + DateTime.UtcNow);
                //error occured in transaction
                order.Status = (int)OrderStatus.PaymentFailed;
                logger.Info("Order Status " + order.Status + " at " + DateTime.UtcNow);
                //order.Remarks = "Payment Failed";
                order.comments = response.ResponseMessage;
            }
            db.SaveChanges();
            ViewData.Model = order;
            logger.Info("GetResponse Method End" + " at " + DateTime.UtcNow);
            return View();
        }

        private string Base64Decode(string sBase64String)
        {
            logger.Info("Base64Decode Method Start" + " at " + DateTime.UtcNow);
            byte[] sBase64String_bytes =
            Convert.FromBase64String(sBase64String);
            logger.Info("Base64Decode Method End" + " at " + DateTime.UtcNow);
            return UnicodeEncoding.Default.GetString(sBase64String_bytes);
            //return UnicodeEncoding.ASCII.GetString(sBase64String_bytes);
        }
        public int DoRecharge(string orderId)
        {
            logger.Info("DoRecharge Method Start" + " at " + DateTime.UtcNow);
            try
            {
                ServiceEngine.ServiceHelper serviceHelper = new ServiceEngine.ServiceHelper();

                ServiceEngine.ServiceOrder serviceOrder = (ServiceEngine.ServiceOrder)Session["serviceOrder"];
                Order order = db.Orders.Find(Convert.ToInt32(orderId));
                db.Entry(order).State = EntityState.Modified;
                string resultString = serviceHelper.DoRecharge(serviceOrder);
                ProcessResult(resultString, order);
                db.SaveChanges();
                logger.Info("DoRecharge Method End" + " at " + DateTime.UtcNow);
                return (int)order.Status;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public ServiceEngine.ServiceOrder GetServiceOrder(Order order)
        {
            logger.Info("GetServiceOrder Method Start" + " at " + DateTime.UtcNow);
            ServiceEngine.ServiceOrder serviceOrder = new ServiceEngine.ServiceOrder();
            logger.Info("GetServiceOrder Method End" + " at " + DateTime.UtcNow);
            return serviceOrder;
        }

    }
}
