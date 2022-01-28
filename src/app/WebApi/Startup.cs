using Akka.Actor;
using Akka.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Persistance;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.RollingFileAlternate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Primitives;
using WebApi.Configuration;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Hubs;
using WebApi.Infrastructure;
using WebApi.Models;
using WebApi.Providers;
using WebApi.Providers.Impl;
using WebApi.Services;
using WebApi.Services.Impl;

namespace WebApi
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("webapi.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"webapi.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            var keyVault = Configuration.GetSection("KeyVault");
            builder.AddAzureKeyVault(
                $"https://{keyVault["Vault"]}.vault.azure.net/",
                keyVault["ClientId"],
                keyVault["ClientSecret"], new DefaultKeyVaultSecretManager());

            Configuration = builder.Build();
            
            var loggerConfiguration =
                new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.RollingFileAlternate(Path.Combine(_env.ContentRootPath, "logs"), "webapi",
                        LogEventLevel.Debug);
                
            if (Configuration["LoggerUrl"] != null)
            {
                loggerConfiguration.WriteTo.Seq(Configuration["LoggerUrl"], LogEventLevel.Debug);
            }

            Log.Logger = loggerConfiguration.CreateLogger();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var settings = new WebSettings();
            Configuration.Bind(settings);

            services.AddSingleton(settings);
            services.AddSingleton(Configuration);
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(settings.ConnectionString));
            services.AddDbContext<DataContext>(options => options.UseSqlServer(settings.ConnectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 10;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                })
              //  .AddSignInManager<ApplicationUser>()
              //  .AddUserManager<ApplicationUser>()
             //   .AddRoleManager<ApplicationUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            AddJsonSerialization(services);

            services.AddSingleton<IRemoteEventPublisher, RemoteEventPublisher>();
            services.AddTransient<IDepositService, DepositService>();
            services.AddTransient<IWithdrawService, WithdrawService>();
            services.AddTransient<IUserIdentityService, UserIdentityService>();
            services.AddTransient<IGameStatisticService, GameStatisticService>();
            services.AddTransient<IGameLogService, GameLogService>();
            services.AddTransient<IReCaptchaValidation, ReCaptchaValidation>();

            services.AddTransient<IAccountService>(provider => new AccountService(
                provider.GetService<IRemoteTransactionManagerProvider>(),
                provider.GetService<IRemoteWavesGatewayProvider>(),
                provider.GetService<IRemoteEventPublisher>())
            {
                DataContext = provider.GetService<DataContext>(),
                UserIdentityService = provider.GetService<IUserIdentityService>(),
                UserManager = provider.GetService<UserManager<ApplicationUser>>()
            });

            services.AddMemoryCache();
            services.AddSession(cfg => { cfg.IdleTimeout = TimeSpan.FromMinutes(settings.Session.Idle); });

            services.AddMvc(
                    opt => { opt.UseCentralRoutePrefix(new RouteAttribute(BaseApiController.BaseRouting)); })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()
                    {
                        CamelCaseText = true
                    });
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });
            

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AppPolicy.Default, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                });
                
                options.AddPolicy(AppPolicy.SystemUser, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(ClaimTypes.Role, AppRoles.SystemUser);
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                });
                
                options.AddPolicy(AppPolicy.GameUser, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(AppRoles.GameUser);
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                });
            });

            
            services.AddAuthentication().AddJwtBearer(
                cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                   // cfg.SaveToken = true;

                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                      
                       RequireExpirationTime = false,
                       ValidIssuer = settings.Tokens.Issuer,
                       ValidAudience = settings.Tokens.Audience,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Tokens.Key)),
                    };

                    cfg.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            StringValues token;
                            
                            if ((context.Request.Path.Value.StartsWith("/chat") || 
                                 context.Request.Path.Value.StartsWith("/balances") ||
                                 context.Request.Path.Value.StartsWith("/deposit") ||
                                 context.Request.Path.Value.StartsWith("/withdraw") ||
                                 context.Request.Path.Value.StartsWith("/chat")) &&
                                context.Request.Query.TryGetValue("hubToken", out token))
                            {
                                context.Token = token;
                            }
                             
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddRouting();
          

            services.AddSingleton(provider =>
            {
                var cfg = File.ReadAllText(Path.Combine(_env.ContentRootPath, "WebEndpoint.hocon"));
                cfg = cfg.Replace("#hostname", settings.AkkaSettings.Hostname);

                var system = ActorSystem.Create("WebEndpoint", ConfigurationFactory.ParseString(cfg));
                return system;
            });
            
           services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin();
            }));
           

            services.AddSignalR(options =>
            {
                options.JsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter
                {
                    CamelCaseText = true
                });
                options.JsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.AddSingleton<IRemoteTransactionManagerProvider, RemoteTransactionManagerProvider>();
            services.AddSingleton<IRemoteUserWithdrawProvider, RemoteUserWithdrawProvider>();
            services.AddSingleton<IRemoteWavesGatewayProvider, RemoteWavesGatewayProvider>();
            services.AddSingleton<IRemoteGameMinefieldProfider, RemoteGameMinefieldProfider>();
            services.AddSingleton<IRemoteChatHubProvider, RemoteChatHubProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            var settings = services.GetService<WebSettings>();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();
            //app.UseSimpleCors();
            app.UseCors("CorsPolicy");
            
            app.UseSignalR(routes =>
            {
                routes.MapHub<BalancesHub>("balances");
                routes.MapHub<DepositHub>("deposit");
                routes.MapHub<WithdrawHub>("withdraw");
                routes.MapHub<ChatHub>("chat");
            });

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMvc();

            RunIdentityMigrations(app);
            CreateRoles(app);
            CreateSystemUser(app, settings.Notifications);
        }

        private static void RunIdentityMigrations(IApplicationBuilder app)
        {
            try
            {
                var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                    {
                        db.Database.Migrate();    
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error durring migration initialization", ex);
            }

        }

        private static void CreateRoles(IApplicationBuilder app)
        {
            try
            {
                var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    var gameUserRole = roleManager.FindByNameAsync(AppRoles.GameUser).Result;
                    if (gameUserRole == null)
                    {
                        gameUserRole = new IdentityRole(AppRoles.GameUser);
                        roleManager.CreateAsync(gameUserRole).Wait();
                    }

                    var systemUserRole = roleManager.FindByNameAsync(AppRoles.SystemUser).Result;
                    if (systemUserRole == null)
                    {
                        systemUserRole = new IdentityRole(AppRoles.SystemUser);
                        roleManager.CreateAsync(systemUserRole).Wait();
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error("Error durring role initialization", ex);
            }
        }

        private static async void CreateSystemUser(IApplicationBuilder app, Notifications notification)
        {
            try
            {
                var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var user = await userManager.FindByNameAsync(notification.UserName);
                    if (user == null)
                    {
                        var notificationUser = new ApplicationUser
                        {
                            UserName = notification.UserName
                        };
                        
                        var createResult = await userManager.CreateAsync(notificationUser, notification.UserPassword);
                        if (createResult.Succeeded)
                        {
                            user = await userManager.FindByNameAsync(notification.UserName);
                            await userManager.AddToRoleAsync(user, AppRoles.SystemUser);
                        }
                        else
                        {
                            Log.Error($"Failed to create ${notification.UserName}", String.Join(",", createResult.Errors));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error("Error system user creation", ex);
            }
        }
        
        private static void AddJsonSerialization(IServiceCollection services)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new SignalRContractResolver()
            };
            var serializer = JsonSerializer.Create(jsonSettings);
            services.Add(new ServiceDescriptor(typeof(JsonSerializer),
                provider => serializer,
                ServiceLifetime.Transient));

        }
    }
}


/*
 balance akka wysyla wiadomosc do proxyaktora
 new Microsoft.AspNetCore.SignalR.HubContext<BalancesHub>(new DefaultHubLifetimeManager<BalancesHub>())
                .Clients.User("sdfdf").InvokeAsync("sfdfdg", 234);

system dziala w klastrze. webapi(client) (role) + singleton server

  */