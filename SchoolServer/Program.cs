using SchoolServer.Application.Services;
using SchoolServer.Application.Interfaces.Auth;
using SchoolServer.Application.Interfaces.Repositories;
using SchoolServer.DataAccess.SQLServer.Repositories;
using SchoolServer.DataAccess.SQLServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SchoolServer.Infrastructure.Authentification;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.Design;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddScoped<UsersServices>();
builder.Services.AddScoped<LessonsService>();
builder.Services.AddScoped<TestsService>();
builder.Services.AddScoped<FavouriteService>();
builder.Services.AddSingleton<ResourceServices>();


builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IJWTProvider, JWTProvider>();

builder.Services.Configure<JWTOptions>(configuration.GetSection("JwtOptions"));
builder.Services.Configure<SchoolServer.DataAccess.SQLServer.AuthorizationOptions>(configuration.GetSection("AuthorizationOptions"));

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<ILessonsRepository, LessonsRepository>();
builder.Services.AddScoped<ITestsRepository, TestsRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration.GetSection("JwtOptions").GetValue<string>("SecretKey") ?? ""))
        };
        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies[
                    configuration.GetSection("JwtOptions").GetValue<string>("JWTCookieName") ?? ""];
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
string connectionString = configuration.GetConnectionString("Local");
#if DEBUG
#else
connectionString = configuration.GetConnectionString("Production");
#endif
builder.Services.AddDbContext<SchoolServerDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#if DEBUG
#else
app.UseCookiePolicy(new CookiePolicyOptions
{
    HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});
#endif




app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
