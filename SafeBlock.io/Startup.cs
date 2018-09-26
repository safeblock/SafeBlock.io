using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using NetEscapades.AspNetCore.SecurityHeaders;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using SafeBlock.io.Models;
using SafeBlock.io.Settings;
using SignalRChat.Hubs;
using WebMarkupMin.AspNetCore2;
using React.AspNet;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;

namespace SafeBlock.io
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private const string DefaultCulture = "en";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GlobalSettings>(Configuration.GetSection("Global"));
            services.Configure<PostgreSQLSettings>(Configuration.GetSection("PostgreSQL"));
            services.Configure<MailingSettings>(Configuration.GetSection("Mailing"));
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddEntityFrameworkNpgsql().AddDbContext<SafeBlockContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("PostgreSQLDefault"));
            });

            services.AddDbContext<SafeBlockContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SafeBlockContext")));

            services.AddTransient<IUsers, Users>();
            services.AddTransient<IBlog, Blog>();
            services.AddTransient<ISupport, Support>();

            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = "SecuredSession";
                options.Configuration = Configuration.GetConnectionString("Redis");
            });
            services.AddSingleton<IDistributedCache, RedisCache>();

            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                //options.Cookie.Domain = "safeblock.io";
                options.LoginPath = "/account/getting-started/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/account/getting-started/register";
            });

            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = Configuration["Recaptcha:SiteKey"],
                SecretKey = Configuration["Recaptcha:SecretKey"]
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo(DefaultCulture),
                    new CultureInfo("fr")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: DefaultCulture, uiCulture: DefaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

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

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/error/{0}");
                app.UseHttpsRedirection();
            }

            // Définit les entêtes de sécurité
            /*app.UseSecurityHeaders(new HeaderPolicyCollection()
                .AddFrameOptionsDeny()
                .AddXssProtectionBlock()
                .AddContentTypeOptionsNoSniff()
                .AddXssProtectionEnabled()
                .AddReferrerPolicyStrictOriginWhenCrossOrigin()
                .RemoveServerHeader());*/
                /*.AddContentSecurityPolicy(builder =>
                {
                    builder.AddObjectSrc().None();
                    builder.AddFormAction().Self();
                    builder.AddFrameAncestors().None();
                }));*/

            var supportedCultures = new[]
            {
                new CultureInfo(DefaultCulture),
                new CultureInfo("fr"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(DefaultCulture),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            // Fournit un accès à la documentation generé par mkdocs
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Documentation/site/")),
                RequestPath = "/documentation",
                EnableDirectoryBrowsing = false,
                EnableDefaultFiles = true,
                DefaultFilesOptions = { DefaultFileNames = {"index.html"}}
            });

            app.UseCookiePolicy();

            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = new FileExtensionContentTypeProvider()
                {
                    Mappings = { new KeyValuePair<string, string>(".asc", "text/plain") }
                }
            });

            app.UseWebMarkupMin();
            app.UseSession();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
