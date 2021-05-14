using System;
using WarehouseLib.Articulations;

namespace WarehouseLib.Options
{
    public struct TrussOptions
    {
        public string TrussType { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double MaxHeight { get; set; }

        public double ClearHeight { get; set; }

        public int BaseType { get; set; }

        public string _articulationType { get; set; }

        public int Divisions { get; set; }

        public string PorticoType { get; set; }

        public int ColumnsCount { get; set; }

        public TrussOptions(string trussType, double width, double height, double maxHeight,
            double clearHeight, int baseType, string articulationType, int divisions, string porticoType,
            int columnsCount)
        {
            TrussType = trussType;
            Width = width;
            Height = height;
            MaxHeight = maxHeight;
            ClearHeight = clearHeight;
            BaseType = baseType;
            _articulationType = articulationType;
            Divisions = divisions;
            PorticoType = porticoType;
            ColumnsCount = columnsCount;
            if (trussType != "Warren" && divisions < 4 &&
                articulationType == ArticulationType.Articulated.ToString())
                throw new Exception(
                    "The 'Articulated' truss types 'Howe', 'Pratt' and 'Warren with studs', cannot have a division count < 4");
            // if (divisions <= 1)
            //     throw new Exception("The columns count should be >= 2");
        }
    }
}