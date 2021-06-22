using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Nodes;

namespace WarehouseLib
{
    public class Axis
    {
        // <summary>
        // get or set the axis Curve
        // </summary>
        public Curve AxisCurve { get; set; }

        // <summary>
        // get or set the axis's node Point3d
        // </summary>

        public Node Origin { get; set; }

        public Axis Twin { get; set; }

        public Axis(Curve axis)
        {
            AxisCurve = axis;
        }
    }
}