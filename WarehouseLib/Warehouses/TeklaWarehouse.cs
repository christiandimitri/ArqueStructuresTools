using System.Collections.Generic;
using System.Diagnostics;
using Rhino.Geometry;
using WarehouseLib.Options;
using WarehouseLib.Profiles;
using WarehouseLib.Trusses;

namespace WarehouseLib.Warehouses
{
    public class TeklaWarehouse
    {
        private WarehouseProfiles _profiles;
        private Warehouse _warehouse;
        public TrussOptions _teklaTrussInputs;
        public WarehouseOptions _teklaWarehouseInputs;

        public TeklaWarehouse(Warehouse warehouse, WarehouseProfiles profiles)
        {
            _warehouse = warehouse;
            _profiles = profiles;
        }

        public Warehouse GetTeklaWarehouse(TrussOptions trussOptions, WarehouseProfiles profiles)
        {
            var warehouse = new Warehouse(_warehouse._plane, trussOptions, _warehouse._warehouseOptions);
            AssignProfiles(profiles, warehouse);
            return warehouse;
        }

        private WarehouseOptions ComputeTeklaWarehouseInputs(WarehouseOptions warehouseOptions,
            WarehouseProfiles profiles)
        {
            var newOptions = warehouseOptions;

            return newOptions;
        }

        public TrussOptions ComputeTeklaTrussInputs(TrussOptions trussInputs, WarehouseProfiles profiles)
        {
            var bottomBeamsProfileHeight = new Catalog().GetCatalog()[profiles.BottomBeamsProfileName].Height;
            var newClearHeight = trussInputs.ClearHeight + bottomBeamsProfileHeight / 2;
            var trussOptions = new TrussOptions(trussInputs.TrussType, trussInputs.Width, trussInputs.Height,
                trussInputs.MaxHeight, newClearHeight, trussInputs.BaseType, trussInputs._articulationType,
                trussInputs.Divisions, trussInputs.PorticoType, trussInputs.ColumnsCount);
            return trussOptions;
        }

        private static void AssignProfiles(WarehouseProfiles profiles, Warehouse warehouse)
        {
            var staticColumnsProfile = new Catalog().GetCatalog()[profiles.TopBeamsProfileName];
            var boundaryColumnsProfile = new Catalog().GetCatalog()[profiles.TopBeamsProfileName];
            var topBeamsProfile = new Catalog().GetCatalog()[profiles.TopBeamsProfileName];
            var bottomBeamsProfile = new Catalog().GetCatalog()[profiles.BottomBeamsProfileName];
            var intermediateBeamsProfile = new Catalog().GetCatalog()[profiles.TopBeamsProfileName];
            foreach (var truss in warehouse.Trusses)
            {
                foreach (var staticColumn in truss.StaticColumns)
                {
                    staticColumn.Profile = staticColumnsProfile;
                }

                if (truss.BoundaryColumns != null)
                {
                    foreach (var boundaryColumn in truss.BoundaryColumns)
                    {
                        boundaryColumn.Profile = boundaryColumnsProfile;
                    }
                    
                }

                truss.TopBeam.Profile = topBeamsProfile;
                truss.BottomBeam.Profile = bottomBeamsProfile;
                truss.IntermediateBeams.Profile = intermediateBeamsProfile;
            }
        }
    }
}