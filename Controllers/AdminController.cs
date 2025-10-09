using QuizAppDotNetFrameWork.Helpers;
using QuizAppDotNetFrameWork.Models;
using QuizAppDotNetFrameWork.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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
                //Action name and Controller Name
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

            var repo = new Repositories.UserRepository();   
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
                          : PasswordHelper.HashPassword(newPassword);   // or our own hash method

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

        // ========== QUIZ ASSIGNMENT ACTIONS ==========

        // GET: Assign Multiple Quizzes to Users
        public ActionResult AssignQuiz()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
                return RedirectToAction("Login", "Users");

            var quizRepo = new QuizRepository();
            var userRepo = new UserRepository();

            var viewModel = new QuizAssignmentViewModel
            {
                DueDate = DateTime.Now.AddDays(7), // Default due date: 1 week from now
                AvailableQuizzes = quizRepo.GetAllQuizzes(),
                UserAssignments = userRepo.GetAllUsers().Select(u => new UserQuizAssignmentItem
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Role = u.Role,
                    SelectedQuizIds = new List<int>(),
                    IsSelected = false
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Assign Multiple Quizzes to Multiple Users
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignQuiz(QuizAssignmentViewModel model)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
                return RedirectToAction("Login", "Users");

            try
            {
                var quizRepo = new QuizRepository();
                int assignmentCount = 0;

                // Process each user assignment
                foreach (var userAssignment in model.UserAssignments.Where(ua => ua.IsSelected && ua.SelectedQuizIds != null && ua.SelectedQuizIds.Any()))
                {
                    foreach (var quizId in userAssignment.SelectedQuizIds)
                    {
                        quizRepo.AssignQuizToUser(userAssignment.UserId, quizId, model.DueDate);
                        assignmentCount++;
                    }
                }

                if (assignmentCount > 0)
                {
                    TempData["SuccessMessage"] = $"Successfully created {assignmentCount} quiz assignment(s)!";
                    return RedirectToAction("ViewAssignments");
                }
                else
                {
                    TempData["ErrorMessage"] = "Please select at least one user and one quiz combination.";
                    return RedirectToAction("AssignQuiz");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error assigning quizzes: " + ex.Message;
                return RedirectToAction("AssignQuiz");
            }
        }

        // GET: View All Assignments
        public ActionResult ViewAssignments()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
                return RedirectToAction("Login", "Users");

            var quizRepo = new QuizRepository();
            var assignments = quizRepo.GetAllQuizAssignments();

            return View(assignments);
        }

        // GET: View Assignments for a specific quiz
        public ActionResult QuizAssignments(int quizId)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
                return RedirectToAction("Login", "Users");

            var quizRepo = new QuizRepository();
            var assignments = quizRepo.GetQuizAssignmentsByQuiz(quizId);
            ViewBag.QuizTitle = quizRepo.GetQuizById(quizId)?.Title ?? "Quiz";

            return View(assignments);
        }

        // POST: Delete Assignment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAssignment(int assignmentId)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            try
            {
                var quizRepo = new QuizRepository();
                // You'll need to add this method to your QuizRepository
                quizRepo.DeleteAssignment(assignmentId);

                TempData["SuccessMessage"] = "Assignment deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting assignment: " + ex.Message;
            }

            return RedirectToAction("ViewAssignments");
        }
    }
}