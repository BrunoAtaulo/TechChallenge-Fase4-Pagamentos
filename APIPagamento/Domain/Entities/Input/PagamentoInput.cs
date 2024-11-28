using Domain.ValueObjects;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [SwaggerSchemaFilter(typeof(PagamentoInputSchemaFilter))]
    public class PagamentoInput
    {


        [JsonProperty("statusPagamento")]
        public string? StatusPagamento { get; set; } = "Pendente";

        [JsonProperty("valorPagamento")]
        public float ValorPagamento { get; set; }

        [JsonProperty("metodoPagamento")]
        public string? MetodoPagamento { get; set; } = "QRCode";

        [Required(ErrorMessage = "A data do pedido é obrigatória")]
        [DataType(DataType.DateTime, ErrorMessage = "A data do pagamento deve estar no formato aaaa-mm-dd HH:mm")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime DataPagamento { get; set; }

        [JsonProperty("idPedido")]
        [ForeignKey("IdPedido")]
        public int IdPedido { get; set; }

    }
}
