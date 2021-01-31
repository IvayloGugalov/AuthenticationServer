using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using AuthenticationServer.API.Services.Repositories;
using AuthenticationServer.API.Services.TokenGenerators;
using Microsoft.Extensions.Configuration;
using AuthenticationServer.API.Models;

namespace AuthenticationServer.API
{
    public class Startup
    {
        private const string CONFIGURATION_KEY = "Authentication";

        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //Bind the Authentication params to an object
            var authConfiguration = new AuthConfiguration();
            this.configuration.Bind(Startup.CONFIGURATION_KEY, authConfiguration);

            services.AddSingleton(authConfiguration);
            services.AddSingleton<AccessTokenGenerator>();
            services.AddSingleton<IUserRepository, UserRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
