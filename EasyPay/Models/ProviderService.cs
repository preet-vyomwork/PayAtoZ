using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace EasyPay.Models
{
    public class ProviderService
    {
        public virtual int ProviderServiceId { get; set; }
        public virtual int ProviderId { get; set; }
        public virtual int ServiceTypeId { get; set; }
        public virtual int ServiceOperatorId { get; set; }

        public decimal Comission { get; set; }
        public string Format { get; set; }


        public virtual ServiceType ServiceType { get; set; }
        public virtual ServiceOperator ServiceOperator { get; set; }
        public virtual Provider Provider { get; set; }

    }
    public class OrderHistory
    {
        [Required]
        public virtual int OrderHistoryId { get; set; }
        public virtual int OrderId { get; set; }
        public virtual int UserProfileId { get; set; }
        public virtual int ProviderServiceId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ItemCode { get; set; }
        public string Remarks { get; set; }
        public decimal Amount { get; set; }
        public decimal Commission { get; set; }
        public string Comments { get; set; }
        public int Status { get; set; }
        public decimal CouponAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Note { get; set; }
        public virtual Order Order { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ProviderService ProviderService { get; set; }
    //    [OrderHistoryId] [int] IDENTITY(1,1) NOT NULL,
    //[OrderId] [int] NOT NULL,
    //[UserProfileId] [int] NOT NULL,
    //[ProviderServiceId] [int] NOT NULL,
    //[OrderDate] [datetime] NOT NULL,
    //[ItemCode] [nvarchar](max) NULL,
    //[Remarks] [nvarchar](max) NULL,
    //[Amount] [decimal](18, 2) NOT NULL,
    //[commission] [decimal](18, 2) NOT NULL,
    //[comments] [nvarchar](max) NULL,
    //[Status] [int] NOT NULL,
    //[CouponAmount] [decimal](18, 2) NULL,
    //[TotalAmount] [decimal](18, 2) NULL,
    //[Comment] [nvarchar](max) NULL,
    }

    public class Order
    {
        public virtual int OrderId { get; set; }
        public virtual int UserProfileId { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual int ProviderServiceId { get; set; }
        public virtual ProviderService ProviderService { get; set; }
        public virtual DateTime OrderDate { get; set; }
        [Required]
        [Display(Name = "Number")]
        //[StringLength(12, ErrorMessage = "The {0} must be 10 digit.", MinimumLength = 8)]
        public virtual string ItemCode { get; set; }
        public virtual string Remarks { get; set; }
        [Required]
        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        public virtual decimal Amount { get; set; }


        [Display(Name = "TotalAmount")]
        [DataType(DataType.Currency)]
        public virtual Nullable<decimal> TotalAmount { get; set; }

        [Display(Name = "CouponAmount")]
        [DataType(DataType.Currency)]
        public virtual Nullable<decimal> CouponAmount { get; set; }
        public virtual decimal commission { get; set; }
        public virtual int Status { get; set; }

        public virtual string AmountString
        {
            get
            {
                return Amount.ToString();
            }
        }
        public string OrderStatus
        {
            get
            {
                switch (Status)
                {
                    case 0: return "pending";
                    case 1: return "pending";
                    case 3: return "pending";
                    case 4: return "Successful";
                    case 5: return "Failed";
                    case 6: return "Refunded";
                    default: return "";

                }
            }
        }

        public string OrderStatusImage
        {
            get
            {
                switch (Status)
                {
                    case 0: return "images/icon.ico";
                    case 1: return "images/icon.ico";
                    case 3: return "images/icon.ico";
                    case 4: return "images/suceess.gif";
                    case 5: return "images/fail.gif";
                    default: return "";

                }
            }
        }

        public virtual string comments { get; set; }
    }

    public enum OrderStatus
    {
        Pending,//0
        PaymentInProgress,//1
        PaymentFailed,//2
        RegargeInProgress,//3
        Success,//4
        Failed,//5
        Refunded,//6,
        PaymentSuceess,//7
        InSufficientBalance,//8
        IsFlaggedPayment,//9
        WrongOrderId//10

    }


    public class ServiceOperator
    {
        public virtual int ServiceOperatorId { get; set; }

        public virtual string OperatorName { get; set; }

        public virtual string OperatorCode { get; set; }

        public virtual int? ServiceTypeId { get; set; }

        public virtual ServiceType ServiceType { get; set; }

        public virtual string ImageUrl { get; set; }
    }

    public class Provider
    {
        public virtual int ProviderId { get; set; }
        public virtual string ProviderName { get; set; }
    }

    public class ServiceType
    {
        public virtual int ServiceTypeId { get; set; }

        public virtual string ServiceTypeName { get; set; }

        public virtual string ImageUrl { get; set; }
    }

    public class UserWallet
    {
        public virtual int UserWalletId { get; set; }
        public virtual int UserProfileId { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual decimal Balance { get; set; }
    }

    public class UserWalletLog
    {
        public virtual int UserWalletLogId { get; set; }
        public virtual int UserProfileId { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual bool IsRefund { get; set; }
        public virtual DateTime TransactionDate { get; set; }
        public virtual string Remarks { get; set; }
        public virtual int Status1 { get; set; }
        public virtual string CommentLog { get; set; }
    }



    public class State
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
    }

    public class TransactionResponse
    {
        public int TransactionResponseID { get; set; }
        public int OrderId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public DateTime DateCreated { get; set; }
        public int PaymentID { get; set; }
        public int MerchantRefNo { get; set; }
        public decimal Amount { get; set; }
        public string Mode { get; set; }
        public string BillingName { get; set; }
        public int TransactionID { get; set; }
        public string IsFlagged { get; set; }
        public int UserId { get; set; }
    }

    public class ResponseText
    {
        public int ResponseTextID { get; set; }
        public int OrderId { get; set; }
        public string ResponseString { get; set; }
        public int UserIdt { get; set; }
    }

    public class ServiceOperatorPlan
    {
        public int ServiceOperatorPlanId { get; set; }
        public int ServiceOperatorId { get; set; }
        public virtual ServiceOperator ServiceOperator { get; set; }
        public string PlanName { get; set; }
        public string Validity { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public int StateId { get; set; }
        public virtual State State { get; set; }
    }

    public class Preference
    {
        public int PreferenceId { get; set; }
        public virtual int UserProfileId { get; set; }
        public virtual int ServiceTypeId { get; set; }
        public virtual int ServiceOperatorId { get; set; }
        public string RechargeNumber { get; set; }
        public string Amount { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        public virtual ServiceOperator ServiceOperator { get; set; }
    }
}