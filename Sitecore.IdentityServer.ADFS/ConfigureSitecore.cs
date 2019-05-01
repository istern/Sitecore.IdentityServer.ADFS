using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Sitecore.Framework.Runtime.Configuration;

namespace Sitecore.IdentityServer.ADFS
{
    public class ConfigureSitecore
    {
        private readonly ILogger<ConfigureSitecore> _logger;
        private readonly AppSettings _appSettings;

        public ConfigureSitecore(ISitecoreConfiguration scConfig, ILogger<ConfigureSitecore> logger)
        {
       
            this._logger = logger;
            this._appSettings = new AppSettings();
            scConfig.GetSection(AppSettings.SectionName);
            scConfig.GetSection(AppSettings.SectionName).Bind((object)this._appSettings.ADFSIdentityProvider);
        }

        public object IdentityServerConstants { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            ADFSIdentityProvider adfsProvider = this._appSettings.ADFSIdentityProvider;
            if (!adfsProvider.Enabled)
                return;
            _logger.LogDebug($"Adding ADFS clientId {adfsProvider.ClientId} Authority {adfsProvider.Authority}  Scheme {adfsProvider.AuthenticationScheme}");
            new AuthenticationBuilder(services).AddOpenIdConnect(adfsProvider.AuthenticationScheme, adfsProvider.DisplayName, (Action<OpenIdConnectOptions>)(options =>
            {

                options.SignInScheme = "idsrv.external";
                options.SignOutScheme = "idsrv";
                options.RequireHttpsMetadata = false;
                options.SaveTokens = true;
                options.Authority = adfsProvider.Authority;
                options.ClientId = adfsProvider.ClientId;
                options.ResponseType = "id_token";
                options.MetadataAddress = adfsProvider.MetadataAddress;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "roles"
                    
                };
                //Added to enable DEBUG to see all claims
                //Can be removed in production
                options.Events = new OpenIdConnectEvents()
                {
                    OnTokenValidated = (context) =>
                    {
                        //This identity include all claims 
                        ClaimsIdentity identity = context.Principal.Identity as ClaimsIdentity;
                        //ADD break POINT to see all the claims, 
                        return Task.FromResult(0);
                    }
                };
            }));

        }
    }
}
