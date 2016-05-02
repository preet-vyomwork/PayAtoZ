using System.Web;
using System.Web.Mvc;
using EasyPay.Filters;

namespace EasyPay
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
			//filters.Add(new AuthorizeAttribute()); 
            //filters.Add(new InitializeSimpleMembershipAttribute());
		}
	}
}