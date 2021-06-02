using System;
using System.Diagnostics;
using Grasshopper.Kernel.Geometry.Delaunay;
using WarehouseLib.Utilities;

namespace WarehouseLib.Profiles
{
    public class WarehouseProfiles
    {
        public string StaticColumnsProfileName;
        public string BoundaryColumnProfileName;
        public string TopBeamsProfileName;
        public string BottomBeamsProfileName;
        public string IntermediateBeamsProfileName;
        public string RoofStrapsProfileName;
        public string FacadeStrapsProfileName;
        public string RoofCablesProfileName;
        public string FacadeCablesProfileName;
        public string RoofBracingProfileName;
        public string ColumnsBracingProfileName;

        public WarehouseProfiles(string staticColumnsProfileName, string boundaryColumnProfileName,
            string topBeamsProfileName,
            string bottomBeamsProfileName,
            string intermediateBeamsProfileName, string roofStrapsProfileName, string facadeStrapsProfileName,
            string roofCablesProfileName, string facadeCablesProfileName, string roofBracingProfileName, string columnsBracingProfileName)
        {
            var catalog = new Catalog().GetCatalog();
            StaticColumnsProfileName = catalog[staticColumnsProfileName].Name;
            BoundaryColumnProfileName = catalog[boundaryColumnProfileName].Name;
            TopBeamsProfileName = catalog[topBeamsProfileName].Name;
            BottomBeamsProfileName = catalog[bottomBeamsProfileName].Name;
            IntermediateBeamsProfileName = catalog[intermediateBeamsProfileName].Name;
            RoofStrapsProfileName = catalog[roofStrapsProfileName].Name;
            FacadeStrapsProfileName = catalog[facadeStrapsProfileName].Name;
            RoofCablesProfileName = catalog[roofCablesProfileName].Name;
            FacadeCablesProfileName = catalog[facadeCablesProfileName].Name;
            RoofBracingProfileName = catalog[roofBracingProfileName].Name;
            ColumnsBracingProfileName = catalog[columnsBracingProfileName].Name;
        }
    }
}