using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.Profiles;
using WarehouseLib.Trusses;

namespace WarehouseLib.Columns
{
    public abstract class Column
    {
        public Line Axis;
        public Plane ProfileOrientationPlane;

        public ProfileDescription Profile;

        public BucklingLengths.BucklingLengths BucklingLengths;

        protected Column()
        {
        }

        protected Line ConstructAxis(Point3d node, Plane plane)
        {
            var axis = new Line(plane.ClosestPoint(node), node);
            return axis;
        }

        public abstract Plane GetTeklaProfileOrientationPlane(Point3d node, Plane plane, int index);
        public BucklingLengths.BucklingLengths ComputeBucklingLengths(Column column, bool straps, double strapsDistance)
        {
            var buckling = new BucklingLengths.BucklingLengths();
            var zBuckling = column.Axis.Length;
            var yBuckling = 0.7 * column.Axis.Length;
            if (straps)
            {
                yBuckling = strapsDistance;
            }
            buckling.BucklingY = yBuckling;
            buckling.BucklingZ = zBuckling;
            return buckling;
        }
    }
}