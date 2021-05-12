namespace WarehouseLib.Options
{
    public struct WarehouseOptions
    {
        public string Typology { get; set; }
        public double Length { get; set; }
        public int PorticoCount { get; set; }
        public bool HasBoundary { get; set; }

        public string RoofBracingType { get; set; }

        public WarehouseOptions(string typology, double length, int porticoCount, bool hasBoundary,
            string roofBracingType)
        {
            Typology = typology;
            Length = length;
            PorticoCount = porticoCount;
            HasBoundary = hasBoundary;
            RoofBracingType = roofBracingType;
        }
    }
}