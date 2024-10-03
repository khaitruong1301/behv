using FranchiseProject.API;
using FranchiseProject.API.Middlewares;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Infrastructures;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
var configuration = builder.Configuration.Get<AppConfiguration>();

// Register services
builder.Services.AddInfrastructuresService("Server=103.97.125.205,1433; User Id=khainetcore; Password=Cybersoft2109@; Database=Franchise; TrustServerCertificate=True; MultipleActiveResultSets=True");

builder.Services.AddWebAPIService();
builder.Services.AddAuthenticationServices(configuration);
builder.Services.AddStackExchangeRedisCache(options => options.Configuration = configuration.RedisConfiguration);
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>(); // Đăng ký CustomUserIdProvider


// Đăng ký AppDbContext với ConnectionString từ cấu hình
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.DatabaseConnection));

// Định nghĩa chính sách CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", corsBuilder =>
    {
        corsBuilder.WithOrigins("http://localhost:5173") // Thay bằng URL của frontend nếu có
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
    });
});

// Thêm SignalR
builder.Services.AddSignalR();

// Đăng ký cấu hình
builder.Services.AddSingleton(configuration);

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Sử dụng DeveloperExceptionPage cho môi trường phát triển
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Áp dụng chính sách CORS
app.UseCors("AllowSpecificOrigin");

// Sử dụng xác thực và phân quyền
app.UseAuthentication();
app.UseAuthorization();

// Map SignalR hub
app.MapHub<NotificationHub>("/notificationHub").RequireCors("AllowSpecificOrigin");

// Map các controllers
app.MapControllers();

app.Run();
