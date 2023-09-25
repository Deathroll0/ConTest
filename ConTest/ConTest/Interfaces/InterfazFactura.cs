using ConTest.Model;

namespace ConTest.Interfaces
{
    public interface IFactura
    {
        double NumeroDocumento { get; }
        double RUTVendedor { get; }
        string DvVendedor { get; }
        double RUTComprador { get; }
        string DvComprador { get; }
        string DireccionComprador { get; }
        double ComunaComprador { get; }
        double RegionComprador { get; }
        double TotalFactura { get; }
        List<DetalleFactura> DetalleFactura { get; }

        List<Factura> ObtenerDatos(IWebHostEnvironment _env);
        List<Factura> calcularTotalFactura(List<Factura> facturas);
    }
}

