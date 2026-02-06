using Microsoft.OpenApi;

namespace API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerInfrastructure(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Product API",
                    Version = "v1"
                });
            });

            return services;
        }

        public static WebApplication UseSwaggerInfrastructure(this WebApplication app)
        {
            // Kiểm tra môi trường ngay trong Extension
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API V1");
                });
            }

            return app;
        }
    }
}
