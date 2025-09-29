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
        public void DeleteQuiz(int quizId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spDeleteQuiz", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuizId", quizId);

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
        public void DeleteQuestion(int questionId)
        {
            string json = $@"{{ ""QuestionId"": {questionId} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spDeleteQuestion", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@QuestionData", json);

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
                            isCorrect = Convert.ToBoolean(reader["IsCorrect"])
                        });
                    }
                }
            }
            return options;
        }

        // Add new option
        public void AddOption(Option option)
        {
            string isCorrect = option.isCorrect ? "true" : "false";
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
            string isCorrect = option.isCorrect ? "true" : "false";
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
        public void DeleteOption(int optionId)
        {
            string json = $@"{{ ""OptionId"": {optionId} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spDeleteOption", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OptionData", json);

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


    }
}