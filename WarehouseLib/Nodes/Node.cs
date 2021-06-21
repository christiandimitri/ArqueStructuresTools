using Rhino.Geometry;

namespace WarehouseLib.Nodes
{
    public struct Node
    {
        public Point3d Position;

        public int BeamIndex;

        public Node(Point3d point3d, int beamIndex)
        {
            Position = point3d;
            BeamIndex = beamIndex;
        }
    }
}