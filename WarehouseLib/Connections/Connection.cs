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
        private readonly List<Point3d> _topNodes;
        private readonly List<Point3d> _bottomNodes;

        public PrattConnection(List<Point3d> topNodes, List<Point3d> bottomNodes)
        {
            _topNodes = topNodes;
            _bottomNodes = bottomNodes;
        }

        public List<Curve> ConstructPrattTruss(int index)
        {
            var bars = new List<Curve>();
            for (var i = 0; i < _topNodes.Count; i += 2)
            {
                if (i < index)
                {
                    var lineA = new Line(_topNodes[i], _bottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                    lineA = new Line(_topNodes[i], _bottomNodes[i + 2]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i == index)
                {
                    var lineA = new Line(_topNodes[i], _bottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i > index)
                {
                    var lineA = new Line(_topNodes[i], _bottomNodes[i - 2]);
                    bars.Add(lineA.ToNurbsCurve());
                    lineA = new Line(_topNodes[i], _bottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
            }

            bars.RemoveAt(0);
            bars.RemoveAt(bars.Count - 1);
            return bars;
        }
    }
}