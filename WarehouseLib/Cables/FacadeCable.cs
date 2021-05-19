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

        public override List<Cable> ConstructCables(List<Point3d> nodes, Curve beam)
        {
            var cables = new List<Cable>();
            var ptA = nodes[0];
            var ptB = beam.PointAtEnd;
            var axis = new Line(ptA, ptB);
            var cable = new FacadeCable();
            cable.Axis = axis;
            cables.Add(cable);

            return cables;
        }
    }
}