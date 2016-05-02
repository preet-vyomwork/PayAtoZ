using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyPay.Models
{
    public class UserRoleViewModel
    {
        public int UserProfileId { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string RoleName  { get; set; }
        public int RoleId  { get; set; }
    }
    public class UserPreference
    {
        public string Amount { get; set; }
        public string RechargeNumber { get; set; }
        public string OperatorName { get; set; }
        public string ServiceTypeName { get; set; }
        public int ServiceTypeId { get; set; }

    }
}