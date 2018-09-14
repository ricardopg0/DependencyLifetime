using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetDependencyLifetime
{
    public class TransientService
    {
        public int Counter = 0;
    }

    public class SingletonService
    {
        public int Counter = 0;
    }

    public class ScopedService
    {
        public int Counter = 0;
    }

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
            services.AddTransient<TransientService>();
            services.AddSingleton<SingletonService>();
            services.AddScoped<ScopedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env)
        {
            app.Use( (context, next) =>
            {
                var singleton = context.RequestServices.GetRequiredService<SingletonService>();
                var scoped = context.RequestServices.GetRequiredService<ScopedService>();
                var transient = context.RequestServices.GetRequiredService<TransientService>();

                singleton.Counter++;
                scoped.Counter++;
                transient.Counter++;

                return next();
            });

            app.Run(async context =>
            {
                var singleton = context.RequestServices.GetRequiredService<SingletonService>();
                var scoped = context.RequestServices.GetRequiredService<ScopedService>();
                var transient = context.RequestServices.GetRequiredService<TransientService>();

                singleton.Counter++;
                scoped.Counter++;
                transient.Counter++;

                await context.Response.WriteAsync($"Singleton: {singleton.Counter}\n");
                await context.Response.WriteAsync($"Scoped: {scoped.Counter}\n");
                await context.Response.WriteAsync($"Transient: {transient.Counter}\n");
            });
        }
    }
}
