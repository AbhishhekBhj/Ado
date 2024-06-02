using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Webapiwithado.Models;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Webapiwithado.ExternalFunctions;
namespace Webapiwithado.DataAccess

    
{
    public class UserDataAccess
    {
        private readonly string _connectionString;
        private readonly CreateJWT _createJWT;

        public UserDataAccess(IConfiguration configuration, CreateJWT createJWT)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _createJWT = createJWT;
        }

        public async Task<ResponseModel> CheckandVerifyOTPAsync(OtpVerfifyModel otpVerifyModel)
        {
            if (otpVerifyModel == null)
            {
                return new ResponseModel
                {
                    Message = "Invalid request data",
                    Status = 400,
                    Data = null
                };
            }

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("verifyotp", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@email", otpVerifyModel.Email);  // Corrected parameter name

                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (await sqlDataReader.ReadAsync())
                            {
                                string otp = sqlDataReader.GetString(sqlDataReader.GetOrdinal("otp"));

                                if (otp == otpVerifyModel.Otp)

                                {
                                    sqlDataReader.Close();
                                    using (SqlCommand sqlCommands = new SqlCommand("verifyuser", sqlConnection))
                                    {
                                        sqlCommands.CommandType = CommandType.StoredProcedure;
                                        sqlCommands.Parameters.AddWithValue("@email", otpVerifyModel.Email);  // Ensure the parameter name matches the stored procedure

                                        int rowsAffected = await sqlCommands.ExecuteNonQueryAsync();

                                        if (rowsAffected > 0)
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
                                                Message = "Failed to update user verification status",
                                                Status = 500,
                                                Data = null
                                            };
                                        }
                                    }
                                }
                                else
                                {
                                    return new ResponseModel
                                    {
                                        Message = "Invalid OTP",
                                        Status = 401,
                                        Data = null
                                    };
                                }
                            }
                            else
                            {
                                return new ResponseModel
                                {
                                    Message = "User not found",
                                    Status = 404,
                                    Data = null
                                };
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine(sqlEx.Message);

                return new ResponseModel
                {
                    Message = "Database error occurred",
                    Status = 500,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return new ResponseModel
                {
                    Message = "An error occurred",
                    Status = 500,
                    Data = null
                };
            }
        }





        public async Task<ResponseModel> SetOtpInUserTableAsync(int otp , string email)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("setotpfrommail", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.AddWithValue("@otp", otp);
                        sqlCommand.Parameters.AddWithValue("@email", email);

                        int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
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
                Console.WriteLine(ex.Message);
                ResponseModel responseModel = new ResponseModel
                {
                    Message = "Failed",
                    Status = 500,
                    Data = JsonConvert.SerializeObject(ex.Message)
                };
                return responseModel;
            }
        }   

        public async Task<ResponseModel> CreateNewUser(User user)
        {
            
            try
            {

                using(SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                     await sqlConnection.OpenAsync();

                using(SqlCommand sqlCommand = new SqlCommand("createnewuser", sqlConnection))
                    {

                    String    hashedPassword = PasswordHasher.HashPassword(user.Password);
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.AddWithValue("@username", user.UserName);

                        sqlCommand.Parameters.AddWithValue("@password", hashedPassword);
                        sqlCommand.Parameters.AddWithValue("@email", user.Email);
                        sqlCommand.Parameters.AddWithValue("@photo", user.Photo);

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
                Console.WriteLine(ex.Message);
                ResponseModel responseModel = new ResponseModel
                {
                    Message = "Failed",
                    Status = 500,
                    Data = JsonConvert.SerializeObject(ex.Message)
                };
                return responseModel;
            }
        }


        public async Task<ResponseModel> LoginUser(UserLogin userLogin)
        {
            var response = new ResponseModel();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("userlogin", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@username", userLogin.UserName);

                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (await sqlDataReader.ReadAsync())
                            {
                                string storedPasswordHash = sqlDataReader.GetString(sqlDataReader.GetOrdinal("password"));

                                if (VerifyPassword(userLogin.Password, storedPasswordHash))
                                {
                                    var user = new User
                                    {
                                        Password = storedPasswordHash,
                                        UserId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("userid")),
                                        UserName = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("username")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("UserName")),
                                        Email = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("email")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("Email")),
                                        Photo = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("photo")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("Photo")),
                                           IsVerified = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("isverified")),
                                           SignedInWithGoogle = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("signedinwithgoogle")),
                                    
                                    };

                                    
                                    
                                    var token = _createJWT.CreateJWTToken(userLogin);  // Generate JWT token

                                    response.Message = "Success";
                                    response.Status = 200;
                                    response.Data = new
                                    {
                                        Token = token,
                                        User = user
                                    };
                                }
                                else
                                {
                                    response.Message = "Invalid password";
                                    response.Status = 401;
                                    response.Data = null;
                                }
                            }
                            else
                            {
                                response.Message = "User not found";
                                response.Status = 404;
                                response.Data = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = "Failed";
                response.Status = 500;
                response.Data = ex.Message;
            }

            return response;
        }

        public async Task<ResponseModel> UpdateUserProfileAsync(User user)
        {

            try
            {
                using(SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using(SqlCommand sqlCommand = new SqlCommand("updateuserdata", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.AddWithValue("@userid", user.UserId);
                        sqlCommand.Parameters.AddWithValue("@username", user.UserName);
                        sqlCommand.Parameters.AddWithValue("@email", user.Email);
                        sqlCommand.Parameters.AddWithValue("@password", user.Password);
                        sqlCommand.Parameters.AddWithValue("@photo", user.Photo);
                        int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();


                    }

                    return new ResponseModel
                    {
                        Message = "Success",
                        Status = 200,
                        
                    };

                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ResponseModel responseModel = new ResponseModel
                {
                    Message = "Failed",
                    Status = 500,
                    Data = JsonConvert.SerializeObject(ex.Message)
                };
                return responseModel;
            }

        }

        public async Task<ResponseModel> GetUserDataAsync(int userId)
        {
            User user = new User();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();
                    string query = "SELECT UserId, UserName, Email, Password, Photo FROM hamroquizapp.dbo.[User] WHERE UserId = @UserId";

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (await sqlDataReader.ReadAsync())
                            {
                                user.UserId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("UserId"));
                                user.UserName = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("UserName")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("UserName"));
                                user.Email = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("Email")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("Email"));
                                user.Password = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("Password")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("Password"));
                                user.Photo = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("Photo")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("Photo"));

                                ResponseModel responseModel = new ResponseModel
                                {
                                    Message = "Success",
                                    Status = 200,
                                    Data = (user) // Ensure Data is properly serialized
                                };

                                return responseModel;
                            }
                            else
                            {
                                return new ResponseModel
                                {
                                    Message = "User not found",
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
                Console.WriteLine(ex.Message);
                ResponseModel responseModel = new ResponseModel
                {
                    Message = "Failed",
                    Status = 500,
                    Data = JsonConvert.SerializeObject(ex.Message)
                };
                return responseModel;
            }
        }

        public async Task<ResponseModel> DeleteUserDataAsync(int userId)
        {
            try
            {
                using(SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    string query = "DELETE FROM hamroquizapp.dbo.[User] WHERE userid = @userId";



                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@userId", userId);
                        int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
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
                                Message = "User not found",
                                Status = 404,
                                Data = null
                            };
                        }   
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ResponseModel responseModel = new ResponseModel
                {
                    Message = "Failed",
                    Status = 500,
                    Data = JsonConvert.SerializeObject(ex.Message)
                };
                return responseModel;
            }
        
        
        
        
        }



        public async Task<ResponseModel> CreateNewUserAsync (User user)


        {


            try
            {
                using(SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    

                    
                    using(SqlCommand sqlCommand = new SqlCommand("createnewuser", sqlConnection))
                    {

                        sqlCommand.CommandType = CommandType.StoredProcedure;


                        sqlCommand.Parameters.AddWithValue("@username", user.UserName);
                        sqlCommand.Parameters.AddWithValue("@password", user.Password);
                        sqlCommand.Parameters.AddWithValue("@email", user.Email);
                        sqlCommand.Parameters.AddWithValue("@photo", user.Photo);


                        int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
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
                Console.WriteLine(ex.Message);
                ResponseModel responseModel = new ResponseModel
                {
                    Message = "Failed",
                    Status = 500,
                    Data = JsonConvert.SerializeObject(ex.Message)
                };
                return responseModel;
            }
        }


      
        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            // Implement the hashing comparison as described in the earlier example
            return PasswordHasher.VerifyPassword(enteredPassword, storedPasswordHash);
        }

    }
}
