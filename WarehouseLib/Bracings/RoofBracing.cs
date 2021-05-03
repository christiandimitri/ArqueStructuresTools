using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;

namespace WarehouseLib.Bracings
{
    public class RoofBracing : Bracing
    {
        public RoofBracing(Line axis) : base(axis)
        {
        }

        public List<Bracing> ConstructBracings(List<Truss> trusses, int index, int count, string type)
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
                if (bar.Length > 0 ||bar.IsValid) bracings.Add(new RoofBracing(bar));
            }

            var warenBars = new List<Bracing>();

            for (int i = 0; i <= count; i++)
            {
                if (i % 2 == 1)
                {
                    Point3d ptA = topBarB.PointAt(parametersA[i]);
                    Point3d ptB = topBarA.PointAt(parametersB[i - 1]);
                    var bar = new Line(ptB, ptA);
                    if (bar.Length > 0 || bar.IsValid) bracings.Add(new RoofBracing(bar));

                    ptA = topBarB.PointAt(parametersA[i]);
                    ptB = topBarA.PointAt(count % 2 == 1 && i == count ? parametersA[i] : parametersB[i + 1]);
                    bar = new Line(ptB, ptA);
                    if (bar.Length > 0 || bar.IsValid) bracings.Add(new RoofBracing(bar));
                }
            }

            // bracings.RemoveAt(0);
            // bracings.RemoveAt(bracings.Count - 1);
            return bracings;
        }
    }
}