using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProgressApplicationMVP.Options;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Interfaces;
using ProjectProgressLibrary.Models.Options;
using ProjectProgressLibrary.StartConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgressApplicationMVP
{
    public class Startup
    {
        private string _connectionType = "";
        private double expectedAppsettingsVersion = 1.3;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Access configuration values here in the ConfigureServices method
            string currentDataStorage = Configuration["ApplicationOptions:CurrentDataStorage"];
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"CurrentDataStorage: {currentDataStorage}");
            Console.ForegroundColor = ConsoleColor.White;

            services.AddLogging();
            services.Configure<ProgressAppInstanceOptions>(Configuration.GetSection("ApplicationOptions"));
            services.Configure<ApplicationOptions>(Configuration.GetSection("ApplicationOptions"));
            services.Configure<EnvironmentOptions>(Configuration.GetSection("Environment"));
            services.Configure<PlatformOptions>(Configuration.GetSection("Platform"));
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSetings"));
            services.Configure<IndexContentOptions>(Configuration.GetSection("IndexContent"));

            double currentAppSettingsVersion = Configuration.GetValue<double>("Environment:AppSettingsVersion");

            CheckAppsettingsVersionMatch(expectedAppsettingsVersion, currentAppSettingsVersion);

            if (!string.IsNullOrWhiteSpace(currentDataStorage) && services is not null)
            {
                if (string.Equals(currentDataStorage, PossibleDataStorage.CSV, StringComparison.OrdinalIgnoreCase))
                {
                    services.AddTransient<IStartConfig, CSVStartConfig>();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Startup => ConfigureServices => Added Service: CSVStartConfig");

                    services.AddTransient<IDataAccess, CSVDataAccess>();

                    Console.WriteLine("Startup => ConfigureServices => Added Service: CSVDataAccess  ");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
                        
            services.AddRazorPages();
        }

        private void CheckAppsettingsVersionMatch(double expectedAppsettingsVersion, double currentAppSettingsVersion)
        {
            if (expectedAppsettingsVersion == currentAppSettingsVersion)
            {

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Startup => ConfigureServices => Version match! {expectedAppsettingsVersion}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Startup => ConfigureServices => Appsettings version issue!: expected: {expectedAppsettingsVersion} -> Current: {currentAppSettingsVersion}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Make sure the appsettings match");
                Console.ForegroundColor = ConsoleColor.White;
                throw new ArgumentException("Appsettings version mis match!");
            }
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
