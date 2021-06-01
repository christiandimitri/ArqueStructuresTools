using System.Collections.Generic;
using System.Diagnostics;
using Rhino.Geometry;
using WarehouseLib.Beams;
using WarehouseLib.Cables;
using WarehouseLib.Options;
using WarehouseLib.Profiles;
using WarehouseLib.Trusses;

namespace WarehouseLib.Warehouses
{
    public class TeklaWarehouse
    {
        private WarehouseProfiles _profiles;
        private Warehouse _warehouse;

        private TrussOptions _teklaTrussInputs;
        // public WarehouseOptions _teklaWarehouseInputs;

        public TeklaWarehouse(Warehouse warehouse, WarehouseProfiles profiles)
        {
            _warehouse = warehouse;
            _profiles = profiles;
            _teklaTrussInputs = ComputeTeklaTrussInputs(_warehouse._trussOptions, profiles);
        }

        public Warehouse GetTeklaWarehouse()
        {
            var warehouse = new Warehouse(_warehouse._plane, _teklaTrussInputs, _warehouse._warehouseOptions);

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

            _warehouse.RoofCables = warehouse.RoofCables;

            warehouse = _warehouse;

            AssignProfiles(_profiles, warehouse);
            return warehouse;
        }
        
        private TrussOptions ComputeTeklaTrussInputs(TrussOptions trussInputs, WarehouseProfiles profiles)
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
            var catalog = new Catalog().GetCatalog();
            var staticColumnsProfile = catalog[profiles.StaticColumnsProfileName];
            var boundaryColumnsProfile = catalog[profiles.BoundaryColumnProfileName];
            var topBeamsProfile = catalog[profiles.TopBeamsProfileName];
            var bottomBeamsProfile = catalog[profiles.BottomBeamsProfileName];
            var intermediateBeamsProfile = catalog[profiles.IntermediateBeamsProfileName];
            var roofStrapProfile = catalog[profiles.RoofStrapsProfileName];
            var facadeStrapProfile = catalog[profiles.FacadeStrapsProfileName];
            var facadeCableProfile = catalog[profiles.FacadeCablesProfileName];
            var roofCableProfile = catalog[profiles.RoofCablesProfileName];
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

            foreach (var strap in warehouse.RoofStraps)
            {
                strap.Profile = roofStrapProfile;
            }

            if (warehouse.FacadeStrapsX != null)
            {
                foreach (var strap in warehouse.FacadeStrapsX)
                {
                    strap.Profile = facadeStrapProfile;
                }
            }

            if (warehouse.FacadeStrapsY != null)
            {
                foreach (var strap in warehouse.FacadeStrapsY)
                {
                    strap.Profile = facadeStrapProfile;
                }
            }

            if (warehouse.FacadeCables != null)
            {
                foreach (var cable in warehouse.FacadeCables)
                {
                    cable.Profile = facadeCableProfile;
                }
            }

            if (warehouse.RoofCables != null)
            {
                foreach (var cable in warehouse.RoofCables)
                {
                    cable.Profile = roofCableProfile;
                }
            }
        }
    }
}