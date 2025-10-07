using QuizAppDotNetFrameWork.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QuizAppDotNetFrameWork.Repositories
{
    public class QuizRepository
    {
        private readonly string connectionString;

        public QuizRepository()
        {
            connectionString = ConfigurationManager.ConnectionStrings["QuizAppConnection"].ConnectionString;
        }

        // ========== QUIZ CRUD METHODS ==========

        #region Returning all the quizes
        public List<Quiz> GetAllQuizzes()
        {
            var quizzes = new List<Quiz>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spGetAllQuizzes", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        quizzes.Add(new Quiz
                        {
                            QuizId = Convert.ToInt32(reader["QuizId"]),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                            CreatedOn = Convert.ToDateTime(reader["CreatedOn"])
                        });
                    }
                }
            }
            return quizzes;
        }
        #endregion



        // Add new quiz
        public int AddQuiz(Quiz quiz)
        {
            string json = $@"{{ ""Title"": ""{quiz.Title.Replace("\"", "\\\"")}"", ""Description"": ""{quiz.Description?.Replace("\"", "\\\"")}"", ""CreatedBy"": {quiz.CreatedBy} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spAddQuiz", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Title", quiz.Title);
                cmd.Parameters.AddWithValue("@Description", quiz.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", quiz.CreatedBy);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        // Update quiz
        public void UpdateQuiz(Quiz quiz)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spUpdateQuiz", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuizId", quiz.QuizId);
                cmd.Parameters.AddWithValue("@Title", quiz.Title);
                cmd.Parameters.AddWithValue("@Description", quiz.Description ?? (object)DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Delete quiz
        // Delete quiz - FIXED to use JSON
        public void DeleteQuiz(int quizId)
        {
            string json = $@"{{ ""QuizId"": {quizId} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spDeleteQuiz", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuizData", json); // Use JSON parameter

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Get quiz by ID
        public Quiz GetQuizById(int quizId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spGetQuizById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuizId", quizId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Quiz
                        {
                            QuizId = Convert.ToInt32(reader["QuizId"]),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                            CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                            CreatedOn = Convert.ToDateTime(reader["CreatedOn"])
                        };
                    }
                }
            }
            return null;
        }

        // ========== QUESTION CRUD METHODS ==========

        // Get questions by QuizId
        public List<Question> GetQuestionsByQuizId(int quizId)
        {
            var questions = new List<Question>();
            string json = $@" {{ ""QuizId"": {quizId} }} ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spGetQuestionsByQuizId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        questions.Add(new Question
                        {
                            QuestionId = Convert.ToInt32(reader["QuestionId"]),
                            QuizId = Convert.ToInt32(reader["QuizId"]),
                            QuestionText = (reader["QuestionText"]).ToString()
                        });
                    }
                }
            }
            return questions;
        }

        // Add new question
        public int AddQuestion(Question question)
        {
            string json = $@"{{ ""QuizId"": {question.QuizId}, ""QuestionText"": ""{question.QuestionText.Replace("\"", "\\\"")}"" }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spAddQuestion", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuestionData", json);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        // Update question
        public void UpdateQuestion(Question question)
        {
            string json = $@"{{ ""QuestionId"": {question.QuestionId}, ""QuizId"": {question.QuizId}, ""QuestionText"": ""{question.QuestionText.Replace("\"", "\\\"")}"" }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spUpdateQuestion", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuestionData", json);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Delete question
        // Delete question - FIXED to use JSON
        public void DeleteQuestion(int questionId)
        {
            string json = $@"{{ ""QuestionId"": {questionId} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spDeleteQuestion", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuestionData", json); // Use JSON parameter

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Get question by ID
        public Question GetQuestionById(int questionId)
        {
            string json = $@"{{ ""QuestionId"": {questionId} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spGetQuestionById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuestionData", json);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Question
                        {
                            QuestionId = Convert.ToInt32(reader["QuestionId"]),
                            QuizId = Convert.ToInt32(reader["QuizId"]),
                            QuestionText = reader["QuestionText"].ToString(),
                            Options = new List<Option>()
                        };
                    }
                }
            }
            return null;
        }

        // ========== OPTION CRUD METHODS ==========

        // Get options by questionId
        public List<Option> GetOptionsByQuestionId(int questionId)
        {
            var options = new List<Option>();
            string json = $@"{{""QuestionId"": {questionId} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spGetOptionsByQuestionId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        options.Add(new Option
                        {
                            OptionId = Convert.ToInt32(reader["OptionId"]),
                            QuestionId = Convert.ToInt32(reader["QuestionId"]),
                            OptionText = reader["OptionText"].ToString(),
                            IsCorrect = Convert.ToBoolean(reader["IsCorrect"])
                        });
                    }
                }
            }
            return options;
        }

        // Add new option
        public void AddOption(Option option)
        {
            string isCorrect = option.IsCorrect ? "true" : "false";
            string json = $@"{{ ""QuestionId"": {option.QuestionId}, ""OptionText"": ""{option.OptionText.Replace("\"", "\\\"")}"", ""IsCorrect"": {isCorrect} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spAddOption", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OptionData", json);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Update option
        public void UpdateOption(Option option)
        {
            string isCorrect = option.IsCorrect ? "true" : "false";
            string json = $@"{{ ""OptionId"": {option.OptionId}, ""QuestionId"": {option.QuestionId}, ""OptionText"": ""{option.OptionText.Replace("\"", "\\\"")}"", ""IsCorrect"": {isCorrect} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spUpdateOption", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OptionData", json);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Delete option
        // Delete option - FIXED to use JSON
        public void DeleteOption(int optionId)
        {
            string json = $@"{{ ""OptionId"": {optionId} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spDeleteOption", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OptionData", json); // Use JSON parameter

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ========== HELPER METHODS ==========

        public List<Question> GetQuestionsWithOptions(int quizId)
        {
            var questions = GetQuestionsByQuizId(quizId);
            foreach (var q in questions)
            {
                q.Options = GetOptionsByQuestionId(q.QuestionId);
            }
            return questions;
        }

        // ========== QUIZ ATTEMPTS & RESULTS METHODS ==========

        // Save quiz attempt
        // Save quiz attempt - FIXED JSON version
        public int SaveQuizAttempt(int userId, int quizId, int score, int totalQuestions, string grade)
        {
            string json = $@"{{
        ""UserId"": {userId},
        ""QuizId"": {quizId},
        ""Score"": {score},
        ""TotalQuestions"": {totalQuestions},
        ""Grade"": ""{grade}""
    }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spSaveQuizAttempt", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);  // Only ONE parameter - @json

                conn.Open();
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        // Save user response
        // Save user response - JSON version
        // Save user response - INDIVIDUAL PARAMETERS version
        public void SaveUserResponse(int userId, int questionId, int selectedOptionId, bool isCorrect, int attemptId)
        {
            string isCorrectStr = isCorrect ? "true" : "false";
            string json = $@"{{
                ""UserId"": {userId},
                ""QuestionId"": {questionId},
                ""SelectedOptionId"": {selectedOptionId},
                ""IsCorrect"": {isCorrectStr},
                ""AttemptId"": {attemptId}
            }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spSaveUserResponse", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);  // Only ONE parameter - @json

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Get user's quiz attempts
        public List<QuizAttempt> GetUserQuizAttempts(int userId)
        {
            var attempts = new List<QuizAttempt>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spGetUserQuizAttempts", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        attempts.Add(new QuizAttempt
                        {
                            AttemptId = Convert.ToInt32(reader["AttemptId"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            QuizId = Convert.ToInt32(reader["QuizId"]),
                            Score = Convert.ToInt32(reader["Score"]),
                            TotalQuestions = Convert.ToInt32(reader["TotalQuestions"]),
                            Grade = reader["Grade"].ToString(),
                            CompletedOn = Convert.ToDateTime(reader["CompletedOn"]),
                            QuizTitle = reader["QuizTitle"].ToString()
                        });
                    }
                }
            }
            return attempts;
        }

        // Get quiz attempt by ID
        public QuizAttempt GetQuizAttemptById(int attemptId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spGetQuizAttemptById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AttemptId", attemptId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new QuizAttempt
                        {
                            AttemptId = Convert.ToInt32(reader["AttemptId"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            QuizId = Convert.ToInt32(reader["QuizId"]),
                            Score = Convert.ToInt32(reader["Score"]),
                            TotalQuestions = Convert.ToInt32(reader["TotalQuestions"]),
                            Grade = reader["Grade"].ToString(),
                            CompletedOn = Convert.ToDateTime(reader["CompletedOn"]),
                            QuizTitle = reader["QuizTitle"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        // ========== QUIZ ASSIGNMENT METHODS ==========

        // Assign quiz to user
        public void AssignQuizToUser(int userId, int quizId, DateTime dueDate)
        {
            string json = $@"{{
        ""UserId"": {userId},
        ""QuizId"": {quizId},
        ""DueDate"": ""{dueDate:yyyy-MM-dd HH:mm:ss}""
    }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spAssignQuizToUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Get assigned quizzes for a specific user
        public List<UserQuizAssignment> GetAssignedQuizzesByUser(int userId)
        {
            var assignments = new List<UserQuizAssignment>();
            string json = $@"{{ ""UserId"": {userId} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spGetAssignedQuizzes", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var assignment = new UserQuizAssignment
                        {
                            AssignmentId = Convert.ToInt32(reader["AssignmentId"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            QuizId = Convert.ToInt32(reader["QuizId"]),
                            AssignedOn = Convert.ToDateTime(reader["AssignedOn"]),
                            DueDate = Convert.ToDateTime(reader["DueDate"]),
                            IsCompleted = Convert.ToBoolean(reader["IsCompleted"]),
                            AttemptId = reader["AttemptId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["AttemptId"]),
                            QuizTitle = reader["QuizTitle"].ToString(),
                            QuestionCount = Convert.ToInt32(reader["QuestionCount"])
                        };

                        // Auto-calculate time limit (1 minute per question)
                        assignment.TimeLimitMinutes = assignment.QuestionCount * 1;

                        assignments.Add(assignment);
                    }
                }
            }
            return assignments;
        }

        // Get all assignments for admin view
        public List<UserQuizAssignment> GetAllQuizAssignments()
        {
            var assignments = new List<UserQuizAssignment>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spGetAllQuizAssignments", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var assignment = new UserQuizAssignment
                        {
                            AssignmentId = Convert.ToInt32(reader["AssignmentId"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            QuizId = Convert.ToInt32(reader["QuizId"]),
                            AssignedOn = Convert.ToDateTime(reader["AssignedOn"]),
                            DueDate = Convert.ToDateTime(reader["DueDate"]),
                            IsCompleted = Convert.ToBoolean(reader["IsCompleted"]),
                            AttemptId = reader["AttemptId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["AttemptId"]),
                            Username = reader["Username"].ToString(),
                            QuizTitle = reader["QuizTitle"].ToString(),
                            QuestionCount = Convert.ToInt32(reader["QuestionCount"])
                        };

                        // Auto-calculate time limit
                        assignment.TimeLimitMinutes = assignment.QuestionCount * 1;

                        assignments.Add(assignment);
                    }
                }
            }
            return assignments;
        }

        // Mark assignment as completed
        public void CompleteQuizAssignment(int assignmentId, int attemptId)
        {
            string json = $@"{{
        ""AssignmentId"": {assignmentId},
        ""AttemptId"": {attemptId}
    }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spCompleteQuizAssignment", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Get assignments for a specific quiz
        public List<UserQuizAssignment> GetQuizAssignmentsByQuiz(int quizId)
        {
            var assignments = new List<UserQuizAssignment>();
            string json = $@"{{ ""QuizId"": {quizId} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spGetQuizAssignmentsByQuiz", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var assignment = new UserQuizAssignment
                        {
                            AssignmentId = Convert.ToInt32(reader["AssignmentId"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            QuizId = Convert.ToInt32(reader["QuizId"]),
                            AssignedOn = Convert.ToDateTime(reader["AssignedOn"]),
                            DueDate = Convert.ToDateTime(reader["DueDate"]),
                            IsCompleted = Convert.ToBoolean(reader["IsCompleted"]),
                            AttemptId = reader["AttemptId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["AttemptId"]),
                            Username = reader["Username"].ToString(),
                            QuizTitle = reader["QuizTitle"].ToString(),
                            QuestionCount = Convert.ToInt32(reader["QuestionCount"])
                        };

                        // Auto-calculate time limit (1 minute per question)
                        assignment.TimeLimitMinutes = assignment.QuestionCount * 1;

                        assignments.Add(assignment);
                    }
                }
            }
            return assignments;
        }

        // Delete assignment
        public void DeleteAssignment(int assignmentId)
        {
            string json = $@"{{ ""AssignmentId"": {assignmentId} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spDeleteAssignment", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Get all quiz attempts for admin view - JSON version
        public List<QuizAttempt> GetAllQuizAttempts()
        {
            var attempts = new List<QuizAttempt>();

            // Empty JSON since we want all attempts
            string json = @"{}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spGetAllQuizAttempts", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        attempts.Add(new QuizAttempt
                        {
                            AttemptId = Convert.ToInt32(reader["AttemptId"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            QuizId = Convert.ToInt32(reader["QuizId"]),
                            Score = Convert.ToInt32(reader["Score"]),
                            TotalQuestions = Convert.ToInt32(reader["TotalQuestions"]),
                            Grade = reader["Grade"].ToString(),
                            CompletedOn = Convert.ToDateTime(reader["CompletedOn"]),
                            Username = reader["Username"].ToString(),
                            QuizTitle = reader["QuizTitle"].ToString()
                        });
                    }
                }
            }
            return attempts;
        }

        // Get user responses by attempt ID - ADD THIS METHOD
        public List<UserResponse> GetUserResponsesByAttempt(int attemptId)
        {
            var responses = new List<UserResponse>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spGetUserResponsesByAttempt", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AttemptId", attemptId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        responses.Add(new UserResponse
                        {
                            ResponseId = Convert.ToInt32(reader["ResponseId"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            QuestionId = Convert.ToInt32(reader["QuestionId"]),
                            SelectedOptionId = Convert.ToInt32(reader["SelectedOptionId"]),
                            IsCorrect = Convert.ToBoolean(reader["IsCorrect"]),
                            AttemptId = Convert.ToInt32(reader["AttemptId"])
                        });
                    }
                }
            }
            return responses;
        }


    }
}