using WarehouseLib.Nodes;

namespace WarehouseLib.Beams
{
    public class BeamAxisHalfEdge
    {
        // <summary>
        // initializes a new instance of the AxisHalfEdge class
        // </summary>
        public BeamAxisHalfEdge()
        {
        }

        // <summary>
        // gets or sets the axis linked to this Half-Edge
        // </summary>
        public BeamAxis Axis { get; set; }


        // <summary>
        // gets or set the axis's Origin Point3d
        // </summary>
        public Node Origin { get; set; }


        // <summary>
        // gets or sets the axis's Destination Point3d
        // </summary>
        public Node Destination { get; set; }


        // <summary>
        // gets or sets the axis's Opposite/the other Half-Edge Axis
        // </summary>
        public BeamAxisHalfEdge Twin { get; set; }


        // <summary>
        // gets or sets the next Half-Edge axis
        // </summary>
        public BeamAxisHalfEdge Next { get; set; }


        // <summary>
        // gets or sets the previous Half-Edge axis
        // </summary>
        public BeamAxisHalfEdge Previous { get; set; }


        // <summary>
        // gets or sets the string representation of the axis beam type e.g. "top" "bottom" "intermediate"
        // </summary>
        public string Position { get; set; }


        // <summary>
        // gets or sets a value indicated whether the axis is located at the boundary or not
        // </summary>
        public bool OnBoundary { get; set; }


        // <summary>
        // gets or sets the axis's index
        // </summary>
        public int Index { get; set; }
    }
}