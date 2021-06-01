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

        public override List<Cable> ConstructCables(List<Point3d> nodes, Curve beam, Plane plane, int index)
        {
            var cables = new List<Cable>();
            var outerPoints = nodes;
            var innerPoints = new List<Point3d>();
            foreach (var node in outerPoints)
            {
                beam.ClosestPoint(node, out double t);
                innerPoints.Add(beam.PointAt(t));
            }

            for (int i = 0; i < outerPoints.Count; i++)
            {
                Point3d ptA = outerPoints[i];
                Point3d ptB = (i > 0)
                    ? innerPoints[i - 1]
                    : outerPoints[i];
                Line axis = new Line(ptA, ptB);
                var cable = new RoofCable();
                cable.Axis = axis;
                cable.ProfileOrientationPlane = GetTeklaProfileOrientationPlane(beam, ptA, plane, 0);
                if (axis.IsValid) cables.Add(cable);
                ptB = (i < outerPoints.Count - 1)
                    ? innerPoints[i + 1]
                    : outerPoints[i];
                axis = new Line(ptA, ptB);
                cable = new RoofCable();
                cable.Axis = axis;
                cable.ProfileOrientationPlane = GetTeklaProfileOrientationPlane(beam, ptA, plane, 0);
                if (axis.IsValid) cables.Add(cable);
            }

            return cables;
        }

        protected override Plane GetTeklaProfileOrientationPlane(Curve beam, Point3d position, Plane plane,
            int index)
        {
            double t;
            beam.ClosestPoint(position, out t);
            var tangent=beam.TangentAt(t);
            var vector = plane.YAxis;
            var normal = Vector3d.CrossProduct(tangent, vector);
            var profilePlane = new Plane(position, normal);
            return profilePlane;
        }
    }
}