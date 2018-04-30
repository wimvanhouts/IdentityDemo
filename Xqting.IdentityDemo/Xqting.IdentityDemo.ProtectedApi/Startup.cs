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
using Swashbuckle.AspNetCore.Swagger;

namespace Xqting.IdentityDemo.ProtectedApi
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
            services.AddMvcCore(). //--> Attention, by defaul this is AddMvc, which is not the same !! https://offering.solutions/blog/articles/2017/02/07/difference-between-addmvc-addmvcore/
                AddAuthorization().
                AddJsonFormatters(). 
                AddApiExplorer(); //--> Because we added only MVC core, we can normally not format swagger anymore because the mvc core cannto render output by default
                                  //    that is why we added JSON formatters. However, we must also add API explore for the swagger output

            //When authentication is used, one must also provide an authentication schema. IF you do not add this, calling
            //the prorected API will generate a server exception because the server knows it must "authenticate", but it doesn know how or what 
            services.AddAuthentication("Bearer")
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = "http://localhost:54321";
                options.RequireHttpsMetadata = false;

                options.ApiName = "demoapi"; //This must be set to one of the same names as in the API resources that are declared in the identity server
                                          //which are the different scopes. This must be a valid scope and will be matched with the scopes in the token of the user
            });

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("http://localhost:55555")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            //Add swagger documentation generation via swashbuckle package for ASP.NET core, set up the service
            //here to make it available 
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Public/Protected API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("default");

            //--> Add to the pipeline but before MVC is added, order is important to build up he pipeline !!
            app.UseAuthentication();

            app.UseMvc();

            //Add SWagger to the request pipeline, this enables the creation of swagger.json upon request (e.g. for tools that can read 
            //swagger documentation), BUT, this is not the swagger GUI !!
            app.UseSwagger();

            //Also add swagger GUI because it is handy, swagger GUI will be available on http://localhost:12345/swagger/
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Public/Protected API V1");
            });
        }
    }
}
