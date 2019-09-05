using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using IdeaTree.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace IdeaTree
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Admin"));
            });

            services.AddMvc().AddRazorPagesOptions(options => {
                options.Conventions.AuthorizeFolder("/Admin", "RequireAdministratorRole");
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<IdeaTreeContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("IdeaTreeContext")));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/Home/login";
                options.LogoutPath = "/Home/signout";
                options.AccessDeniedPath = "/Home";
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
            };
            app.UseCookiePolicy(cookiePolicyOptions);
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                
                //routes.MapRoute("deletecomment", "idea/DeleteComment", defaults: new { controller = "Idea", action = "DeleteComment" });
                //routes.MapRoute("postcomment", "idea/PostComment", defaults: new { controller = "Idea", action = "PostComment" });
                routes.MapRoute("voteidea", "idea/vote/{*id}", defaults: new { controller = "Idea", action = "Vote" });
                routes.MapRoute("idea", "idea/{*id}", defaults: new { controller = "Idea", action = "Index" });
                routes.MapRoute("profile", "profile/{*name}", defaults: new { controller = "Home", action = "Profile" });
                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
