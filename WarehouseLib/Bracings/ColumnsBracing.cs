using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Bracings
{
    public class ColumnsBracing : Bracing
    {
        public ColumnsBracing()
        {
        }
        public override List<Bracing> ConstructBracings(List<Point3d> nodes, Curve beam)
        {
            throw new System.NotImplementedException();
        }
    }
}