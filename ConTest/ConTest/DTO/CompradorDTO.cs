namespace ConTest.DTO
{
    public class CompradorDTO
    {
        public double RUTComprador { get; set; }
        public string DvComprador { get; set; }
        public int CantidadCompras { get; set; }
    }

    public class CompradorTotalCompras : CompradorDTO
    {
        public double TotalMontoCompras { get; set; }
    }
}
