using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Authorization.Infrastructure;
using Owin;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Web.Http;

[assembly: OwinStartup(typeof(WebAPI_IS4.Startup))]

namespace WebAPI_IS4
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseCors(CorsOptions.AllowAll);
            /*
            var IDSBearerOption = new IdentityServerBearerTokenAuthenticationOptions
            {
                AuthenticationType = "Bearer",
                Authority = "https://localhost:5001",
                //ValidationMode = ValidationMode.Local,
                ValidationMode = ValidationMode.Local,
                RequiredScopes = new[] { "api1" },
                ClientId = "testResource",
                PreserveAccessToken = true,
                
            };

            app.UseIdentityServerBearerTokenAuthentication(IDSBearerOption);
            */


            var IDSBearerOption = new Microsoft.Owin.Security.Jwt.JwtBearerAuthenticationOptions
            {

                TokenValidationParameters = new TokenValidationParameters()
                {
                    //ValidAudience = "https://localhost:5001" ,
                    //ValidIssuer = "testResource" ,
                    //SaveSigninToken =true,
                    RoleClaimType = "role",

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    //ValidateIssuerSigningKey = false,
                    ValidIssuer = "testResource",
                    ValidAudience = "https://localhost:5001",
                    
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_secret_key_12345"))

                },
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active

            };
            app.UseJwtBearerAuthentication(IDSBearerOption);



            
            //Add a policy "Apiscope"
            app.UseAuthorization(opt =>
            {
                opt.AddPolicy("Apiscope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    //policy.RequireClaim("Scope", "api1", "api2");
                    policy.RequireClaim("Scope", "api1");

                });
            }
            );

            //Add a policy "Apiscope2"
            app.UseAuthorization(opt =>
            {
                opt.AddPolicy("Apiscope2", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("Scope", "api2");

                });

            });

            app.UseAuthorization(opt =>
            {
                opt.AddPolicy("adminusers", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("role", "admin");                    

                });

            });

            

            //configure web api
            var config = new HttpConfiguration();

            
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.

            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(IDSBearerOption.AuthenticationType));

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Formatters.Remove(config.Formatters.XmlFormatter);



            //app.UseCors(CorsOptions.AllowAll);
            //app.UseNLog((eventType) => LogLevel.Debug);




            app.UseWebApi(config);
        }
    }
}
