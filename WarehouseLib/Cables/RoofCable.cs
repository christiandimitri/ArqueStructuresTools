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
            var outerPoints = nodes;
            var innerPoints = new List<Point3d>();
            foreach (var node in outerPoints)
            {
                beam.ClosestPoint(node, out double t);
                innerPoints.Add(beam.PointAt(t));
            }

            for (int i = 0; i < outerPoints.Count - 1; i++)
            {
                var cable = new RoofCable();
                var axis = new Line(outerPoints[i], innerPoints[i + 1]);
                cable.Axis = axis;
                cables.Add(cable);
                if (i > 0)
                {
                    axis = new Line(outerPoints[i], innerPoints[i - 1]);
                    cable = new RoofCable();
                    cable.Axis = axis;
                    cables.Add(cable);
                }
            }

            return cables;
        }
    }
}