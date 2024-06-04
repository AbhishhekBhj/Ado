using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Webapiwithado.DataAccess;
using Webapiwithado.ExternalFunctions;
using Webapiwithado.Interface;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});



// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<WidgetDataAcess>();
builder.Services.AddSingleton<UserDataAccess>();
builder.Services.AddSingleton<QuizDataAccess>();
builder.Services.AddSingleton<MailDataAccess>();
builder.Services.AddSingleton<CreateJWT>();
builder.Services.AddSingleton<EmailSender>();
builder.Services.AddSingleton<EmailService>();


builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication(); // Add this line to use authentication middleware
app.UseAuthorization(); 

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapDefaultControllerRoute();
});

app.Run();
