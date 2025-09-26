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

        //Returning all the quizes using List
        public List<Quiz> GetAllQuizzes()
        {
            var quizzes = new List<Quiz>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spGetAllQuizzes", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        quizzes.Add(new Quiz
                        {
                            Id = Convert.ToInt32(reader["QuizId"]),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            createdBy = Convert.ToInt32(reader["CreatedBy"]),
                            CreatedOn = Convert.ToDateTime(reader["CreatedOn"])
                        });
                    }
                }
            }
            return quizzes;
        }

        //Get questions by QuizId
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

        //Get options by questionId
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