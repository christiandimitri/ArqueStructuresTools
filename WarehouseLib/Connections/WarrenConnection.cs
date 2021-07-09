using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.Articulations;

namespace WarehouseLib.Connections
{
    public class WarrenConnection : Connections
    {
        public WarrenConnection(List<Point3d> topNodes, List<Point3d> bottomNodes, string articulationType) : base(topNodes,
            bottomNodes, articulationType)
        {
        }

        public override List<Curve> ConstructConnections()
        {
            var axis = new List<Curve>();
            var tempBottomNodes = new List<Point3d>(BottomNodes);
            if (_articulationType != ArticulationType.Articulated.ToString())
            {
                tempBottomNodes.RemoveAt(0);
                tempBottomNodes.RemoveAt(tempBottomNodes.Count - 1);
            }

            for (int i = 0; i < tempBottomNodes.Count; i++)
            {
                var lineA = new Line(TopNodes[i], tempBottomNodes[i]);
                var lineB = new Line(TopNodes[i + 1], tempBottomNodes[i]);
                if (lineA.IsValid) axis.Add(lineA.ToNurbsCurve());
                if (lineB.IsValid) axis.Add(lineB.ToNurbsCurve());
            }

            return axis;
        }
    }
}