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

        public override List<Bracing> ConstructBracings(List<Truss> trusses, int count, int index)
        {
            throw new System.NotImplementedException();
        }
    }
}