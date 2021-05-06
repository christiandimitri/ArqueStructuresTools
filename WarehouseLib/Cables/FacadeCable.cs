using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Cables
{
    public class FacadeCable : Cable
    {
        public FacadeCable(Line axis) : base(axis)
        {
        }

        public override List<Cable> ConstructCables(List<Truss> trusses)
        {
            throw new System.NotImplementedException();
        }
    }
}