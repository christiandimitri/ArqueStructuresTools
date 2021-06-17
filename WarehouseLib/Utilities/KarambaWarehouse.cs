using System.Collections.Generic;
using System.Diagnostics;
using Rhino.Geometry;
using WarehouseLib.Bracings;
using WarehouseLib.Cables;
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

        public List<Point3d> StAndresBottomNodes { get; set; }

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
            // var stAndreDistance = ComputeDistanceBetweenStAndresCross();
            for (var i = 0; i < _warehouse.Trusses.Count; i++)
            {
                var truss = _warehouse.Trusses[i];
                var karambaTruss = new KarambaTruss(truss);
                karambaTrusses.Add(karambaTruss);
            }

            // _warehouse.Trusses = new List<Truss>();
            KarambaTrusses = new List<KarambaTruss>(karambaTrusses);
        }

        private double ComputeDistanceBetweenStAndresCross()
        {
            var distances = new List<double>();

            for (var i = 1; i < Trusses.Count - 1; i++)
            {
                var truss = Trusses[i];
                var tempCloud = new PointCloud(truss.BottomNodes);
                foreach (var node in StAndresBottomNodes)
                {
                    var index = tempCloud.ClosestPoint(node);
                }
            }

            return 0.0;
        }
    }
}