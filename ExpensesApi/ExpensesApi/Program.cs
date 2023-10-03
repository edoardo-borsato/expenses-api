using ExpensesApi.Registries;
using Microsoft.Azure.Cosmos;
using ExpensesApi.Settings;
using ExpensesApi.Utility.CosmosDb;
using ExpensesApi.Utility;
using ExpensesApi.Repositories;
using ExpensesApi.Controllers;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

namespace ExpensesApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder
                .AddAzureWebAppDiagnostics()
                .AddConsole()
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information)
                .AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Information)
                .SetMinimumLevel(LogLevel.Debug));

            var builder = WebApplication.CreateBuilder(args);

            var cosmosDbSettings = builder.Configuration.GetSection("CosmosDB").Get<CosmosDb>()!;
            var cosmosClient = new CosmosClient(cosmosDbSettings.AccountEndpoint, cosmosDbSettings.Key);
            var cosmosClientWrapper = new CosmosClientWrapper(cosmosClient);
            var expensesContainer = cosmosClientWrapper.GetContainer(cosmosDbSettings.DatabaseName, cosmosDbSettings.ContainerName);
            var watch = new Watch();
            var repository = new ExpensesRepository(expensesContainer);
            var filterFactory = new FilterFactory();
            var validator = new QueryParametersValidator();
            var registry = new ExpensesRegistry(loggerFactory, repository, filterFactory, watch);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Expenses API", Version = "v1" });

            });

            builder.Services.AddSingleton(_ => loggerFactory);
            builder.Services.AddSingleton<IExpensesRegistry>(_ => registry);
            builder.Services.AddSingleton<IQueryParametersValidator>(_ => validator);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Expenses API v1"));

            if (app.Environment.IsDevelopment())
            {
                // 
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }
}