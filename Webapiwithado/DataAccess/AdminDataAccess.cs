using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Webapiwithado.DTOs;
using Webapiwithado.Interface;
using Webapiwithado.Models;

namespace Webapiwithado.DataAccess
{
    public class AdminDataAccess : IAdmin
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AdminDataAccess(IConfiguration configuration) {

            _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        }

        public Task<ResponseModel> GetQuizByIdAsync(int id)
        {
            throw new NotImplementedException();
        }


        public async Task<ResponseModel> GetAllQuizesAsync()
        {
            try
            {
                List<Quiz> quizes = new List<Quiz>();

                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("getquizwithcontentbyid", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            Quiz quiz = new Quiz
                            {
                                QuizID = Convert.ToInt32(reader["QuizID"]),
                                QuizTitle = reader["title"].ToString(),
                                QuizDescription = reader["description"].ToString(),
                                QuizDate = Convert.ToDateTime(reader["createdat"]),
                                Photo = reader["Photo"].ToString(),
                                ContentType = Convert.ToInt32(reader["ContentType"]),
                                TypeName = reader["TypeName"].ToString()
                            };

                            quizes.Add(quiz);
                        }
                    }   

                    return new ResponseModel { Status = 200, Message = "Quizes fetched successfully", Data = quizes };


                }

                

            }

            catch (SqlException ex)
            {
                return new ResponseModel { Status = 500, Message = ex.Message };
            }



            catch (Exception ex)
            {
                return new ResponseModel { Status = 500, Message = ex.Message };
            }
        }


        public async Task<ResponseModel> Get3BestPerformersForQuizbyIdAsync(int id)
        {
            try
            {
                List<UserQuizScore> userQuizScores = new List<UserQuizScore>();

                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using(SqlCommand sqlCommand = new SqlCommand("get3bestperformersforparticularquiz", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@quizid", id);

                        using(SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while(await sqlDataReader.ReadAsync())
                            {

                                UserQuizScore userQuiz = new UserQuizScore { 
                                
                                Title = sqlDataReader.GetString(sqlDataReader.GetOrdinal("title")),
                                Username = sqlDataReader.GetString(sqlDataReader.GetOrdinal("username")),
                                Score = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("score")),
                                TimeStamp = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("takenat"))
                                
                                };

                                


                                userQuizScores.Add(userQuiz);
                                

                            }

                        }

                        return new ResponseModel { Status = 200, Message = "Best performers fetched successfully", Data = userQuizScores };

                        



                    }
                }

            }
            catch (SqlException ex)
            {
                return new ResponseModel { Status = 500, Message = ex.Message };
            }

            catch (Exception ex)
            {
                return new ResponseModel { Status = 500, Message = ex.Message };
            }

            }
        

        public  async Task<ResponseModel> Get3WorstPerformersForQuizbyIdAsync(int id)
        {
            try
            {
                List<UserQuizScore> userQuizScores = new List<UserQuizScore>();

                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("get3worstperformersforparticularquiz", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@quizid", id);

                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await sqlDataReader.ReadAsync())
                            {

                                UserQuizScore userQuiz = new UserQuizScore
                                {

                                    Title = sqlDataReader.GetString(sqlDataReader.GetOrdinal("title")),
                                    Username = sqlDataReader.GetString(sqlDataReader.GetOrdinal("username")),
                                    Score = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("score")),
                                    TimeStamp = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("takenat"))

                                };




                                userQuizScores.Add(userQuiz);


                            }

                        }

                        return new ResponseModel { Status = 200, Message = "Best performers fetched successfully", Data = userQuizScores };





                    }
                }

            }
            catch (SqlException ex)
            {
                return new ResponseModel { Status = 500, Message = ex.Message };
            }

            catch (Exception ex)
            {
                return new ResponseModel { Status = 500, Message = ex.Message };
            }

        }

        public async Task<ResponseModel> AddQuizAsync(QuizUpdateDto quizUpdateDto)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("addnewquizobject", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        
                        sqlCommand.Parameters.AddWithValue("@title", quizUpdateDto.QuizTitle);
                        sqlCommand.Parameters.AddWithValue("@description", quizUpdateDto.QuizDescription);
                        sqlCommand.Parameters.AddWithValue("@photo", quizUpdateDto.Photo);
                        sqlCommand.Parameters.AddWithValue("@contenttype", quizUpdateDto.ContentType);


                        int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new ResponseModel { Status = 200, Message = "Quiz updated successfully" };
                        }

                        else
                        {
                            return new ResponseModel { Status = 500, Message = "Quiz not updated" };
                        }


                    }
                }

            }

            catch (SqlException ex)
            {
                return new ResponseModel { Status = 500, Message = ex.Message };
            }

            catch (Exception ex)
            {
                return new ResponseModel { Status = 500, Message = ex.Message };
            }
            
        }


        public async Task<ResponseModel> UpdateQuizAsync(QuizUpdateDto quizUpdateDto)
        {
            try
            {
                using(SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using(SqlCommand sqlCommand = new SqlCommand("updatequizobject", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.AddWithValue("@quizid", quizUpdateDto.QuizID);
                        sqlCommand.Parameters.AddWithValue("@title", quizUpdateDto.QuizTitle);
                        sqlCommand.Parameters.AddWithValue("@description", quizUpdateDto.QuizDescription);
                        sqlCommand.Parameters.AddWithValue("@photo", quizUpdateDto.Photo);
                        sqlCommand.Parameters.AddWithValue("@contenttype",  quizUpdateDto.ContentType);


                        int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new ResponseModel { Status = 200, Message = "Quiz updated successfully" };
                        }

                        else
                        {
                            return new ResponseModel { Status = 500, Message = "Quiz not updated" };
                        }


                    }
                }

            }
            catch (SqlException ex)
            {
                return new ResponseModel { Status = 500, Message = ex.Message };
            }

            catch (Exception ex)
            {
                return new ResponseModel { Status = 500, Message = ex.Message };
            }
        }

        public async Task<ResponseModel> DeleteQuizAsync(int id)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("deletequizobjectbyid", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.AddWithValue("@quizid", id);

                        int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new ResponseModel { Status = 200, Message = "Quiz deleted successfully" };
                        }

                        else
                        {
                            return new ResponseModel { Status = 500, Message = "Quiz not deleted" };
                        }
                    }
                }

            }
            catch(SqlException ex)
            {
                return new ResponseModel { Status = 500, Message = ex.Message };
            }
            catch(Exception ex)
            {
                return new ResponseModel { Status = 500, Message = ex.Message };


            }
            {

            }
        }

        public Task<ResponseModel> AddContentAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> AddOptionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> AddQuestionAsync()
        {
            throw new NotImplementedException();
        }

        

        public Task<ResponseModel> AddUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> DeleteContentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> DeleteOptionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> DeleteQuestionAsync(int id)
        {
            throw new NotImplementedException();
        }

       

        public Task<ResponseModel> DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> GetAllContentAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> GetAllQuestionsForAQuizAsync()
        {
            throw new NotImplementedException();
        }

        

        public Task<ResponseModel> GetAllUserAnswersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> GetOptionByQuestionIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> GetQuestionByQuizIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        

        public Task<ResponseModel> GetUserAnswerByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> GetUserByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> UpdateContentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> UpdateOptionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> UpdateQuestionAsync()
        {
            throw new NotImplementedException();
        }

        

        public Task<ResponseModel> UpdateUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> UpdateQuizAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> UpdateQuestionAsync(QuizUpdateDto quizUpdateDto)
        {
            throw new NotImplementedException();
        }
    }
}
