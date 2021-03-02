using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArqueStructuresTools.Duopich
{
    class DuopichPoints
    {
        public static List<Point3d> UpperBasePoints(Plane plane, int spanOne, int spanTwo, int maxHeight, int crHeight, int clHeight)
        {
            List<Point3d> upperBasePoints = new List<Point3d>();
            Point3d pt1 = new Point3d(plane.Origin.X - spanOne, plane.Origin.Y, clHeight);
            Point3d pt3 = new Point3d(plane.Origin.X + spanTwo, plane.Origin.Y,crHeight);
            Point3d pt2 = new Point3d(plane.Origin.X,plane.Origin.Y, maxHeight);
            upperBasePoints.Add(pt1);
            upperBasePoints.Add(pt2);
            upperBasePoints.Add(pt3);
            return upperBasePoints;
        }
    }
}
