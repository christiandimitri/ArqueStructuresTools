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

        protected override Plane GetTeklaProfileOrientationPlane(Curve beam, Point3d position, Plane plane, int index)
        {
            return plane;
        }

        public override List<Bracing> ConstructBracings(List<Point3d> nodes, Curve beam, Plane plane, int index)
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
                bracing.ProfileOrientationPlane = GetTeklaProfileOrientationPlane(beam, node, plane, index);
                bracings.Add(bracing);
            }

            return bracings;
        }
    }
}