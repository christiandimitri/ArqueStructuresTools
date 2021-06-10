using System;
using WarehouseLib.Bracings;
using WarehouseLib.Trusses;

namespace WarehouseLib.Options
{
    public struct WarehouseOptions
    {
        public string Typology { get; set; }
        public double Length { get; set; }
        public int PorticoCount { get; set; }
        public bool HasBoundary { get; set; }
        public string RoofBracingType { get; set; }
        public double FacadeCablesThreshold { get; set; }
        public int StAndreCrossCount { get; set; }


        public WarehouseOptions(string typology, double length, int porticoCount, bool hasBoundary,
            string roofBracingType,double facadeCablesThreshold, int stAndreCrossCount)
        {
            if (!Enum.IsDefined(typeof(GeometricalTypology), typology))
                throw new Exception("Warehouse roof typology should be either: Flat, Arch, Monopich, Doublepich");
            if (length <= 0) throw new Exception("Warehouse cannot have 0 length!!");
            if (porticoCount <= 2) throw new Exception("Warehouse cannot have portico count <= 2");
            if (facadeCablesThreshold <= 0.5) throw new Exception("Warehouse cannot have portico count <= 0.5");
            if (!Enum.IsDefined(typeof(RoofBracingType), roofBracingType))
                throw new Exception("Warehouse roof connection should be either be a: Cable, Bracing");

            Typology = typology;
            Length = length;
            PorticoCount = porticoCount;
            HasBoundary = hasBoundary;
            RoofBracingType = roofBracingType;
            FacadeCablesThreshold = facadeCablesThreshold;
            StAndreCrossCount = stAndreCrossCount;
        }
    }
}