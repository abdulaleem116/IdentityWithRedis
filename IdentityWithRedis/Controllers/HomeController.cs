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
            var currentLoggedUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            Session["CurrentLoggedInUserName"] = currentLoggedUser.UserName;
            ViewBag.loggedInUserName = currentLoggedUser.UserName;
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

            //Retrieve messages of loggedIn/Current User and receiver messages
            var lstSampleObject = RedisCacheHelper.Get<List<SampleObject>>("redisChat");
            List<SampleObject> chatList = new List<SampleObject>();
            if(lstSampleObject != null)
            {
                foreach(var items in lstSampleObject)
                {
                  if((items.senderUName == currentLoggedUser.UserName && items.receiverUName == receiverObject.UserName) ||
                      (items.senderUName == receiverObject.UserName && items.receiverUName == currentLoggedUser.UserName)
                        )
                    {
                        chatList.Add(items);
                    }
                }
            }
            return View(chatList);
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
            //SampleObject sampleObject = new SampleObject
            //{
            //    senderUName = senderName,
            //    receiverUName = receiverName,
            //    textMsg = msginput
            //};
            //RedisCacheHelper.Set("test1", sampleObject);
            return View();
        }
        [HttpPost]
        public ActionResult saveDataInRedis(string senderName,string receiverName, string msginput)
        {
            var lstSampleObject = RedisCacheHelper.Get<List<SampleObject>>("redisChat");
            if (lstSampleObject == null)
            {
                lstSampleObject = new List<SampleObject>{
                     new SampleObject
                                     {
                                         senderUName = senderName,
                                         receiverUName = receiverName,
                                         textMsg = msginput
                                     }
                };
            }
            else
            {
                lstSampleObject.Add(
                new SampleObject
                {
                    senderUName = senderName,
                    receiverUName = receiverName,
                    textMsg = msginput
                });
            }
            RedisCacheHelper.Set("redisChat", lstSampleObject);
            return Json(new { status = true });
        }
        public ActionResult LiveConnectionTest()
        {
         return View();
        }
        public ActionResult LiveConnectionTest1()
        {
            return View();
        }
    }
   
}