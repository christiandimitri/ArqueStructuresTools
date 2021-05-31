using System.Collections.Generic;
using System.Diagnostics;
using Rhino.Geometry;
using WarehouseLib.Beams;
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
        // public WarehouseOptions _teklaWarehouseInputs;

        public TeklaWarehouse(Warehouse warehouse, WarehouseProfiles profiles)
        {
            _warehouse = warehouse;
            _profiles = profiles;
        }

        public Warehouse GetTeklaWarehouse(TrussOptions trussOptions, WarehouseProfiles profiles)
        {
            var warehouse = new Warehouse(_warehouse._plane, trussOptions, _warehouse._warehouseOptions);

            for (int i = 0; i < _warehouse.Trusses.Count; i++)
            {
                var trussA = warehouse.Trusses[i];

                var trussB = _warehouse.Trusses[i];

                if (trussB.IntermediateBeams != null)
                {
                    trussB.IntermediateBeams = new IntermediateBeams();
                    trussB.IntermediateBeams.Axis = trussA.IntermediateBeamsAxisCurves;
                    trussB.IntermediateBeams.ProfileOrientationPlane = trussA.IntermediateBeams.ProfileOrientationPlane;
                }

                if (trussB.BottomBeam != null)
                {
                    trussB.BottomBeam.Axis = trussA.BottomBeamAxisCurves;
                    trussB.BottomBeam.ProfileOrientationPlane = trussA.BottomBeam.ProfileOrientationPlane;
                }
            }

            warehouse = _warehouse;

            AssignProfiles(profiles, warehouse);
            return warehouse;
        }

        // private WarehouseOptions ComputeTeklaWarehouseInputs(WarehouseOptions warehouseOptions,
        //     WarehouseProfiles profiles)
        // {
        //     var newOptions = warehouseOptions;
        //
        //     return newOptions;
        // }

        public TrussOptions ComputeTeklaTrussInputs(TrussOptions trussInputs, WarehouseProfiles profiles)
        {
            var bottomBeamsProfileHeight = new Catalog().GetCatalog()[profiles.BottomBeamsProfileName].Height;
            var newClearHeight = trussInputs.ClearHeight + bottomBeamsProfileHeight / 2;
            var topBeamsHeight = new Catalog().GetCatalog()[profiles.TopBeamsProfileName].Height / 2;
            var newHeight = trussInputs.Height - topBeamsHeight;
            var newMaxHeight = trussInputs.MaxHeight - topBeamsHeight;

            var trussOptions = new TrussOptions(trussInputs.TrussType, trussInputs.Width, newHeight,
                newMaxHeight, newClearHeight, trussInputs.BaseType, trussInputs._articulationType,
                trussInputs.Divisions, trussInputs.PorticoType, trussInputs.ColumnsCount);
            return trussOptions;
        }

        private static void AssignProfiles(WarehouseProfiles profiles, Warehouse warehouse)
        {
            var staticColumnsProfile = new Catalog().GetCatalog()[profiles.StaticColumnsProfileName];
            var boundaryColumnsProfile = new Catalog().GetCatalog()[profiles.BoundaryColumnProfileName];
            var topBeamsProfile = new Catalog().GetCatalog()[profiles.TopBeamsProfileName];
            var bottomBeamsProfile = new Catalog().GetCatalog()[profiles.BottomBeamsProfileName];
            var intermediateBeamsProfile = new Catalog().GetCatalog()[profiles.IntermediateBeamsProfileName];
            foreach (var truss in warehouse.Trusses)
            {
                if (truss.StaticColumns != null)
                {
                    foreach (var staticColumn in truss.StaticColumns)
                    {
                        staticColumn.Profile = staticColumnsProfile;
                    }
                }

                if (truss.BoundaryColumns != null)
                {
                    foreach (var boundaryColumn in truss.BoundaryColumns)
                    {
                        boundaryColumn.Profile = boundaryColumnsProfile;
                    }
                }

                if (truss.TopBeam != null)
                {
                    truss.TopBeam.Profile = topBeamsProfile;
                }

                if (truss.BottomBeam != null)
                {
                    truss.BottomBeam.Profile = bottomBeamsProfile;
                }

                if (truss.IntermediateBeams != null)
                {
                    truss.IntermediateBeams.Profile = intermediateBeamsProfile;
                }
            }
        }
    }
}