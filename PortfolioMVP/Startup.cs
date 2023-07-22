using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Interfaces;
using ProjectProgressLibrary.Models.Options;
using ProjectProgressLibrary.StartConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioMVP
{
    public class Startup
    {
        private readonly string _connectionType = "";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _connectionType = configuration.GetSection("ApplicationOptions").GetValue<string>("CurrentDataStorage");

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.Configure<ApplicationOptions>(Configuration.GetSection("ApplicationOptions"));
            services.Configure<EnvironmentOptions>(Configuration.GetSection("Environment"));
            services.Configure<PlatformOptions>(Configuration.GetSection("Platform"));

            if (!string.IsNullOrWhiteSpace(_connectionType))
            {
                if (string.Equals(_connectionType, PossibleDataStorage.CSV, StringComparison.CurrentCultureIgnoreCase) && services is not null)
                {
                    services.AddTransient<IStartConfig, CSVStartConfig>();
                    services.AddTransient<IDataAccess, CSVDataAccess>();
                    //Comment
                }
            }
            services.AddRazorPages();
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
