using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.StartConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgressApplicationMVP
{
    public class Startup
    {
        private readonly string _connectionType = "";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _connectionType = configuration.GetSection("DataStorageType").GetValue<string>("Current");

            Console.WriteLine(configuration);
            Console.WriteLine(configuration.GetSection("DataStorageType").GetValue<string>("Current"));
            Console.WriteLine(_connectionType);

            if (string.IsNullOrWhiteSpace(configuration.GetSection("DataStorageType").GetValue<string>("Current")))
            {
                _connectionType = "CSV";
                Console.WriteLine("!! No connection specified !!");
                Console.WriteLine(_connectionType);
            }

            if (string.IsNullOrWhiteSpace(_connectionType))
            {
                Console.WriteLine("!! No connection type  !!");
            }

        }  

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (!string.IsNullOrWhiteSpace(_connectionType.ToLower()) && services is not null)
            {
                if (_connectionType.ToLower() == "csv")
                {
                    services.AddTransient<IStartConfig, CSVStartConfig>();
                    services.AddTransient<IDataAccess, CSVDataAccess>();
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
