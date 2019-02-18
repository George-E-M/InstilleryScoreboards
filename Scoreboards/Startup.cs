using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scoreboards.Data;
using Scoreboards.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scoreboards.Data.Models;
using Scoreboards.Hubs;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Web.Http.WebHost;
using System.Web.Http;

namespace Scoreboards
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; } // Original http 502.5
        //public IConfiguration Configuration; // Testing solution. It did not work

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            // Adding DB Context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("SQL_DB_CONNECTION")
                    ));
            // Adding user Identities.
            services.AddIdentity<ApplicationUser, ApplicationRole>(
                options => {
                    options.Stores.MaxLengthForKeys = 128;
                    options.User.AllowedUserNameCharacters = 
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI() // Uses default Account/Manage/_Layout.cshtml
                .AddDefaultTokenProviders();
            
            

            // Injects IUserGame interface into Scoreboards.
            services.AddScoped<IUserGame, UserGameService>();

            // Injects IUserGame interface into Scoreboards.
            services.AddScoped<IApplicationUser, ApplicationUserService>();

            // Injects IGame interface into Scoreboards.
            services.AddScoped<IGame, GameService>();

            // Injects IMonthlyWinners interface into Scoreboards.
            services.AddScoped<IMonthlyWinners, MonthlyWinnersService>();

            // Injects IUpload interface to upload UserProfile pictures
            services.AddScoped<IUpload, UploadService>();

            // Adding EmailSender service to confirm email via SendGrid
            services.AddScoped<IEmailSender, EmailService>();

            ////Add DataSeed Comment out for Deployment
            //services.AddTransient<DataSeed>();

            // Add SignalR services
            services.AddSignalR();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        } 

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
              IApplicationBuilder app
            , IHostingEnvironment env
            //, DataSeed dataSeed // Comment out for Deployment
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //// Seed Default Data // Comment out for Deployment
            //dataSeed.Initiate().Wait();

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // for wwwroot in main project

            app.UseCookiePolicy();

            app.UseAuthentication();

            //Adding SignalR
            app.UseSignalR(route =>
            {
                route.MapHub<ScoreboardsHub>("/scoreboardsHub");
            });

            //app.UseMvc();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "api",
                    template: "api/{controller}/{id}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");  
            });

            
        }
    }
}
