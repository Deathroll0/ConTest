using ConTest.Interfaces;
using System.Text.Json;

namespace ConTest.Model
{
    public class Factura : IFactura
    {
        public double NumeroDocumento { get; set; }
        public double RUTVendedor { get; set; }
        public string DvVendedor { get; set; }
        public double RUTComprador { get; set; }
        public string DvComprador { get; set; }
        public string DireccionComprador { get; set; }
        public double ComunaComprador { get; set; }
        public double RegionComprador { get; set; }
        public double TotalFactura { get; set; }
        public List<DetalleFactura> DetalleFactura { get; set; }

        public List<Factura> ObtenerDatos(IWebHostEnvironment _env)
        {
            string rootPath = _env.ContentRootPath;
            string rutaJson = Path.Combine(rootPath, "JsonBD", "JsonEjemplo.json");
            string lecturaJson = System.IO.File.ReadAllText(rutaJson);
            List<Factura> facturas = JsonSerializer.Deserialize<List<Factura>>(lecturaJson);
            return facturas;
        }

        public List<Factura> calcularTotalFactura(List<Factura> facturas)
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
