using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SafeBlock.io.Services;

namespace SafeBlock.io
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region ASCII ART
            Console.WriteLine(@"   _____        __     ____  _            _      _       
  / ____|      / _|   |  _ \| |          | |    (_)      
 | (___   __ _| |_ ___| |_) | | ___   ___| | __  _  ___  
  \___ \ / _` |  _/ _ \  _ <| |/ _ \ / __| |/ / | |/ _ \ 
  ____) | (_| | ||  __/ |_) | | (_) | (__|   < _| | (_) |
 |_____/ \__,_|_| \___|____/|_|\___/ \___|_|\_(_)_|\___/ 
");
            #endregion

            Console.WriteLine("Running SafeBlock.io Application, Founded by Clint Mourlevat.\n");
            
            CreateWebHostBuilder(args).Build().Run();
        }
        
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.SetBasePath(Directory.GetCurrentDirectory());

                    if (!env.IsDevelopment())
                    {
                        Console.Write("Please enter the Vault Token: ");
                        string vaultToken = CliUsing.ReadPassword();
            
                        Console.WriteLine("Thank you ! You must restart the application if you made a mistake.\n");
                        
                        config.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            {"VaultToken", vaultToken}
                        });
                    }
                    
                    config.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddCommandLine(args);
                })
                .UseUrls("http://localhost:1989")
                .UseStartup<Startup>();
        }
    }
}

