using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Connections
{
    public abstract class Connection
    {
        protected Connection()
        {
        }
    }

    public class PrattConnection : Connection
    {
        private List<Point3d> TopNodes;
        private List<Point3d> BottomNodes;

        public PrattConnection(List<Point3d> topNodes, List<Point3d> bottomNodes)
        {
            TopNodes = topNodes;
        }

        public List<Curve> ConstructPrattTruss(int index)
        {
            var bars = new List<Curve>();
            for (var i = 0; i < TopNodes.Count; i += 2)
            {
                if (i < index)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                    lineA = new Line(TopNodes[i], BottomNodes[i + 2]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i == index)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i > index)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i - 2]);
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