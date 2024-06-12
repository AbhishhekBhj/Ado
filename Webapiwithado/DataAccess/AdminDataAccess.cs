using Microsoft.AspNetCore.Mvc.ModelBinding;
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
                Console.WriteLine(ex.Message);
                return new ResponseModel { Status = 500, Message = ex.Message };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ResponseModel { Status = 500, Message = ex.Message };


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

        

        

        public Task<ResponseModel> AddUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> DeleteContentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> DeleteOptionAsync(int id)
        {
            try
            {

                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();


                    using (SqlCommand sqlCommand = new SqlCommand("deleteanoption", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.AddWithValue("@optionid", id);

                        int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new ResponseModel { Status = 200, Message = "Option Object Deleted Successfully", Data = null };
                        }

                        else
                        {
                            return new ResponseModel { Status = 400, Message = "Option Object Delete Unsucessfully" };
                        }


                    }
                }
            }
            catch (SqlAlreadyFilledException ex)
            {

                return new ResponseModel { Status = 500, Message = ex.ToString() };



            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = 500, Message = ex.ToString() };


            }
        }

        public async Task<ResponseModel> DeleteQuestionAsync(int id)
        {
            try {
            
            using(SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();


                    using(SqlCommand sqlCommand = new SqlCommand("deletequizquestion", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.AddWithValue("@questionid", id);

                  int rowsAffected =       await sqlCommand.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new ResponseModel {Status=200, Message="Question Object Deleted Successfully", Data=null };
                        }

                        else
                        {
                            return new ResponseModel { Status = 400, Message = "Question Object Delete Unsucessfully" };
                        }


                    }
                }
            
            
            
            }
            catch(SqlAlreadyFilledException ex)
            {
                return new ResponseModel { Status=500,Message=ex.ToString()};
            }
            catch(SqlException ex)
            {
                return new ResponseModel { Status = 500, Message = ex.ToString() };

            }

            catch(Exception ex)
            {
                return new ResponseModel { Status = 500, Message = ex.ToString() };

            }
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

        public async Task<ResponseModel> GetAllUsersAsync(int pagesize, int rowsperpage)
        {
            List<User> users = new List<User>();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("getalluserdata", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.AddWithValue("@pagenumber", pagesize);
                        sqlCommand.Parameters.AddWithValue("@rowsperpage", rowsperpage);

                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await sqlDataReader.ReadAsync())
                            {
                                User user = new User
                                {
                                    UserId = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("userid")) ? 0 : sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("userid")),
                                    UserName = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("username")) ? string.Empty : sqlDataReader.GetString(sqlDataReader.GetOrdinal("username")),
                                    Email = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("email")) ? string.Empty : sqlDataReader.GetString(sqlDataReader.GetOrdinal("email")),
                                    Photo = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("photo")) ? string.Empty : sqlDataReader.GetString(sqlDataReader.GetOrdinal("photo")),
                                    Password = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("password")) ? string.Empty : sqlDataReader.GetString(sqlDataReader.GetOrdinal("password")),
                                    IsVerified = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("isverified")) ? 0 : sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("isverified")),
                                    Otp = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("otp")) ? string.Empty : sqlDataReader.GetString(sqlDataReader.GetOrdinal("otp")),
                                    RoleName = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("rollname")) ? string.Empty : sqlDataReader.GetString(sqlDataReader.GetOrdinal("rollname")),
                                    CreatedAt = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("created_at")) ? DateTime.MinValue : sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("created_at")),
                               SignedInWithGoogle = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("signedinwithgoogle")) ? 0 : sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("signedinwithgoogle")),
                                    UpdatedOn = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("updated_on")) ? DateTime.MinValue : sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("updated_on"))
                                
                                };

                                users.Add(user);
                            }
                        }
                    }
                }

                return new ResponseModel { Status = 200, Message = "User Data Fetched Successfully", Data = users };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = 500, Message = ex.ToString() };
            }
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

        public async Task<ResponseModel> UpdateQuestionAsync(UpdateQuestionDTO updateQuestionDTO)
        {
            try
            {

                using(SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using(SqlCommand sqlCommand = new SqlCommand("updatequestiondetails",sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.AddWithValue("@questionid", updateQuestionDTO.QuestionID);
                        sqlCommand.Parameters.AddWithValue("@questionText", updateQuestionDTO.QuestionText);
                        sqlCommand.Parameters.AddWithValue("@questionType", updateQuestionDTO.QuestionType);


                        int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new ResponseModel { 
                            Status=200, Message="Question Object Updated Successfully"};
                        }

                        else
                        {
                            return new ResponseModel
                            {
                                Status = 400,
                                Message = "Question Object Update Unsuccessful"
                            };
                        }



                    }
                }

            }
            catch(SqlAlreadyFilledException ex)
            {
                return new ResponseModel { Status=500,Message=ex.ToString()};

            }

            catch (Exception ex)
            {
                return new ResponseModel { Status = 500, Message = ex.ToString() };

            }
        }

        

        public Task<ResponseModel> UpdateUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> UpdateQuizAsync(int id)
        {
            throw new NotImplementedException();
        }

        

        public async Task<ResponseModel> GetAllContentTypeAync()
        {
            try
            {
                using(SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using(SqlCommand sqlCommand = new SqlCommand("getallcontenttype", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        List<ContentType> contentTypes = new List<ContentType>();

                        using(SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while(await sqlDataReader.ReadAsync())
                            {
                                ContentType contentType = new ContentType
                                {
                                    ContentTypeID = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("ContentTypeID")),
                                    TypeName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("TypeName"))
                                };

                                contentTypes.Add(contentType);
                            }
                        }

                        return new ResponseModel { Status = 200, Message = "Content types fetched successfully", Data = contentTypes };
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

        public async Task<ResponseModel> AddQuestionAsync(AddQuestionDTO addQuestionDTO)
        {
            try
            {
                

                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();


                    //this will insert and return back the question id
                    string insertquery = @"
                    INSERT INTO hamroquizapp.dbo.Question (QuizId, QuestionText, QuestionType)
                    OUTPUT INSERTED.QuestionId
                    VALUES (@QuizId, @QuestionText, @QuestionType)";

                    using(SqlCommand sqlCommand = new SqlCommand(insertquery, sqlConnection, sqlTransaction))
                    {
                        sqlCommand.Parameters.AddWithValue("@QuizId", (object)addQuestionDTO.QuizID ?? DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@QuestionText", addQuestionDTO.QuestionText);
                        sqlCommand.Parameters.AddWithValue("@QuestionType", (object)addQuestionDTO.QuestionType ?? DBNull.Value);
                   
                    


                        //get the question id of the question that was just inserted
                    int questionId = (int)await sqlCommand.ExecuteScalarAsync();



                        string insertOptionQuery = @"
INSERT INTO hamroquizapp.dbo.[Option] (QuestionId, OptionText, IsCorrect)
                    VALUES (@QuestionId, @OptionText, @IsCorrect)";


                        foreach (var option in addQuestionDTO.Options)
                        {
                            using (SqlCommand sqlCommand1 = new SqlCommand(insertOptionQuery, sqlConnection, sqlTransaction))
                            {
                                sqlCommand1.Parameters.AddWithValue("@QuestionId", questionId);
                                sqlCommand1.Parameters.AddWithValue("@OptionText", option.OptionText);
                                sqlCommand1.Parameters.AddWithValue("@IsCorrect", option.IsCorrect);

                                await sqlCommand1.ExecuteNonQueryAsync();
                            }
                        }

                        sqlTransaction.Commit();

                        return new ResponseModel { Status = 200, Message = "Question added successfully" };
                    
                    
                    
                    
                    
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

        public async Task<ResponseModel> GetDashBoardDataAsync()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("GetAdminDashboardData", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            Dashboard dashboardData = new Dashboard();

                            // Read first result set (ObjectCounts)
                            if (await sqlDataReader.ReadAsync())
                            {
                                dashboardData.TablesCount = new TablesCount
                                {
                                    QuizCount = sqlDataReader.GetInt32(0),
                                    QuestionCount = sqlDataReader.GetInt32(1),
                                    UserCount = sqlDataReader.GetInt32(2),
                                    ContentCount = sqlDataReader.GetInt32(3)
                                };
                            }

                            // Move to the next result set
                            await sqlDataReader.NextResultAsync();

                            // Read second result set (PopularQuizzes)
                            List<PopularQuiz> popularQuizzes = new List<PopularQuiz>();
                            while (await sqlDataReader.ReadAsync())
                            {
                                popularQuizzes.Add(new PopularQuiz
                                {
                                    QuizId = sqlDataReader.GetInt32(0),
                                    QuizName = sqlDataReader.GetString(1),
                                    TotalAttempt = sqlDataReader.GetInt32(2)
                                });
                            }
                            dashboardData.PopularQuizzes = popularQuizzes;

                            // Move to the next result set
                            await sqlDataReader.NextResultAsync();

                            // Read third result set (NewUserCount)
                            if (await sqlDataReader.ReadAsync())
                            {
                                dashboardData.NewUserCount = sqlDataReader.GetInt32(0);
                            }

                            // Move to the next result set
                            await sqlDataReader.NextResultAsync();

                            // Read fourth result set (UserScores)
                            List<UserScore> userScores = new List<UserScore>();
                            while (await sqlDataReader.ReadAsync())
                            {
                                userScores.Add(new UserScore
                                {
                                    Username = sqlDataReader.GetString(0),
                                    UserId = sqlDataReader.GetInt32(1),
                                    QuizAttempt = sqlDataReader.GetInt32(2),
                                    TotalScore = sqlDataReader.GetInt32(3)
                                    
                                });
                            }

                            // Set the UserScores property of the dashboardData object
                            dashboardData.UserScores = userScores;

                            return new ResponseModel
                            {
                                Status = 200,
                                Message = "Success",
                                Data = dashboardData
                            };
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

        
    }
}
