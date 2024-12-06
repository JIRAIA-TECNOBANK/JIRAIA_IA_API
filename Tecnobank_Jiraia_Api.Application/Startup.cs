using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using Teckey_KeyReport.Application.Configurations;
using Teckey_KeyReport.CrossCutting.DependencyInjection;
using Tecnobank_Jiraia_Api.Application.Workers;
using Tecnobank_Jiraia_Api.CrossCutting.DependencyInjection;
using Tecnobank_Jiraia_Api.Data.Context;

namespace Tecnobank_Jiraia_Api.Application
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddControllers();

            services.ConfigureApplicationCookie(opts =>
            {
                opts.ExpireTimeSpan = TimeSpan.FromHours(1);
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            ConfigureService.ConfigureDependenciesService(services);
            ConfigureRepository.ConfigureDependenciesRepository(services, Configuration["ConnectionStrings:Regulatorio"]);

            #region Swagger

            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = Configuration["ApiDocTitle"],
                    }
                );

                s.SchemaFilter<SwaggerSchemaFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                s.IncludeXmlComments(xmlPath);
            });

            #endregion

            #region CORS

            services.AddCors(o => o.AddPolicy("AllowAllOrigins", construt =>
                {
                    construt.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                })
            );

            #endregion

            services.AddDistributedMemoryCache();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<RegulatorioContext>();
            services.AddDbContext<RegulatorioContext>(ServiceLifetime.Transient);

            services.AddHostedService<ArquivoCompiladoWorker>();
            services.AddHostedService<ArquivoNormativoWorker>();

            services.Configure<HostOptions>(hostOptions =>
            {
                hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(options => options.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
            );

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseSession();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
                context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=()");

                await next();
            });

            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "API JiraIA - Hackathon");
                s.DefaultModelsExpandDepth(-1);
                s.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                s.RoutePrefix = string.Empty;
            });

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMethods("/sureroute", new string[] { "GET", "HEAD", "OPTIONS", "TRACE" }, async context =>
                {
                    string contentRootPath = env.ContentRootPath;
                    string caminhoArquivoHTML = Path.Combine(contentRootPath, "Assets/sureroute.html");

                    if (File.Exists(caminhoArquivoHTML))
                    {
                        context.Response.ContentType = "text/html";
                        await context.Response.SendFileAsync(caminhoArquivoHTML);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsync("Arquivo HTML não encontrado");
                    }
                });
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
        }
    }
}
