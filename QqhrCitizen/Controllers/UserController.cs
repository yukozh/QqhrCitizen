﻿using QqhrCitizen.Filters;
using QqhrCitizen.Models;
using QqhrCitizen.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QqhrCitizen.Controllers
{
    public class UserController : BaseController
    {

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        #region 注册
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(vRegister model)
        {

            User user1 = new User();
            user1 = db.Users.Where(u => u.Username == model.Username).SingleOrDefault();
            if (user1 == null)
            {
                User user = new User { Username = model.Username, Password = Helpers.Encryt.GetMD5(model.Password), Role = Role.User, Realname = model.Realname, Email = model.Email, Phone = model.Phone };
                user.Birthday = Convert.ToDateTime("2012-1-1");
                db.Users.Add(user);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    return Message("注册失败，请重新注册");
                }
            }
            else
            {
                ModelState.AddModelError("", "用户名不可用！");
            }
            return View();
        } 
        #endregion

        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated == true)
            {
                return Redirect("/");
            }
            return View();
        }


        [HttpPost]
        public ActionResult Login(vLogin model)
        {

            if (ModelState.IsValid)
            {
                User user = new User();
                model.Password = Helpers.Encryt.GetMD5(model.Password);
                user = db.Users.Where(u => u.Username == model.Username && u.Password == model.Password).SingleOrDefault();
                if (user == null)
                {
                    ModelState.AddModelError("", "用户名或密码错误！");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        #region show
        /// <summary>
        /// show
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [BaseAuth(Roles = "User")]
        [HttpGet]
        public ActionResult Show(int id)
        {
            User user = new User();
            user = db.Users.Find(id);
            ViewBag.user = new vUser(user);
            return View();
        } 
        #endregion

        #region 修改个人西信息
        /// <summary>
        /// 修改个人密码信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [BaseAuth(Roles = "User")]
        [HttpGet]
        public ActionResult PwEdit(int id)
        {
            User user = db.Users.Find(id);
            ViewBag.user = user;
            return View();
        } 
        #endregion

        #region 修改个人西信息
        /// <summary>
        /// 修改个人密码信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [BaseAuth(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PwEdit(vUserPwEdit model)
        {
            User user1 = db.Users.Find(model.ID);
            ViewBag.user = user1;
            //user.Password = model.PasswordNew;
            //user.Age = model.Age;
            //user.Sex = model.Sex;
            //user.Phone = model.Phone;
            //user.Address = model.Address;
            //user.Birthday = model.Birthday;
            //user.Email = model.Email;
            //user.Picture = model.Picture;
            //user.Realname = model.Realname;


            if (ModelState.IsValid)
            {
                User user = db.Users.Find(model.ID);

                //user.Username = model.Username;
                //FormsAuthentication.SignOut();

                FormsAuthentication.SetAuthCookie(model.Username, false);

                if (!string.IsNullOrEmpty(model.Password))
                {
                    if (!user.Password.Equals(Helpers.Encryt.GetMD5(model.Password)))
                    {
                        ModelState.AddModelError("", "原始密码输入不正确");
                    }
                    else
                    {
                        user.Password = Helpers.Encryt.GetMD5(model.PasswordNew);
                        db.SaveChanges();
                        return RedirectToAction("Show/" + model.ID);
                    }
                }
                else
                {
                    db.SaveChanges();
                    return RedirectToAction("Show/" + model.ID);
                }
            }
            else
            {
                ModelState.AddModelError("", "修改的信息输入错误!");
            }
            return View(model);
        } 
        #endregion

         #region 注销

        /// <summary>
        ///  注销 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        } 
        #endregion


        [BaseAuth(Roles="User")]
        public ActionResult ShowPicture(int id)
        {
            User user = new User();
            user = db.Users.Find(id);
            return File(user.Picture, "image/jpg");
        }

        
    }
}