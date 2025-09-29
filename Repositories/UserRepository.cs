using QuizAppDotNetFrameWork.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QuizAppDotNetFrameWork.Repositories
{
    public class UserRepository
    {
        private readonly string connectionString;

        public UserRepository()
        {
            connectionString = ConfigurationManager.ConnectionStrings["QuizAppConnection"].ConnectionString;
        }

        // Returns every user row (no filter)
        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("spGetAllUsers", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        users.Add(new User
                        {
                            UserId = Convert.ToInt32(r["UserId"]),
                            Username = r["Username"].ToString(),
                            Role = r["Role"].ToString()
                        });
                    }
                }
            }
            return users;   // never null
        }

        //public int AddUser(string username, string passwordHash, string role)
        //{
        //    using(SqlConnection conn = new SqlConnection(connectionString))
        //    using (SqlCommand cmd = new SqlCommand("AddUser", conn))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@Username", username);
        //        cmd.Parameters.AddWithValue ("@PasswordHash", passwordHash);
        //        cmd.Parameters.AddWithValue("@Role", role);

        //        conn.Open();
        //        return Convert.ToInt32(cmd.ExecuteScalar());

        //    }

        //}

        //Acts as a register or login form using Stored procedure
        public int AddUser(string username, string passwordHash, string role)
        {
            string json = $@"{{ ""Username"": ""{username}"", ""PasswordHash"": ""{passwordHash}"", ""Role"": ""{role}"" }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("AddUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }



        //public void UpdateUser(int userId, string username, string passwordHash, string role)
        //{

        //    using(SqlConnection conn = new SqlConnection(connectionString))
        //    using (SqlCommand cmd = new SqlCommand("UpdateUser", conn))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@UserId", userId);
        //        cmd.Parameters.AddWithValue("@Username", username);
        //        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
        //        cmd.Parameters.AddWithValue("@Role", role);

        //        conn.Open();
        //        cmd.ExecuteNonQuery();

        //    }
        //}

        //Updating existing users
        public void UpdateUser(int userId, string username, string passwordHash, string role)
        {
            string json = $@"{{ ""UserId"": {userId}, ""Username"": ""{username}"", ""PasswordHash"": ""{passwordHash}"", ""Role"": ""{role}"" }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("UpdateUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        //Delete a user
        public void DeleteUser(int userId)
        {
            string json = $@"{{ ""UserId"": {userId} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("DeleteUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        //public void DeleteUser(int userId)
        //{
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        using (SqlCommand cmd = new SqlCommand("DeleteUser", conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@UserId", userId);
        //            conn.Open();

        //            cmd.ExecuteNonQuery();

        //        }
        //    }
        //}


        //Get used by Id
        public User GetUserById(int userId)
        {
            string json = $@"{{ ""UserId"": {userId} }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("GetUserById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            Username = reader["Username"].ToString(),
                            Role = reader["Role"].ToString()
                        };
                    }
                }
            }
            return null;
        }


        //public User GetUserById(int userId)
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    using (SqlCommand cmd = new SqlCommand("GetUserById", conn))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@UserId", userId);

        //        conn.Open();

        //        using(SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            if(reader.Read())
        //            {
        //                return new User
        //                {
        //                    UserId = Convert.ToInt32(reader["UserId"]),
        //                    Username = reader["Username"].ToString(),
        //                    Role = reader["Role"].ToString()
        //                };
        //            }
        //        }
        //    }

        //    return null;
        //}

        //get userbyname
        public User GetUserByName(string name)
        {
            string json = $@"{{ ""Username"": ""{name}"" }}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("GetUserByUserName", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            Username = reader["Username"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            Role = reader["Role"].ToString()
                        };
                    }
                }
            }
            return null;
        }


        //public User GetUserByName(String name)
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    using (SqlCommand cmd = new SqlCommand("GetUserByUserName", conn))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue ("@Username", name);

        //        conn.Open();

        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            if (reader.Read())
        //            {
        //                return new User
        //                {
        //                    UserId = Convert.ToInt32(reader["UserId"]),
        //                    Username = reader["Username"].ToString(),
        //                    PasswordHash = reader["PasswordHash"].ToString(),
        //                    Role = reader["Role"].ToString()
        //                };
        //            }
        //        }
        //    }
        //    return null;
        //}

    }
}