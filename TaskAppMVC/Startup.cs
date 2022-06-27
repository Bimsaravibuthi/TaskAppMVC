using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options=> {
            //        options.LoginPath = "/User/Login";
            //        options.Cookie.Name = "MyCookieAuth";
            //        options.AccessDeniedPath = "/Home/AccessDenied";
            //        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            //    });

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("LoggedUsersOnly", policy => policy
            //       .RequireClaim("User_ID"));
            //});

            services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
            {
                options.LoginPath = "/User/Login";
                options.Cookie.Name = "MyCookieAuth";
                options.AccessDeniedPath = "/Home/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            });

            services.AddAuthorization(options => {
                options.AddPolicy("LoggedUsersOnly", policy => policy
                   .RequireClaim("User_ID"));

                options.AddPolicy("AdminOnly", policy => policy
                    .RequireClaim("User_ID")
                    .RequireClaim("NormalUser", "True")
                    .RequireClaim("Admin", "True"));

                options.AddPolicy("MustBelongToHRDepartment", policy => policy
                    .RequireClaim("Department", "HR"));

                options.AddPolicy("ManagersOnly", policy => policy
                    .RequireClaim("User_ID")
                    .RequireClaim("NormalUser", "True"));
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
