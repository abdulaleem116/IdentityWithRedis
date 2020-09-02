using IdentityWithRedis.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentityWithRedis.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;
        public HomeController()
        {

        }
        public HomeController (ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var users = await UserManager.Users.ToListAsync();
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();


            string userId = User.Identity.GetUserId();
            if (await UserManager.IsInRoleAsync(userId, ConstantData.UserRole))
            {
                foreach (var item in users)
                {
                    if (await UserManager.IsInRoleAsync(item.Id, ConstantData.AdminRole))
                    {
                        applicationUsers.Add(item);
                    }
                }
            }
            else
            {
                foreach (var item in users)
                {
                    if (await UserManager.IsInRoleAsync(item.Id, ConstantData.UserRole))
                    {
                        applicationUsers.Add(item);
                    }
                }
            }
            //foreach(var item in users)
            //{
            //    if(await UserManager.IsInRoleAsync(item.Id,ConstantData.UserRole))
            //    {
            //        applicationUsers.Add(item);
            //    }
            //}
            return View(applicationUsers);
        }
        [Authorize]
        public async Task<ActionResult> Chat(string id)
        {
            var receiverObject = await UserManager.Users.SingleOrDefaultAsync(a => a.Id == id);
            ViewBag.receiverName = receiverObject.UserName;
            var currentLoggedUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            ViewBag.SenderName = currentLoggedUser.UserName;
            ViewBag.currentLoggedUserName = currentLoggedUser.UserName;
           return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult RegisterUser()
        {
            return View();
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        /*Redis Testing */
        public ActionResult RedisTest()
        {
            SampleObject sampleObject = new SampleObject
            {
                Country = "Brazil",
                Id = 7,
                Name = "Mané"
            };
            RedisCacheHelper.Set("test1", sampleObject);
            return View();
        }
        [HttpPost]
        public ActionResult saveDataInRedis()
        {
            var lstSampleObject = RedisCacheHelper.Get<List<SampleObject>>("test2");
            if (lstSampleObject == null)
            {
                lstSampleObject = new List<SampleObject>{
                     new SampleObject
                                     {
                                         Country = "Brazil",
                                         Id = 7,
                                         Name = "Mané"
                                     }
                };
            }
            else
            {
                lstSampleObject.Add(
                new SampleObject
                {
                    Country = "Brazil",
                    Id = 7,
                    Name = "Mané"
                });
            }
            RedisCacheHelper.Set("test2", lstSampleObject);
            return Json(new { status = true });
        }
    }
}