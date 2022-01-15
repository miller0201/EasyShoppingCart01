using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EasyShoppingCart01.Models;//表單驗證會使用到

namespace EasyShoppingCart01.Controllers
{
    [Authorize]//指定此控制器皆須通過驗證才可執行
    public class MemberController : Controller
    {
        //建立db資料庫
        EasyShoppingCartEntities1 db = new EasyShoppingCartEntities1();
        // GET: Member/Index
        public ActionResult Index()
        {
            //將取得產品放入products
            var products = db.tProduct.OrderByDescending(m=>m.fPId).ToList();
            //檢視index套用_LayoutMember,使用products模型
            return View("../Home/Index", "_LayoutMember",products);
        }
        //GET:Member/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }
        //GET:Member/ShoppingCar
        public ActionResult ShoppingCar()
        {
            //取得會員帳號給fUserId
            string fUserId = User.Identity.Name;
            //找出購物車內容
            var orderDetails = db.tOrderDetail.Where(m => m.fUserId == fUserId && m.fIsApproved == "否").ToList();
            return View(orderDetails);
        }
        [HttpPost]
        //POST:Member/ShoppingCar
        public ActionResult ShoppingCar(string fReceiver,string fEmail,string fAddress)
        {
            //會員帳號指定給fUserId
            string fUserId = User.Identity.Name;
            
            //建立訂單資料
            tOrder order = new tOrder();
            order.fUserId = fUserId;
            order.fReceiver = fReceiver;
            order.fEmail = fEmail;
            order.fAddress = fAddress;
            order.fDate = DateTime.Now;

            db.tOrder.Add(order);
            //找出購物車狀態產品
            var carList = db.tOrderDetail.Where(m=>m.fIsApproved =="否" && m.fUserId==fUserId).ToList();
            //確認訂購產品
            foreach(var item in carList)
            {
                item.fIsApproved = "是";
            }
            //更新資料庫
            db.SaveChanges();
            return RedirectToAction("OrderList");
        }
        //GET:Member/AddCar
        public ActionResult AddCar(string fPId)
        {
            //建立唯一識別值給guid當作訂單編號
            string guid = Guid.NewGuid().ToString();
            //取得會員帳號指定給fUserId
            string fUserId = User.Identity.Name;
            //找出產品狀態在購物車-fIsApproved == "否"
            var currentCar = db.tOrderDetail.Where(m =>m.fUserId == fUserId &&m.fIsApproved == "否" && m.fPId == fPId).FirstOrDefault();
            //currentCar == null 產品不在購物車狀態
            if(currentCar  == null)
            {
                //將目前購產品指給product
                var product = db.tProduct.Where(m => m.fPId==fPId).FirstOrDefault();
                //將產品放到訂單
                tOrderDetail orderdetail = new tOrderDetail();
                orderdetail.fOrderGuid = guid;
                orderdetail.fUserId = fUserId;
                orderdetail.fPId = fPId;
                orderdetail.fName = product.fName;
                orderdetail.fPrice = product.fPrice;
                orderdetail.fQty = 1;
                orderdetail.fIsApproved = "否";
                db.tOrderDetail.Add(orderdetail);
            }
            else
            {
                //購物車狀態
                currentCar.fQty += 1;
            }
            db.SaveChanges();
            return RedirectToAction("ShoppingCar");
        }
        public ActionResult DeleteCar(int fId)
        {
            //依Id找出在購物車內要刪除的產品
            var orderDetail = db.tOrderDetail.Where(m => m.fId == fId).FirstOrDefault();
            //刪除購物車產品
            db.tOrderDetail.Remove(orderDetail);
            db.SaveChanges();
            return RedirectToAction("ShoppingCar");
        }
        //GET:Member/OderList
        public ActionResult OrderList()
        {
            //找出會員帳號並指定給fUserId
            string fUserId = User.Identity.Name;
            //找出會員訂單並依照時間進行排序
            var orders = db.tOrder.Where(m => m.fUserId == fUserId).OrderByDescending(m => m.fDate).ToList();
            //檢視訂單模型
            return View(orders);
        }
        //GET:Member/Detail
        public ActionResult OrderDetail(string fOrderGuid)
        {
            //確定下訂單的訂單
            var orderDetails = db.tOrderDetail.Where(m => m.fOrderGuid == fOrderGuid).ToList();
            return View(orderDetails);
        }
    }
}