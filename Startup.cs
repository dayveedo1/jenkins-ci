using EazyMobile.DAL.Data.Config;
using EazyMobile.DAL.Data.Interfaces;
using EazyMobile.DAL.Data.Models;
using EazyMobile.DAL.Data.Repos;
using EazyMobile.DAL.Data.Security;
using EazyMobile.DAL.Data.ServiceExtension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EazyMobileAPI
{
    public class Startup
    {
        private static readonly string AllowOrigins = "AllowAll";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EazyMobileContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("EazyMobileAPI"))
          );

            services.AddAuthentication();
            services.ConfigureIdentity();
            services.ConfigureJwt(Configuration);

            services.AddCors(cs => {
                cs.AddPolicy(AllowOrigins,
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            //services.AddHttpClient("eChannel", config =>
            //{
            //    config.BaseAddress = new Uri("http://63.250.52.14:5007/");
            //    config.DefaultRequestHeaders.Accept.Clear();
            //    config.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //});

            services.AddScoped<IEazyMobileRepo, EazyMobileRepo>();

            var sendGridConfig = Configuration.GetSection("SendGridEmailOptions").Get<SendGridEmailServiceConfig>();
            services.AddSingleton(sendGridConfig);

            services.AddTransient<ISendGridEmailService, SendGridEmailService>();
            services.AddTransient<IOTP, OTP>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                { Title = "EazyMobileAPI", Version = "v1" });

                //Enabling token based authentication in Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                    },
                        new string[]{}

                    }
                });
            });

            services.AddControllers().AddNewtonsoftJson(
                op => op.SerializerSettings
                        .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                string swaggerJsonPath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonPath}/swagger/v1/swagger.json", "EazyMobileAPI v1");
            });


            app.UseHttpsRedirection();

            app.UseCors("AllowOrigins");

            app.UseRouting();

            //app.Use(async (context, next) =>
            //{
            //    await next();

            //    if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            //    {
            //        await context.Response.WriteAsync("Access Denied");
            //    }
            //});

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
