﻿using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Output;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace App.Test._2_Application.Services
{
    public class PagamentoServiceTests
    {
        private readonly IPagamentoService _AppServices;
        private readonly Mock<IPagamentoRepository> _Repository = new();

        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();

        public PagamentoServiceTests()
        {
            // Configure variáveis de ambiente
            Environment.SetEnvironmentVariable("MERCADO_PAGO_BASE_URL", "https://api.mercadopago.com");
            Environment.SetEnvironmentVariable("MERCADO_PAGO_CRIAR_QR_ORDER_PATH", "/v1/payments");
            Environment.SetEnvironmentVariable("MERCADO_PAGO_AUTHORIZATION", "Bearer YOUR_ACCESS_TOKEN");
            Environment.SetEnvironmentVariable("MERCADO_PAGO_SPONSOR_ID", "123456");
            Environment.SetEnvironmentVariable("WEBHOOK_ENDPOINT", "https://yourwebhookendpoint.com");

            // Mock HttpClient
            var client = new HttpClient(_httpMessageHandlerMock.Object);

            // Inject Mock Repository and HttpClient
            _AppServices = new PagamentoService(_Repository.Object, client);
        }

        #region [GET]
        [Trait("Categoria", "PagamentoService")]
        [Fact(DisplayName = "GetStatusPagamento Retorna Status Pagamento")]
        public async Task GetStatusPagamento_RetornaPagamentoStatusOutput()
        {
            // Arrange
            var idPedido = 1;
            var pagamentoStatus = "Pendente";
            var pagamento = new Pagamento
            {
                IdPedido = idPedido,
                StatusPagamento = pagamentoStatus
            };

            _Repository.Setup(repo => repo.GetPagamentoByIdPedido(idPedido)).ReturnsAsync(pagamento);

            // Act
            var result = await _AppServices.GetStatusPagamento(idPedido);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pagamentoStatus, result.StatusPagamento);
        }

        [Trait("Categoria", "PagamentoService")]
        [Fact(DisplayName = "GetStatusPagamento Retorna PagamentoStatusOutput Null")]
        public async Task GetStatusPagamento_RetornaPagamentoStatusOutputNull()
        {
            // Arrange
            var idPedido = 1;
            Pagamento pagamento = null;

            _Repository.Setup(repo => repo.GetPagamentoByIdPedido(idPedido)).ReturnsAsync(pagamento);

            // Act
            var result = await _AppServices.GetStatusPagamento(idPedido);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region [POST]
        [Trait("Categoria", "PagamentoService")]
        [Fact(DisplayName = "ProcessarPagamento Retorna Pagamento OK")]
        public async Task ProcessarPagamento_RetornaPagamentoOutput()
        {
            // Arrange
            var pagamentoInput = new PagamentoInput
            {
                IdPedido = 1,
                ValorPagamento = 100.0f,
                MetodoPagamento = "QRCode",
                StatusPagamento = "Pendente"
            };

            var pagamentoOutput = new PagamentoOutput
            {
                ValorPagamento = pagamentoInput.ValorPagamento,
                MetodoPagamento = pagamentoInput.MetodoPagamento,
                StatusPagamento = pagamentoInput.StatusPagamento,
                IdPedido = pagamentoInput.IdPedido
            };

            _Repository.Setup(repo => repo.PostPagamento(pagamentoInput)).ReturnsAsync(pagamentoInput);

            // Act
            var result = await _AppServices.ProcessarPagamento(pagamentoInput);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pagamentoOutput.IdPagamento, result.IdPagamento);
            Assert.Equal(pagamentoOutput.ValorPagamento, result.ValorPagamento);
            Assert.Equal(pagamentoOutput.MetodoPagamento, result.MetodoPagamento);
            Assert.Equal(pagamentoOutput.StatusPagamento, result.StatusPagamento);
            Assert.Equal(pagamentoOutput.IdPedido, result.IdPedido);
        }
        #endregion

        #region [GET_QRCODE]


        [Trait("Categoria", "PagamentoService")]
        [Fact(DisplayName = "ObterQRCodePagamento Retorna QRCodeOutput Null")]
        public async Task ObterQRCodePagamento_RetornaQRCodeOutputNull()
        {
            // Arrange
            var idPedido = 1;
            Pagamento pagamento = null;

            _Repository.Setup(repo => repo.GetPagamentoByIdPedido(idPedido)).ReturnsAsync(pagamento);

            // Act
            var result = await _AppServices.ObterQRCodePagamento(idPedido);

            // Assert
            Assert.Null(result);
        }
        #endregion
    }
}