using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Bracings
{
    public class ColumnsBracing : Bracing
    {
        public ColumnsBracing(Line axis) : base(axis)
        {
        }

        public override List<Bracing> ConstructBracings(List<Truss> trusses)
        {
            throw new System.NotImplementedException();
        }
    }
}