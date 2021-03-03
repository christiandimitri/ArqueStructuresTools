using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArqueStructuresTools.Monopich
{
    class MonopichPoints
    {
        public static List<Point3d> UpperBasePoints(Plane plane, int spanOne, ref int clHeight, ref int crHeight)
        {
            List<Point3d> superiorBasePoints = new List<Point3d>();
            Point3d pt1 = new Point3d(plane.Origin.X - spanOne / 2, plane.Origin.Y,clHeight);
            Point3d pt3 = new Point3d(plane.Origin.X + spanOne / 2, plane.Origin.Y,crHeight);
            Point3d pt2 = new Point3d((pt3.X + pt1.X) / 2, (pt3.Y + pt1.Y) / 2, (pt3.Z + pt1.Z) / 2);
            superiorBasePoints.Add(pt1);
            superiorBasePoints.Add(pt2);
            superiorBasePoints.Add(pt3);
            return superiorBasePoints;
        }
        public static List<Point3d> ThickBasePoints(List<Vector3d> normals, double offset, List<Point3d> upperBasePoints, double middleHypothenus)
        {
            Point3d tempPoint;
            List<Point3d> thickBasePoints = new List<Point3d>();
            for (int i = 0; i < normals.Count; i++)
            {
                tempPoint = new Point3d(i != 1 ? offset * normals[i] + upperBasePoints[i] : middleHypothenus * normals[i] + upperBasePoints[i]);
                thickBasePoints.Add(tempPoint);
            }
            return thickBasePoints;
        }
    }
}
