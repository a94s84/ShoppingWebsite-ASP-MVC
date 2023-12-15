using Newtonsoft.Json;
using shoppingWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace shoppingWebsite.Controllers
{
    public class HomeController : Controller
    {
        private dbShoppingCarEntities1 db = new dbShoppingCarEntities1();
        public ActionResult Index()
        {
            var products = db.table_Product.OrderByDescending(m => m.Id).ToList();
            return View(products);
        }

        public ActionResult Login()
        {
            return View(new table_Member());
        }

        [HttpPost]
        public ActionResult Login(string LoginUserId, string LoginPassword)
        {
            if (LoginUserId == null || LoginPassword == null) return View(new table_Member());

            var member = db.table_Member.Where(m => m.UserId == LoginUserId && m.Password == LoginPassword).FirstOrDefault();
            if (member == null)
            {
                ViewBag.ErrorMessage = "帳號or密碼錯誤，請重新確認登入";
                return View(new table_Member());
            }

            Session["Welcome"] = $"{member.Name} 您好";
            FormsAuthentication.RedirectFromLoginPage(LoginUserId, true);
            return RedirectToAction("Index", "Member");
        }

        public ActionResult Register()
        {
            return View("Login", new table_Member());
        }

        [HttpPost]
        public ActionResult Register(table_Member RegisterMember)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DataScript = "<script> $(function () { $(\".js-tabNav p\").eq(1).click(); }) </script>";
                return View("Login", new table_Member());
            }

            var member = db.table_Member.Where(m => m.UserId == RegisterMember.UserId).FirstOrDefault();
            if (member == null)
            {
                db.table_Member.Add(RegisterMember);
                db.SaveChanges();
                Session["Welcome"] = $"{RegisterMember.Name} 您好";
                FormsAuthentication.RedirectFromLoginPage(RegisterMember.UserId, true);
                return RedirectToAction("Index", "Member");
            };

            ViewBag.Message = "帳號已被使用，請重新註冊";
            ViewBag.DataScript = "<script> $(function () { $(\".js-tabNav p\").eq(1).click(); }) </script>";
            return View("Login", new table_Member());
        }
    }
}