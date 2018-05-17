using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using SafeBlock.Io.Models;
using SafeBlock.Io.Settings;
using SignalRChat.Hubs;
using WebMarkupMin.AspNetCore2;

namespace SafeBlock.Io
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
            services.Configure<VaultSettings>(Configuration.GetSection("Vault"));
            services.Configure<PostgreSQLSettings>(Configuration.GetSection("PostgreSQL"));
            
            services.AddEntityFrameworkNpgsql().AddDbContext<SafeBlockContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("PostgreSQLDefault"));
            });
            
            services.AddMemoryCache();
            
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = "/account/getting-started/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/account/getting-started/register";
            });
            
            services.AddDistributedMemoryCache();
            
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            
            services.AddMvc();

            services.AddWebMarkupMin(options =>
            {
                options.AllowMinificationInDevelopmentEnvironment = true;
                options.AllowCompressionInDevelopmentEnvironment = true;
            })
            .AddHtmlMinification(options =>
            {
                options.MinificationSettings.RemoveRedundantAttributes = true;
                options.MinificationSettings.RemoveHttpProtocolFromAttributes = true;
                options.MinificationSettings.RemoveHttpsProtocolFromAttributes = true;
            })
            .AddHttpCompression();
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
                app.UseExceptionHandler("/home/error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseAuthentication();
            
            app.UseWebMarkupMin();
            
            /*app.UseSignalR(routes => 
            {
                routes.MapHub<NotificationHub>("/chathub");
            });*/

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
