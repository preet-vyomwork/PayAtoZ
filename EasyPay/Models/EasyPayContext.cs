using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Diagnostics;
using WebMatrix.WebData;

namespace EasyPay.Models
{
    //public sealed class MyModelEasyPayContextSingleton
    //{
    //    private static readonly EasyPayContext instance = new EasyPayContext();

    //    static MyModelEasyPayContextSingleton() { }

    //    private MyModelEasyPayContextSingleton() { }

    //    public static EasyPayContext Instance
    //    {
    //        get
    //        {
    //            return instance;
    //        }
    //    }
    //}  

    public class EasyPayContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<EasyPay.Models.EasyPayContext>());

        public EasyPayContext()
            : base("name=EasyPayContext")
        {
        }

        public DbSet<ProviderService> ProviderServices { get; set; }

        public DbSet<ServiceType> ServiceTypes { get; set; }

        public DbSet<ServiceOperator> ServiceOperators { get; set; }

        public DbSet<Provider> Providers { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserWallet> UserWallets { get; set; }
        public DbSet<UserWalletLog> UserWalletLog { get; set; }

        public DbSet<State> States { get; set; }

        public DbSet<TransactionResponse> Responses { get; set; }

        public DbSet<ResponseText> ResponseText { get; set; }

        public DbSet<Merchant> Merchants { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<ServiceOperatorPlan> ServiceOperatorPlans { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UsersInRole> UsersInRoles { get; set; }
        public DbSet<OrderCoupon> OrderCoupons { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<CouponHistory> CouponHistories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public override int SaveChanges()
        {

            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            return 0;

        }

    }


}
