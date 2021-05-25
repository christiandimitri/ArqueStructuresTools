using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Columns
{
    public class BoundaryColumn : Column
    {
        public BoundaryColumn()
        {
        }


        public List<Column> GenerateColumns(List<Point3d> nodes, Plane plane, int index)
        {
            var columns = new List<Column>();
            foreach (var t in nodes)
            {
                var axis = ConstructAxis(t, plane);
                var column = new BoundaryColumn
                {
                    Axis = axis,
                    ProfileOrientationPlane = GetTeklaProfileOrientationPlane(t, plane, index)
                };
                columns.Add(column);
            }

            return columns;
        }

        public override Plane GetTeklaProfileOrientationPlane(Point3d node, Plane plane, int index)
        {
            var pt = plane.ClosestPoint(node);
            var normal = (index > 0) ? plane.YAxis : -plane.YAxis;
            var profilePlane = new Plane(pt, normal);
            return profilePlane;
        }
    }
}