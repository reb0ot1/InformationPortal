using CovidInformationPortal.Services.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;

namespace CovidInformationPortal.Server.Utilities
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase<TDbContext>(
            this IServiceCollection services,
            IConfiguration configuration) where TDbContext : DbContext
        => services
            .AddScoped<DbContext, TDbContext>()
            .AddDbContext<TDbContext>(options => options
            .UseSqlServer(configuration.ApplyConnectionString(),
                          sqlOptions => sqlOptions
                          .EnableRetryOnFailure(
                              maxRetryCount: 10, 
                              maxRetryDelay: TimeSpan.FromSeconds(30), 
                              errorNumbersToAdd: null)));

        public static IServiceCollection AddScheduler(
            this IServiceCollection services)
            => services.AddQuartz(q =>
            {
                q.SchedulerId = "Scheduler-Core";

                // we take this from appsettings.json, just show it's possible
                // q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";

                // as of 3.3.2 this also injects scoped services (like EF DbContext) without problems
                q.UseMicrosoftDependencyInjectionJobFactory();

                // or for scoped service support like EF Core DbContext
                // q.UseMicrosoftDependencyInjectionScopedJobFactory();

                // these are the defaults
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

                // quickest way to create a job with single trigger is to use ScheduleJob
                // (requires version 3.2)
                q.ScheduleJob<GetDataJob2>(trigger => trigger
                    .WithIdentity("scrape data job")
                    .StartNow()
                    //Add this configuration to the appsettings.json
                    .WithCronSchedule("0 0 9,12,18 ? * *")
                    //.WithCronSchedule("0 */5 * ? * *")
                    .WithDescription("job gathering data")
                );
            }).AddQuartzHostedService(s =>
            {
                s.WaitForJobsToComplete = true;
            });
    }
}
