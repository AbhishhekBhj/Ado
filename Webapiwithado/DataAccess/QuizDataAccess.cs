using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Webapiwithado.Models;
using Newtonsoft.Json;
using Webapiwithado.DTOs;
using System.Data;


namespace Webapiwithado.DataAccess
{
    public class QuizDataAccess
    {
        private readonly string _connectionString;



        public QuizDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }


        public async Task<ResponseModel> GetUserQuizScoreForAParticularQuiz(int userid, int quizid)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("getuserscoreforparticularquiz", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@userid", userid);
                        sqlCommand.Parameters.AddWithValue("@quizid", quizid);

                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (sqlDataReader.HasRows)
                            {
                                await sqlDataReader.ReadAsync(); // Move to the first record
                                var userQuizScore = new UserQuizScore
                                {
                                    Username = sqlDataReader["username"].ToString() ?? "",
                                    Title = sqlDataReader["title"].ToString() ?? "",
                                    Score = Convert.ToInt32(sqlDataReader["score"]),
                                    TimeStamp = Convert.ToDateTime(sqlDataReader["TakenAt"])
                                    
                                };

                                return new ResponseModel
                                {
                                    Data = userQuizScore,
                                    Message = "Success",
                                    Status = 200
                                };
                            }
                            else
                            {
                                return new ResponseModel
                                {
                                    Message = "No score found for the given user and quiz",
                                    Status = 404,
                                    Data = null
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception (ex.Message)
                return new ResponseModel
                {
                    Message = "An error occurred",
                    Status = 500,
                    Data = ex.Message
                };
            }
        }




        public async Task<ResponseModel> GetUserTop3WorstQuizAsync(int userid)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("getusers3worstquiz", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@userid", userid);

                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (sqlDataReader.HasRows)
                            {
                                List<UserQuizScore> userQuizScores = new List<UserQuizScore>();
                                while (await sqlDataReader.ReadAsync())
                                {
                                    userQuizScores.Add(
                                                                               new UserQuizScore
                                                                               {
                                                                                   Username = sqlDataReader["username"].ToString() ?? "",
                                                                                   Title = sqlDataReader["title"].ToString() ?? "",
                                                                                   Score = Convert.ToInt32(sqlDataReader["score"]),
                                                                                   TimeStamp = Convert.ToDateTime(sqlDataReader["takenat"])
                                                                               }
                                                                                                                      );
                                }
                                return new ResponseModel
                                {
                                    Data = userQuizScores,
                                    Message = "Success",
                                    Status = 200
                                };
                            }
                            else
                            {
                                return new ResponseModel
                                {
                                    Message = "No quiz found for the given user",
                                    Status = 404,
                                    Data = null
                                };
                            }
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Message = "An error occurred",
                    Status = 500,
                    Data = ex.Message
                };
            }
        }



        public async Task<ResponseModel> GetUserTop3BestQuizAsync(int userid)
        {
            try
            {
                using(SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("getuserstop3bestquiz", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@userid", userid);

                        using(SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if(sqlDataReader.HasRows)
                            {
                                List<UserQuizScore> userQuizScores = new List<UserQuizScore>();
                                while(await sqlDataReader.ReadAsync())
                                {
                                    userQuizScores.Add(
                                                                               new UserQuizScore
                                                                               {
                                            Username = sqlDataReader["username"].ToString() ?? "",
                                            Title = sqlDataReader["title"].ToString() ?? "",
                                            Score = Convert.ToInt32(sqlDataReader["score"]),
                                            TimeStamp = Convert.ToDateTime(sqlDataReader["takenat"])
                                        }
                                                                                                                      );
                                }
                                return new ResponseModel
                                {
                                    Data = userQuizScores,
                                    Message = "Success",
                                    Status = 200
                                };
                            }
                            else
                            {
                                return new ResponseModel
                                {
                                    Message = "No quiz found for the given user",
                                    Status = 404,
                                    Data = null
                                };
                            }
                        }
                    }   
                }
            
            }

            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Message = "An error occurred",
                    Status = 500,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResponseModel> GetQuizObjectByIDAsync(int quizid)
        {
            QuizDTO quizDTO = new QuizDTO();
            List<QuestionDTO> questionDTOs = new List<QuestionDTO>();
            Dictionary<int, QuestionDTO> questionMap = new Dictionary<int, QuestionDTO>();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("getquizobjectbyid", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@quizid", quizid);

                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await sqlDataReader.ReadAsync())
                            {
                                if (quizDTO.QuizID == 0)
                                {
                                    quizDTO.QuizID = Convert.ToInt32(sqlDataReader["quizid"]);
                                    quizDTO.QuizTitle = sqlDataReader["title"].ToString();
                                    quizDTO.QuizDescription = sqlDataReader["description"].ToString();
                                    quizDTO.Photo = sqlDataReader["photo"].ToString();
                                    quizDTO.ContentType = sqlDataReader["TypeName"].ToString();
                                    
                                }

                                int questionID = Convert.ToInt32(sqlDataReader["questionid"]);
                                if (!questionMap.ContainsKey(questionID))
                                {
                                    var questionDTO = new QuestionDTO
                                    {
                                        QuestionID = questionID,
                                        QuestionText = sqlDataReader["questiontext"].ToString(),
                                        QuestionType = sqlDataReader["QuestionType"].ToString(),
                                        Options = new List<OptionDTO>()
                                    };
                                    questionMap[questionID] = questionDTO;
                                    questionDTOs.Add(questionDTO);
                                }

                                var optionDTO = new OptionDTO
                                {
                                    OptionID = Convert.ToInt32(sqlDataReader["optionid"]),
                                    OptionText = sqlDataReader["optiontext"].ToString(),
                                    IsCorrect = Convert.ToInt32(sqlDataReader["iscorrect"]) == 1
                                };
                                questionMap[questionID].Options.Add(optionDTO);
                            }
                        }
                    }
                }

                quizDTO.Questions = questionDTOs;

                return new ResponseModel
                {
                    Message = "Success",
                    Status = 200,
                    Data = quizDTO
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Message = "An error occurred",
                    Status = 500,
                    Data = ex.Message
                };
            }
        }


        public async Task<ResponseModel> GetAllQuizAsync(int pageNumber, int pageSize)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    // Change the SQL command to call the stored procedure
                    var sqlQuery = "getallquiz";  // Name of the stored procedure

                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;  // Set command type to stored procedure


                        sqlCommand.Parameters.AddWithValue("@PageNumber", pageNumber);
                        sqlCommand.Parameters.AddWithValue("@PageSize", pageSize);
                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            List<Quiz> quizList = new List<Quiz>();
                            while (await sqlDataReader.ReadAsync())
                            {
                                quizList.Add(
                                    new Quiz
                                    {
                                        QuizID = Convert.ToInt32(sqlDataReader["quizid"]),
                                        QuizTitle = sqlDataReader["title"].ToString(),
                                        QuizDescription = sqlDataReader["description"].ToString(),
                                        QuizDate = Convert.ToDateTime(sqlDataReader["createdat"]),
                                        TypeName = sqlDataReader["typename"].ToString(),
                                        Photo = sqlDataReader["photo"].ToString()
                                        
                                    }
                                );
                            }

                            ResponseModel responseModel = new ResponseModel
                            {
                                Data = quizList,
                                Message = "Success",
                                Status = 200
                            };
                            return responseModel;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ResponseModel responseModel = new ResponseModel
                {
                    Message = "Failed",
                    Status = 500,
                    Data = JsonConvert.SerializeObject(sqlEx.Message)
                };
                return responseModel;
            }
        }


        public async Task<ResponseModel> PostQuizScoreAsync(SubmitQuizScore submitQuizScore)
        {
            try
            {

                using(SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using(SqlCommand sqlCommand = new SqlCommand("submituserquiz", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@userid", submitQuizScore.UserID);
                        sqlCommand.Parameters.AddWithValue("@quizid", submitQuizScore.QuizID);
                        sqlCommand.Parameters.AddWithValue("@score", submitQuizScore.Score);

                        int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();

                        if(rowsAffected > 0)
                        {
                            return new ResponseModel
                            {
                                Message = "Success",
                                Status = 200,
                                Data = null
                            };
                        }
                        else
                        {
                            return new ResponseModel
                            {
                                Message = "Failed",
                                Status = 500,
                                Data = null
                            };
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Message = "An error occurred",
                    Status = 500,
                    Data = ex.Message
                };
            }






        }



        





















            }
        }
    

