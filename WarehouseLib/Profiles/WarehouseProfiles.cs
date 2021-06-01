using System;
using System.Diagnostics;
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

        public WarehouseProfiles(string staticColumnsProfileName, string boundaryColumnProfileName,
            string topBeamsProfileName,
            string bottomBeamsProfileName,
            string intermediateBeamsProfileName, string roofStrapsProfileName)
        {
            var catalog = new Catalog().GetCatalog();
            StaticColumnsProfileName = catalog[staticColumnsProfileName].Name;
            BoundaryColumnProfileName = catalog[boundaryColumnProfileName].Name;
            TopBeamsProfileName = catalog[topBeamsProfileName].Name;
            BottomBeamsProfileName = catalog[bottomBeamsProfileName].Name;
            IntermediateBeamsProfileName = catalog[intermediateBeamsProfileName].Name;
            RoofStrapsProfileName = catalog[roofStrapsProfileName].Name;
        }
    }
}