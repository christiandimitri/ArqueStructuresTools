﻿using System;
using WarehouseLib.Articulations;
using WarehouseLib.Connections;

namespace WarehouseLib.Options
{
    public struct TrussInputs
    {
        public ConnectionType TrussType { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double MaxHeight { get; set; }

        public double ClearHeight { get; set; }

        public int BaseType { get; set; }

        public string _articulationType { get; set; }

        public int Divisions { get; set; }

        public PorticoType PorticoType { get; set; }

        public int ColumnsCount { get; set; }
        
        public double FacadeStrapsDistance { get; set; }

        public TrussInputs(ConnectionType trussType, double width, double height, double maxHeight,
            double clearHeight, int baseType, string articulationType, int divisions, PorticoType porticoType,
            int columnsCount, double facadeStrapsDistance)
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
            FacadeStrapsDistance = facadeStrapsDistance;
            if (facadeStrapsDistance <= 0) throw new Exception("Warehouse facade straps cannot have 0 length!!");
            if (height < clearHeight) throw new Exception("The truss height cannot be < than the clear height");
            if (maxHeight < clearHeight) throw new Exception("The truss max height cannot be < than the clear height");
            if (Divisions <= 1) throw new Exception("A normal Truss cannot have a division <= 1");
            if (trussType != ConnectionType.Warren && divisions < 4 &&
                articulationType == ArticulationType.Articulated.ToString())
                throw new Exception(
                    "The 'Articulated' truss types 'Howe', 'Pratt' and 'Warren with studs', cannot have a division count < 4");
            if (columnsCount <= 1)
                throw new Exception("The columns count should be >= 2");
            if (facadeStrapsDistance>height)
            {
                throw new Exception("The facade straps distance can not be > the truss columns height");
            }
        }
    }
}