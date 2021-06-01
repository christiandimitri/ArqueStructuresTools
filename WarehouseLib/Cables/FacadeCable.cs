using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Cables
{
    public class FacadeCable : Cable
    {
        public double Threshold;

        public FacadeCable()
        {
        }

        public override List<Cable> ConstructCables(List<Point3d> nodes, Curve beam, Plane plane, int index)
        {
            var cables = new List<Cable>();
            var width = Convert.ToInt32(nodes[0].DistanceTo(beam.PointAtStart));
            var height = Convert.ToInt32(beam.GetLength());
            var ratio = Convert.ToInt32(height / width * Threshold);
            if (ratio == 0)
            {
                ratio = 1;
            }

            var parameters = beam.DivideByCount(ratio, true);
            var outsideNodes = new List<Point3d>();
            var insideNodes = new List<Point3d>();

            for (int i = 0; i < parameters.Length; i++)
            {
                var ptB = beam.PointAt(parameters[i]);
                var ptA = new Point3d(ptB.X, nodes[0].Y, ptB.Z);
                outsideNodes.Add(ptA);
                insideNodes.Add(ptB);
            }

            for (int i = 0; i < outsideNodes.Count; i++)
            {
                Point3d ptA = outsideNodes[i];
                Point3d ptB = (i > 0)
                    ? insideNodes[i - 1]
                    : outsideNodes[i];
                Line axis = new Line(ptA, ptB);
                var cable = new FacadeCable();
                cable.Axis = axis;
                cable.ProfileOrientationPlane = GetTeklaProfileOrientationPlane(beam, ptA, plane, index);
                if (axis.IsValid) cables.Add(cable);
                if (i > 0 || i < outsideNodes.Count - 1)
                {
                    axis = new Line(outsideNodes[i], insideNodes[i]);
                    cable = new FacadeCable();
                    cable.Axis = axis;
                    if (axis.IsValid) cables.Add(cable);
                }

                ptB = (i < outsideNodes.Count - 1)
                    ? insideNodes[i + 1]
                    : outsideNodes[i];
                axis = new Line(ptA, ptB);
                cable = new FacadeCable();
                cable.Axis = axis;
                cable.ProfileOrientationPlane = GetTeklaProfileOrientationPlane(beam, ptA, plane, index);
                if (axis.IsValid) cables.Add(cable);
            }

            cables.RemoveAt(0);
            cables.RemoveAt(cables.Count - 1);
            return cables;
        }

        protected override Plane GetTeklaProfileOrientationPlane(Curve beam, Point3d position, Plane plane, int index)
        {
            var normal = (index == 0) ? plane.XAxis : -plane.XAxis;
            var orientationPlane = new Plane(position, normal);
            return orientationPlane;
        }
    }
}