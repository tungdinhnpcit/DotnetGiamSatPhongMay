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
                    Title = "GiamSat API",
                    Version = "v1"
                });
            });

            return services;
        }

        public static WebApplication UseSwaggerInfrastructure(this WebApplication app)
        {
            // Kiểm tra môi trường ngay trong Extension
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GiamSat API V1");
                    c.RoutePrefix = "swagger"; // Đảm bảo truy cập qua /swagger
                });
            //}

            return app;
        }
    }
}
