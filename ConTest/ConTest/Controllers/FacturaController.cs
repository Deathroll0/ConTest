using ConTest.DTO;
using ConTest.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ConTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FacturaController : ControllerBase
    {
        // GET: FacturaController
        /// <summary>
        /// Obtiene todos los elementos.
        /// </summary>
        [HttpGet("ObtenerListadoFacturas")]
        public ActionResult ObtenerListadoFacturas()
        {
            try
            {
                List<Factura> facturas = ObtenerDatos();
                facturas = calcularTotalFactura(facturas);

                string jsonFormateado = JsonSerializer.Serialize(facturas);

                return Ok(jsonFormateado);
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            
        }

        [HttpGet("BusquedaPorRut/{rut}")]
        public ActionResult BusquedaPorRut(int rut)
        {
            try
            {
                List<Factura> facturas = ObtenerDatos();

                List<Factura> facturasEncontradas = facturas.Where(factura => factura.RUTComprador == rut).ToList();

                if (facturasEncontradas.Count == 0)
                {
                    return NotFound($"No se encontraron facturas para el RUT del comprador: {rut}");
                }

                return Ok(facturasEncontradas);
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("CompradorMasCompras")]
        public ActionResult CompradorMasCompras()
        {
            try
            {
                List<Factura> facturas = ObtenerDatos();

                List<CompradorDTO> ordenCompradoresMasCompras = facturas
                    .GroupBy(factura => new { Rut = factura.RUTComprador, Dv = factura.DvComprador })
                    .Select(grupo => new CompradorDTO
                    {
                        RUTComprador = grupo.Key.Rut,
                        DvComprador = grupo.Key.Dv,
                        CantidadCompras = grupo.Count()
                    })
                    .ToList();
                CompradorDTO compradorMasCompras = ordenCompradoresMasCompras.OrderByDescending(comprador => comprador.CantidadCompras).First();
                return Ok(compradorMasCompras);
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("ListaCompradoresMontoTotal")]
        public ActionResult ListaCompradoresMontoTotal()
        {
            try
            {
                List<Factura> facturas = ObtenerDatos();
                facturas = calcularTotalFactura(facturas);
                List<CompradorTotalCompras> compradoresConMontoTotal = facturas
                .GroupBy(factura => new { Rut = factura.RUTComprador, Dv = factura.DvComprador })
                .Select(grupo => new CompradorTotalCompras
                {
                    RUTComprador = grupo.Key.Rut,
                    DvComprador = grupo.Key.Dv,
                    CantidadCompras = grupo.Count(),
                    TotalMontoCompras = grupo.Sum(factura => factura.TotalFactura)
                }).OrderByDescending(comprador => comprador.CantidadCompras).ToList();
                return Ok(compradoresConMontoTotal);
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("BusquedaPorComuna/{id?}")]
        public ActionResult BusquedaPorComuna(int? id)
        {
            try
            {
                List<Factura> facturas = ObtenerDatos();
                if (id.HasValue) 
                {
                    var facturasPorComuna = facturas.Where(factura => factura.ComunaComprador == id).ToList();
                    if (facturasPorComuna.Count == 0)
                    {
                        return NotFound($"No se encontraron facturas para la comuna con ID: {id}");
                    }

                    return Ok(facturasPorComuna);
                }
                else
                {
                    var facturasAgrupadasPorComuna = facturas
                        .GroupBy(factura => factura.ComunaComprador)
                        .ToDictionary(grupo => grupo.Key, grupo => grupo.ToList());
                    return Ok(facturasAgrupadasPorComuna);
                }
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        private readonly IWebHostEnvironment _env;

        public FacturaController(IWebHostEnvironment env)
        {
            _env = env;
        }

        private List<Factura> ObtenerDatos()
        {
            string rootPath = _env.ContentRootPath;

            string rutaJson = Path.Combine(rootPath, "JsonBD", "JsonEjemplo.json");
            string lecturaJson = System.IO.File.ReadAllText(rutaJson);
            List<Factura> facturas = JsonSerializer.Deserialize<List<Factura>>(lecturaJson);
            return facturas;
        }

        private List<Factura> calcularTotalFactura(List<Factura> facturas)
        {
            foreach (var factura in facturas)
            {
                double suma = factura.DetalleFactura.Sum(detalle => detalle.TotalProducto);
                factura.TotalFactura = suma;
            }
            return facturas;
        }
    }

    
}

