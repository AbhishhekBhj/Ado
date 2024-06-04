using System.Data.SqlClient;
using System.Data;
using Webapiwithado.ExternalFunctions;

namespace Webapiwithado.DataAccess
{
    public class MailDataAccess
    {
        private readonly EmailSender _emailSender;
        private readonly string _connectionString;

        public MailDataAccess(EmailSender emailSender, IConfiguration configuration)
        {
            _emailSender = emailSender;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> SendOtpEmailAsync(string email, int otp)
        {
            try
            {
                string subject = "🎉 Verify Your Email Address - Quiz App 🎉";
                string body = $"👋 Hello Quiz Master!\n\n" +
                              "Thank you for signing up for our quiz app! To complete your registration, please verify your email address by using the OTP (One-Time Password) provided below:\n\n" +
                              $"Your OTP: {otp} 🔐\n\n" +
                              "This OTP is valid for the next 10 minutes. Please do not share this OTP with anyone.\n\n" +
                              "If you did not request this email, please ignore it.\n\n" +
                              "Ready to start quizzing? Let's go! 🚀\n\n" +
                              "Best regards,\nHamro Quiz App Team 🌟";

                await _emailSender.SendEmailAsync(email, subject, body);
               
                return true;
            }
            catch (Exception ex)
            {
                // Log exception (if you have a logging mechanism)
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task SetOtpInUserTableAsync(int otp, string email)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                using (SqlCommand sqlCommand = new SqlCommand("SetOtpInUserTable", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@otp", otp);
                    sqlCommand.Parameters.AddWithValue("@Email", email);
                    await sqlCommand.ExecuteNonQueryAsync();
                }
            }
        }

        public int GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }
    }
}
