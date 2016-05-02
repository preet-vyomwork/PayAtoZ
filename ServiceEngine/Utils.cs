using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Configuration;


namespace ServiceEngine
{
    public class Utils
    {
        public string SaveFile(HttpPostedFileBase file)
        {
            string path = "";
            if (file != null && file.ContentLength > 0)
                try
                {
                    path = Path.Combine(HttpContext.Current.Server.MapPath("~/Images"),Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    //ViewBag.Message = "File uploaded successfully";
                    path = path.Substring(path.IndexOf("\\Images"));
                }
                catch (Exception ex)
                {
                    //ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
               
                //ViewBag.Message = "You have not specified a file.";
            }
            return path;
        }
        public void mailUser(string subject, string body, string recipient, string userName)
        {
            //string userName = User.Identity.Name;
            //var email = (from userProfile in db.UserProfiles
            //             where userProfile.UserName == userName
            //             select userProfile.Email).FirstOrDefault();
            //email = "dolly.vyomwork@gmail.com";
            StringBuilder sb = new StringBuilder();
            sb.Append(body);
            string smtpServer = WebConfigurationManager.AppSettings["SMTPServer"].ToString();
            int port = Convert.ToInt32(Convert.ToString(WebConfigurationManager.AppSettings["SMTPPort"]));
            string from = WebConfigurationManager.AppSettings["AdminEmail"].ToString();
            string pwd = WebConfigurationManager.AppSettings["AdminPassword"].ToString();
            System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(new System.Net.Mail.MailAddress(from, "PayAtoZ Recharge Information"), new System.Net.Mail.MailAddress(recipient));
            m.Subject = subject;
            //m.Body = string.Format("Dear {0}, <BR/>Thank you for your registration.<BR/>Please use following token {1} to confirm your user.",model.UserName, model.UserToken);
            m.Body = string.Format(sb.ToString(), userName);
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient(smtpServer, port);
            smtp.Credentials = new System.Net.NetworkCredential(from, pwd);
            smtp.EnableSsl = false;
            smtp.Send(m);
        }
    }
}
