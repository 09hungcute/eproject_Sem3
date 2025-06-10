using CareerGuidancePlatform.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5037");

// 🔗 Cấu hình chuỗi kết nối MySQL
var connectionString = "server=127.0.0.1;port=3306;database=career_guidance;user=root;password=;";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ⚙️ Các dịch vụ MVC và Session
builder.Services.AddControllers();        // ✅ Cho Web API
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddAuthorization();

var app = builder.Build();

// 📁 Cho phép dùng file tĩnh (nếu có frontend Razor, HTML, JS,...)
app.UseStaticFiles();

// ❌ Tắt HTTPS redirect nếu không cấu hình HTTPS
// app.UseHttpsRedirection(); // COMMENT để tránh lỗi cảnh báo

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");



// ✅ Cho API Controller
app.MapControllers();

// ✅ Cho MVC Controller (nếu có view)
app.MapDefaultControllerRoute();

app.Run();
