﻿using System.Collections.Generic;
using System.Diagnostics;
using WarehouseLib.Beams;
using WarehouseLib.Options;
using WarehouseLib.Profiles;
using WarehouseLib.Warehouses;

namespace WarehouseLib.Utilities
{
    public class TeklaWarehouse
    {
        private WarehouseProfiles _profiles;
        private Warehouse _warehouse;
        private TrussInputs _teklaTrussInputs;

        public TeklaWarehouse(Warehouse warehouse, WarehouseProfiles profiles)
        {
            _warehouse = warehouse;
            _profiles = profiles;
            _teklaTrussInputs = ComputeTeklaTrussInputs(_warehouse.TrussInputs, profiles);
            Debug.WriteLine(_teklaTrussInputs.Height);
        }

        public Warehouse GetTeklaWarehouse()
        {
            var warehouse = new Warehouse(_warehouse._plane, _teklaTrussInputs, _warehouse._warehouseOptions);

            for (int i = 0; i < _warehouse.Trusses.Count; i++)
            {
                var trussA = warehouse.Trusses[i];

                var trussB = _warehouse.Trusses[i];
                
                // <summary>
                // Todo top beam
                // </summary>
                
                
                if (trussB.IntermediateBeams != null)
                {
                    var axisList = new List<BeamAxis>();
                    for (int j = 0; j < trussA.IntermediateBeamsBaseCurves.Count; j++)
                    {
                        var axis = new BeamAxis(trussA.IntermediateBeamsBaseCurves[i]);
                        axisList.Add(axis);
                    }

                    trussB.IntermediateBeams = new Beam()
                    {
                        SkeletonAxis = axisList,
                        ProfileOrientationPlane = trussA.IntermediateBeams.ProfileOrientationPlane,
                        Position = "Intermediate"
                    };
                }

                if (trussB.BottomBeam != null)
                {
                    var axisList = new List<BeamAxis>();
                    for (int j = 0; j < trussA.BottomBeamBaseCurves.Count; j++)
                    {
                        var axis = new BeamAxis(trussA.BottomBeamBaseCurves[i]);
                        axisList.Add(axis);
                    }

                    trussB.BottomBeam = new Beam()
                    {
                        SkeletonAxis = axisList,
                        ProfileOrientationPlane = trussA.BottomBeam.ProfileOrientationPlane,
                        Position = "Bottom"
                    };
                }
            }

            _warehouse.RoofCables = warehouse.RoofCables;
            _warehouse.RoofBracings = warehouse.RoofBracings;
            _warehouse.Crosses = warehouse.Crosses;
            _warehouse.ColumnsBracings = warehouse.ColumnsBracings;
            _warehouse.FacadeCables = warehouse.FacadeCables;
            warehouse = _warehouse;

            AssignProfiles(_profiles, warehouse);
            return warehouse;
        }

        private TrussInputs ComputeTeklaTrussInputs(TrussInputs trussInputs, WarehouseProfiles profiles)
        {
            var bottomBeamsProfileHeight = new Catalog().GetCatalog()[profiles.BottomBeamsProfileName].Height / 2;
            var newClearHeight = trussInputs.ClearHeight + bottomBeamsProfileHeight;
            var topBeamsProfileHeight = new Catalog().GetCatalog()[profiles.TopBeamsProfileName].Height / 2;
            var newHeight = trussInputs.Height - topBeamsProfileHeight;
            var newMaxHeight = trussInputs.MaxHeight - topBeamsProfileHeight;

            var trussOptions = new TrussInputs(trussInputs.TrussType, trussInputs.Width, newHeight,
                newMaxHeight, newClearHeight, trussInputs.BaseType, trussInputs._articulationType,
                trussInputs.Divisions, trussInputs.PorticoType, trussInputs.ColumnsCount,
                trussInputs.FacadeStrapsDistance);
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
            var roofBracingProfile = catalog[profiles.RoofBracingProfileName];
            var columnsBracingProfile = catalog[profiles.ColumnsBracingProfileName];
            var stAndreProfile = catalog[profiles.StAndreProfileName];
            var porticoBeamsProfile = catalog[profiles.PorticoBeamProfileName];
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
                    truss.TopBeam.Profile = truss.BottomBeam.SkeletonAxis.Count is 0 ? porticoBeamsProfile : topBeamsProfile;
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

            if (warehouse.RoofStraps != null)
            {
                foreach (var strap in warehouse.RoofStraps)
                {
                    strap.Profile = roofStrapProfile;
                }
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

            if (warehouse.RoofBracings != null)
            {
                foreach (var roofBracing in warehouse.RoofBracings)
                {
                    roofBracing.Profile = roofBracingProfile;
                }
            }

            if (warehouse.ColumnsBracings != null)
            {
                foreach (var bracing in warehouse.ColumnsBracings)
                {
                    bracing.Profile = columnsBracingProfile;
                }
            }

            if (warehouse.Crosses != null)
            {
                foreach (var cross in warehouse.Crosses)
                {
                    cross.Profile = stAndreProfile;
                }
            }
        }
    }
}