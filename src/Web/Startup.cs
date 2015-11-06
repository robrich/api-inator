namespace ApiInator.Web {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using ApiInator.Web.Controllers;
    using Microsoft.AspNet.Authentication.Cookies;
    using Microsoft.AspNet.Authentication.OAuth;
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Diagnostics.Entity;
    using Microsoft.AspNet.Hosting;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.NodeServices;
    using Microsoft.Data.Entity;
    using Microsoft.Dnx.Runtime;
    using Microsoft.Framework.Configuration;
    using Microsoft.Framework.DependencyInjection;
    using Microsoft.Framework.Logging;
    using Newtonsoft.Json.Linq;
    using Web.Models;
    using Web.Services;

    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            // Setup configuration sources.

            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // This reads the configuration keys from the secret store.
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }
            builder.AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            string connString = this.Configuration["Data:DefaultConnection:ConnectionString"];

            // Add Entity Framework services to the services container.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApiInatorDbContext>(o => o.UseSqlServer(connString));

            // Add Identity services to the services container.
            services.AddAuthentication(options => options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

            // Add MVC services to the services container.
            services.AddMvc();

            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
            // services.AddWebApiConventions();

            services.AddNodeServices();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IInatorRepository, InatorRepository>();
            services.AddTransient<IEndpointRepository, EndpointRepository>();
            services.AddTransient<IUserCurrentService, UserCurrentService>();
            services.AddTransient<ICsharpCompileHelper, CsharpCompileHelper>();
            services.AddTransient<IJavaScriptCompileHelper, JavaScriptCompileHelper>();
            services.AddTransient<InatorConstraint, InatorConstraint>();
            
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IUserRepository userRepository, InatorConstraint inatorConstraint)
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            // Configure the HTTP request pipeline.

            // Add the following to the request pipeline only in development environment.
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage(DatabaseErrorPageOptions.ShowAll);
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // sends the request to the following path or controller action.
                app.UseExceptionHandler("/Home/Error");
            }

            // Add the platform handler to the request pipeline.
            app.UseIISPlatformHandler();

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            app.UseCookieAuthentication(options => {
                options.AutomaticAuthentication = true;
                //options.AutomaticChallenge = true;
                options.LoginPath = new PathString("/login");
            });

            string githubClientId = this.Configuration["GitHubClientId"];
            string githubClientSecret = this.Configuration["GitHubClientSecret"];

            // Add GitHub Authentication
            // http://www.jerriepelser.com/blog/introduction-to-aspnet5-generic-oauth-provider
            // https://github.com/aspnet/Security/blob/dev/samples/SocialSample/Startup.cs
            app.UseOAuthAuthentication(new OAuthOptions {
                AuthenticationScheme = "GitHub",
                DisplayName = "Github",
                ClientId = githubClientId,
                ClientSecret = githubClientSecret,
                CallbackPath = new PathString("/signin-github"),
                AuthorizationEndpoint = "https://github.com/login/oauth/authorize",
                TokenEndpoint = "https://github.com/login/oauth/access_token",
                SaveTokensAsClaims = false,
                UserInformationEndpoint = "https://api.github.com/user",
                // Retrieving user information is unique to each provider.
                Events = new OAuthEvents {
                    OnCreatingTicket = async context => {
                        // Get the GitHub user

                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var githubUser = JObject.Parse(await response.Content.ReadAsStringAsync());

                        var id = githubUser.Value<int>("id");
                        if (id < 1) {
                            throw new ArgumentNullException("id");
                        }
                        var login = githubUser.Value<string>("login");
                        var name = githubUser.Value<string>("name");
                        var avitarUrl = githubUser.Value<string>("avatar_url");

                        User user = userRepository.GetByGitHubId(id) ?? new User { GitHubId = id };
                        user.Login = login;
                        user.Name = name;
                        user.AvitarUrl = avitarUrl;
                        userRepository.Save(user);

                        context.Identity.AddClaim(new Claim(
                            ClaimTypes.NameIdentifier, user.UserId.ToString()
                        ));

                        if (!string.IsNullOrEmpty(name)) {
                            context.Identity.AddClaim(new Claim(
                                "urn:github:name", name,
                                ClaimValueTypes.String, context.Options.ClaimsIssuer
                            ));
                        }
                        if (!string.IsNullOrEmpty(avitarUrl)) {
                            context.Identity.AddClaim(new Claim(
                                "urn:github:avitar", avitarUrl,
                                ClaimValueTypes.String, context.Options.ClaimsIssuer
                            ));
                        }

                        if (user.IsAdmin) {
                            context.Identity.AddClaim(new Claim(
                                ClaimTypes.Role, "admin"
                            ));
                        }
                        
                    }
                }
            });
            
            // Add MVC to the request pipeline.
            app.UseMvc(routes => {

                routes.MapRoute(
                    name: "api",
                    template: "{*pathInfo}",
                    defaults: new {controller = "HandleApi", action="Index"},
                    constraints: new { pathInfo = inatorConstraint } // FRAGILE: matches unused parameter in controller
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );

                // Uncomment the following line to add a route for porting Web API 2 controllers.
                // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
            });
        }
    }
}
