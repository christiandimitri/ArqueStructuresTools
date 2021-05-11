namespace WarehouseLib.Options
{
    public struct TrussInputs
    {
        public TrussInputs(string trussType, double width, double height, double maxHeight,
            double clearHeight, int baseType, string articulationType, int divisions, string porticoType)
        {
            TrussType = trussType;
            Width = width;
            Height = height;
            MaxHeight = maxHeight;
            ClearHeight = clearHeight;
            BaseType = baseType;
            ArticulationType = articulationType;
            Divisions = divisions;
            PorticoType = porticoType;
        }

        public string TrussType { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double MaxHeight { get; set; }
        public double ClearHeight { get; set; }
        public int BaseType { get; set; }
        public string ArticulationType { get; set; }
        public int Divisions { get; set; }
        public string PorticoType { get; set; }
    }
}