using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArqueStructuresTools.Monopich
{
    class MonopichCurves
    {
        public static List<Curve> UpperBaseCurves(List<Point3d> points)
        {
            List<Curve> baseCurves = new List<Curve>();
            Line line1 = new Line(points[0], points[1]);
            Line line2 = new Line(points[2], points[1]);
            baseCurves.Add(line1.ToNurbsCurve());
            baseCurves.Add(line2.ToNurbsCurve());
            return baseCurves;
        }
        public static List<Curve> LowerBaseThickCurves()
        {
            throw new NotImplementedException();
        }
    }
}
