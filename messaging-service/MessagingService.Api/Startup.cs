using MessagingService.Api.Helpers;
using MessagingService.Api.Services;
using MessagingService.DataAccess.Repositories;
using MessagingService.Logging;
using MessagingService.MessagePublisher;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

namespace MessagingService.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCors(options => options.AddDefaultPolicy(b => b.WithOrigins("http//localhost:5000").AllowAnyHeader().AllowAnyMethod()));
            services.AddCors(options => options.AddDefaultPolicy(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MessagingService.Api", Version = "v1" });
            });
            
            services.AddScoped<IMongoClient>(x => new MongoClient(connectionString: Configuration.GetConnectionString("MongoDB")));

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ILogger, ElasticSearchLogger>();
            services.AddScoped<IMessagePublisher, RedisMessagePublisher>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILogger logger)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MessagingService.Api v1"));

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.ConfigureExceptionHandler(logger);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });
        }
    }
}
