using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Columns
{
    public class BoundaryColumn : Column
    {
        public BoundaryColumn(Line axis)
        {
            Axis=axis;
        }

        public override List<Column> GenerateColumns(List<Point3d> nodes, Plane plane)
        {
            var columns = new List<Column>();
            foreach (var t in nodes)
            {
                var axis = ConstructAxis(t, plane);
                var column = new BoundaryColumn(axis);
                columns.Add(column);
            }
            
            return columns;
        }
    }
}