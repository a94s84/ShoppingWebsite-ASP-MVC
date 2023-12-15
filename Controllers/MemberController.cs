using shoppingWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace shoppingWebsite.Controllers
{
    public class MemberController : Controller
    {

        dbShoppingCarEntities1 db = new dbShoppingCarEntities1();
        
        [Authorize]
        public ActionResult Index()
        {
            var products = db.table_Product.OrderByDescending(m => m.Id).ToList();
            return View("~/Views/Home/Index.cshtml", "_LayoutMember", products);
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult ShoppingCart()
        {
            string UserId = User.Identity.Name;
            var orderDetails = db.table_OrderDetail.Where(m => m.UserId == UserId && m.IsApproved == "N").ToList();
            return View(orderDetails);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ShoppingCar(string Receiver, string Email, string Address)
        {
            string userId = User.Identity.Name;
            string OrderGuid = OrderHelper.GenerateNo().ToString();

            var order = new table_Order();
            order.OrderGuid = OrderGuid;
            order.UserId = userId;
            order.Receiver = Receiver;
            order.Email = Email;
            order.Address = Address;
            order.Date = DateTime.Now;
            db.table_Order.Add(order);

            var carList = db.table_OrderDetail.Where(m => m.IsApproved == "N" && m.UserId == userId).ToList();
            foreach (var item in carList)
            {
                item.OrderGuid = OrderGuid;
                item.IsApproved = "Y";
            }
            db.SaveChanges();
            return RedirectToAction("OrderList");
        }

        [HttpPost]
        public ActionResult AddCar(string ProductId, int Num)
        {
            string userId = User.Identity.Name;
            var currentCar = db.table_OrderDetail.Where(m => m.ProductId == ProductId &&  m.IsApproved=="N" && m.UserId == userId).FirstOrDefault();
            
            if(currentCar == null)
            {
                var product = db.table_Product.Where(m => m.ProductId ==ProductId).FirstOrDefault();
                var orderDetail = new table_OrderDetail();
                orderDetail.UserId = userId;
                orderDetail.ProductId = product.ProductId;
                orderDetail.Name = product.Name;
                orderDetail.Image = product.Image;
                orderDetail.Price = product.Price;
                orderDetail.Quantity = Num;
                orderDetail.IsApproved = "N";
                db.table_OrderDetail.Add(orderDetail);
            }
            else
            {
                currentCar.Quantity+= Num;
            }
            db.SaveChanges();
            return Json(new { msg = "加入購物車成功!" });
        }

        [Authorize]
        public ActionResult DeleteCar(int Id)
        {
            var orderDetails = db.table_OrderDetail.Where(m => m.Id == Id).FirstOrDefault();
            db.table_OrderDetail.Remove(orderDetails);
            db.SaveChanges();
            return RedirectToAction("ShoppingCart");
        }

        [Authorize]
        public ActionResult OrderList()
        {
           string userId = User.Identity.Name;
            var orders = db.table_Order.Where(m => m.UserId == userId).OrderByDescending(m => m.Date).ToList();
            return View(orders);
        }

        [Authorize]
        public ActionResult OrderDetail(string OrderGuid)
        {
            var orderDetails = db.table_OrderDetail.Where(m => m.OrderGuid == OrderGuid).ToList();
            var orderInfo = db.table_Order.Where(t => t.OrderGuid == OrderGuid).FirstOrDefault();

            var showOrderDetail = new ShowOrderDetail
            {
                OrderDetail = orderDetails,
                Order = orderInfo
            };

            return View(showOrderDetail);
        }

        public class OrderHelper
        {
            private static readonly object Locker = new object();
            private static int _sn = 0;
            public static string GenerateNo()
            {
                lock (Locker)
                {
                    if (_sn == 1000)
                    {
                        _sn = 0;
                    }
                    else
                    {
                        _sn++;
                    }
                    Thread.Sleep(100);
                    return "A00B" + DateTime.Now.ToString("yyyyMMddHHmmss") + _sn.ToString().PadLeft(3, '0');
                }
            }
        }
    }
}
