using System;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

using AuthenticationServer.API.Models;
using AuthenticationServer.API.Services.Repositories;
using AuthenticationServer.API.Services.TokenGenerators;
using AuthenticationServer.API.Services.TokenValidators;
using AuthenticationServer.API.Services.Authenticators;

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
            services.AddSingleton<TokenGenerator>();
            services.AddSingleton<RefreshTokenGenerator>();
            services.AddSingleton<RefreshTokenValidator>();
            services.AddSingleton<AccessTokenGenerator>();
            services.AddSingleton<Authenticator>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>();


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfiguration.AccessTokenSecretKey)),
                        ValidIssuer = authConfiguration.Issuer,
                        ValidAudience = authConfiguration.Audience,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

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
