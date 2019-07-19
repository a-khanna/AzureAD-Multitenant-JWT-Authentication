using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace awesomeappAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            }));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://login.microsoftonline.com/common/";
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidAudiences = new List<string> { "f0d664cd-09bf-42b0-9b96-fdc9cbc77ebf" },
                        ValidateIssuer = false
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = OnAuthenticationFailed,
                        OnTokenValidated = OnTokenValidated
                    };
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Awesome API", Version = "v1" });
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        private Task OnTokenValidated(TokenValidatedContext context)
        {
            var validIssuers = FakeDatabaseService.Tenants.Select(t => $"https://login.microsoftonline.com/{t}/v2.0");
            if (!validIssuers.Contains(context.Principal.Claims.FirstOrDefault(c => c.Type == "iss").Value))
                context.Fail("Issuer Validation failed!");

            return Task.FromResult(0);
        }

        private Task OnAuthenticationFailed(AuthenticationFailedContext context)
        {
            return Task.FromResult(0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseCors("AllowAll");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Awesome API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
