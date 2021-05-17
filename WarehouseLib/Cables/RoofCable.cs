using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Cables
{
    public class RoofCable : Cable
    {
        public RoofCable()
        {
        }

        public override List<Cable> ConstructCables(List<Point3d> nodes, Curve beam)
        {
            var cables = new List<Cable>();

            foreach (var node in nodes)
            {
                beam.ClosestPoint(node, out double t);
                var axis = new Line(node, beam.PointAt(t));
                var bracing = new RoofCable();
                bracing.Axis = axis;
                cables.Add(bracing);
            }

            return cables;
        }
    }
}