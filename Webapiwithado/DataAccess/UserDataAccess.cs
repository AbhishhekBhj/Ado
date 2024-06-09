using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Webapiwithado.Models;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Webapiwithado.ExternalFunctions;
using Webapiwithado.Controllers;
namespace Webapiwithado.DataAccess

    
{
    public class UserDataAccess
    {
        private readonly string _connectionString;
        private readonly CreateJWT _createJWT;
       
        private readonly MailDataAccess mailDataAccess;

        public UserDataAccess(IConfiguration configuration, CreateJWT createJWT, MailDataAccess mailDataAccess)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _createJWT = createJWT;
          
            this.mailDataAccess = mailDataAccess;
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
                        sqlCommand.Parameters.AddWithValue("@email", otpVerifyModel.Email);  

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





        public async Task<bool> SetOtpInUserTableAsync(int otp , string email)
        {

            Console.WriteLine(otp+"otp");
            Console.WriteLine(email+"email inside");
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
                        Console.WriteLine(rowsAffected+"rows affected");

                        if (rowsAffected > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
               
                return false;
            }
        }

        public async Task<ResponseModel> RegisterUserWithGoogle(UserRegisterModel user)
        {
            try
            {
                // Validate input
                if (user == null)
                    return new ResponseModel { Message = "Invalid user data", Status = 400, Data = null };

                // Hash password (you should hash the password before passing it to this method)
                string hashedPassword = PasswordHasher.HashPassword(user.Password);

                // Call stored procedure to register user
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("signupwithgoogle", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@username", user.UserName);
                        sqlCommand.Parameters.AddWithValue("@password", hashedPassword);
                        sqlCommand.Parameters.AddWithValue("@email", user.Email);
                        sqlCommand.Parameters.AddWithValue("@isverified", 1);  // Default value for isverified
                        sqlCommand.Parameters.AddWithValue("@signedinwithgoogle", 1);  // Default value for signedinwithgoogle

                        sqlCommand.Parameters.AddWithValue("@photo", user.Photo);

                        // Execute the query
                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                // User registered successfully, retrieve user data
                                await reader.ReadAsync(); // Move to the first row
                                User registeredUser = new User
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("userid")),
                                    UserName = reader.GetString(reader.GetOrdinal("username")),
                                    Email = reader.GetString(reader.GetOrdinal("email")),
                                    Photo = reader.GetString(reader.GetOrdinal("photo")),
                                    IsVerified = reader.GetInt32(reader.GetOrdinal("isverified")),
                                    SignedInWithGoogle = reader.GetInt32(reader.GetOrdinal("signedinwithgoogle"))
                                    // You can retrieve other user properties if needed
                                };

                                return new ResponseModel { Message = "User registered successfully", Status = 200, Data = registeredUser };
                            }
                            else
                            {
                                // No rows returned, registration failed
                                return new ResponseModel { Message = "Failed to register user", Status = 500, Data = null };
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine(sqlEx.Message);
                // Log SQL exception
                return new ResponseModel { Message = "Database operation failed", Status = 500,  };
            }
            catch (Exception ex)
            {
                // Log generic exception
                return new ResponseModel { Message = "An error occurred", Status = 500,  };
            }
        }

        public async Task<ResponseModel> CreateNewUser(UserRegisterModel user)
        {
            User user2 = new User();
            int otp = GenerateOtp();
            Console.WriteLine(user.UserName + " username");
            Console.WriteLine(user.Email + " email");

            Dictionary<string, dynamic> myDictionary = new Dictionary<string, dynamic>();
            UserLogin userLoginMode = new UserLogin
            {
                UserName = user.UserName,
                Password = user.Password
            };

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("createnewuser", sqlConnection))
                    {
                        string hashedPassword = PasswordHasher.HashPassword(user.Password);
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.AddWithValue("@username", user.UserName);
                        sqlCommand.Parameters.AddWithValue("@password", hashedPassword);
                        sqlCommand.Parameters.AddWithValue("@email", user.Email);

                        int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            bool emailSent = await mailDataAccess.SendOtpEmailAsync(user.Email, otp);
                            Console.WriteLine(emailSent + " email sent");
                            if (emailSent)
                            {
                                bool otpSet = await SetOtpInUserTableAsync(otp, user.Email);
                                Console.WriteLine(otpSet + " otp set");
                                if (otpSet)
                                {
                                    user2 = await GetUserDataByUsernameAsync(user.UserName);
                                    Console.WriteLine(user2.ToString());
                                    if (user2 != null)
                                    {
                                        var tokens = _createJWT.CreateJWTToken(userLoginMode);

                                        string accessToken = tokens["accessToken"];
                                        string refreshToken = tokens["refreshToken"];

                                        // Add user details and tokens to the dictionary
                                        myDictionary["user"] = user2;
                                        myDictionary["accessToken"] = accessToken;
                                        myDictionary["refreshToken"] = refreshToken;

                                        return new ResponseModel
                                        {
                                            Message = "Success",
                                            Status = 200,
                                            Data = myDictionary
                                        };
                                    }
                                    else
                                    {
                                        Console.WriteLine("Failed to retrieve user data");
                                        return new ResponseModel
                                        {
                                            Message = "Failed to retrieve user data",
                                            Status = 500,
                                            Data = null
                                        };
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Failed to set OTP in user table");
                                    return new ResponseModel
                                    {
                                        Message = "Failed to set OTP",
                                        Status = 500,
                                        Data = null
                                    };
                                }
                            }
                            else
                            {
                                Console.WriteLine("Failed to send OTP email");
                                return new ResponseModel
                                {
                                    Message = "Failed to send OTP email",
                                    Status = 500,
                                    Data = null
                                };
                            }
                        }
                        else
                        {
                            Console.WriteLine("Failed to insert new user");
                            return new ResponseModel
                            {
                                Message = "Failed to insert new user",
                                Status = 500,
                                Data = null
                            };
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Log the detailed SQL exception
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                return new ResponseModel
                {
                    Message = "Database operation failed",
                    Status = 500,
                    Data = JsonConvert.SerializeObject(sqlEx.Message)
                };
            }
            catch (Exception ex)
            {
                // Log the generic exception
                Console.WriteLine($"Error: {ex.Message}");
                return new ResponseModel
                {
                    Message = "An error occurred",
                    Status = 500,
                    Data = JsonConvert.SerializeObject(ex.Message)
                };
            }
        }

        private int GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }




        public async Task<ResponseModel> LoginUserWithGoogle(UserLoginGoogle userLoginGoogle)
        {
            var response = new ResponseModel();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("loginwithgoogle", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@email", userLoginGoogle.Email);

                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (await sqlDataReader.ReadAsync())
                            {
                                var user = new User
                                {
                                    UserId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("userid")),
                                    UserName = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("username")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("username")),
                                    Email = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("email")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("email")),
                                    Photo = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("photo")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("photo")),
                                    IsVerified = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("isverified")),
                                    SignedInWithGoogle = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("signedinwithgoogle")),
                                    Otp = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("otp")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("otp"))
                                };

                                var tokens = _createJWT.CreateJWTToken(userLoginGoogle);

                                response.Message = "Success";
                                response.Status = 200;
                                response.Data = new
                                {
                                    Token = tokens,
                                    User = user
                                };
                            }
                            else
                            {
                                // Handle case where user is not found or not signed in with Google
                                response.Message = "User not found or not signed in with Google";
                                response.Status = 404;
                                response.Data = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.Message = "Failed";
                response.Status = 500;
                response.Data = ex.Message;
            }

            return response;
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

                                           Otp = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("otp")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("otp"))
                                    
                                    };

                                    var tokens = _createJWT.CreateJWTToken(userLogin);

                                    if (tokens != null)
                                    {
                                        string accessToken = tokens["accessToken"];
                                        string refreshToken = tokens["refreshToken"];
                                        // Use the tokens here
                                    }

                                    // Generate JWT token

                                    response.Message = "Success";
                                    response.Status = 200;
                                    response.Data = new
                                    {
                                        Token = tokens,
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





        public async Task<User> GetUserDataByUsernameAsync(string username)
        {
            User user = new User();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("getuserdatabyusername", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@username", username);

                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (await sqlDataReader.ReadAsync())
                            {
                                Console.WriteLine("inside");

                                user.UserId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("userid"));
                                user.UserName = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("username")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("username"));
                                user.Email = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("email")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("email"));
                                user.Photo = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("photo")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("photo"));
                                user.IsVerified = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("isverified"));
                                user.SignedInWithGoogle = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("signedinwithgoogle"));
                                user.Otp = sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("otp")) ? "" : sqlDataReader.GetString(sqlDataReader.GetOrdinal("otp"));

                                // Print user data to console for debugging
                                Console.WriteLine($"UserId: {user.UserId}");
                                Console.WriteLine($"UserName: {user.UserName}");
                                Console.WriteLine($"Email: {user.Email}");
                                Console.WriteLine($"Photo: {user.Photo}");
                                Console.WriteLine($"IsVerified: {user.IsVerified}");
                                Console.WriteLine($"SignedInWithGoogle: {user.SignedInWithGoogle}");
                                Console.WriteLine($"Otp: {user.Otp}");
                            }

                            return user;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("Failed to retrieve user data", ex);
            }
        }



        public async Task<bool> UpdateUserProfilePicture(string photoPath, int userId)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("updateuserprofilepicture", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.AddWithValue("@photo", photoPath);
                        sqlCommand.Parameters.AddWithValue("@userid", userId);

                        int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();
                        return rowsAffected > 0;

                    }
                }
            }
            catch (Exception ex)
            {
                return false;
                // Log or handle the exception as needed
                throw new Exception("Failed to update user profile picture", ex);
            }
        }



        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            // Implement the hashing comparison as described in the earlier example
            return PasswordHasher.VerifyPassword(enteredPassword, storedPasswordHash);
        }

    }
}
