using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EasyPay.Models;
using WebMatrix.WebData;
using System.Threading;
using System.Globalization;

namespace EasyPay
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_AcquireRequestState(object sender, EventArgs e)
		{
			//Create culture info object   
			//CultureInfo ci = new CultureInfo("en-IN");
			//System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
			//System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
			
		}

		protected void Session_Start() { }

		protected void Application_Start()
		{
            log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

			//CultureInfo ci = new CultureInfo("en-IN");
			//Thread.CurrentThread.CurrentUICulture = ci;
			//Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
			//System.Data.Entity.Database.SetInitializer(new System.Data.Entity.CreateDatabaseIfNotExists<EasyPay.Models.EasyPayContext>());
			System.Data.Entity.Database.SetInitializer<EasyPayContext>(null);
			//WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserProfileId", "UserName", autoCreateTables: false);
			AreaRegistration.RegisterAllAreas();
			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();
		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			
		}
	}
}