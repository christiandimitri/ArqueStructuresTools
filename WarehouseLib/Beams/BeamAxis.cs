using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Nodes;

namespace WarehouseLib.Beams
{
    public class BeamAxis
    {
        // <summary>
        // initializes a new instance of the AxisHalfEdge class
        // </summary>

        public BeamAxis(Curve axis)
        {
            AxisCurve = axis;
        }

        // <summary>
        // gets or sets the axis Curve
        // </summary>
        public Curve AxisCurve { get; set; }


        // <summary>
        // gets or sets the beam's axis index
        // </summary>
        public int Index { get; set; }


        // <summary>
        // gets the adjacent Nodes of this given edge
        // </summary>
        // <returns></returns>
        public List<Node> AdjacentNodes()
        {
            var nodes = new List<Node> {this.HalfEdge.Origin, this.HalfEdge.Twin.Origin};
            return nodes;
        }

        // <summary>
        // gets the adjacentAxis of this axis
        // </summary>
        // <returns></returns>
        public List<BeamAxis> AdjacentAxis()
        {
            var axis = new List<BeamAxis>();
            axis.AddRange(this.HalfEdge.Origin.AdjancentAxis());
            axis.AddRange(this.HalfEdge.Twin.Origin.AdjancentAxis());

            return axis;
        }

        // <summary>
        // gets or sets the Half-Edge linked to this axis
        // </summary>
        public BeamAxisHalfEdge HalfEdge { get; set; }
    }
}