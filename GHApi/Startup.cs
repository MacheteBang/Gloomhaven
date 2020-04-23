using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using GHApi.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

using GHApi.Bots;

namespace GHApi
{
    public class Startup
    {
        public static string BotAppId { get; private set; }
        public static string BotAppSecret { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddDbContext<GHContext>(opt => opt.UseInMemoryDatabase("GH"));
            services.AddDbContext<GHContext>(opt => opt.UseSqlServer(Configuration["GH_Database"]));

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, GloomBot>();

            // When pushing out nested objects, this ensures the cycle reference is bypassed.
            //https://stackoverflow.com/questions/59199593/net-core-3-0-possible-object-cycle-was-detected-which-is-not-supported
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddControllers();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Grab these configuration items for later.
            BotAppId = Configuration["MicrosoftAppId"];
            BotAppSecret = Configuration["MicrosoftAppPassword"];

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
