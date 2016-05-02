using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using log4net;

namespace ServiceEngine
{
    public class ServiceProviderFactory
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static iServiceProvider GetServiceProvider(ServiceType serviceType, ProviderType providerType)
        {
            if (providerType == ProviderType.JOLO)
                return new JOLOCServiceProvider();
            else
            {
                switch (serviceType)
                {
                    case ServiceType.Mobile:
                        return new MobileServiceProvider();
                    case ServiceType.DTH:
                        return new DTHServiceProvider();
                    case ServiceType.DataCard:
                        return new DataCardServiceProvider();
                    default:
                        return null;
                }
            }
        }
    }

    public interface iServiceProvider
    {
        string DoRecharge(ServiceOrder order);
        string CheckWallet();
        string GetTransactionStatus(string orderId, int serviceTypeId);
    }
    public abstract class JOLOServiceProvider : iServiceProvider
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public JOLOServiceProvider()
        {
            Credentials credentials;
            Configuration.CredentialDict.TryGetValue(ServiceEngine.ProviderType.JOLO, out credentials);
            //Key = "cinnovative_solutions581";
            //MerchantId = "innovative_solutions";
            Key = credentials.Key;
            MerchantId = credentials.MerchantId;

        }
        public string Key { get; set; }
        public string MerchantId { get; set; }
        public string MerchantPassword { get; set; }

        public string DoRecharge(ServiceOrder order)
        {
            int mode = Configuration._joloAPIMode;

            string result = string.Empty;
            //string parameterString = "http://jolo.in/api/recharge.php?mode=1&key=" + Key + "&operator=" + order.OperatorCode + "&service=" + order.ItemCode + "&amount=" + order.Amount + "&orderid=" + order.OrderNumber;
            string parameterString = "http://joloapi.com/api/recharge2.php?mode=" + mode + "&userid=" + MerchantId.Trim() + "&key=" + Key.Trim() + "&operator=" + order.OperatorCode.Trim() + "&service=" + order.ItemCode.Trim() + "&amount=" + order.Amount.ToString().Trim() + "&orderid=" + order.OrderNumber.Trim();
            result = CallJoloService(parameterString);
            return result;
        }

        public string GetTransactionStatus(string orderId, int serviceTypeId)
        {

            string result = string.Empty;
            // string parameterString = "http://jolo.in/api/rechargestatus.php?userid=" + MerchantId + "&txn=" + orderId;
            string parameterString = "http://joloapi.com/api/rechargestatus_client.php?userid=" + MerchantId + "&key=" + Key + "&servicetype=" + serviceTypeId + "&txn=" + orderId;

            result = CallJoloService(parameterString);
            return result;
            //return TFService.getTransactionStatus(MerchantId, MerchantPassword, orderId);
        }

        public string CheckWallet()
        {
            string result = string.Empty;
            string parameterString = "http://jolo.in/api/rechargebalance.php?key=" + MerchantId;
            result = CallJoloService(parameterString);
            return result;

            //http://jolo.in/api/rechargebalance.php?key=appkey
        }

        private string CallJoloService(string URL)
        {
            logger.Info("Method Start" + " at " + DateTime.UtcNow);
            WebRequest request = WebRequest.Create(URL);
            //WebRequest request = WebRequest.Create("http://jolo.in/api/recharge_advance.php?mode=1&key=cinnovative_solutions581&operator=UN&service=7383048530&amount=50&orderid=a13");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content. 
            string result = reader.ReadToEnd();
            logger.Info("Recharege Status: " + result + " at " + DateTime.UtcNow);
            return result;
        }
    }

    public abstract class TFServiceProvider : iServiceProvider
    {
        public TFServiceProvider()
        {
            TFService = new TFRechargeService.Service();
            MerchantId = "9979891148";
            MerchantPassword = "0900";
        }
        public TFRechargeService.Service TFService { get; set; }
        public string MerchantId { get; set; }
        public string MerchantPassword { get; set; }

        public abstract string DoRecharge(ServiceOrder order);

        public string GetTransactionStatus(string orderId, int serviceTypeId)
        {
            return TFService.getTransactionStatus(MerchantId, MerchantPassword, orderId);
        }

        public string CheckWallet()
        {
            return TFService.checkWallet(MerchantId, MerchantPassword);
        }

    }
    public class JOLOCServiceProvider : JOLOServiceProvider
    {

    }
    public class MobileServiceProvider : TFServiceProvider
    {
        /// <summary>
        /// this function is used  for recharge.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public override string DoRecharge(ServiceOrder order)
        {
            return TFService.Recharge(order.OperatorCode, order.ItemCode.ToString(), order.Amount.ToString(), MerchantId, MerchantPassword, "Mobile", "RCH", order.OrderNumber);
        }
    }

    public class DTHServiceProvider : TFServiceProvider
    {
        public override string DoRecharge(ServiceOrder order)
        {
            return TFService.Recharge(order.OperatorCode, order.ItemCode, order.Amount.ToString(), MerchantId, MerchantPassword, "DTH", "RCH", order.OrderNumber);
        }
    }

    public class DataCardServiceProvider : TFServiceProvider
    {
        public override string DoRecharge(ServiceOrder order)
        {
            TFRechargeService.Service webservice = new TFRechargeService.Service();
            return TFService.Recharge(order.OperatorCode, order.ItemCode, order.Amount.ToString(), MerchantId, MerchantPassword, "Mobile", "RCH", order.OrderNumber);
        }
    }

    public enum ServiceType
    {
        Mobile,
        MobilePostpaid,
        DTH,
        DataCard
    }

    public enum ProviderType
    {
        TechFreedom,
        JOLO
    }
    public class ServiceHelper
    {
        public string DoRecharge(ServiceOrder serviceOrder)
        {
            iServiceProvider iserviceProvider = ServiceProviderFactory.GetServiceProvider(serviceOrder.ServiceType, serviceOrder.ProviderType);
            return iserviceProvider.DoRecharge(serviceOrder);
        }

        public string GetTransactionResult(string orderId, int serviceTypeId)
        {
            iServiceProvider iserviceProvider = ServiceProviderFactory.GetServiceProvider(ServiceType.Mobile, ProviderType.JOLO);
            return iserviceProvider.GetTransactionStatus(orderId, serviceTypeId);
        }

        public string ChecktWallet()
        {
            iServiceProvider iserviceProvider = ServiceProviderFactory.GetServiceProvider(ServiceType.Mobile, ProviderType.JOLO);
            return iserviceProvider.CheckWallet();
        }
    }

    public class ServiceOrder
    {
        //public ServiceOrder()
        //{
        //    UserId = "innovative";
        //}
        public ServiceType ServiceType { get; set; }
        public ProviderType ProviderType { get; set; }
        public decimal Amount { get; set; }
        public string ItemCode { get; set; }
        public string OrderNumber { get; set; }
        public string OperatorCode { get; set; }
        //public string UserId { get; set; }
    }
}
