
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using projectUsers.Data.Context;
using projectUsers.Data.Models;
using System.Security.Claims;
using System.Text;

namespace projectUsers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            #region default
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            #endregion
            // Add services to the container.
            #region DataBase
            var connectionString = builder.Configuration.GetConnectionString("JumiaDb");
            builder.Services.AddDbContext<UsersContext>(
                
                options =>options.UseSqlServer(connectionString) );
            #endregion
            #region Configuring aspnet Identity (UserManager)
            builder.Services.AddIdentity<User, IdentityRole>(
                options => 
                {
               options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    
                }
                ).AddEntityFrameworkStores<UsersContext>();
            #endregion

            #region configuring authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "ahmed";
                options.DefaultChallengeScheme= "ahmed";
            }).AddJwtBearer("ahmed", options => {
                var secretKey = builder.Configuration.GetValue<string>("secretKey");
                var keyInBytes = Encoding.ASCII.GetBytes(secretKey);
                var key = new SymmetricSecurityKey(keyInBytes);
                //   var algorithmAndKey = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                options.TokenValidationParameters = new TokenValidationParameters{
                    IssuerSigningKey = key,
                     ValidateIssuer= false,
                     ValidateAudience= false
                };
            });
            #endregion

            #region configuring authorization
            builder.Services.AddAuthorization(options =>
            {
                // options.AddPolicy("roles", policy => policy.RequireClaim("role","User","Seller"));
                options.AddPolicy("EgyptOnly", policy =>
        policy.RequireClaim("Nationality", "Egyptian", "Saudi")
        .RequireClaim(ClaimTypes.Role, "Admin"));

                options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin")); //just write role in string to fix it
                options.AddPolicy("UserOnly", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
                options.AddPolicy("SellerOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Seller"));

                options.AddPolicy("UserOrSeller", policy => policy.RequireClaim(ClaimTypes.Role, "User" ,"Seller"));
                options.AddPolicy("AdminOrSeller", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "Seller"));
                options.AddPolicy("AdminOrUser", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "User"));
                options.AddPolicy("AdminOrUserOrSeller", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "User","Seller"));

                options.AddPolicy("ElkoFelkol", policy => policy.RequireClaim(ClaimTypes.NameIdentifier, "1"));
            });
            #endregion
            var app = builder.Build();
            #region middlewares
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
            #endregion
        }
    }
}
