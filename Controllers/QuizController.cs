using QuizAppDotNetFrameWork.Models;
using QuizAppDotNetFrameWork.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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
        public ActionResult Questions(int quizId, int? assignmentId = null)
        {
            // Store assignment ID in session if provided (for assignment tracking)
            if (assignmentId.HasValue)
            {
                Session["CurrentAssignmentId"] = assignmentId.Value;
            }

            List<Question> questions = quizRepo.GetQuestionsWithOptions(quizId);
            ViewBag.QuizId = quizId;
            ViewBag.QuizTitle = quizRepo.GetQuizById(quizId)?.Title ?? "Quiz";

            // Add time limit calculation for assignments
            ViewBag.TimeLimit = questions.Count * 1; // 1 minute per question

            // Store if this is an assigned quiz for the timer
            ViewBag.IsAssignedQuiz = assignmentId.HasValue;

            return View(questions);
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
        // Edit question (GET) - ADD THIS!
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

        // Edit question (POST) - REPLACE YOUR CURRENT ONE
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

                    // Validate options
                    if (options == null || options.Count(o => !string.IsNullOrEmpty(o.OptionText)) < 2)
                    {
                        ModelState.AddModelError("", "At least 2 options are required.");
                    }
                    else if (options.Count(o => o.IsCorrect) != 1)
                    {
                        ModelState.AddModelError("", "Exactly one option must be marked as correct.");
                    }
                    else
                    {
                        // Update options (delete existing and add new ones)
                        var existingOptions = quizRepo.GetOptionsByQuestionId(question.QuestionId);
                        foreach (var existingOption in existingOptions)
                        {
                            quizRepo.DeleteOption(existingOption.OptionId);
                        }

                        // Add updated options
                        foreach (var option in options)
                        {
                            if (!string.IsNullOrEmpty(option.OptionText))
                            {
                                option.QuestionId = question.QuestionId;
                                quizRepo.AddOption(option);
                            }
                        }

                        TempData["SuccessMessage"] = "Question and options updated successfully!";
                        return RedirectToAction("ManageQuestions", new { quizId = question.QuizId });
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating question: " + ex.Message);
                }
            }

            ViewBag.Quiz = quizRepo.GetQuizById(question.QuizId);
            return View(question);
        }

        // ADD THIS HELPER METHOD TO YOUR QUIZCONTROLLER CLASS
        private List<Option> ParseOptionsFromForm(FormCollection form, int questionId)
        {
            var options = new List<Option>();

            // Look for option fields in the form
            int i = 0;
            while (true)
            {
                string optionText = form[$"Options[{i}].OptionText"];
                string isCorrectValue = form[$"Options[{i}].IsCorrect"];

                // Stop if no more options found
                if (string.IsNullOrEmpty(optionText))
                    break;

                // Parse the OptionId if it exists
                int optionId = 0;
                string optionIdValue = form[$"Options[{i}].OptionId"];
                if (!string.IsNullOrEmpty(optionIdValue) && int.TryParse(optionIdValue, out int parsedId))
                {
                    optionId = parsedId;
                }

                // Convert dropdown value to boolean
                bool isCorrect = (isCorrectValue?.ToLower() == "true");

                options.Add(new Option
                {
                    OptionId = optionId,
                    QuestionId = questionId,
                    OptionText = optionText.Trim(),
                    IsCorrect = isCorrect
                });

                i++;
            }

            return options;
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

        [HttpPost]
        public ActionResult SubmitQuiz(int quizId, FormCollection form)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Parse answers from form collection using the new naming convention
            var selectedOptions = new Dictionary<int, int>();

            foreach (string key in form.AllKeys)
            {
                if (key.StartsWith("q_") && !string.IsNullOrEmpty(form[key]))
                {
                    int questionId = int.Parse(key.Substring(2)); // Remove "q_" prefix
                    int selectedOptionId = int.Parse(form[key]);
                    selectedOptions[questionId] = selectedOptionId;

                    // Debug output
                    System.Diagnostics.Debug.WriteLine($"Found answer: Q{questionId} -> Option{selectedOptionId}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"Total answers found: {selectedOptions.Count}");

            int userId = Convert.ToInt32(Session["UserId"]);
            int score = 0;
            var allQuestions = quizRepo.GetQuestionsWithOptions(quizId);
            int totalQuestions = allQuestions.Count;

            foreach (var question in allQuestions)
            {
                if (selectedOptions.ContainsKey(question.QuestionId))
                {
                    int selectedOptionId = selectedOptions[question.QuestionId];
                    var selectedOption = question.Options.FirstOrDefault(o => o.OptionId == selectedOptionId);

                    if (selectedOption != null && selectedOption.IsCorrect)
                    {
                        score++;
                        System.Diagnostics.Debug.WriteLine($"✓ Correct: Q{question.QuestionId}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"✗ Wrong: Q{question.QuestionId}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ No answer: Q{question.QuestionId}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"Final Score: {score}/{totalQuestions}");

            // Calculate percentage and grade
            double percentage = totalQuestions > 0 ? (double)score / totalQuestions * 100 : 0;
            string grade = GetGrade(percentage);

            // Save quiz attempt
            int attemptId = quizRepo.SaveQuizAttempt(userId, quizId, score, totalQuestions, grade);

            // Save individual responses
            foreach (var kvp in selectedOptions)
            {
                var question = allQuestions.FirstOrDefault(q => q.QuestionId == kvp.Key);
                bool isCorrect = question?.Options.Any(o => o.OptionId == kvp.Value && o.IsCorrect) == true;
                quizRepo.SaveUserResponse(userId, kvp.Key, kvp.Value, isCorrect, attemptId);
            }

            // ✅ NEW: SAFELY mark assignment as completed if this was an assigned quiz
            // This WON'T affect regular quiz taking at all!
            if (Session["CurrentAssignmentId"] != null)
            {
                try
                {
                    int assignmentId = (int)Session["CurrentAssignmentId"];
                    quizRepo.CompleteQuizAssignment(assignmentId, attemptId);
                    Session["CurrentAssignmentId"] = null; // Clear assignment ID
                    System.Diagnostics.Debug.WriteLine($"✅ Assignment {assignmentId} marked as completed");
                }
                catch (Exception ex)
                {
                    // If assignment completion fails, don't break the quiz submission
                    System.Diagnostics.Debug.WriteLine($"⚠️ Assignment completion failed: {ex.Message}");
                    // Continue normally - don't throw error
                }
            }

            // Redirect to results page (EXACTLY SAME AS BEFORE)
            return RedirectToAction("QuizResults", new
            {
                quizId = quizId,
                score = score,
                totalQuestions = totalQuestions,
                percentage = percentage,
                grade = grade
            });
        }

        // Display user's quiz results
        public ActionResult Results()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            // Get all quiz attempts for this user
            var userAttempts = quizRepo.GetUserQuizAttempts(userId);

            // Calculate statistics
            ViewBag.TotalAttempts = userAttempts.Count;
            ViewBag.AverageScore = userAttempts.Count > 0 ? userAttempts.Average(a => a.Percentage) : 0;
            ViewBag.BestScore = userAttempts.Count > 0 ? userAttempts.Max(a => a.Percentage) : 0;

            return View(userAttempts);
        }

        private string GetGrade(double percentage)
        {
            if (percentage >= 90) return "A+";
            else if (percentage >= 80) return "A";
            else if (percentage >= 70) return "B";
            else if (percentage >= 60) return "C";
            else if (percentage >= 50) return "D";
            else return "F";
        }

        public ActionResult QuizResults(int quizId, int score, int totalQuestions, double percentage, string grade)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // ✅ FIX: Handle quizId=0 by finding the actual quiz from attempt data
            Quiz quiz = null;

            if (quizId == 0)
            {
                // Try to find the most recent attempt for this user
                var userAttempts = quizRepo.GetUserQuizAttempts((int)Session["UserId"]);
                var recentAttempt = userAttempts
                    .Where(a => a.Score == score && a.TotalQuestions == totalQuestions)
                    .OrderByDescending(a => a.CompletedOn)
                    .FirstOrDefault();

                if (recentAttempt != null)
                {
                    quizId = recentAttempt.QuizId;
                    quiz = quizRepo.GetQuizById(quizId);
                }
            }
            else
            {
                quiz = quizRepo.GetQuizById(quizId);
            }

            if (quiz == null)
            {
                // If quiz still not found, create a dummy quiz object to prevent errors
                quiz = new Quiz
                {
                    QuizId = quizId,
                    Title = "Quiz",
                    Description = "Quiz results"
                };
                // Don't redirect - just show results with generic title
            }

            ViewBag.Quiz = quiz;
            ViewBag.Score = score;
            ViewBag.TotalQuestions = totalQuestions;
            ViewBag.Percentage = percentage;
            ViewBag.Grade = grade;

            return View();
        }

        public ActionResult AssignedQuizzes()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            int userId = (int)Session["UserId"];
            var assignments = quizRepo.GetAssignedQuizzesByUser(userId); // FIXED: use instance, not static
            return View(assignments);
        }



    }
}