using Microsoft.AspNetCore.Identity.UI.Services;
using Webapiwithado.DataAccess;
using Webapiwithado.ExternalFunctions;

public class EmailService 
{
   
    private readonly UserDataAccess _userDataAccess;
    private readonly EmailSender _emailSender;

    public EmailService(EmailSender emailSender, UserDataAccess userDataAccess)
    {
        _emailSender = emailSender;
        _userDataAccess = userDataAccess;
    }

    public async Task<bool> SendEmailAsync(string email)
    {
        try
        {
            // Generate a random OTP
            Random random = new Random();
            int otp = random.Next(100000, 999999);

            // Construct the email body
            string subject = "🎉 Verify Your Email Address - Quiz App 🎉";
            string body = $"👋 Hello Quiz Master!\n\n" +
                          "Thank you for signing up for our quiz app! To complete your registration, please verify your email address by using the OTP (One-Time Password) provided below:\n\n" +
                          $"Your OTP: {otp} 🔐\n\n" +
                          "This OTP is valid for the next 10 minutes. Please do not share this OTP with anyone.\n\n" +
                          "If you did not request this email, please ignore it.\n\n" +
                          "Ready to start quizzing? Let's go! 🚀\n\n" +
                          "Best regards,\nHamro Quiz App Team 🌟";

            // Send the email
            await _emailSender.SendEmailAsync(email, subject, body);
            

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    
}
