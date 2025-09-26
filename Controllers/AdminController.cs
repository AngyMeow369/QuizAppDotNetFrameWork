using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizAppDotNetFrameWork.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin Dashboard
        public ActionResult Index()
        {
            // Protect this page so only admins can access
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            ViewBag.Username = Session["Username"];
            return View();
        }

        // Example: Manage Users (placeholder for now)
        public ActionResult ManageUsers()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            return View();
        }
    }
}