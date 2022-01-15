using EasyShoppingCart01.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EasyShoppingCart01.Controllers
{
    public class HomeController : Controller
    {
        //建立可存取EasyShoppingEntities資料庫的類別物件
        EasyShoppingCartEntities1 db = new EasyShoppingCartEntities1();
        // GET: Home
        public ActionResult Index()
        {
            //將取出物件放入products的list
            var products = db.tProduct.OrderByDescending(m=>m.fId).ToList();
            return View(products);
        }
        // GET: Home/Login
        public ActionResult Login()
        {
            return View();
        }
        // POST: Home/Login
        [HttpPost]
        public ActionResult Login(string fUserId, string fPwd)
        {
            //依帳密取得會員
            var member = db.tMember.Where(m => m.fUserId == fUserId && m.fPwd == fPwd).FirstOrDefault();
            //未註冊
            if (member == null)
            {
                ViewBag.Message = "帳密錯誤，登入失敗";
                return View();
            }
            //用session變數紀錄歡迎詞
            Session["Welcome"] = member.fName + "歡迎光臨";
            //使用帳號密碼通過驗證
            FormsAuthentication.RedirectFromLoginPage(fUserId, true);
            return RedirectToAction("Index", "member");
        }

        // GET: Home Register
        public ActionResult Register()
        {
            return View();
        }
        // POST: Home Register
        [HttpPost]
        public ActionResult Register(tMember pMember)
        {
            //模型未通過驗證
            if(ModelState.IsValid == false)
            {
                return View();
            }
            //依帳號取得會員指定給member
            var member = db.tMember.Where(m => m.fUserId == pMember.fUserId).FirstOrDefault();
            //會員未註冊，所以能執行註冊動作
            if( member == null)
            {
                //將會員紀錄新增到tMember資料表
                db.tMember.Add(pMember);
                db.SaveChanges();
                //跳到login介面登入
                return RedirectToAction("Login");
            }
            ViewBag.Message = "此帳號已有人註冊";
            return View();

        }

    }
}