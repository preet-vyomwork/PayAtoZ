using EasyPay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyPay.Controllers
{
    public class RolesController : Controller
    {
        EasyPayContext _dbContext = new EasyPayContext();
        //
        // GET: /Roles/

        public ActionResult Index()
        {
           
            var query = new List<UserRoleViewModel>();
            UserRoleViewModel userRole = new UserRoleViewModel();
            List<UserRoleViewModel> userRoleList = new List<UserRoleViewModel>();
            
            try
            {
                
                query = (from user in _dbContext.Memberships
                         join userRoles in _dbContext.UsersInRoles on user.UserId equals userRoles.UserId into rolesOfUser

                         from rou in rolesOfUser
                         join roles in _dbContext.Roles on rou.RoleId equals roles.RoleId into getRoles

                         from gr in getRoles
                         join userInfo in _dbContext.UserProfiles on user.UserId equals userInfo.UserProfileId into users
                         from u in users.DefaultIfEmpty()

                         select new UserRoleViewModel
                         {
                             RoleName = (gr.RoleName == null) ? "User" : gr.RoleName,
                             UserName = (u.UserName == null) ? "None" : u.UserName,
                             Email = (u.Email == null) ? "None" : u.Email,
                             //RegistrationDate = (u.RegistrationDate == null) ? DateTime.MinValue : (DateTime)DbFunctions.TruncateTime(u.RegistrationDate),
                             //RegistrationType = (u.RegistrationType == null) ? "None" : u.RegistrationType,
                             //IsActive = (bool)u.IsActive,
                            UserProfileId = user.UserId,
                             RoleId = gr.RoleId
                         }).OrderBy(o => o.UserName).ToList();


                query.ForEach(q =>
                        {
                            userRole.UserProfileId = q.UserProfileId;
                            userRole.Email = q.Email;

                            userRole.RoleName = q.RoleName;
                            userRole.UserName = q.UserName;

                            userRoleList.Add(userRole);
                        });
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
            return View(query);
        }
        #region UpdateUserDetails
        //GET:

        // This is a get method which is used to edit all the details of a user
        public ActionResult EditUserRole(int userId=0)
        {
            //logger.Info("In RolesController EditUserRole (Get)method userId:" + userId + " at " + DateTime.UtcNow);

            UserRoleViewModel userRoleViewModel = new UserRoleViewModel();
            var account = new AccountController();
           
            try
            {
                
                //get details of a user based on userId from UserInfoes table
                //var thisUser = entities.UserInfoes.Where(x => x.Id == userId).FirstOrDefault();
                var thisUser = _dbContext.UserProfiles.Where(u => u.UserProfileId == userId).FirstOrDefault();

               
                //get AspNetUser Id from UserInfo-Id  
                //var aspNetUser = entities.AspNetUsers.Where(u => u.UserInfo_Id == userId).FirstOrDefault();
                var aspNetUser = _dbContext.Memberships.Where(u => u.UserId == userId).FirstOrDefault();
                
                //get RoleId from AspNetUser Id
                //var userRole = entities.AspNetUserRoles.Where(ur => ur.UserId == aspNetUser.Id).FirstOrDefault();
                var userRole = _dbContext.UsersInRoles.Where(ur => ur.UserId == aspNetUser.UserId).FirstOrDefault();

                var role = _dbContext.Roles.Where(r => r.RoleId == userRole.RoleId).FirstOrDefault();

                //logger.Info("In RolesController EditUserRole (Get)method UserName:" + thisUser.UserName + " at " + DateTime.UtcNow);
                if (thisUser != null)
                {
                    userRoleViewModel.UserProfileId = thisUser.UserProfileId;
                    userRoleViewModel.Email = thisUser.Email;
                    userRoleViewModel.Address = thisUser.Address;
                    userRoleViewModel.Age = Convert.ToInt32(thisUser.Age);
                    userRoleViewModel.RoleName = role.RoleName;
                    userRoleViewModel.UserName = thisUser.UserName;
                }
                ViewBag.Roles = _dbContext.Roles.Select(r => r.RoleName).ToList();
                //logger.Info("In RolesController EditUserRole (Get)method ends at " + DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                //logger.Error("In RolesController EditUserRole (Get)method Error: " + ex.Message + " " + ex.InnerException + ex.StackTrace + " at " + DateTime.UtcNow);
                throw ex;
            }

            var list = _dbContext.Roles.OrderBy(r => r.RoleName).ToList().Select(rr => new SelectListItem { Value = rr.RoleName.ToString(), Text = rr.RoleName }).ToList();
            ViewBag.Roles = list;

            return View(userRoleViewModel);
        }

        // This is a post method which will update record of a user in database

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserRole(UserRoleViewModel userRoleViewModel)
        {
            //logger.Info("In RolesController EditUserRole (Post) method userId:" + userRoleViewModel.UserId + " at " + DateTime.UtcNow);
            //VenturaHDBEntities entities = new VenturaHDBEntities();
            EasyPayContext context = new EasyPayContext();
            //
            var userInfo = context.UserProfiles.Where(u => u.UserProfileId == userRoleViewModel.UserProfileId).FirstOrDefault();
            // AspNetRole aspNetRole = new AspNetRole();

            var aspNetUser = context.Memberships.Where(u => u.UserId == userInfo.UserProfileId).FirstOrDefault();

            //get RoleId from AspNetUser Id


            var aspNetRole = context.Roles.Where(r => r.RoleName == userRoleViewModel.RoleName).FirstOrDefault();
            var userRole = context.UsersInRoles.Where(ur => ur.UserId == aspNetUser.UserId).FirstOrDefault();
            try
            {
                if (userRoleViewModel != null)
                {
                    // _dbContext.Entry(userRoleViewModel).State = EntityState.Modified;
                    //Update user info in database
                    userInfo.Address = (userRoleViewModel.Address != null) ? userRoleViewModel.Address : userInfo.Address;
                    userInfo.Age = (userRoleViewModel.Age != 0) ? userRoleViewModel.Age : userInfo.Age;
                    userInfo.Email = (userRoleViewModel.Email != null) ? userRoleViewModel.Email : userInfo.Email;
                    //userInfo.City = userRoleViewModel.RoleId;
                    //userRole.Roles.RoleName = userRoleViewModel.RoleName;
                    userInfo.UserName = userRoleViewModel.UserName;
                    //userInfo.UserProfileId = userRoleViewModel.UserProfileId;
                    
                    if (aspNetRole.RoleId != 0)
                    {
                        userRole.RoleId = aspNetRole.RoleId;
                        userRole.UserId = userRoleViewModel.UserProfileId;
                    }
                    context.SaveChanges();
                    //logger.Info("In RolesController EditUserRole (Post) method Role:" + userRoleViewModel.Role + " at " + DateTime.UtcNow);
                }
                //logger.Info("In RolesController EditUserRole (Post) method ends at " + DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                //logger.Error("In RolesController EditUserRole (Post) method Error: " + ex.Message + " " + ex.InnerException + ex.StackTrace + " at " + DateTime.UtcNow);
                //return View();
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region CreateRoles
        // GET: /Roles/Create

        // Implementing methods for Creating Roles for user

        public ActionResult CreateRole()
        {
            //logger.Info("In RolesController CreateRole (Get)method at " + DateTime.UtcNow);
            return View();
        }

        // POST: /Roles/Create
        [HttpPost]
        public ActionResult CreateRole(FormCollection collection)
        {
            //logger.Info("In RolesController CreateRole (Post)method at " + DateTime.UtcNow);
            try
            {
                _dbContext.Roles.Add(new Role()
                {
                    RoleName = collection["RoleName"]
                });
                _dbContext.SaveChanges();
                ViewBag.ResultMessage = "Role created successfully !";
                //logger.Info("In RolesController CreateRole (Post)method ends at " + DateTime.UtcNow);

                return RedirectToAction("RolesListing");
            }
            catch (Exception ex)
            {
                //logger.Error("In RolesController CreateRole (Post)method Error: " + ex.Message + " " + ex.InnerException + ex.StackTrace + " at " + DateTime.UtcNow);
                return View();
            }
        }
        #endregion

        #region RoleList

        // Displays list of roles from AspNetRoles table
        public ActionResult RolesListing()
        {
            //logger.Info("In RolesController RolesListing method at " + DateTime.UtcNow);
            var roles = _dbContext.Roles.ToList();
            //logger.Info("In RolesController RolesListing method roles " + roles.Count() + " at " + DateTime.UtcNow);
            //logger.Info("In RolesController RolesListing method ends at " + DateTime.UtcNow);
            return View(roles);
        }
        #endregion

        #region DeleteRole
        public ActionResult Delete(int id)
        {
            //logger.Info("In RolesController Delete method at " + DateTime.UtcNow);
            try
            {
                var thisRole = _dbContext.Roles.Where(r => r.RoleId.Equals(id)).FirstOrDefault();
                //logger.Info("In RolesController Delete method - Remove role: " + thisRole.Name + " at " + DateTime.UtcNow);
                _dbContext.Roles.Remove(thisRole);
                _dbContext.SaveChanges();
                //logger.Info("In RolesController Delete method ends at " + DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                //logger.Error("In RolesController Delete method Error: " + ex.Message + " " + ex.InnerException + ex.StackTrace + " at " + DateTime.UtcNow);
                throw ex;
            }
            return RedirectToAction("RolesListing");
        }
        #endregion

        #region UpdateRoleDetails
        // GET: /Roles/Edit/5
        public ActionResult EditRole(int id)
        {
            //logger.Info("In RolesController EditRole (Get)method Role " + roleName + " at " + DateTime.UtcNow);
            var thisRole = new Role();
            try
            {
                //logger.Info("In RolesController EditRole (Get)method Role " + roleName + " at " + DateTime.UtcNow);
                thisRole = _dbContext.Roles.Where(r => r.RoleId.Equals(id)).FirstOrDefault();
                //logger.Info("In RolesController EditRole (Get)method ends at " + DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                //logger.Error("In RolesController EditRole (Get)method Error: " + ex.Message + " " + ex.InnerException + ex.StackTrace + " at " + DateTime.UtcNow);
                throw ex;
            }
            return View(thisRole);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(Role role)
        {
            //logger.Info("In RolesController EditRole (Post)method Role " + role.Id + " at " + DateTime.UtcNow);
            try
            {
                _dbContext.Entry(role).State = EntityState.Modified;
                _dbContext.SaveChanges();
                //logger.Info("In RolesController EditRole (Post)method ends at " + DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                //logger.Error("In RolesController EditRole (Post)method Error: " + ex.Message + " " + ex.InnerException + ex.StackTrace + " at " + DateTime.UtcNow);
                return View();
            }
            return RedirectToAction("RolesListing");
        }
        #endregion

    }
}
