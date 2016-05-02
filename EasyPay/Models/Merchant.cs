using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EasyPay.Models
{
	public class Category
	{
		public virtual int CategoryId { get; set; }
		public virtual string CategoryName { get; set; }
		public virtual string  CategoryClassName { get; set; }
	}
    public class Merchant
    {
        [Key]
        public virtual int MerchantId { get; set; }
        public virtual string MerchantName { get; set; }
        public virtual string LogoImagePath { get; set; }
        public virtual string URL { get; set; }
        public virtual string Address { get; set; }
        public virtual string City { get; set; }
        public virtual string PostalCode { get; set; }

        public string EmailId { get; set; }
        public string ContactNo { get; set; }
        public virtual string Terms { get; set; }
        [Required]
        public virtual int CategoryId { get; set; }
        [Required]
        public virtual int UserProfileId { get; set; }
        public virtual Category Category { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        [Required]
        public int State_StateID { get; set; }
        [ForeignKey("State_StateID")]
        public virtual State State { get; set; }

    }
    public class Coupon
    {
        [Key]
        public virtual int CouponId { get; set; }
        public virtual int MerchantId { get; set; }

        public string CouponTitle { get; set; }
        public string Store { get; set; }
        public string Type { get; set; }
        public string CouponCode { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public bool IsValid { get; set; }
        public bool Published { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public virtual DateTime ValidityStart { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public virtual DateTime ValidityEnd { get; set; }
        public virtual string CouponImagepath { get; set; }
        [Required]
        public virtual int CouponValue { get; set; }
        public int TotalCoupons { get; set; }
        public int CouponsUsed { get; set; }
        public int AvailableCoupons { get; set; }
        public virtual string Terms { get; set; }
        public virtual int AdditionalFees { get; set; }
        public string ValidInCities { get; set; }

        public virtual Merchant Merchant { get; set; }
        [Required]
        public int Category_CategoryId { get; set; }
        [ForeignKey("Category_CategoryId")]
        public virtual Category Category { get; set; }

    }
    public class CouponHistory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CouponId { get; set; }
        public virtual int MerchantId { get; set; }
        public string CouponTitle { get; set; }
        public string Store { get; set; }
        public string Type { get; set; }
        public string CouponCode { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public bool IsValid { get; set; }
        public bool Published { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public virtual DateTime ValidityStart { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public virtual DateTime ValidityEnd { get; set; }
        public virtual string CouponImagepath { get; set; }
        [Required]
        public virtual int CouponValue { get; set; }
        public int TotalCouponsAdded { get; set; }
       
        public virtual string Terms { get; set; }
        public virtual int AdditionalFees { get; set; }
        public string ValidInCities { get; set; }
        public DateTime CouponsAddedDate { get; set; }

        public virtual Merchant Merchant { get; set; }
        public virtual Category Category { get; set; }

    }

    public class OrderCoupon
    {
        public virtual int OrderCouponId { get; set; }
        public virtual int OrderId { get; set; }
        public virtual int CouponId { get; set; }
        [Required]
        [Display(Name = "CouuponAmount")]
        [DataType(DataType.Currency)]
        public virtual decimal CouponAmount { get; set; }
    }
    public class UserMerchantViewModel
    {
        public RegisterModel RegisterModel { get; set; }
        public  Merchant Merchant { get; set; }
    }

}
