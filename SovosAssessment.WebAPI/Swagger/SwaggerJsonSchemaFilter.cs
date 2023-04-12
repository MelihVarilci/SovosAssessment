using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SovosAssessment.WebAPI.Swagger
{
    public class SwaggerJsonSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var jsonMediaType = schema.Properties
                        .FirstOrDefault(p => p.Value.Type == "application/json");

            if (jsonMediaType.Key != null)
            {
                schema.Properties.Clear();
                schema.Properties.Add(jsonMediaType);
            }
            else
            {
                schema.Properties.Clear();
            }
        }
    }
}
