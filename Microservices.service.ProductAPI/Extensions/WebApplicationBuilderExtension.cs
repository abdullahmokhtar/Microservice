using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Microservices.Services.ProductAPI.Extensions;

public static class WebApplicationBuilderExtension
{
    public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = builder.Configuration["ApiSettings:Issuer"],
                ValidAudience = builder.Configuration["ApiSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["ApiSettings:SecretKey"]))
            };
        });
        return builder;
    }
}
