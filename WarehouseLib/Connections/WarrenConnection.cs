using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;

namespace WarehouseLib.Connections
{
    public class WarrenConnection : Connections
    {
        private int AcuteAnglePointIndex;

        public WarrenConnection(List<Point3d> topNodes, List<Point3d> bottomNodes, int index) : base(topNodes,
            bottomNodes)
        {
            AcuteAnglePointIndex = index;
        }

        public override List<Curve> ConstructConnections()
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

            var midStudAxis = new Line(tempBottomNodes[AcuteAnglePointIndex], tempTopNodes[AcuteAnglePointIndex]);

            axis.Insert(AcuteAnglePointIndex, midStudAxis.ToNurbsCurve());

            return axis;
        }
    }
}