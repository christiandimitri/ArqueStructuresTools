using System;
using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Beams;

namespace WarehouseLib.Nodes
{
    public class Node
    {
        // <summary>
        // get or set the node point3d
        // </summary>
        public Point3d Position;

        // <summary>
        // get or set the incident axis edge to the node
        // </summary>
        public BeamAxis IncidentAxis { get; set; }

        // <summary>
        // returns a list of all of the adjacent axis to the node
        // </summary>
        public List<BeamAxis> AdjancentAxis()
        {
            throw new InvalidOperationException();
        }
        // <summary>
        // initialize a new instance of the Node
        // </summary>
        public Node(Point3d point3d)
        {
            Position = point3d;
        }
        
    }
}