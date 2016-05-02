using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using EasyPay.Models;
using System.Web.Security;
using System.Linq;
namespace EasyPay.Filters
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
	{
		private static SimpleMembershipInitializer _initializer;
		private static object _initializerLock = new object();
		private static bool _isInitialized;

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			// Ensure ASP.NET Simple Membership is initialized only once per app start
			LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
		}

		private class SimpleMembershipInitializer
		{
			public SimpleMembershipInitializer()
			{
				Database.SetInitializer<EasyPayContext>(null);

				try
				{
					using (var context = new EasyPayContext())
					{
						if (!context.Database.Exists())
						{
							// Create the SimpleMembership database without Entity Framework migration schema
							//((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
						}
					}

					WebSecurity.InitializeDatabaseConnection("EasyPayContext", "UserProfile", "UserProfileId", "UserName", autoCreateTables: false );

					var roles = (SimpleRoleProvider)Roles.Provider;
					var membership = (SimpleMembershipProvider)System.Web.Security.Membership.Provider;

					if (!roles.RoleExists("Admin"))
					{
						roles.CreateRole("Admin");
					}
					if (membership.GetUser("jaydeepadm", false) == null)
					{
						membership.CreateUserAndAccount("jaydeepadm", "19811982");
					}
					if (!roles.GetRolesForUser("jaydeepadm").Contains("Admin"))
					{
						roles.AddUsersToRoles(new[] { "jaydeepadm" }, new[] { "Admin" });
					}
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
				}
			}
		}
	}
}
