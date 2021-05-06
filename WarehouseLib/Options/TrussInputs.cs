namespace WarehouseLib.Options
{
    public struct TrussInputs
    {
        public TrussInputs(string trussType, int typology, double width, double height, double maxHeight,
            double clearHeight, int baseType, string articulationType, int divisions)
        {
            TrussType = trussType;
            Typology = typology;
            Width = width;
            Height = height;
            MaxHeight = maxHeight;
            ClearHeight = clearHeight;
            BaseType = baseType;
            ArticulationType = articulationType;
            Divisions = divisions;
        }
        public string TrussType { get; set; }
        public int Typology { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double MaxHeight { get; set; }
        public double ClearHeight { get; set; }
        public int BaseType { get; set; }
        public string ArticulationType { get; set; }
        public int Divisions { get; set; }
    }
}