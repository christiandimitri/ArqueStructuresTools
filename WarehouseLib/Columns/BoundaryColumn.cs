using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Columns
{
    public class BoundaryColumn : Column
    {
        public BoundaryColumn(Line axis) : base(axis)
        {
        }

        public List<Column> GenerateBoundaryColumns(List<Point3d> nodes, Plane plane)
        {
            var boundaryColumns = new List<Column>();
            var positions = nodes;
            var columns = new List<Column>();


            boundaryColumns.AddRange(columns);
            return boundaryColumns;
        }
    }
}