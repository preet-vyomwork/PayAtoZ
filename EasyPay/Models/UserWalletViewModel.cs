using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyPay.Models
{
    public class UserWalletViewModel
    {
        public int UserProfileId { get; set; }
        public UserWalletLog UserWalletLog { get; set; }
        public UserWallet UserWallet { get; set; }
    }
    public class UserPreferenceViewModel
    {
        public string RechargeNumber { get; set; }
        public string Amount { get; set; }
        public string ServiceTypeName { get; set; }
        public int ServiceTypeId { get; set; }
        public string OperatorName { get; set; }
    }
}