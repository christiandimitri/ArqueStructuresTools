using System;
using System.Collections.Generic;
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

        public override List<Cable> ConstructCables(List<Point3d> nodes, Curve beam)
        {
            var cables = new List<Cable>();
            // var ptA = nodes[0];
            // var ptB = beam.PointAtEnd;
            // var axis = new Line(ptA, ptB);
            // var cable = new FacadeCable {Axis = axis};
            // cables.Add(cable);
            //
            // ptA = nodes[1];
            // ptB = beam.PointAtStart;
            // axis = new Line(ptA, ptB);
            // cable = new FacadeCable {Axis = axis};
            // cables.Add(cable);

            var width = Convert.ToInt32(nodes[0].DistanceTo(beam.PointAtStart));
            var height = Convert.ToInt32(beam.GetLength());
            var ratio = Convert.ToInt32(height / width * Threshold);
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
                var cable = new RoofCable();
                cable.Axis = axis;
                if (axis.IsValid) cables.Add(cable);
                ptB = (i < outsideNodes.Count - 1)
                    ? insideNodes[i + 1]
                    : outsideNodes[i];
                axis = new Line(ptA, ptB);
                cable = new RoofCable();
                cable.Axis = axis;
                if (axis.IsValid) cables.Add(cable);

                // axis = new Line(outsideNodes[i], insideNodes[i]);
                // cable.Axis = axis;
                // cables.Add(cable);
            }

            return cables;
        }
    }
}