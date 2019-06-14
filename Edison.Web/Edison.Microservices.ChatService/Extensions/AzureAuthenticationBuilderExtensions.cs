﻿using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Edison.Core.Common;

namespace Microsoft.AspNetCore.Authentication
{
    public static class AzureAdServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddAzureAdBearer(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            builder.AddJwtBearer(AuthenticationBearers.AzureAD, options =>
            {
                options.Audience = configuration["AzureAd:ClientId"];
                options.TokenValidationParameters.ValidIssuers = new List<string>()
                {
                    $"{configuration["AzureAd:Instance"]}{configuration["AzureAd:TenantId"]}/v2.0" //For Issuer validation for MSAL v2
                };
                options.Authority = $"{configuration["AzureAd:Instance"]}{configuration["AzureAd:TenantId"]}";
            })
            .AddJwtBearer(AuthenticationBearers.AzureB2C, options =>
            {
                options.Audience = configuration["AzureAdB2CWeb:ClientId"];
                options.Authority = $"{configuration["AzureAdB2CWeb:Instance"]}/{configuration["AzureAdB2CWeb:Domain"]}/{configuration["AzureAdB2CWeb:SignUpSignInPolicyId"]}/v2.0";
            });
            return builder;
        }
    }
}
