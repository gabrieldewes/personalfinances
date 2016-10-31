using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyApp.Repositories;

namespace MyApp {
    public class Startup {
        public Startup(IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment()){
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddMvc();

            services.AddAuthorization(options => {
                /*options.AddPolicy("User",
                    policy => {
                        policy.RequireAuthenticatedUser();
                        policy.RequireRole("User");
                    });

                options.AddPolicy("Admin",
                    policy => {
                        policy.RequireAuthenticatedUser();
                        policy.RequireRole("Admin");
                    });*/

                options.AddPolicy("Cookie", policy => policy.RequireRole("Cookie"));
            });

            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<IFinancesRepository, FinancesRepository>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
             else {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseStaticFiles();
            app.UseSession();
            
            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationScheme = "Cookie",
                LoginPath = new PathString("/Account/Login"),
                AccessDeniedPath = new PathString("/Account/Forbidden"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseMvc();
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
