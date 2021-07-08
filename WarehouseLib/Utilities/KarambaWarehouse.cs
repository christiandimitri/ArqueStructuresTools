using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.Beams;
using WarehouseLib.Bracings;
using WarehouseLib.Cables;
using WarehouseLib.Columns;
using WarehouseLib.Crosses;
using WarehouseLib.Trusses;
using WarehouseLib.Warehouses;

namespace WarehouseLib.Utilities
{
    public class KarambaWarehouse
    {
        public Warehouse _warehouse { get; set; }
        public List<KarambaTruss> KarambaTrusses { get; set; }
        public List<Truss> Trusses { get; set; }
        public List<Strap> RoofStraps { get; set; }

        public List<Strap> FacadeStrapsX { get; set; }
        public List<Strap> FacadeStrapsY { get; set; }
        public List<Cable> RoofCables { get; set; }
        public List<Bracing> RoofBracings { get; set; }
        public List<Cable> FacadeCables { get; set; }
        public List<Bracing> ColumnsBracings { get; set; }
        public List<Cross> Crosses { get; set; }

        public KarambaWarehouse(Warehouse warehouse)
        {
            _warehouse = warehouse;
            Trusses = _warehouse.Trusses;
            RoofStraps = warehouse.RoofStraps;
            FacadeStrapsX = warehouse.FacadeStrapsX;
            FacadeStrapsY = warehouse.FacadeStrapsY;
            RoofCables = warehouse.RoofCables;
            RoofBracings = warehouse.RoofBracings;
            FacadeCables = warehouse.FacadeCables;
            ColumnsBracings = warehouse.ColumnsBracings;
            Crosses = warehouse.Crosses;
            ReplaceTrussesWithKaramabaTrusses();
        }

        private void ReplaceTrussesWithKaramabaTrusses()
        {
            var karambaTrusses = new List<KarambaTruss>();
            for (var i = 0; i < _warehouse.Trusses.Count; i++)
            {
                var truss = _warehouse.Trusses[i];
                var karambaTruss = new KarambaTruss(truss);
                karambaTrusses.Add(karambaTruss);
            }

            // _warehouse.Trusses = new List<Truss>();
            KarambaTrusses = new List<KarambaTruss>(karambaTrusses);
            RecomputeBucklingLengths();
        }

        private void RecomputeBucklingLengths()
        {
            // var indices = new List<int>();
            int i = 0;
            for (i = _warehouse._warehouseOptions.HasBoundary ? 1 : 0;
                _warehouse._warehouseOptions.HasBoundary ? i < KarambaTrusses.Count - 1 : i < KarambaTrusses.Count;
                i++)
            {
                // int index = 0;
                // int k = 0;
                var karambaTruss = KarambaTrusses[i];
                var truss = _warehouse.Trusses[i];
                if (karambaTruss.StAndresBottomNodes != null)
                {
                    SetBeamsBucklingLengthsBetweenStAndresCrosses(karambaTruss);
                }
                else
                {
                    SetBeamsBucklingLengthsBetweenNodes(karambaTruss);
                }
            }
        }

        private void SetBeamsBucklingLengthsBetweenNodes(KarambaTruss karambaTruss)
        {
            karambaTruss.Karamba3DBottomBeams.BucklingLengths =
                new List<BucklingLengths.BucklingLengths>(karambaTruss.Karamba3DBottomBeams.BucklingLengths);
        }

        private void SetBeamsBucklingLengthsBetweenStAndresCrosses(KarambaTruss karambaTruss)
        {
            var bucklings = new List<BucklingLengths.BucklingLengths>();
            var beam = karambaTruss.Karamba3DBottomBeams;
            var distances = ComputeDistancesBetweenStAndreCrosses(karambaTruss);


            bucklings = beam.SetTrussBeamBucklingLengthsBetweenStAndresCrosses(beam, distances);
            beam.BucklingLengths = bucklings;
        }

        private List<double> ComputeDistancesBetweenStAndreCrosses(KarambaTruss karambaTruss)
        {
            var distances = new List<double>();
            for (int i = 0; i < karambaTruss.Karamba3DBottomBeams.SkeletonAxis.Count; i++)
            {
                distances.Add(1.5);
            }


            return distances;
        }

        private List<double> RepeatAddDistancesByCount(int axisListCount, double distance)
        {
            var distances = new List<double>();
            for (int i = 0; i < axisListCount; i++)
            {
                distances.Add(distance);
            }

            return distances;
        }
    }
}