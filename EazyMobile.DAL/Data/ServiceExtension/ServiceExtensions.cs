using EazyMobile.DAL.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.ServiceExtension
{
   public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            //var builder = services.AddIdentity<User, IdentityRole>(x => x.User.RequireUniqueEmail = true);
            var builder = services.AddIdentity<User, IdentityRole>(x => {
                x.User.RequireUniqueEmail = true;
                // Default Lockout settings.
                //x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                //x.Lockout.MaxFailedAccessAttempts = 3;
                //x.Lockout.AllowedForNewUsers = true;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddEntityFrameworkStores<EazyMobileContext>().AddDefaultTokenProviders();
        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = jwtSettings.GetSection("SKEY").Value;

            services.AddAuthentication(opt =>
            {
                //adding Authentication to the application, which i'm setting the default to JWT
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //check for whatever request the application recieves & verify it is valid
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    //parameters it should use to validate a token it recieves
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //validate the issuer which we set in the appsetting.json
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateActor = false,
                        //validate the Lifesan duration of the token
                        ValidateLifetime = true,
                        //validate the signing key which we created
                        ValidateIssuerSigningKey = true,
                        //the valid issuer is Issuer gotten from the appsetting.json
                        ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                        //Encrypting the issuer signing key
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    };

                    ///
                    options.Events = new JwtBearerEvents()
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            // Ensure we always have an error and error description.
                            if (string.IsNullOrEmpty(context.Error))
                                context.Error = "invalid_token";

                            if (string.IsNullOrEmpty(context.ErrorDescription))
                                context.ErrorDescription = "This request requires a valid JWT access token to be provided";

                            // Add some extra context for expired tokens.
                            if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                var authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
                                context.Response.Headers.Add("x-token-expired", authenticationException.Expires.ToString("o"));
                                context.ErrorDescription = $"The token expired on {authenticationException.Expires.ToString("o")}";
                            }

                            return context.Response.WriteAsync(JsonSerializer.Serialize(new
                            {
                                error = context.Error,
                                error_description = context.ErrorDescription
                            }));

                        }
                    };
                });
        }
    }
}
