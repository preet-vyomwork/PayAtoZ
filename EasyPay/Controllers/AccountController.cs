using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using EasyPay.Filters;
using EasyPay.Models;
using System.Net.Mail;
using System.Web.Configuration;
using System.Text;
using System.Data;
using log4net;
using System.Drawing;

namespace EasyPay.Controllers
{
    //[Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        EasyPayContext _context = new EasyPayContext();
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            logger.Info("Method Start" + " at " + DateTime.UtcNow);
            EasyPayContext context = new EasyPayContext();
            var thisUser = context.UserProfiles.Where(t=> t.UserName==model.UserName).FirstOrDefault();
            var thisUserRole=context.UsersInRoles.Where(r=>r.UserId==thisUser.UserProfileId).FirstOrDefault();
            var thisRole = Roles.GetRolesForUser(thisUser.UserName).FirstOrDefault();
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                if (thisRole == "Merchant")
                {
                    return RedirectToAction("DashBoard", "Merchant", new { id = thisUser.UserProfileId });
                }
                else if (thisRole == "Admin")
                {
                    return RedirectToAction("Index", "Order", new { page = 1 });
                }
                else
                {
                    return RedirectToLocal(returnUrl);
                }
                
            }

            // If we got this far, something failed, redisplay form
            logger.Info("The user name or password provided is incorrect" + " at " + DateTime.UtcNow);
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            logger.Info("Method Start" + " at " + DateTime.UtcNow);
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }
        public string RegisterUser(RegisterModel model)
        {
            model.UserToken = RandomString(6);
            var userInfo = new { Address = model.Address, City = model.City, StateID = model.StateID, PostalCode = model.PostalCode, Email = model.Email, Phone = model.Phone, BirthDate = model.BirthDate, UserToken = model.UserToken };
            model.DBUserToken = WebSecurity.CreateUserAndAccount(model.UserName, model.Password, userInfo, true);
            logger.Info("Registration Done" + " at " + DateTime.UtcNow);
            return model.DBUserToken;
        }
        //
        // GET: /Account/Register

        [AllowAnonymous]
        //[Authorize(Roles = "Admin")]
        public ActionResult Register(string returnUrl)
        {
            ViewData.Model = new RegisterModel();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model, string returnUrl)
        {
            logger.Info("Method Start" + " at " + DateTime.UtcNow);

            if (ModelState.IsValid)
            {
                model.IsSkip = "Undefined";
                // Attempt to register the user
                try
                {
                    string realCaptcha = Session["captcha"].ToString();
                    if (model.Captcha == realCaptcha)
                    {


                        //model.UserToken = RandomString(6);
                        //var userInfo = new { Address = model.Address, City = model.City, StateID = model.StateID, PostalCode = model.PostalCode, Email = model.Email, Phone = model.Phone, BirthDate = model.BirthDate, UserToken = model.UserToken };
                        //model.DBUserToken = WebSecurity.CreateUserAndAccount(model.UserName, model.Password, userInfo, true);
                        //logger.Info("Registration Done" + " at " + DateTime.UtcNow);
                        RegisterUser(model);



                        StringBuilder sb = new StringBuilder();
                        sb.Append(" <div style=\"clear:left\"><div style=\"float: right\"><img src=\"http://payatoz.com/images/logo.png\" alt=\"PayAtoZ\"' /></div></div><div class=\"line\"></div>");
                        sb.Append("<div><div>Dear {0},</div><p><span>Welcome to PayAtoZ !</span></p><p>Thank you for your registration and choosing us as your recharge service provider.");
                        sb.Append("At <b>PayAtoZ</b> you can recharge your <b>Mobile, DTH</b> and <b>DataCard</b> in an easy and secure manner.</p><b>Need Confirmation</b><p>");
                        sb.Append("Please use following token <b>{1}</b> to confirm your user. Once you confirmed then next time PayAtoZ will not ask again for token.");
                        sb.Append("<p>UserName : <b>{0}</b></p></p><span>Once you logged in to your account, you will be able to:</span>");
                        sb.Append(" <ul><li>Proceed through checkout faster when making a recharge,</li><li>Use wallet to recharge faster and easily,</li>");
                        sb.Append("<li>Check the status of your recharges,</li><li>View previous recharges,</li><li>Make changes to your account information,</li>");
                        sb.Append("<li>Change your password. </li></ul>");
                        sb.Append(" <div class=\"line\"></div><div>We Are Here For You!</div><p>If you have any questions about your account or any other matter, please feel free to contact us at info@payatoz.com - we'd love to help you out! We kindly thank you for your interest.");
                        sb.Append("</p>Yours sincerely,<p>The PayAtoZ Family</p></div>");

                        string smtpServer = WebConfigurationManager.AppSettings["SMTPServer"].ToString();
                        int port = Convert.ToInt32(Convert.ToString(WebConfigurationManager.AppSettings["SMTPPort"]));
                        string from = WebConfigurationManager.AppSettings["AdminEmail"].ToString();
                        string pwd = WebConfigurationManager.AppSettings["AdminPassword"].ToString();
                        MailMessage m = new MailMessage(new System.Net.Mail.MailAddress(from, "PayAtoZ Registration"), new System.Net.Mail.MailAddress(model.Email));
                        m.Subject = "Email confirmation";
                        //m.Body = string.Format("Dear {0}, <BR/>Thank you for your registration.<BR/>Please use following token {1} to confirm your user.",model.UserName, model.UserToken);
                        m.Body = string.Format(sb.ToString(), model.UserName, model.UserToken);
                        m.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient(smtpServer, port);
                        smtp.Credentials = new System.Net.NetworkCredential(from, pwd);
                        smtp.EnableSsl = false;
                        smtp.Send(m);
                        logger.Info("Mail Sent" + " at " + DateTime.UtcNow);
                        Session["RegisterModel"] = model;
                        ////WebSecurity.Login(model.UserName, model.Password);

                        //If a Role "User" doesn't exist in the AspNetRole table then first create a new role
                        EasyPayContext context = new EasyPayContext();
                        string newRoleName = "User";
                        if (!Roles.RoleExists(newRoleName))
                        {
                            Roles.CreateRole(newRoleName);
                        }

                        //var role = context.Roles.SingleOrDefault(r => r.RoleName == "User");
                        //if (role == null)
                        //{
                        //    context.Roles.Add(new Role()
                        //    {
                        //        RoleName = "User"
                        //    });
                        //    context.SaveChanges();
                        //}

                        //Assign a role "User" to newly registered user
                        //Role aspNetRole = new Role();
                        //aspNetRole.RoleName = "User";

                        var userProfile = (from u in context.UserProfiles.Where(u => u.UserName == model.UserName) select u).FirstOrDefault();

                        var checkUserRole = context.UsersInRoles.ToList().FirstOrDefault(r => r.UserId == userProfile.UserProfileId);
                        if (checkUserRole == null)
                        {

                            context.UsersInRoles.Add(new UsersInRole()
                                {
                                    RoleId = context.Roles.Where(r => r.RoleName == newRoleName).Select(r => r.RoleId).FirstOrDefault(),
                                    UserId = userProfile.UserProfileId

                                    //RoleId=Roles.
                                });
                            context.SaveChanges();
                            //var account = new AccountController();
                            //account.UserManager.AddToRole(user.Id, aspNetRole.Name);
                        }
                        return RedirectToAction("CheckYourMail", new { url = returnUrl, email = model.Email });
                    }
                     else
                        ModelState.AddModelError("", "Verification Code is Incorrect");
                        ViewBag.ErrorCaptcha = "Your verification code is incorrect";
                }
                catch (MembershipCreateUserException e)
                {
                    logger.Error("Error: " + e.InnerException + " " + e.StackTrace + " at " + DateTime.UtcNow);
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
         public CaptchImageAction Image()
        {
            string randomText = SelectRandomWord(6);
            Session["captcha"] = randomText;
            HttpContext.Session["RandomText"] = randomText;
            return new CaptchImageAction() { BackgroundColor = Color.LightGray, 
                       RandomTextColor = Color.Black, RandomText = randomText };
        }
        private string SelectRandomWord(int numberOfChars)
        {       
            if (numberOfChars > 36)
            {
                throw new InvalidOperationException("Random Word Characters cannot be greater than 36");
            }
            char[] columns = new char[36];
            for (int charPos = 65; charPos < 65 + 26; charPos++)
                columns[charPos - 65] = (char)charPos;
            for (int intPos = 48; intPos <= 57; intPos++)
                columns[26 + (intPos - 48)] = (char)intPos;
            StringBuilder randomBuilder = new StringBuilder();
            Random randomSeed = new Random();
            for (int incr = 0; incr < numberOfChars; incr++)
            {
                randomBuilder.Append(columns[randomSeed.Next(36)].ToString());
            }
            return randomBuilder.ToString();
        }

        /// <summary>
        /// It will generate random string.
        /// </summary>
        /// <param name="Size"></param>
        /// <returns></returns>
        private string RandomString(int Size)
        {
            logger.Info("RandomString Method Start" + " at " + DateTime.UtcNow);
            string input = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            char ch;
            Random random = new Random();
            for (int i = 0; i < Size; i++)
            {
                ch = input[random.Next(0, input.Length)];
                builder.Append(ch);
            }
            logger.Info("RandomString Method End" + " at " + DateTime.UtcNow);
            return builder.ToString();
        }

        [AllowAnonymous]
        public ActionResult CheckYourMail(string url, string email)
        {
            ViewBag.Email = email;
            ViewBag.ReturnUrl = url;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CheckYourMail(string returnUrl, string email, string txtUserToken)
        {
            logger.Info("CheckYourMail Method Start" + " at " + DateTime.UtcNow);
            RegisterModel model = Session["RegisterModel"] as RegisterModel;
            if (model.UserToken == txtUserToken)
            {
                WebSecurity.ConfirmAccount(model.UserName, model.DBUserToken);
                if (WebSecurity.Login(model.UserName, model.Password))
                {
                    EasyPayContext db = new EasyPayContext();
                    UserWallet userwallet = new Models.UserWallet();
                    MembershipUser membershipUser = System.Web.Security.Membership.GetUser();
                    userwallet.UserProfileId = WebSecurity.GetUserId(model.UserName);
                    userwallet.Balance = 0;
                    db.UserWallets.Add(userwallet);
                    db.SaveChanges();
                    return RedirectToLocal(returnUrl);
                }
            }
            logger.Info("CheckYourMail Method End" + " at " + DateTime.UtcNow);
            return View();
        }

        [AllowAnonymous]
        public ActionResult ConfirmEmail(string userName, string userEmail, string userToken)
        {
            logger.Info("ConfirmEmail Method Start" + " at " + DateTime.UtcNow);
            ViewBag.UserName = userName;
            if (WebSecurity.ConfirmAccount(userToken))
            {
                logger.Info("ConfirmEmail Method End" + " at " + DateTime.UtcNow);
                return RedirectToAction("ConfirmSuccess");
            }
            else
            {
                logger.Info("ConfirmEmail Method End" + " at " + DateTime.UtcNow);
                return RedirectToAction("ConfirmFailure");
            }
        }

        [AllowAnonymous]
        public ActionResult ConfirmSuccess()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ConfirmFailure()
        {
            return View();
        }


        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            logger.Info("Disassociate Method Start" + " at " + DateTime.UtcNow);
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }
            logger.Info("Disassociate Method End" + " at " + DateTime.UtcNow);
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            logger.Info("Manage Method Start" + " at " + DateTime.UtcNow);
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }
            logger.Info("Manage Method End" + " at " + DateTime.UtcNow);
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string UserName)
        {
            logger.Info("ForgotPassword Method Start" + " at " + DateTime.UtcNow);
            //check user existance
            var user = System.Web.Security.Membership.GetUser(UserName);
            if (user == null)
            {
                TempData["Message"] = "User Not exist.";
            }
            else
            {
                //generate password token
                var token = WebSecurity.GeneratePasswordResetToken(UserName);
                //create url with above token
                var resetLink = "<a href='" + Url.Action("ResetPassword", "Account", new { un = UserName, rt = token }, "http") + "'>Reset Password</a>";
                //get user emailid

                var emailid = (from i in _context.UserProfiles
                               where i.UserName == UserName
                               select i.Email).FirstOrDefault();
                //send mail
                string subject = "Password Reset Token";
                string body = "<b>Please find the Password Reset Token</b><br/>" + resetLink; //edit it
                try
                {
                    SendEMail(emailid, subject, body);
                    TempData["Message"] = "Mail Sent.";
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "Error occured while sending email." + ex.Message;
                }
                //only for testing
                TempData["Message"] = resetLink;
            }
            logger.Info("ForgotPassword Method End" + " at " + DateTime.UtcNow);
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string un, string rt)
        {
            logger.Info("ResetPassword Method Start" + " at " + DateTime.UtcNow);
            //TODO: Check the un and rt matching and then perform following
            //get userid of received username
            int userid = (from i in _context.UserProfiles
                          where i.UserName == un
                          select i.UserProfileId).FirstOrDefault();

            int useridFromRt = WebSecurity.GetUserIdFromPasswordResetToken(rt);
            //check userid and token matches
            //bool any = (from j in WebSecurity.GetUserIdFromPasswordResetToken(rt)
            //            where (j.UserId == userid)
            //            && (j.PasswordVerificationToken == rt)
            //            //&& (j.PasswordVerificationTokenExpirationDate < DateTime.Now)
            //            select j).Any();

            if (userid.Equals(useridFromRt))
            {
                //generate random password
                string newpassword = GenerateRandomPassword(6);
                //reset password
                bool response = WebSecurity.ResetPassword(rt, newpassword);
                if (response == true)
                {
                    //get user emailid to send password
                    var emailid = (from i in _context.UserProfiles
                                   where i.UserName == un
                                   select i.Email).FirstOrDefault();
                    //send email
                    string subject = "New Password";
                    string body = "<b>Please find the New Password</b><br/>" + newpassword; //edit it
                    try
                    {
                        SendEMail(emailid, subject, body);
                        TempData["Message"] = "We have sent new password to your registered mail account.";
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = "Error occured while sending email." + ex.Message;
                    }

                    //display message
                    //TempData["Message"] = "Success! Check email we sent. Your New Password Is " + newpassword;
                }
                else
                {
                    TempData["Message"] = "Hey, avoid random request on this page.";
                }
            }
            else
            {
                TempData["Message"] = "Username and reset token not maching.";
            }

            logger.Info("ResetPassword Method End" + " at " + DateTime.UtcNow);
            return View();
        }
        private string GenerateRandomPassword(int length)
        {
            logger.Info("GenerateRandomPassword Method Start" + " at " + DateTime.UtcNow);
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-*&#+";
            char[] chars = new char[length];
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            logger.Info("GenerateRandomPassword Method End" + " at " + DateTime.UtcNow);
            return new string(chars);
        }
        public void SendEMail(string email, string subject, string body)
        {
            logger.Info("SendEMail Method Start" + " at " + DateTime.UtcNow);
            string smtpServer = WebConfigurationManager.AppSettings["SMTPServer"].ToString();
            int port = Convert.ToInt32(Convert.ToString(WebConfigurationManager.AppSettings["SMTPPort"]));
            string from = WebConfigurationManager.AppSettings["AdminEmail"].ToString();
            string pwd = WebConfigurationManager.AppSettings["AdminPassword"].ToString();
            MailMessage m = new MailMessage(new System.Net.Mail.MailAddress(from, "Payatoz"), new System.Net.Mail.MailAddress(email));
            m.Subject = subject;
            m.Body = body;
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient(smtpServer, port);
            smtp.Credentials = new System.Net.NetworkCredential(from, pwd);
            smtp.EnableSsl = false;
            smtp.Send(m);
            logger.Info("SendEMail Method End" + " at " + DateTime.UtcNow);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            logger.Info("ExternalLoginCallback Method Start" + " at " + DateTime.UtcNow);
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                logger.Info("ExternalLoginCallback Method End" + " at " + DateTime.UtcNow);
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                logger.Info("ExternalLoginCallback Method End" + " at " + DateTime.UtcNow);
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                logger.Info("ExternalLoginCallback Method End" + " at " + DateTime.UtcNow);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                logger.Info("ExternalLoginCallback Method End" + " at " + DateTime.UtcNow);
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            logger.Info("ExternalLoginConfirmation Method Start" + " at " + DateTime.UtcNow);
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (EasyPayContext db = new EasyPayContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            logger.Info("ExternalLoginConfirmation Method End" + " at " + DateTime.UtcNow);
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            logger.Info("ExternalLoginsList Method Start" + " at " + DateTime.UtcNow);
            ViewBag.ReturnUrl = returnUrl;
            logger.Info("ExternalLoginsList Method End" + " at " + DateTime.UtcNow);
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            logger.Info("RemoveExternalLogins Method Start" + " at " + DateTime.UtcNow);
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            logger.Info("RemoveExternalLogins Method End" + " at " + DateTime.UtcNow);
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {

            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
