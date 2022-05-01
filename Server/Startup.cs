using CovidInformationPortal.Data;
using CovidInformationPortal.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using AutoMapper;
using Quartz;
using System;
using System.Collections;
using CovidInformationPortal.Services.Utilities;

namespace CovidInformationPortal.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //TEST
            var connectionString = String.Format("Server={0},{1};Database={2};User Id={3};Password={4}",
                Environment.GetEnvironmentVariable("DB_HOST"),
                Environment.GetEnvironmentVariable("DB_PORT"),
                Environment.GetEnvironmentVariable("DB_NAME"),
                Environment.GetEnvironmentVariable("SA_USER"),
                Environment.GetEnvironmentVariable("SA_PASSWORD"));

            services.AddDbContext<CovidInformationContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IInformationService, InformationService>();
            services.AddScoped<IDataGatheringService, DataGatheringService>();
            services.AddAutoMapper(typeof(Startup));
            //services.AddQuartz(q =>
            //{
            //    q.SchedulerId = "Scheduler-Core"; 

            //    // we take this from appsettings.json, just show it's possible
            //    // q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";

            //    // as of 3.3.2 this also injects scoped services (like EF DbContext) without problems
            //    q.UseMicrosoftDependencyInjectionJobFactory();

            //    // or for scoped service support like EF Core DbContext
            //    // q.UseMicrosoftDependencyInjectionScopedJobFactory();

            //    // these are the defaults
            //    q.UseSimpleTypeLoader();
            //    q.UseInMemoryStore();
            //    q.UseDefaultThreadPool(tp =>
            //    {
            //        tp.MaxConcurrency = 10;
            //    });

            //    // quickest way to create a job with single trigger is to use ScheduleJob
            //    // (requires version 3.2)
            //    q.ScheduleJob<GetDataJob2>(trigger => trigger
            //        .WithIdentity("scrape data job")
            //        .StartNow()
            //        .WithCronSchedule("0 * * ? * *")
            //        //.WithCronSchedule("0 0 20,22 ? * *")
            //        .WithDescription("job gathering data")
            //    );
            //});

            //services.AddQuartzHostedService(s =>
            //{
            //    s.WaitForJobsToComplete = true;
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }
            app.UseCors(opp =>
            {
                opp.AllowAnyOrigin();
            });
            //app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
