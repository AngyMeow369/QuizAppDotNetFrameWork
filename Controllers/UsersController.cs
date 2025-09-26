using System;
using System.Web.Mvc;
using QuizAppDotNetFrameWork.Helpers;
using QuizAppDotNetFrameWork.Repositories;


namespace QuizAppDotNetFrameWork.Controllers
{
    public class UsersController : Controller

    {
        //Repo object
        private readonly UserRepository _userRepo = new UserRepository();

        // GET: User
      
        //Get of Register
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(String username, string password, string role)
        {
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Message = "Username and password are required";
                return View();
            }

            //Hashing the passwords here
            string hashedPassword = PasswordHelper.HashPassword(password);

            //saves to the Db
            //UserRepository userRepo = new UserRepository();
            int newUserId = _userRepo.AddUser(username, hashedPassword, role);

            ViewBag.Message = "Registered successfully! User ID: " + newUserId;
            return View();

        }

        //Get of Login
        public ActionResult Login()
        {
            return View();
        }

        //Post of Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = _userRepo.GetUserByName(username);
            if(user == null)
            {
                ViewBag.Message = "Invalid Username or password";
                return View();
            }

            bool isPassWordValid = PasswordHelper.VerifyPassword(password, user.PasswordHash);
            if(!isPassWordValid)
            {
                ViewBag.Message = "Invalid Username or password";
                return View();
            }

            //setting session
            Session["UserId"] = user.UserId;
            Session["Username"] = user.Username;
            Session["Role"] = user.Role;

            if(user.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            } else
            {
                return RedirectToAction("Index", "Quiz");
            }
        }

        //logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        

    }
}