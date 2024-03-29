﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GloomBot
{
    public class Startup
    {
        public static MicrosoftAppCredentials BotCredentials { get; private set; }
        public static string ApiUrl_BattleGoals { get; private set; }
        public static string GloomHavenDBUrl_Events { get; private set; }
        public static string GloomHavenDBUrl_Items { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Add the database
            services.AddSingleton<GloomBot.Bots.GloomBot, GloomBot.Bots.GloomBot>();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, GloomBot.Bots.GloomBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Grab these configuration items for later.
            BotCredentials = new MicrosoftAppCredentials(Configuration["MicrosoftAppId"], Configuration["MicrosoftAppPassword"]);
            ApiUrl_BattleGoals = Configuration["GHApiUrl_BattleGoals"];
            GloomHavenDBUrl_Events = Configuration["GloomhavenDBUrl_EventCards"];
            GloomHavenDBUrl_Items = Configuration["GloomhavenDBUrl_Items"];

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
