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

        public KarambaWarehouse(Warehouse warehouse)
        {
            _warehouse = warehouse;
            Trusses = _warehouse.Trusses;
            ReplaceTrussesWithKaramabaTrusses();
            RoofStraps = warehouse.RoofStraps;
            FacadeStrapsX = warehouse.FacadeStrapsX;
            FacadeStrapsY = warehouse.FacadeStrapsY;
            RoofCables = warehouse.RoofCables;
            RoofBracings = warehouse.RoofBracings;
            FacadeCables = warehouse.FacadeCables;
            ColumnsBracings = warehouse.ColumnsBracings;
            Crosses = warehouse.Crosses;
        }

        private void ReplaceTrussesWithKaramabaTrusses()
        {
            var karambaTrusses = new List<KarambaTruss>();
            for (var i=0;i<_warehouse.Trusses.Count;i++)
            {
                var truss = _warehouse.Trusses[i];
                var karambaTruss = new KarambaTruss(truss);
                karambaTrusses.Add(karambaTruss);
            }
        
            // _warehouse.Trusses = new List<Truss>();
            KarambaTrusses = new List<KarambaTruss>(karambaTrusses);
        }
    }
}