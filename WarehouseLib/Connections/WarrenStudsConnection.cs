using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.Articulations;

namespace WarehouseLib.Connections
{
    public class WarrenStudsConnection : Connections
    {
        public WarrenStudsConnection(List<Point3d> topNodes, List<Point3d> bottomNodes, string articulationType) : base(
            topNodes,
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
                var lineA = new Line(TopNodes[i + i], tempBottomNodes[i]);
                var lineB = new Line(TopNodes[i + i + 1], tempBottomNodes[i]);
                var lineC = new Line(TopNodes[i + i + 2], tempBottomNodes[i]);
                if (lineA.IsValid) axis.Add(lineA.ToNurbsCurve());
                if (lineB.IsValid) axis.Add(lineB.ToNurbsCurve());
                if (lineC.IsValid) axis.Add(lineC.ToNurbsCurve());
            }

            return axis;
        }
    }
}