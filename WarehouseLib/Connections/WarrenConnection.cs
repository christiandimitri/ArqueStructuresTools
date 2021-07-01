using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;

namespace WarehouseLib.Connections
{
    public class WarrenConnection : Connections
    {
        public WarrenConnection(List<Point3d> topNodes, List<Point3d> bottomNodes) : base(topNodes,
            bottomNodes)
        {
        }

        public List<Curve> ConstructConnection()
        {
            var tempTopNodes = BottomNodes;
            var tempBottomNodes = TopNodes;
            var axis = new List<Curve>();
            for (var i = 0; i < tempTopNodes.Count; i++)
            {
                if (i % 2 == 1)
                {
                    var lineA = new Line(tempBottomNodes[i - 1], tempTopNodes[i]);
                    axis.Add(lineA.ToNurbsCurve());
                    lineA = new Line(tempBottomNodes[i + 1], tempTopNodes[i]);
                    axis.Add(lineA.ToNurbsCurve());
                }
            }
            return axis;
        }

        public override List<Curve> ConstructConnections()
        {
            var axis = new List<Curve>();

            for (int i = 0; i < BottomNodes.Count; i++)
            {
                var lineA = new Line(TopNodes[i], BottomNodes[i]);
                var lineB = new Line(TopNodes[i + 1], BottomNodes[i]);
                if (lineA.IsValid) axis.Add(lineA.ToNurbsCurve());
                if (lineB.IsValid) axis.Add(lineB.ToNurbsCurve());
            }

            return axis;
        }
    }
}