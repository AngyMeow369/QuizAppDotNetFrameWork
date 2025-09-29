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
            List<Question> questions = quizRepo.GetQuestionsWithOptions(quizId);
            ViewBag.QuizId = quizId;
            ViewBag.QuizTitle = quizRepo.GetQuizById(quizId)?.Title ?? "Quiz";
            return View(questions);
        }

        // Display options for a question (partial view)
        public ActionResult Options(int questionId)
        {
            List<Option> options = quizRepo.GetOptionsByQuestionId(questionId);
            return PartialView(options);
        }

        // -----------------------------------
        // ADMIN QUIZ CRUD ACTIONS
        // -----------------------------------

        // Display all quizzes (admin)
        public ActionResult ManageQuizzes()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            List<Quiz> quizzes = quizRepo.GetAllQuizzes();
            return View(quizzes);
        }

        // Add a new quiz (GET)
        public ActionResult AddQuiz()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            return View();
        }

        // Add a new quiz (POST)
        [HttpPost]
        public ActionResult AddQuiz(Quiz quiz)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Set the createdBy to current user ID
                    quiz.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    quiz.CreatedOn = DateTime.Now;

                    int newQuizId = quizRepo.AddQuiz(quiz);
                    TempData["SuccessMessage"] = "Quiz added successfully!";
                    return RedirectToAction("ManageQuizzes");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error adding quiz: " + ex.Message);
                }
            }
            return View(quiz);
        }

        // Edit quiz (GET)
        public ActionResult EditQuiz(int quizId)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            Quiz quiz = quizRepo.GetQuizById(quizId);
            if (quiz == null)
            {
                return HttpNotFound();
            }
            return View(quiz);
        }

        // Edit quiz (POST)
        [HttpPost]
        public ActionResult EditQuiz(Quiz quiz)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    quizRepo.UpdateQuiz(quiz);
                    TempData["SuccessMessage"] = "Quiz updated successfully!";
                    return RedirectToAction("ManageQuizzes");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating quiz: " + ex.Message);
                }
            }
            return View(quiz);
        }

        // Delete quiz
        public ActionResult DeleteQuiz(int quizId)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            try
            {
                quizRepo.DeleteQuiz(quizId);
                TempData["SuccessMessage"] = "Quiz deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting quiz: " + ex.Message;
            }

            return RedirectToAction("ManageQuizzes");
        }

        // -----------------------------------
        // QUESTION MANAGEMENT ACTIONS
        // -----------------------------------

        // FIXED: Manage questions for a quiz
        public ActionResult ManageQuestions(int? quizId)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            // Check if quizId is provided
            if (quizId == null)
            {
                TempData["ErrorMessage"] = "Please select a quiz first.";
                return RedirectToAction("ManageQuizzes");
            }

            var quiz = quizRepo.GetQuizById(quizId.Value);
            if (quiz == null)
            {
                TempData["ErrorMessage"] = "Quiz not found.";
                return RedirectToAction("ManageQuizzes");
            }

            ViewBag.Quiz = quiz;
            var questions = quizRepo.GetQuestionsWithOptions(quizId.Value);
            return View(questions);
        }

        // Add this new action to your QuizController
        public ActionResult QuestionsBank()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            // Get all questions from all quizzes for the bank
            var allQuizzes = quizRepo.GetAllQuizzes();
            var allQuestions = new List<Question>();

            foreach (var quiz in allQuizzes)
            {
                var questions = quizRepo.GetQuestionsWithOptions(quiz.QuizId);
                allQuestions.AddRange(questions);
            }

            return View(allQuestions);
        }

        // Add question (GET)
        public ActionResult AddQuestion(int quizId)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            var quiz = quizRepo.GetQuizById(quizId);
            if (quiz == null)
            {
                return HttpNotFound();
            }

            ViewBag.Quiz = quiz;
            return View();
        }

        // Add question (POST)
        [HttpPost]
        public ActionResult AddQuestion(Question question, List<Option> options)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Add the question
                    int questionId = quizRepo.AddQuestion(question);

                    // Add options for this question
                    if (options != null)
                    {
                        foreach (var option in options)
                        {
                            if (!string.IsNullOrEmpty(option.OptionText))
                            {
                                option.QuestionId = questionId;
                                quizRepo.AddOption(option);
                            }
                        }
                    }

                    TempData["SuccessMessage"] = "Question and options added successfully!";
                    return RedirectToAction("ManageQuestions", new { quizId = question.QuizId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error adding question: " + ex.Message);
                }
            }

            ViewBag.Quiz = quizRepo.GetQuizById(question.QuizId);
            return View(question);
        }

        // Edit question (GET)
        public ActionResult EditQuestion(int questionId)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            var question = quizRepo.GetQuestionById(questionId);
            if (question == null)
            {
                return HttpNotFound();
            }

            question.Options = quizRepo.GetOptionsByQuestionId(questionId);
            ViewBag.Quiz = quizRepo.GetQuizById(question.QuizId);
            return View(question);
        }

        // Edit question (POST)
        [HttpPost]
        public ActionResult EditQuestion(Question question, List<Option> options)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the question
                    quizRepo.UpdateQuestion(question);

                    // Update options (simplified - in real scenario, you might want to handle updates more carefully)
                    // First delete existing options, then add new ones
                    var existingOptions = quizRepo.GetOptionsByQuestionId(question.QuestionId);
                    foreach (var existingOption in existingOptions)
                    {
                        quizRepo.DeleteOption(existingOption.OptionId);
                    }

                    // Add updated options
                    if (options != null)
                    {
                        foreach (var option in options)
                        {
                            if (!string.IsNullOrEmpty(option.OptionText))
                            {
                                option.QuestionId = question.QuestionId;
                                quizRepo.AddOption(option);
                            }
                        }
                    }

                    TempData["SuccessMessage"] = "Question and options updated successfully!";
                    return RedirectToAction("ManageQuestions", new { quizId = question.QuizId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating question: " + ex.Message);
                }
            }

            ViewBag.Quiz = quizRepo.GetQuizById(question.QuizId);
            return View(question);
        }

        // Delete question
        public ActionResult DeleteQuestion(int questionId)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            try
            {
                var question = quizRepo.GetQuestionById(questionId);
                if (question != null)
                {
                    quizRepo.DeleteQuestion(questionId);
                    TempData["SuccessMessage"] = "Question deleted successfully!";
                    return RedirectToAction("ManageQuestions", new { quizId = question.QuizId });
                }
                TempData["ErrorMessage"] = "Question not found!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting question: " + ex.Message;
            }

            return RedirectToAction("ManageQuizzes");
        }

        // -----------------------------------
        // OPTION MANAGEMENT ACTIONS (Partial)
        // -----------------------------------

        // Add option (AJAX)
        [HttpPost]
        public JsonResult AddOption(Option option)
        {
            try
            {
                quizRepo.AddOption(option);
                return Json(new { success = true, message = "Option added successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding option: " + ex.Message });
            }
        }

        // Delete option (AJAX)
        [HttpPost]
        public JsonResult DeleteOption(int optionId)
        {
            try
            {
                quizRepo.DeleteOption(optionId);
                return Json(new { success = true, message = "Option deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting option: " + ex.Message });
            }
        }

        // -----------------------------------
        // USER RESPONSE SUBMISSION (For Future)
        // -----------------------------------
        /*
        [HttpPost]
        public ActionResult SubmitResponse(UserResponse response)
        {
            if (Session["UserId"] != null)
            {
                response.UserId = Convert.ToInt32(Session["UserId"]);
                // quizRepo.SubmitUserResponse(response); // To be implemented
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "User not logged in" });
        }
        */
    }
}