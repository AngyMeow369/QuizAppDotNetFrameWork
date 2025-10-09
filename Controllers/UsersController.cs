using QuizAppDotNetFrameWork.Helpers;
using QuizAppDotNetFrameWork.Models;
using QuizAppDotNetFrameWork.Repositories;
using System;
using System.Configuration;
using System.Web.Mvc;


namespace QuizAppDotNetFrameWork.Controllers
{
    public class UsersController : Controller

    {
        //Repo objects
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
            
            int newUserId = _userRepo.AddUser(username, hashedPassword, role);

            ViewBag.Message = "Registered successfully! User Name" + username;
            return View();

        }

        //Get of Login
        public ActionResult Login()
        {
            return View();
        }

        //Post of Login with just Session authentication only, no longer using. kept for learning purposes.Jwt used instead.
        //[HttpPost]
        //public ActionResult Login(string username, string password)
        //{
        //    var user = _userRepo.GetUserByName(username);
        //    if(user == null)
        //    {
        //        ViewBag.Message = "Invalid Username or password";
        //        return View();
        //    }

        //    bool isPassWordValid = PasswordHelper.VerifyPassword(password, user.PasswordHash);
        //    if(!isPassWordValid)
        //    {
        //        ViewBag.Message = "Invalid Username or password";
        //        return View();
        //    }

        //    //setting session
        //    Session["UserId"] = user.UserId;
        //    Session["Username"] = user.Username;
        //    Session["Role"] = user.Role;

        //    if(user.Role == "Admin")
        //    {
        //        return RedirectToAction("Index", "Admin");
        //    } else
        //    {
        //        return RedirectToAction("Index", "Quiz");
        //    }
        //}


        //logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        //New login jwt based auth used here as per learning purpose. works with token. 
        [HttpPost]
        public ActionResult LoginJwt(LoginRequest request)
        {
            var user = _userRepo.GetUserByName(request.Username);
            if (user == null)
            {
                return Json(new { success = false, message = "Invalid username or password" });
            }

            bool isPassWordValid = PasswordHelper.VerifyPassword(request.Password, user.PasswordHash);
            if (!isPassWordValid)
            {
                return Json(new { success = false, message = "Invalid username or password" });
            }

            // ✅ JWT: Generate token
            var jwtSettings = new JwtSettings
            {
                Secret = ConfigurationManager.AppSettings["Jwt:Secret"],
                Issuer = ConfigurationManager.AppSettings["Jwt:Issuer"],
                Audience = ConfigurationManager.AppSettings["Jwt:Audience"],
                ExpireMinutes = int.Parse(ConfigurationManager.AppSettings["Jwt:ExpireMinutes"] ?? "60")
            };

            var jwtService = new JwtService(jwtSettings);
            var token = jwtService.GenerateToken(user);

            // ✅ Also set session for backward compatibility
            Session["UserId"] = user.UserId;
            Session["Username"] = user.Username;
            Session["Role"] = user.Role;

            return Json(new
            {
                success = true,
                token = token,
                user = new
                {
                    userId = user.UserId,
                    username = user.Username,
                    role = user.Role
                },
                redirectUrl = user.Role == "Admin" ? "/Admin/Index" : "/Quiz/Index"
            });
        }


    }
}