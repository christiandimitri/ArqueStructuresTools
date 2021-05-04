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

        public List<Bracing> ConstructBracings(List<Truss> trusses, int index, int count)
        {
            var bracings = new List<Bracing>();

            var topBarA = Curve.JoinCurves(trusses[index].TopBars)[0];
            var topBarB = Curve.JoinCurves(trusses[index > 0 ? index - 1 : index + 1].TopBars)[0];

            var parametersA = topBarA.DivideByCount(count - 1, true);
            var parametersB = topBarB.DivideByCount(count - 1, true);


            for (int i = 1; i < count - 1; i++)
            {
                var ptA = new Point3d(topBarA.PointAt(parametersA[i]));
                var ptB = new Point3d(topBarB.PointAt(parametersB[i]));
                var bracing = new Line(ptA, ptB);
                if (bracing.IsValid) bracings.Add(new RoofBracing(bracing));
            }

            return bracings;
        }

        public List<Bracing> ConstructWarrenStudsBracings(List<Truss> trusses, int index, int count)
        {
            var bracings = new List<Bracing>();

            var topBarA = Curve.JoinCurves(trusses[index].TopBars)[0];
            var topBarB = Curve.JoinCurves(trusses[index > 0 ? index - 1 : index + 1].TopBars)[0];

            var parametersA = topBarA.DivideByCount(count - 1, true);
            var parametersB = topBarB.DivideByCount(count - 1, true);

            for (int i = 0; i < count - 1; i++)
            {
                var ptA = new Point3d(topBarA.PointAt(parametersA[i]));
                var ptB = new Point3d(topBarB.PointAt(parametersB[i]));
                var line = new Line(ptA, ptB);
                // bracings.Add(new RoofBracing(line));
            }

            var warrenConnections = new List<Bracing>();
            for (int i = 0; i < count - 1; i++)
            {
                var ptA = new Point3d(topBarA.PointAt(parametersA[i]));
                var ptB = new Point3d(topBarB.PointAt(parametersB[i]));
                var line = new Line(ptA, ptB);
                bracings.Add(new RoofBracing(line));
                ptA = new Point3d(topBarA.PointAt(parametersA[i]));
                ptB = new Point3d(topBarB.PointAt(parametersB[i + 1]));
                line = new Line(ptA, ptB);

                if (i % 2 == 0) bracings.Add(new RoofBracing(line));
                if (i % 2 == 1)
                {
                    ptA = new Point3d(topBarA.PointAt(parametersA[i + 1]));
                    ptB = new Point3d(topBarB.PointAt(parametersB[i]));
                    line = new Line(ptA, ptB);
                    bracings.Add(new RoofBracing(line));
                }
            }

            bracings.RemoveAt(0);
            return bracings;
        }
    }
}