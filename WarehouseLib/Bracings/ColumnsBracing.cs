using System;
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
            var bracings = new List<Bracing>();
            foreach (var node in nodes)
            {
                var ptA = node;
                beam.ClosestPoint(node, out double t);
                var ptB = beam.PointAt(t);
                var axis = new Line(ptA, ptB);
                var bracing = new ColumnsBracing();
                bracing.Axis = axis;
                bracings.Add(bracing);
            }

            return bracings;
        }
    }
}