using Domain.Entities;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Domain.ValueObjects
{
    public class PagamentoInputSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {

            if (context.Type == typeof(PagamentoInput))
            {

                var modeloPagamento = new OpenApiObject
                {

                    ["StatusPagamento"] = new OpenApiString("Pendente"),
                    ["ValorPagamento"] = new OpenApiInteger(0),
                    ["MetodoPagamento"] = new OpenApiString("string"),
                    ["DataPagamento"] = new OpenApiString(DateTime.Now.ToString("yyyy-MM-dd HH:mm")),
                    ["IdPedido"] = new OpenApiInteger(0),
                };

                schema.Example = modeloPagamento;
            }
        }
    }
}
