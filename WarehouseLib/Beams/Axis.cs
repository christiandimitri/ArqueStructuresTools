using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Nodes;

namespace WarehouseLib
{
    public struct Axis
    {
        public Curve AxisCurve;

        public List<Node> Nodes;

        public Axis(Curve axis, List<Node> nodes)
        {
            AxisCurve = axis;
            Nodes = nodes;
        }
    }
}