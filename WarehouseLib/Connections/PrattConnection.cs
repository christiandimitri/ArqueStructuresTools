using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Connections
{
    public class PrattConnection : Connections
    {
        public PrattConnection(List<Point3d> topNodes, List<Point3d> bottomNodes, string articulationType) : base(
            topNodes,
            bottomNodes, articulationType)
        {
        }

        public override List<Curve> ConstructConnections()
        {
            var bars = new List<Curve>();
            for (var i = 0; i < TopNodes.Count; i++)
            {
                if (i < MidPointIndex)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                    lineA = new Line(TopNodes[i], BottomNodes[i + 1]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i == MidPointIndex)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i > MidPointIndex)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i - 1]);
                    bars.Add(lineA.ToNurbsCurve());
                    lineA = new Line(TopNodes[i], BottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
            }

            bars.RemoveAt(0);
            bars.RemoveAt(bars.Count - 1);
            return bars;
        }
    }
}