using Application.Interfaces;
using Application.Services;
using Data.Context;
using Data.Repository;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Globalization;

namespace APIPagamento
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<MongoDBContext>();


            builder.Services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, JsonPatchSample.MyJPIF.GetJsonPatchInputFormatter());
            });


            builder.Services.AddScoped<IPagamentoService, PagamentoService>();


            builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();



            builder.Services.AddHostedService<PagamentoWorkerService>();
            builder.Services.AddScoped<IPagamentoScopedService, PagamentoScopedService>();

            builder.Services.AddControllers().AddNewtonsoftJson();


            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });


            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(typeof(AjustaDataHoraLocal));
            }).AddNewtonsoftJson(options =>
            {

                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            });

            // Saiba mais sobre como configurar o Swagger/OpenAPI em https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            var licenseUrl = Environment.GetEnvironmentVariable("LicenseUrl");

            builder.Services.AddSwaggerGen(
                c =>
                {

                    c.EnableAnnotations();

                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Tech Challenge - Grupo24 - Fase 4",
                        Description = "Documentação dos endpoints da API sobre o uso de microsserviços de pagamento.",
                        Contact = new OpenApiContact() { Name = "Tech Challenge - Grupo 24" },
                        License = new OpenApiLicense() { Name = "MIT License", Url = licenseUrl != null ? new Uri(licenseUrl) : null },
                        Version = "1.0.11"
                    });


                    c.SchemaFilter<PagamentoInputSchemaFilter>();
                }
            );

            var app = builder.Build();


            app.Use((context, next) =>
            {
                context.Response.Headers.ContentType = "application/json; charset=utf-8";
                context.Response.Headers.ContentEncoding = "utf-8";
                context.Response.Headers.ContentLanguage = CultureInfo.CurrentCulture.Name;
                return next();
            });


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                    c.DefaultModelRendering(ModelRendering.Example);
                    c.DisplayOperationId();
                    c.EnableDeepLinking();
                    c.EnableFilter();
                    c.ShowExtensions();
                    c.EnableValidator();

                });

                app.UseReDoc(c =>
                {
                    c.DocumentTitle = "REDOC API Documentation";
                    c.SpecUrl = "/swagger/v1/swagger.json";
                });
            }


            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
