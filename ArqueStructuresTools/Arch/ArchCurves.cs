using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArqueStructuresTools.Arch
{
    class ArchCurves
    {
        public static List<Curve> UpperBaseCurves(List<Point3d> points)
        {
            List<Curve> arcs = new List<Curve>();
            Arc arc = new Arc(points[0], points[1], points[2]);
            double[] parameters = arc.ToNurbsCurve().DivideByCount(4, true);
            List<Point3d> tempList = new List<Point3d>();
            for(int i=0; i < parameters.Length; i++)
            {
                tempList.Add(arc.ToNurbsCurve().PointAt(parameters[i]));
            }
            Arc arc1 = new Arc(tempList[0], tempList[1], tempList[2]);
            arcs.Add(arc1.ToNurbsCurve());
            Arc arc2 = new Arc(tempList[4], tempList[3], tempList[2]);
            arcs.Add(arc2.ToNurbsCurve());
            return arcs;
        }
        public static List<Curve> LowerBaseThickCurves()
        {
            throw new NotImplementedException();
        }
    }
}
