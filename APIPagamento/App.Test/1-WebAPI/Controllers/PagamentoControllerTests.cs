﻿using API.Controllers;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Output;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Domain.Tests._1_WebAPI
{
    public class PagamentoControllerTests
    {
        private readonly PagamentoController _controller;
        private readonly Mock<IPagamentoService> _AppService = new();

        public PagamentoControllerTests()
        {
            _controller = new PagamentoController(_AppService.Object);
        }

        #region [GET]
        [Trait("Categoria", "PagamentoController")]
        [Fact(DisplayName = "BuscaStatusPagamentoPedido OkResult")]
        public async Task GetClientes_ReturnsOkResult_GetStatusPagamentoPedido()
        {
            // Arrange
            var idPedido = 1;
            var statusPagamento = "Pendente";
            var pagamentoStatusOutput = new PagamentoStatusOutput
            {
                StatusPagamento = statusPagamento
            };

            _AppService.Setup(service => service.GetStatusPagamento(idPedido)).ReturnsAsync(pagamentoStatusOutput);

            // Act
            var actionResult = await _controller.GetStatusPagamentoPedido(idPedido);

            // Assert
            var okResult = Assert.IsType<ActionResult<PagamentoStatusOutput>>(actionResult);
            var model = Assert.IsType<PagamentoStatusOutput>(okResult.Value);
            Assert.Equal(statusPagamento, model.StatusPagamento);

        }

        [Trait("Categoria", "PagamentoController")]
        [Fact(DisplayName = "BuscaStatusPagamentoPedido NoContent")]
        public async Task GetClientes_ReturnsNoContent_GetStatusPagamentoPedido()
        {
            // Arrange
            var idPedido = 1;
            PagamentoStatusOutput pagamentoStatusOutput = null;

            _AppService.Setup(service => service.GetStatusPagamento(idPedido)).ReturnsAsync(pagamentoStatusOutput);

            // Act
            var actionResult = await _controller.GetStatusPagamentoPedido(idPedido);

            // Assert
            Assert.IsType<NoContentResult>(actionResult.Result);
        }

        [Trait("Categoria", "PagamentoController")]
        [Fact(DisplayName = "BuscaStatusPagamentoPedido RetornaErroInterno")]
        public async Task GetClientes_ReturnsInternalServerError_GetStatusPagamentoPedido()
        {
            // Arrange
            var idPedido = 1;

            _AppService.Setup(service => service.GetStatusPagamento(idPedido)).ThrowsAsync(new Exception());

            // Act
            var actionResult = await _controller.GetStatusPagamentoPedido(idPedido);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, objectResult.StatusCode);
        }
        #endregion

        #region [POST]
        [Trait("Categoria", "PagamentoController")]
        [Fact(DisplayName = "ProcessarPagamento Retorna CreatedAtActionResult")]
        public async Task ProcessarPagamento_RetornaCreatedAtActionResult()
        {
            // Arrange
            var pagamentoInput = new Pagamento
            {
                StatusPagamento = "Pendente",
                ValorPagamento = 100.0f,
                DataPagamento = DateTime.Parse("2024-05-17 23:47"),
                MetodoPagamento = "QRCode",
                IdPedido = 1
            };

            var pagamentoOutput = new PagamentoOutput
            {
                ValorPagamento = pagamentoInput.ValorPagamento,
                MetodoPagamento = pagamentoInput.MetodoPagamento,
                StatusPagamento = pagamentoInput.StatusPagamento,
                IdPedido = pagamentoInput.IdPedido
            };

            _AppService.Setup(service => service.ProcessarPagamento(pagamentoInput)).ReturnsAsync(pagamentoOutput);

            // Act
            var actionResult = await _controller.ProcessarPagamento(pagamentoInput);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var model = Assert.IsType<PagamentoOutput>(createdAtActionResult.Value);
            Assert.Equal(pagamentoOutput.IdPagamento, model.IdPagamento);
            Assert.Equal(pagamentoOutput.ValorPagamento, model.ValorPagamento);
            Assert.Equal(pagamentoOutput.MetodoPagamento, model.MetodoPagamento);
            Assert.Equal(pagamentoOutput.StatusPagamento, model.StatusPagamento);
            Assert.Equal(pagamentoOutput.IdPedido, model.IdPedido);
        }

        [Trait("Categoria", "PagamentoController")]
        [Fact(DisplayName = "ProcessarPagamento Retorna BadRequest")]
        public async Task ProcessarPagamento_RetornaBadRequest()
        {
            // Arrange
            var pagamentoInput = new Pagamento
            {
                StatusPagamento = "Pendente",
                ValorPagamento = 100.0f,
                DataPagamento = DateTime.Parse("2024-05-17 23:47"),
                MetodoPagamento = "QRCode",
                IdPedido = 12
            };

            _controller.ModelState.AddModelError("DataPagamento", "DataPagamento é obrigatório");

            // Act
            var actionResult = await _controller.ProcessarPagamento(pagamentoInput);

            // Assert
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Trait("Categoria", "PagamentoController")]
        [Fact(DisplayName = "ProcessarPagamento Retorna BadRequest QuandoPagamentoInputInvalido")]
        public async Task ProcessarPagamento_RetornaBadRequest_QuandoPagamentoInputInvalido()
        {
            // Arrange
            var pagamentoInput = new Pagamento
            {
                StatusPagamento = "Pendente",
                ValorPagamento = 100.0f,
                DataPagamento = DateTime.Parse("2024-05-17 23:47"),
                MetodoPagamento = "QRCode"
            };

            _controller.ModelState.AddModelError("DataPagamento", "DataPagamento é obrigatório");

            // Act
            var actionResult = await _controller.ProcessarPagamento(pagamentoInput);

            // Assert
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }


        [Trait("Categoria", "PagamentoController")]
        [Fact(DisplayName = "ProcessarPagamento RetornaErroInterno")]
        public async Task ProcessarPagamento_RetornaErroInterno()
        {
            // Arrange
            var pagamentoInput = new Pagamento
            {
                StatusPagamento = "Pendente",
                ValorPagamento = 100.0f,
                DataPagamento = DateTime.Parse("2024-05-17 23:47"),
                MetodoPagamento = "QRCode",
                IdPedido = 11
            };

            _AppService.Setup(service => service.ProcessarPagamento(pagamentoInput)).ThrowsAsync(new Exception());

            // Act
            var actionResult = await _controller.ProcessarPagamento(pagamentoInput);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, objectResult.StatusCode);
        }
        #endregion

        #region [GET_QRCODE]
        [Trait("Categoria", "PagamentoController")]
        [Fact(DisplayName = "ObterQRCodePagamento RetornaOkResult")]
        public async Task ObterQRCodePagamento_RetornaOkResult()
        {
            // Arrange
            var idPedido = 1;
            var qrCodeData = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==";
            var qrCodeOutput = new QRCodeOutput
            {
                qr_data = qrCodeData
            };

            _AppService.Setup(service => service.ObterQRCodePagamento(idPedido)).ReturnsAsync(qrCodeOutput);

            // Act
            var actionResult = await _controller.ObterQRCodePagamento(idPedido);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var model = Assert.IsType<QRCodeOutput>(okResult.Value);
            Assert.Equal(qrCodeData, model.qr_data);
        }

        [Trait("Categoria", "PagamentoController")]
        [Fact(DisplayName = "ObterQRCodePagamento RetornaErroInterno")]
        public async Task ObterQRCodePagamento_RetornaErroInterno()
        {
            // Arrange
            var idPedido = 1;

            _AppService.Setup(service => service.ObterQRCodePagamento(idPedido)).ThrowsAsync(new Exception());

            // Act
            var actionResult = await _controller.ObterQRCodePagamento(idPedido);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, objectResult.StatusCode);
        }
        #endregion

    }
}
