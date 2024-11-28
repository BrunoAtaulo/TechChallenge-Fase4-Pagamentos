using Application.Interfaces;
using Domain.Entities;
using Domain.EntitiesDTO;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Transactions;

namespace Application.Services
{
    public class PagamentoMessageService : IDisposable
    {

        private readonly IPagamentoService _pagamentoService;
        private readonly ILogger<PagamentoMessageService> _logger;

        public PagamentoMessageService(IPagamentoService pagamentoService, ILogger<PagamentoMessageService> logger)
        {

            _pagamentoService = pagamentoService;
            _logger = logger;


        }



        private async Task ReceberMensagemAsync(string mensagem)
        {
            try
            {
                PedidoDTO pedido = JsonSerializer.Deserialize<PedidoDTO>(mensagem);

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    PagamentoInput pagamentoInput = new PagamentoInput
                    {
                        IdPedido = pedido.IdPedido,
                        DataPagamento = DateTime.Now,
                        ValorPagamento = pedido.ValorTotal
                    };

                    await _pagamentoService.ProcessarPagamento(pagamentoInput);

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem recebida.");
                throw; // Rethrow exception para reinfileirar mensagem via PagamentoMessageQueue
            }
        }

        public void Dispose()
        {

            GC.SuppressFinalize(this);
        }
    }
}
