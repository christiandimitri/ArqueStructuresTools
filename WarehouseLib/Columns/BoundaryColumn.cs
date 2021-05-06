using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Columns
{
    public class BoundaryColumn : Column
    {
        public Line Axis;

        public BoundaryColumn(Line axis)
        {
            Axis = axis;
        }

        public List<Column> GenerateBoundaryColumns(List<Point3d> nodes, Plane plane)
        {
            var boundaryColumns = new List<Column>();
            var positions = nodes;
            var columns = new List<Column>();
            for (int i = 0; i < positions.Count; i++)
            {
                var column = new Column();
                var boundaryColumn = new BoundaryColumn(column.ConstructColumn(positions[i], plane));
                columns.Add(boundaryColumn);
            }

            boundaryColumns.AddRange(columns);
            return boundaryColumns;
        }
    }
}