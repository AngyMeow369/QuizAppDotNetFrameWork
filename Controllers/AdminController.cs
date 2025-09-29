using QuizAppDotNetFrameWork.Helpers;
using QuizAppDotNetFrameWork.Models;
using QuizAppDotNetFrameWork.Repositories;
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
                return RedirectToAction("Login", "Users");

            var repo = new Repositories.UserRepository();   // or use DI later
            var users = repo.GetAllUsers() ?? new List<Models.User>();
            return View(users);                             // hand the list to the view
        }

        // GET /Admin/EditUser?userId=5
        public ActionResult EditUser(int? userId)
        {
            if (!userId.HasValue) return new HttpStatusCodeResult(400);
            var user = new Repositories.UserRepository().GetUserById(userId.Value);
            if (user == null) return HttpNotFound();

            return View(user);          // pass the User entity straight to the view
        }

        // POST /Admin/EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(User model, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(model.Username))
                ModelState.AddModelError("Username", "Required");

            if (!ModelState.IsValid) return View(model);

            var repo = new UserRepository();
            var existing = repo.GetUserById(model.UserId);

            // keep old hash if admin left box empty
            string hash = string.IsNullOrWhiteSpace(newPassword)
                          ? existing.PasswordHash
                          : PasswordHelper.HashPassword(newPassword);   // or your own hash method

            repo.UpdateUser(model.UserId, model.Username, hash, model.Role);
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]                      // ONLY post
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(int? userId)
        {
            if (!userId.HasValue) return new HttpStatusCodeResult(400);

            new UserRepository().DeleteUser(userId.Value);
            return RedirectToAction("ManageUsers");
        }
    }
}