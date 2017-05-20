using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using TumblrTruck.DB;
using TumblrTruck.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace TumblrTruck.ConsoleApp
{
    class Program
    {
        public static IConfigurationRoot Configuration;

        static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(environment))
                environment = "Development";
            
            //config
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", optional: true);

            Configuration = builder.Build();

            var serviceProvider = new ServiceCollection()
               .Configure<TumblrConfig>(Configuration.GetSection("Tumblr"))
               .AddDbContext<TumblrDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqliteConnection")))
               .AddTransient<TumblrTruck>()
               .BuildServiceProvider();

            //serviceProvider.GetService<TumblrTruck>().Run();
            using (var truck = serviceProvider.GetService<TumblrTruck>())
            {
                truck.Run();
            }

            // var _config = new TumblrConfig
            // {
            //     ApiKey = Configuration["Tumblr:ApiKey"],
            //     Hostname = Configuration["Tumblr:Hostname"]
            // };

            // var optionsBuilder = new DbContextOptionsBuilder<TumblrDbContext>();
            // optionsBuilder.UseSqlite(Configuration.GetConnectionString("SqliteConnection"));

            // using (var context = new TumblrDbContext(optionsBuilder.Options))
            // {
            //     var truck = new TumblrTruck(context, _config);
            //     truck.Run();
            // }
                
        }
    }

    //https://docs.microsoft.com/en-us/ef/core/miscellaneous/configuring-dbcontext#using-idbcontextfactorytcontext
    public class BloggingContextFactory : IDbContextFactory<TumblrDbContext>
    {
       public TumblrDbContext Create(DbContextFactoryOptions options)
       {
           var builder = new DbContextOptionsBuilder<TumblrDbContext>();
           builder.UseSqlite("Data Source=TumblrLocalDB.db");

           return new TumblrDbContext(builder.Options);
       }
    }
}