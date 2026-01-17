using Microsoft.OpenApi.Models;
using Asp.Versioning.ApiExplorer;

namespace Presentation.Extensions
{
    public static class SwaggerExtension
    {
        public static void AddSwaggerExtension(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new OpenApiInfo { Title = "HCRM API", Version = description.GroupName });
                }
            });
        }
    }
}