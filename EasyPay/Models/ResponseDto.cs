using System;
using System.Collections.Specialized;
using System.Web.Configuration;
using System.Security.Cryptography;
using System.Text;
namespace EasyPay.Models
{
    public class ResponseDto
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public DateTime DateCreated { get; set; }
        public int PaymentID { get; set; }
        public int MerchantRefNo { get; set; }
        public decimal Amount { get; set; }
        public string Mode { get; set; }
        public string BillingName { get; set; }
        public int TransactionID { get; set; }
        public bool IsFlagged { get; set; }
    }

    public class RequestParams
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public string FormName { get; set; }
        public string SecretKey { get; set; }
        public string Account_ID { get; set; }
        public string Refrense_No { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string User_Name { get; set; }
        public string User_Address { get; set; }
        public string User_City { get; set; }
        public string User_State { get; set; }
        public string User_PostalCode { get; set; }
        public string User_Country { get; set; }
        public string User_Email { get; set; }
        public string User_Phone { get; set; }
        public string Return_Url { get; set; }
        public string Mode { get; set; }
        public string Secure_Hash { get; set; }

        public NameValueCollection GenerateResponse(UserProfile user, string amount,string returnURL)
        {
            Url = "https://secure.ebs.in/pg/ma/sale/pay/";
            Return_Url = returnURL;
            Method = "post";
            FormName = "form1";
            SecretKey = WebConfigurationManager.AppSettings["SecretKey"].ToString();
            Account_ID = WebConfigurationManager.AppSettings["AccountId"].ToString();
            Amount = amount;
            Refrense_No = Convert.ToString(user.UserProfileId);
            Mode = WebConfigurationManager.AppSettings["Mode"].ToString();

            string input = SecretKey + "|" + Account_ID + "|" + Amount + "|" + Refrense_No + "|" + Return_Url + "|" + Mode;

            MD5 md5 = MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }

            Secure_Hash = sb.ToString();

            NameValueCollection formFields = new NameValueCollection();
            formFields.Add("account_id", Account_ID);
            formFields.Add("reference_no", Refrense_No);
            formFields.Add("amount", Amount);
            formFields.Add("description", "Testing");
            formFields.Add("name", user.UserName);
            formFields.Add("address", user.Address);
            formFields.Add("city", user.City);
            formFields.Add("state", user.State.StateName);
            formFields.Add("postal_code", user.PostalCode);
            formFields.Add("country", "IND");
            formFields.Add("email", user.Email);
            formFields.Add("phone", user.Phone);
            formFields.Add("ship_name", user.UserName);
            formFields.Add("ship_address", user.Address);
            formFields.Add("ship_city", user.City);
            formFields.Add("ship_state", user.State.StateName);
            formFields.Add("ship_postal_code", user.PostalCode);
            formFields.Add("ship_country", "IND");
            formFields.Add("ship_phone", user.Phone);
            formFields.Add("return_url", Return_Url);
            formFields.Add("mode", Mode);
            formFields.Add("secure_hash", Secure_Hash);
            return formFields;
        }

        public void GetResponse(string DR, int userProfileId, decimal amount, ref UserWalletLog entity)
        {
            EasyPayContext db = new EasyPayContext();
            ResponseText responseString = new ResponseText();
            responseString.UserIdt = userProfileId;
            responseString.ResponseString = DR;
            db.ResponseText.Add(responseString);
            db.SaveChanges();

            string sQS;
            string[] aQS;
            string pwd = WebConfigurationManager.AppSettings["SecretKey"].ToString();
            DR = DR.Replace(' ', '+');
            sQS = Base64Decode(DR);
            DR = RC4.Decrypt(pwd, sQS, false);
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
            response.UserId = response.MerchantRefNo;
            db.Responses.Add(response);
            db.SaveChanges();

            if (response.ResponseCode != null || response.ResponseCode != string.Empty)
            {
                if (response.ResponseCode == "0")
                {
                    //successful transaction
                    if (response.IsFlagged.ToLower().Equals("no"))
                    {
                        //successful transaction
                        if (response.MerchantRefNo == userProfileId && response.Amount == amount)
                        {
                            entity.Status1 = (int)OrderStatus.PaymentSuceess;
                            entity.CommentLog = response.ResponseMessage;
                        }
                        else
                        {
                            entity.Status1 = (int)OrderStatus.Pending;
                            entity.CommentLog = response.ResponseMessage;
                            //// response with wrong userid or amount
                        }
                    }
                    else
                    {
                        ////pending trasaction
                        entity.Status1 = (int)OrderStatus.Pending;
                        entity.CommentLog = response.ResponseMessage;
                    }
                }
                else
                {
                    ////declined trasaction or erroroccured
                    entity.Status1 = (int)OrderStatus.PaymentFailed;
                    entity.CommentLog = response.ResponseMessage;
                }
            }
            else
            {
                ////error occured in transaction
                entity.Status1 = (int)OrderStatus.PaymentFailed;
                entity.CommentLog = response.ResponseMessage;
            }
        }

        private string Base64Decode(string sBase64String)
        {
            byte[] sBase64String_bytes =
            Convert.FromBase64String(sBase64String);
            return UnicodeEncoding.Default.GetString(sBase64String_bytes);
        }
    }
}