using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Cables
{
    public class FacadeCable : Cable
    {
        public FacadeCable()
        {
        }

        public override List<Cable> ConstructCables(List<Truss> trusses, int count, int index)
        {
            throw new System.NotImplementedException();
        }
    }
}