using System;
using System.Collections.Generic;
using System.Web.Mvc;
using QuizAppDotNetFrameWork.Repositories;
using QuizAppDotNetFrameWork.Models;

namespace QuizAppDotNetFrameWork.Controllers
{
    public class QuizController : Controller
    {
        private readonly QuizRepository quizRepo = new QuizRepository();

        // -----------------------------------
        // USER-FACING ACTIONS
        // -----------------------------------

        // Display all quizzes for users
        public ActionResult Index()
        {
            if (Session["Role"] == null || Session["Role"].ToString().Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Login", "Users");
            }

            ViewBag.Username = Session["Username"];
            List<Quiz> quizzes = quizRepo.GetAllQuizzes();
            return View(quizzes);
        }

        // Display questions for a selected quiz
        public ActionResult Questions(int quizId)
        {
            List<Question> questions = quizRepo.GetQuestionsWithOptions(quizId); // repository method combining questions + options
            ViewBag.QuizId = quizId;
            return View(questions);
        }

        // Display options for a question (partial view)
        public ActionResult Options(int questionId)
        {
            List<Option> options = quizRepo.GetOptionsByQuestionId(questionId);
            return PartialView(options);
        }

        // -----------------------------------
        // ADMIN-FACING ACTIONS
        // -----------------------------------

        // Display all quizzes (admin)
        [Authorize(Roles = "Admin")]
        public ActionResult ManageQuizzes()
        {
            List<Quiz> quizzes = quizRepo.GetAllQuizzes();
            return View(quizzes);
        }

        // Add a new quiz (GET)
        [Authorize(Roles = "Admin")]
        public ActionResult AddQuiz()
        {
            return View();
        }

        //// Add a new quiz (POST)
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public ActionResult AddQuiz(Quiz quiz)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        quizRepo.AddQuiz(quiz); // Implement AddQuiz JSON SP in repository
        //        return RedirectToAction("ManageQuizzes");
        //    }
        //    return View(quiz);
        //}

        //// Edit quiz (GET)
        //[Authorize(Roles = "Admin")]
        //public ActionResult EditQuiz(int quizId)
        //{
        //    Quiz quiz = quizRepo.GetQuizById(quizId); // Implement GetQuizById JSON SP in repo
        //    return View(quiz);
        //}

        //// Edit quiz (POST)
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public ActionResult EditQuiz(Quiz quiz)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        quizRepo.UpdateQuiz(quiz); // Implement UpdateQuiz JSON SP in repo
        //        return RedirectToAction("ManageQuizzes");
        //    }
        //    return View(quiz);
        //}

        //// Delete quiz
        //[Authorize(Roles = "Admin")]
        //public ActionResult DeleteQuiz(int quizId)
        //{
        //    quizRepo.DeleteQuiz(quizId); // Implement DeleteQuiz JSON SP in repo
        //    return RedirectToAction("ManageQuizzes");
        //}

        //// -----------------------------------
        //// USER RESPONSE SUBMISSION
        //// -----------------------------------

        //[HttpPost]
        //public ActionResult SubmitResponse(UserResponse response)
        //{
        //    if (Session["UserId"] != null)
        //    {
        //        response.UserId = Convert.ToInt32(Session["UserId"]); // Or Guid if using Guid
        //        quizRepo.SubmitUserResponse(response); // Implement SubmitUserResponse JSON SP in repo
        //        return Json(new { success = true });
        //    }
        //    return Json(new { success = false, message = "User not logged in" });
        //}
    }
}

