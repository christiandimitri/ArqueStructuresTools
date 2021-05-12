using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Connections
{
    public class HoweConnection : Connections
    {
        private readonly int _index;
        private readonly string _articulationType;
        public HoweConnection(List<Point3d> topNodes, List<Point3d> bottomNodes, int index, string articulationType) :
            base(topNodes, bottomNodes)
        {
            _index = index;
            _articulationType = articulationType;
        }

        public override List<Curve> ConstructConnections()
        {
            var tempTopNodes = BottomNodes;
            var tempBottomNodes = TopNodes;
            var bars = new List<Curve>();
            for (var i = 0; i < tempTopNodes.Count; i += 2)
            {
                if (i < _index)
                {
                    var lineA = new Line(tempBottomNodes[i], tempTopNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                    lineA = new Line(tempBottomNodes[i + 2], tempTopNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i == _index)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i > _index)
                {
                    var lineA = new Line(tempBottomNodes[i - 2], tempTopNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                    lineA = new Line(tempBottomNodes[i], tempTopNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
            }

            bars.RemoveAt(0);
            bars.RemoveAt(bars.Count - 1);
            if (_articulationType == "Articulated")
            {
                bars.RemoveAt(0);
                var lineA = new Line(TopNodes[0], BottomNodes[2]);
                bars.Insert(0, lineA.ToNurbsCurve());
                bars.RemoveAt(bars.Count - 1);
                lineA = new Line(TopNodes[TopNodes.Count - 1], BottomNodes[BottomNodes.Count - 3]);
                bars.Add(lineA.ToNurbsCurve());
            }

            return bars;
        }
    }
}