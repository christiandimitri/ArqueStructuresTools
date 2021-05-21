using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Columns
{
    public class StaticColumn : Column
    {
        public StaticColumn()
        {
        }

        public List<Column> GenerateColumns(List<Point3d> nodes, Plane plane)
        {
            // TODO: Create columns here using trusses!
            var axisA = ConstructAxis(nodes[0], plane);
            var axisB = ConstructAxis(nodes[2], plane);
            var columnA = new StaticColumn();
            columnA.Axis = axisA;
            var planeA = GetTeklaProfileOrientationPlane(nodes[0], plane, 0);
            columnA.ProfileOrientationPlane = planeA;
            var columnB = new StaticColumn();
            columnB.Axis = axisB;
            var planeB = GetTeklaProfileOrientationPlane(nodes[2], plane, 2);
            columnB.ProfileOrientationPlane = planeB;
            var columns = new List<Column> {columnA, columnB};
            return columns;
        }

        public override Plane GetTeklaProfileOrientationPlane(Point3d node, Plane plane, int index)
        {
            var pt = plane.ClosestPoint(node);
            var vectorX = Vector3d.ZAxis;
            var vectorY = (index == 0) ? -plane.YAxis : plane.YAxis;
            var profilePlane = new Plane(pt, vectorX, vectorY);
            return profilePlane;
        }
    }
}