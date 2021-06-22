using System;
using System.Collections.Generic;
using Rhino.Geometry;

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
        public Axis IncidentAxis { get; set; }

        // <summary>
        // returns a list of all of the adjacent axis to the node
        // </summary>
        public List<Axis> AdjancentAxis()
        {
            throw new InvalidOperationException();
        }
        // <summary>
        // initialize a new instance of the Node
        // </summary>
        public Node(Point3d point3d, Axis incidentAxis)
        {
            Position = point3d;
        }
        
    }
}