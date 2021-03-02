using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArqueStructuresTools.Straight
{
    class StraightPoints
    {
        public static List<Point3d> UpperBasePoints(Plane plane, int spanOne, int maxHeight)
        {
            List<Point3d> upperBasePoints = new List<Point3d>();
            Point3d pt1 = new Point3d(plane.Origin.X - spanOne / 2, plane.Origin.Y,maxHeight);
            Point3d pt3 = new Point3d(plane.Origin.X + spanOne / 2, plane.Origin.Y, maxHeight);
            Point3d pt2 = new Point3d((pt3.X + pt1.X) / 2, (pt3.Y + pt1.Y) / 2,maxHeight);
            upperBasePoints.Add(pt1);
            upperBasePoints.Add(pt2);
            upperBasePoints.Add(pt3);
            return upperBasePoints;
        }
    }
}
