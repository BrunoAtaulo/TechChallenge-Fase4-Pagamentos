using Domain.Entities;
using Domain.Entities.Output;
using Domain.Interfaces;
using Newtonsoft.Json;
using System.Transactions;

namespace Application.Interfaces
{
    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoRepository _pagamentoRepository;


        private readonly string _baseUrlMercadoPago = Environment.GetEnvironmentVariable("MERCADO_PAGO_BASE_URL");
        private readonly string _pathCriarOrdemMercadoPago = Environment.GetEnvironmentVariable("MERCADO_PAGO_CRIAR_QR_ORDER_PATH");
        private readonly string _pathConsultarOrdemMercadoPago = Environment.GetEnvironmentVariable("MERCADO_PAGO_CONSULTAR_QR_ORDER_PATH");
        private readonly string _authorizationMercadoPago = Environment.GetEnvironmentVariable("MERCADO_PAGO_AUTHORIZATION");
        private readonly int _sponsorIdMercadoPago = Convert.ToInt32(Environment.GetEnvironmentVariable("MERCADO_PAGO_SPONSOR_ID"));
        private readonly string _endpointWebhook = Environment.GetEnvironmentVariable("WEBHOOK_ENDPOINT");
        private readonly HttpClient _httpClient;

        public PagamentoService(IPagamentoRepository pagamentoRepository,

                                HttpClient httpClient
                                )
        {
            _pagamentoRepository = pagamentoRepository;

            _httpClient = httpClient;
        }


        public async Task<PagamentoStatusOutput?> GetStatusPagamento(int idPedido)
        {
            var pagamentoStatus = await _pagamentoRepository.GetPagamentoByIdPedido(idPedido);

            if (pagamentoStatus == null)
                return null;

            return new PagamentoStatusOutput
            {
                StatusPagamento = pagamentoStatus.StatusPagamento,
            };
        }

        public async Task<PagamentoOutput> ProcessarPagamento(PagamentoInput pagamentoInput)
        {


            var novoPagamento = await _pagamentoRepository.PostPagamento(pagamentoInput);


            var pagamentoOutput = new PagamentoOutput
            {

                ValorPagamento = novoPagamento.ValorPagamento,
                MetodoPagamento = novoPagamento.MetodoPagamento,
                StatusPagamento = novoPagamento.StatusPagamento,
                IdPedido = novoPagamento.IdPedido
            };

            return pagamentoOutput;
        }

        public async Task<QRCodeOutput?> ObterQRCodePagamento(int idPedido)
        {
            var pagamento = await _pagamentoRepository.GetPagamentoByIdPedido(idPedido);

            if (pagamento == null)
                return null;

            double valorPagamento = (double)Math.Round(pagamento.ValorPagamento, 2);

            var payLoad = new PayloadQRCodeOutput()
            {
                description = string.Format("Pedido_{0}", pagamento.IdPedido),
                external_reference = pagamento.Id.ToString(),
                items =
                [
                    new()
                    {
                        title = string.Format("Pagamento_{0}", pagamento.Id),
                        description = "external_reference >>> Id do Pagamento",
                        unit_price = valorPagamento,
                        quantity = 1,
                        unit_measure = "unit",
                        total_amount = valorPagamento,
                    }
                ],
                notification_url = _endpointWebhook,
                sponsor = new Sponsor()
                {
                    id = _sponsorIdMercadoPago
                },
                title = string.Format("Pedido_{0}_Pagamento_{1}", pagamento.IdPedido, pagamento.Id),
                total_amount = valorPagamento
            };

            var qrCode = await CriarOrdemPagamentoMercadoPago(payLoad);
            return qrCode;
        }

        public async Task<QRCodeOutput> CriarOrdemPagamentoMercadoPago(PayloadQRCodeOutput payLoad)
        {
            var urlQrCode = _baseUrlMercadoPago + _pathCriarOrdemMercadoPago;
            var request = new HttpRequestMessage(HttpMethod.Put, urlQrCode);

            request.Headers.Add("Authorization", _authorizationMercadoPago);

            var json = JsonConvert.SerializeObject(payLoad);
            var content = new StringContent(json, null, "application/json");

            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            var responseJson = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Response JSON: {responseJson}");

            var qrcodeOutput = JsonConvert.DeserializeObject<QRCodeOutput>(responseJson);

            return qrcodeOutput;
        }

        public async Task ProcessarNotificacaoPagamento(long id_merchant_order)
        {
            var ordemPagamento = await ConsultarOrdemPagamentoMercadoPago(id_merchant_order);

            if (ordemPagamento.external_reference != null)
            {
                var pagamento = await _pagamentoRepository.GetPagamentoById(ordemPagamento.external_reference);
                string statusPagamento = string.Empty;

                foreach (var pgto in ordemPagamento.payments)
                {
                    statusPagamento = pgto.status;
                    if (statusPagamento.Equals("approved"))
                        break;
                }

                switch (statusPagamento)
                {
                    case "rejected":
                        statusPagamento = "Rejeitado";
                        break;
                    case "approved":
                        statusPagamento = "Aprovado";
                        break;
                    default:
                        break;
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (!statusPagamento.Equals(pagamento.StatusPagamento))
                    {
                        pagamento.StatusPagamento = statusPagamento;
                        await _pagamentoRepository.PutPagamento(pagamento);
                    }

                    if (ordemPagamento.order_status.Equals("paid"))
                    {
                        string message = JsonConvert.SerializeObject(pagamento);

                    }

                    scope.Complete();
                }
            }
        }

        private async Task<OrdemPagamento> ConsultarOrdemPagamentoMercadoPago(long merchant_order)
        {
            var client = new HttpClient();
            var urlConsultarOrdem = _baseUrlMercadoPago + string.Format(_pathConsultarOrdemMercadoPago, merchant_order);
            var request = new HttpRequestMessage(HttpMethod.Get, urlConsultarOrdem);
            request.Headers.Add("Authorization", _authorizationMercadoPago);
            var response = await client.SendAsync(request);
            var responseJson = await response.Content.ReadAsStringAsync();
            var ordemPagamento = JsonConvert.DeserializeObject<OrdemPagamento>(responseJson);

            return ordemPagamento;
        }
    }
}
