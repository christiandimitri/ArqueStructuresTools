using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Bracings;

namespace WarehouseLib.Cables
{
    public class Cable
    {
        public Line Axis;
        public Cable(Line axis)
        {
            Axis = axis;
        }
        public List<Bracing> ConstructCables(List<Truss> trusses, int index, int count, string type)
        {
            var bracings = new List<Bracing>();

            var topBarA = Curve.JoinCurves(trusses[index].TopBars)[0];
            var topBarB = Curve.JoinCurves(trusses[index > 0 ? index - 1 : index + 1].TopBars)[0];

            var parametersA = topBarA.DivideByCount(count, true);
            var parametersB = topBarB.DivideByCount(count, true);

            for (int i = 0; i <= count; i++)
            {
                Point3d ptA = topBarA.PointAt(parametersA[i]);
                Point3d ptB = topBarB.PointAt(parametersB[i]);
                Line bar = new Line(ptA, ptB);
                bracings.Add(new RoofBracing(bar));

                if (i > 0 && i % 2 == 0)
                {
                    ptA = new Point3d(topBarA.PointAt(parametersA[i]));
                    ptB = new Point3d(topBarB.PointAt(parametersB[i - 1]));
                }

                bar = new Line(ptA, ptB);
                bracings.Add(new RoofBracing(bar));
                
                if (i <count && i % 2 == 0)
                {
                    ptA = new Point3d(topBarA.PointAt(parametersA[i]));
                    ptB = new Point3d(topBarB.PointAt(parametersB[i + 1]));
                }

                bar = new Line(ptA, ptB);
                bracings.Add(new RoofBracing(bar));
            }

            bracings.RemoveAt(0);
            bracings.RemoveAt(bracings.Count-1);
            return bracings;
        }
    }
}