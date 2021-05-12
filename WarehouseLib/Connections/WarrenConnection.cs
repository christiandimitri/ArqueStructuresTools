using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Connections
{
    public class WarrenConnection : Connections
    {
        public WarrenConnection(List<Point3d> topNodes, List<Point3d> bottomNodes) : base(topNodes, bottomNodes)
        {
        }

        public override List<Curve> ConstructConnections()
        {
            var tempTopNodes = BottomNodes;
            var tempBottomNodes = TopNodes;
            var bars = new List<Curve>();
            for (var i = 0; i < tempTopNodes.Count; i++)
            {
                if (i % 2 == 1)
                {
                    var lineA = new Line(tempBottomNodes[i - 1], tempTopNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                    lineA = new Line(tempBottomNodes[i + 1], tempTopNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
            }

            return bars;
        }
    }
}