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
        // get or set the axis's Origin Point3d
        // </summary>

        public Node Origin { get; set; }
        
        // <summary>
        // get or set the axis's Destination Point3d
        // </summary>

        public Node Destination { get; set; }
        
        // <summary>
        // get or set the axis's Opposite/the other Half-Edge Axis
        // </summary>

        public Axis Twin { get; set; }
        
        // <summary>
        // get or set the next axis
        // </summary>

        public Axis Next { get; set; }
        
        // <summary>
        // get or set the previous axis
        // </summary>
        
        public Axis Previous { get; set; }
        
        // <summary>
        // get or set the axis's beam type e.g. "top" "bottom" "intermediate"
        // </summary>

        public string Position { get; set; }
        
        // <summary>
        // get or set a value indicated whether the axis is located at the boundary or not
        // </summary>

        public bool OnBoundary { get; set; }
        
        // <summary>
        // get or set the axis's index
        // </summary>

        public int Index { get; set; }

        public Axis(Curve axis)
        {
            AxisCurve = axis;
        }

    }
}