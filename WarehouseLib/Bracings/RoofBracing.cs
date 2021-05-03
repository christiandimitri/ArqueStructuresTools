using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;

namespace WarehouseLib.Bracings
{
    public class RoofBracing : Bracing
    {
        public RoofBracing(Line axis, string type) : base(axis, type)
        {
        }

        public List<Bracing> ConstructBracings(List<Truss> trusses, int index, int count,string type)
        {
            var bracings = new List<Bracing>();

            var topBarA = Curve.JoinCurves(trusses[index].TopBars)[0];
            var topBarB = Curve.JoinCurves(trusses[index > 0 ? index - 1 : index + 1].TopBars)[0];

            var parametersA = topBarA.DivideByCount(count, true);
            var parametersB = topBarB.DivideByCount(count, true);

            for (int i = 0; i <= count; i += 2)
            {
                Point3d ptA = topBarA.PointAt(parametersA[i]);
                Point3d ptB = topBarB.PointAt(parametersB[i]);
                Line bar = new Line(ptA, ptB);
                bracings.Add(new RoofBracing(bar, type));

                if (i < count)
                {
                    ptA = new Point3d(topBarA.PointAt(parametersA[i]));
                    ptB = new Point3d(topBarB.PointAt(parametersB[i + 1]));
                }

                bar = new Line(ptA, ptB);
                bracings.Add(new RoofBracing(bar, type));

                if (i > 0)
                {
                    ptA = new Point3d(topBarA.PointAt(parametersA[i]));
                    ptB = new Point3d(topBarB.PointAt(parametersB[i - 1]));
                }

                bar = new Line(ptA, ptB);
                bracings.Add(new RoofBracing(bar, type));
            }

            return bracings;
        }
    }
}