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

        public BucklingLengths.BucklingLengths ComputeBucklingLengths(Column column, bool straps, double strapsDistance, bool hasBucklingLength)
        {
            var buckling = new BucklingLengths.BucklingLengths();
            var zBuckling = column.Axis.Length;
            var yBuckling = 1 * column.Axis.Length;
            if (straps && hasBucklingLength)
            {
                yBuckling = strapsDistance;
            }
            else if (!hasBucklingLength)
            {
                yBuckling = 0.0;
                zBuckling = 0.0;
            }

            buckling.BucklingY = yBuckling;
            buckling.BucklingZ = zBuckling;
            return buckling;
        }
    }
}