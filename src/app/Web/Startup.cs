using Akka.Actor;
using Akka.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Persistance;
using Serilog;
using System;
using System.IO;
using System.Security.Claims;
using System.Security.Principal;
using Web.Configuration;
using Web.Data;
using Web.Hubs;
using Web.Models;
using Web.Providers;
using Web.Services;
using Web.Services.Impl;

namespace Web
{
  public class Startup
  {
    private readonly IHostingEnvironment _env;
    const string TriggerDepositAt = "TriggerDepositAt";

    public Startup(IHostingEnvironment env)
    {
      _env = env;
      Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .CreateLogger();

      var builder = new ConfigurationBuilder()
        .SetBasePath(_env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

      Configuration = builder.Build();
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      var settings = new WebSettings();
      Configuration.Bind(settings);

      services.AddSingleton(settings);
      services.AddSingleton(Configuration);
      services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(settings.ConnectionString));

      services.AddDbContext<DataContext>(options =>
        options.UseSqlServer(settings.ConnectionString));

      services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
          options.Password.RequiredLength = 10;
          options.Password.RequireLowercase = false;
          options.Password.RequireUppercase = false;
          options.Password.RequireNonAlphanumeric = false;
          options.Password.RequireDigit = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

      AddJsonSerialization(services);

      services.AddSingleton<IEventPublisher, EventPublisher>();
      services.AddTransient<IDepositService, DepositService>();
      services.AddTransient<IWithdrawService, WithdrawService>();
      services.AddTransient<IUserIdentityService, UserIdentityService>();
      services.AddTransient<IStatisticService, StatisticService>();
      services.AddTransient<IGameLogService, GameLogService>();

      services.AddTransient<IAccountService>(provider => new AccountService(
        provider.GetService<ITransactionManagerProvider>(),
        provider.GetService<IWavesGatewayProvider>(),
        provider.GetService<IEventPublisher>())
      {
        DataContext = provider.GetService<DataContext>(),
        UserIdentityService = provider.GetService<IUserIdentityService>(),
        UserManager = provider.GetService<UserManager<ApplicationUser>>()
      });



      services.AddMemoryCache();
      services.AddSession(cfg =>
      {
        cfg.IdleTimeout = TimeSpan.FromMinutes(settings.Session.Idle);
      });

      services.AddMvc(options =>
        {
          options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        })
        .AddJsonOptions(options =>
        {
          options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()
          {
            CamelCaseText = true
          });
          options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        });

      services.AddAntiforgery(options =>
      {
        options.Cookie.Name = "XSRF-TOKEN";
        options.Cookie.HttpOnly = false;
      });

      services.Configure<RazorViewEngineOptions>(options => { });
      services.AddRecaptcha(new RecaptchaOptions
      {
        SiteKey = settings.Recaptcha.SiteKey,
        SecretKey = settings.Recaptcha.SecretKey
      });

      services.AddAuthentication().AddCookie(cfg => cfg.SlidingExpiration = true);
      services.AddAuthorization(options =>
      {
        options.AddPolicy("CanUseHubs", policy => policy.RequireRole("CanUseHubs"));
        options.AddPolicy("User", policy => policy.RequireRole("User"));
      });

      services.AddSingleton(provider =>
      {
        var cfg = File.ReadAllText(Path.Combine(_env.ContentRootPath, "WebEndpoint.hocon"));
        cfg = cfg.Replace("#hostname", settings.AkkaCfg.Hostname);

        var system = ActorSystem.Create("WebEndpoint", ConfigurationFactory.ParseString(cfg));
        return system;
      });

      services.AddSignalR(options =>
      {
        options.JsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter
        {
          CamelCaseText = true
        });
        options.JsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
      });

      services.AddSingleton<ITransactionManagerProvider, TransactionManagerProvider>();
      services.AddSingleton<IUserWithdrawProvider, UserWithdrawProvider>();
      services.AddSingleton<IWavesGatewayProvider, WavesGatewayProvider>();
      services.AddSingleton<IBalanceProvider, BalanceProvider>();
      services.AddSingleton<IGameMinefieldProfider, GameMinefieldProfider>();

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

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
    {
      var settings = services.GetService<WebSettings>();

      if (env.IsDevelopment())
      {
        //app.UseDeveloperExceptionPage();
        //app.UseBrowserLink();
        //app.UseDatabaseErrorPage();
        app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
        {
          HotModuleReplacement = false
        });
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();
      app.UseAuthentication();
      app.UseSession();

      app.Use(async (context, next) =>
      {
        if (context.Request.Query.ContainsKey("serverToken"))
        {
          if (context.Request.Query["serverToken"] == settings.Notifications.ServerToken)
          {
            var identity = new GenericIdentity("NotificationUser");
            identity.AddClaim(new Claim(ClaimTypes.Role, "CanUseHubs"));
            context.User = new ClaimsPrincipal(identity);
          }
        }

        await next();
      });

      //app.Use(async (context, next) =>
      //{
      //    if (context.User.Identity.IsAuthenticated && context.User.IsInRole("User"))
      //    {
      //        var triggerDepositAt = context.Session.Get<DateTime>(TriggerDepositAt);
      //        var depositService = services.GetService<IDepositService>();

      //        if (triggerDepositAt == DateTime.MinValue)
      //        {
      //            context.Session.Set(TriggerDepositAt, DateTime.UtcNow);
      //            await depositService.TriggerDepositAsync(context.User.Identity.Name);
      //        }
      //        else
      //        {
      //            var timeSpan = DateTime.UtcNow - triggerDepositAt;
      //            if (timeSpan.Minutes > settings.Session.Idle)
      //            {
      //                context.Session.Set(TriggerDepositAt, DateTime.UtcNow);
      //                await depositService.TriggerDepositAsync(context.User.Identity.Name);
      //            }
      //        }
      //    }
      //    await next.Invoke();
      //});

      app.UseSignalR(routes =>
      {
        routes.MapHub<BalanceHub>("balance");
        routes.MapHub<DepositHub>("deposit");
        routes.MapHub<WithdrawHub>("withdraw");
      });

      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "default_route",
          template: "{controller}/{action}/{id?}",
          defaults: new
          {
            controller = "Home",
            action = "Index"
          });

        routes.MapSpaFallbackRoute(
          name: "spa-fallback",
          constraints: new {network = "free"},
          defaults: new {controller = "Game", action = "Index"});
      });

      CreateRoles(app);
    }

    private static void CreateRoles(IApplicationBuilder app)
    {
      try
      {
        IServiceScopeFactory scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
          RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

          var userRole = roleManager.FindByNameAsync("User").Result;
          if (userRole == null)
          {
            userRole = new IdentityRole("User");
            var x = roleManager.CreateAsync(userRole).Result;
          }

          var canUseHubsRole = roleManager.FindByNameAsync("CanUseHubs").Result;
          if (canUseHubsRole == null)
          {
            canUseHubsRole = new IdentityRole("CanUseHubs");
            var x = roleManager.CreateAsync(canUseHubsRole).Result;
          }
        }
      }
      catch
      {
        //skip
      }

    }
  }

  public static class SessionExtensions
  {
    public static void Set<T>(this ISession session, string key, T value)
    {
      session.SetString(key, JsonConvert.SerializeObject(value));
    }

    public static T Get<T>(this ISession session, string key)
    {
      var value = session.GetString(key);
      return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
  }
}

