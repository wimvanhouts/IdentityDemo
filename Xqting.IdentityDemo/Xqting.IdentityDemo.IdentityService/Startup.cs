using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Xqting.IdentityDemo.IdentityService
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
            services.AddMvc(); //--> Use MVC is needed to server the login UI for the identity server


            services.AddIdentityServer()
                .AddDeveloperSigningCredential()                            // --> key and certificates and stuff and alike to encrypt things. Must be change in production 
                .AddInMemoryIdentityResources(Config.GetIdentityResources())// --> OpenID uses scopes concept for properties of an identity that must be provided in the token :http://docs.identityserver.io/en/release/quickstarts/3_interactive_login.html#adding-the-ui
                .AddInMemoryApiResources(Config.GetApiResources())          // --> Resource definitions or "scopes" that are defined
                .AddInMemoryClients(Config.GetClients())                    // --> Clients (applications! not users) that can connect to the api because they will have access to one ore more scopes (defined in the APiResources)
                .AddTestUsers(Config.GetUsers());                           // --> Users that can get access to the API. these are the actual people

            //Note from the documentation: AddTestUsers will set things up vor the "resource owner password gran" mechanism
            //THIS MECHANISM IS ONLY INTENDED FOR TRUSTED CLIENT APPLICATION WITH A USER THAT LOGS IN. A BROWSER IS NOT CONSIDERED A TRUSTED CLIENT APPLICATION
            //and as such this way of working is not advised for an angular application for example that runs in the browser
            //http://docs.identityserver.io/en/release/quickstarts/2_resource_owner_passwords.html
            //quote --> The spec recommends using the resource owner password grant only for “trusted” (or legacy) applications.Generally speaking you are typically
            //far better off using one of the interactive OpenID Connect flows when you want to authenticate a user and request access tokens.
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Put the identity server in the pipeline
            app.UseIdentityServer();

            //These are needed because of UI pages for login
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
