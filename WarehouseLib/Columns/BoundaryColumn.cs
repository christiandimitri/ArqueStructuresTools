using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Columns
{
    public class BoundaryColumn : Column
    {
        public BoundaryColumn()
        {
        }

        public override List<Column> GenerateColumns(List<Point3d> nodes, Plane plane)
        {
            var columns = new List<Column>();
            foreach (var t in nodes)
            {
                var axis = ConstructAxis(t, plane);
                var column = new BoundaryColumn {Axis = axis};
                columns.Add(column);
            }
            
            return columns;
        }

        public override List<Plane> GenerateOrientatonPlanes(List<Curve> axis)
        {
            throw new System.NotImplementedException();
        }
    }
}