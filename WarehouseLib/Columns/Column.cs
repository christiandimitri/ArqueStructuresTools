using Rhino.Geometry;

namespace WarehouseLib.Columns
{
    public class Column
    {
        public Column()
        {
        }

        public Line Axis { get; set; }

        public Line ConstructColumn(Point3d node, Plane plane)
        {
            var axis = new Line(plane.ClosestPoint(node), node);
            return axis;
        }
    }
}