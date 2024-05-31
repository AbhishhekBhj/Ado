using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Webapiwithado.Models;
using Newtonsoft.Json;
using Webapiwithado.DTOs;


namespace Webapiwithado.DataAccess
{
    public class QuizDataAccess
    {
        private readonly string _connectionString;



        public QuizDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }


        //public async Task<ResponseModel> GetQuizContent(int quizId)
        //{

        //    String quizTitle = "";
        //    String quizDescription = "";
        //    DateTime quizDate = DateTime.Now;
        //    List<String> questions = new List<string>();
        //    List<String> options = new List<string>();
        //    String correctAnswer = "";
        //    int questionId = 0;
        //    String questionText;
        //    List<int> optionIds = new List<int>();


        //    try
        //    {
        //        using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
        //        {
        //            await sqlConnection.OpenAsync();

        //            using (SqlCommand sqlCommand = new SqlCommand("getquizobjectbyid", sqlConnection))
        //            {
        //                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
        //                sqlCommand.Parameters.AddWithValue("@quizid", quizId);

        //                using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
        //                {
        //                    if (sqlDataReader.HasRows)


        //                    {

        //                        QuizDTO quizDTO = new QuizDTO();
        //                        OptionDTO optionDTO = new OptionDTO();
        //                        QuestionDTO questionDTO = new QuestionDTO();
        //                        Quiz quiz = new Quiz();

        //                        while (await sqlDataReader.ReadAsync())
        //                        {
        //                            for (int i = 0; i < sqlDataReader.FieldCount; i++)
        //                            {
        //                                quizId = Convert.ToInt32(sqlDataReader["quizid"]);
        //                                quizTitle = sqlDataReader["title"].ToString();
        //                                quizDescription = sqlDataReader["description"].ToString();
        //                                quizDate = Convert.ToDateTime(sqlDataReader["createdat"]);

        //                                questionId = Convert.ToInt32(sqlDataReader["questionid"]);
        //                                questionText= sqlDataReader["questiontext"].ToString();
        //                                if(questions.Contains(questionText) == false)
        //                                {
        //                                    questions.Add(questionText);
        //                                }
        //                                optionIds.Add(Convert.ToInt32(sqlDataReader["optionid"]));

        //                                options.Add(sqlDataReader["optiontext"].ToString());

        //                                if (sqlDataReader["iscorrect"].ToString() == "1")
        //                                {
        //                                    correctAnswer = sqlDataReader["optiontext"].ToString();
        //                                }
        //                                else
        //                                {
        //                                    correctAnswer = "No correct answer found";
        //                                }

        //                                OptionDTO option = new OptionDTO
        //                                {
        //                               OptionID=     optionDTO = Convert.ToInt32(sqlDataReader["optionid"]);
        //                                optionDTO.OptionText = sqlDataReader["optiontext"].ToString();
        //                                optionDTO.IsCorrect = Convert.ToBoolean(sqlDataReader["iscorrect"]);
        //                            };

        //                                questionDTO.QuestionID = Convert.ToInt32(sqlDataReader["questionid"]);
        //                                questionDTO.QuestionText = sqlDataReader["questiontext"].ToString();
        //                                questionDTO.Answers.Add(optionDTO);

        //                                quizDTO.QuizID = Convert.ToInt32(sqlDataReader["quizid"]);
        //                                quizDTO.QuizTitle = sqlDataReader["title"].ToString();
        //                                quizDTO.QuizDescription = sqlDataReader["description"].ToString();
        //                                quizDTO.Questions.Add(questionDTO);
                                        
                                        
                                        

                                       

                                       
        //                            }
                                    
        //                            Console.WriteLine("Quiz object: " + JsonConvert.SerializeObject(quiz));
                                   
        //                        }
        //                        return new ResponseModel
        //                        {
        //                            Data = quizDTO,
        //                            Message = "Success",
        //                            Status = 200
        //                        };



        //                    }

        //                    else
        //                    {
        //                        return new ResponseModel
        //                        {
        //                            Message = "Failed",
        //                            Status = 500,
        //                            Data = "No quiz found with the given ID"
        //                        };
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (SqlException sqlExc)
        //    {
        //        return new ResponseModel
        //        {
        //            Message = "Failed",
        //            Status = 500,
        //            Data = sqlExc.Message
        //        };
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return new ResponseModel
        //        {
        //            Message = "Failed",
        //            Status = 500,
        //            Data = "An error occurred while fetching quiz content: " + ex.Message
        //        };
        //    }
        //}


        public async Task<ResponseModel> GetAllQuizAsync()
        {



            try
            {

                using(SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    var sqlQuery = "Select * from hamroquizapp.dbo.Quiz";


                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection))
                    {
                       using(SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            
                            List<Quiz> quizList = new List<Quiz>();
                            while(await sqlDataReader.ReadAsync())
                            {
                                quizList.Add(
                                    new Quiz
                                    {
                                        QuizID = Convert.ToInt32(sqlDataReader["quizid"]),
                                        QuizTitle = sqlDataReader["title"].ToString(),
                                        QuizDescription = sqlDataReader["description"].ToString(),
                                        QuizDate = Convert.ToDateTime(sqlDataReader["createdat"])
                                    }

                                    );



                                }
                            ResponseModel responseModel = new ResponseModel { 
                            Data = (quizList),
                            Message = "Success",
                            Status = 200
                            };
                            return responseModel;


                        };

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
                throw;
            }
                    
                }
            }
        }
    

