using Xunit;
using Moq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using ConTest.Controllers;
using ConTest.Interfaces;
using ConTest.Model;
using Microsoft.AspNetCore.Http;
using ConTest.DTO;
using System.Web.Http.Results;

public class FacturaControllerTests
{
    [Fact]
    public void ObtenerListadoFacturas_PruebaTryCatch()
    {
        var facturaServiceMock = new Mock<IFactura>();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(env => env.EnvironmentName).Returns("Development");
        var controller = new FacturaController(envMock.Object, facturaServiceMock.Object);

        facturaServiceMock.Setup(service => service.ObtenerDatos(It.IsAny<IWebHostEnvironment>())).Throws(new Exception("Error"));

        var result = controller.ObtenerListadoFacturas();

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        Assert.Equal("Error", statusCodeResult.Value);
    }

    [Fact]
    public void ObtenerListadoFacturas_RespuestaCorrecta()
    {
        var facturaServiceMock = new Mock<IFactura>();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(env => env.EnvironmentName).Returns("Development"); 
        var controller = new FacturaController(envMock.Object, facturaServiceMock.Object);

        
        facturaServiceMock.Setup(service => service.ObtenerDatos(It.IsAny<IWebHostEnvironment>())).Returns(new List<Factura>());
        facturaServiceMock.Setup(service => service.calcularTotalFactura(It.IsAny<List<Factura>>()))
                         .Returns((List<Factura> facturas) => facturas);

        var result = controller.ObtenerListadoFacturas();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public void BusquedaPorRut_RespuestaCorrecta()
    {
        
        var facturaServiceMock = new Mock<IFactura>();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(env => env.EnvironmentName).Returns("Development"); 
        var controller = new FacturaController(envMock.Object, facturaServiceMock.Object);

        
        int rutBuscado = 12345;
        List<Factura> facturas = new List<Factura>
        {
            new Factura { RUTComprador = 12345 },
            new Factura { RUTComprador = 67890 },
        };

        facturaServiceMock.Setup(service => service.ObtenerDatos(It.IsAny<IWebHostEnvironment>())).Returns(facturas);

        var result = controller.BusquedaPorRut(rutBuscado);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var facturasEncontradas = Assert.IsAssignableFrom<List<Factura>>(okResult.Value);
        Assert.Single(facturasEncontradas);
        Assert.Equal(rutBuscado, facturasEncontradas[0].RUTComprador);
    }

    [Fact]
    public void BusquedaPorRut_NoEncuentraRespuesta()
    {
        
        var facturaServiceMock = new Mock<IFactura>();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(env => env.EnvironmentName).Returns("Development"); 
        var controller = new FacturaController(envMock.Object, facturaServiceMock.Object);
        
        int rutBuscado = 12345;
        List<Factura> facturas = new List<Factura>
        {
            new Factura { RUTComprador = 67890 },
            new Factura { RUTComprador = 98765 },
        };

        facturaServiceMock.Setup(service => service.ObtenerDatos(It.IsAny<IWebHostEnvironment>())).Returns(facturas);

        var result = controller.BusquedaPorRut(rutBuscado);
        
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"No se encontraron facturas para el RUT del comprador: {rutBuscado}", notFoundResult.Value);
    }

    [Fact]
    public void CompradorMasCompras_RespuestaCorrecta()
    {
        
        var facturaServiceMock = new Mock<IFactura>();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(env => env.EnvironmentName).Returns("Development"); 
        var controller = new FacturaController(envMock.Object, facturaServiceMock.Object);

        
        List<Factura> facturas = new List<Factura>
        {
            new Factura { RUTComprador = 12345, DvComprador = "A" },
            new Factura { RUTComprador = 12345, DvComprador = "A" },
            new Factura { RUTComprador = 67890, DvComprador = "B" },
        };

        facturaServiceMock.Setup(service => service.ObtenerDatos(It.IsAny<IWebHostEnvironment>())).Returns(facturas);

        var result = controller.CompradorMasCompras();
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        var compradorDTO = Assert.IsType<CompradorDTO>(okResult.Value);
        Assert.Equal(12345, compradorDTO.RUTComprador);
        Assert.Equal("A", compradorDTO.DvComprador);
        Assert.Equal(2, compradorDTO.CantidadCompras);
    }

    [Fact]
    public void ListaCompradoresMontoTotal_RespuestaCorrecta()
    {
        var facturaServiceMock = new Mock<IFactura>();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(env => env.EnvironmentName).Returns("Development"); 
        var controller = new FacturaController(envMock.Object, facturaServiceMock.Object);

        List<Factura> facturas = new List<Factura>
        {
            new Factura { RUTComprador = 12345, DvComprador = "A", TotalFactura = 10000},
            new Factura { RUTComprador = 12345, DvComprador = "A", TotalFactura = 20000},
            new Factura { RUTComprador = 67890, DvComprador = "B", TotalFactura = 30000},
        };

        facturaServiceMock.Setup(service => service.ObtenerDatos(It.IsAny<IWebHostEnvironment>())).Returns(facturas);
        facturaServiceMock.Setup(service => service.calcularTotalFactura(It.IsAny<List<Factura>>())).Returns(facturas);

        var result = controller.ListaCompradoresMontoTotal();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var compradoresConMontoTotal = Assert.IsAssignableFrom<List<CompradorTotalCompras>>(okResult.Value);
        Assert.Equal(12345, compradoresConMontoTotal[0].RUTComprador);
        Assert.Equal("A", compradoresConMontoTotal[0].DvComprador);
        Assert.Equal(2, compradoresConMontoTotal[0].CantidadCompras);
        Assert.Equal(30000, compradoresConMontoTotal[0].TotalMontoCompras);
    }

    [Fact]
    public void BusquedaPorComuna_RespuestaCorrecta_ConId()
    {
        
        var facturaServiceMock = new Mock<IFactura>();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(env => env.EnvironmentName).Returns("Development"); 
        var controller = new FacturaController(envMock.Object, facturaServiceMock.Object);

        int comunaId = 1;
        List<Factura> facturas = new List<Factura>
        {
            new Factura { ComunaComprador = 1 },
            new Factura { ComunaComprador = 2 },
            new Factura { ComunaComprador = 1 },
        };

        facturaServiceMock.Setup(service => service.ObtenerDatos(It.IsAny<IWebHostEnvironment>())).Returns(facturas);

        var result = controller.BusquedaPorComuna(comunaId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var facturasPorComuna = Assert.IsAssignableFrom<List<Factura>>(okResult.Value);
        Assert.Equal(comunaId, facturasPorComuna[0].ComunaComprador);
    }

    [Fact]
    public void BusquedaPorComuna_NoEncuentraRespuesta_SinId()
    {
        
        var facturaServiceMock = new Mock<IFactura>();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(env => env.EnvironmentName).Returns("Development"); 
        var controller = new FacturaController(envMock.Object, facturaServiceMock.Object);
        
        int comunaIdNoExistente = 999;
        List<Factura> facturas = new List<Factura>
        {
            new Factura { ComunaComprador = 1 },
            new Factura { ComunaComprador = 2 },
            new Factura { ComunaComprador = 3 },
        };

        facturaServiceMock.Setup(service => service.ObtenerDatos(It.IsAny<IWebHostEnvironment>())).Returns(facturas);

        var result = controller.BusquedaPorComuna(comunaIdNoExistente);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void BusquedaPorComuna_RespuestaCorrecta_IdNulo()
    {
        
        var facturaServiceMock = new Mock<IFactura>();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(env => env.EnvironmentName).Returns("Development"); 
        var controller = new FacturaController(envMock.Object, facturaServiceMock.Object);

        List<Factura> facturas = new List<Factura>
        {
            new Factura { ComunaComprador = 1 },
            new Factura { ComunaComprador = 2 },
            new Factura { ComunaComprador = 1 },
        };

        facturaServiceMock.Setup(service => service.ObtenerDatos(It.IsAny<IWebHostEnvironment>())).Returns(facturas);

        var result = controller.BusquedaPorComuna(null);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var facturasAgrupadasPorComuna = Assert.IsAssignableFrom<Dictionary<double, List<Factura>>>(okResult.Value);
        Assert.Equal(2, facturasAgrupadasPorComuna.Count);
    }

}