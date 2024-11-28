using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class PagamentoScopedService : IPagamentoScopedService
    {

        private readonly ILogger<PagamentoScopedService> _logger;

        public PagamentoScopedService(ILogger<PagamentoScopedService> logger)
        {

            _logger = logger;
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serviço de escopo de pagamento iniciado.");



            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(1000, cancellationToken);
                }
                catch (OperationCanceledException)
                {

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao receber mensagem.");

                }
            }

            _logger.LogInformation("Serviço de escopo de pagamento encerrado.");
        }
    }
}
