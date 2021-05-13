using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Connections
{
    public class PrattConnection : Connections
    {
        private readonly int _index;

        public PrattConnection(List<Point3d> topNodes, List<Point3d> bottomNodes, int index) : base(topNodes,
            bottomNodes)
        {
            _index = index;
        }

        public override List<Curve> ConstructConnections()
        {
            var bars = new List<Curve>();
            for (var i = 0; i < TopNodes.Count; i ++)
            {
                if (i < _index)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                    lineA = new Line(TopNodes[i], BottomNodes[i + 1]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i == _index)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i > _index)
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