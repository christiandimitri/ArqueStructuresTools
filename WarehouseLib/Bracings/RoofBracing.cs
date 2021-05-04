using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;

namespace WarehouseLib.Bracings
{
    public class RoofBracing : Bracing
    {
        public int Count;
        public int Index;

        public RoofBracing(Line axis, int index, int count) : base(axis)
        {
            Count = count;
            Index = index;
        }

        public override List<Bracing> ConstructBracings(List<Truss> trusses)
        {
            var bracings = new List<Bracing>();

            var topBarA = Curve.JoinCurves(trusses[Index].TopBars)[0];
            var topBarB = Curve.JoinCurves(trusses[Index > 0 ? Index - 1 : Index + 1].TopBars)[0];

            var parametersA = topBarA.DivideByCount(Count - 1, true);
            var parametersB = topBarB.DivideByCount(Count - 1, true);


            for (int i = 1; i < Count - 1; i++)
            {
                var ptA = new Point3d(topBarA.PointAt(parametersA[i]));
                var ptB = new Point3d(topBarB.PointAt(parametersB[i]));
                var bracing = new Line(ptA, ptB);
                if (bracing.IsValid) bracings.Add(new RoofBracing(bracing, Index, Count));
            }

            return bracings;
        }

        public List<Bracing> ConstructWarrenStudsBracings(List<Truss> trusses)
        {
            var bracings = new List<Bracing>();

            var topBarA = Curve.JoinCurves(trusses[Index].TopBars)[0];
            var topBarB = Curve.JoinCurves(trusses[Index > 0 ? Index - 1 : Index + 1].TopBars)[0];

            var parametersA = topBarA.DivideByCount(Count - 1, true);
            var parametersB = topBarB.DivideByCount(Count - 1, true);

            for (int i = 0; i < Count - 1; i++)
            {
                var ptA = new Point3d(topBarA.PointAt(parametersA[i]));
                var ptB = new Point3d(topBarB.PointAt(parametersB[i]));
                var line = new Line(ptA, ptB);
                bracings.Add(new RoofBracing(line, Index, Count));
                ptA = new Point3d(topBarA.PointAt(parametersA[i]));
                ptB = new Point3d(topBarB.PointAt(parametersB[i + 1]));
                line = new Line(ptA, ptB);

                if (i % 2 == 0) bracings.Add(new RoofBracing(line, Index, Count));
                if (i % 2 == 1)
                {
                    ptA = new Point3d(topBarA.PointAt(parametersA[i + 1]));
                    ptB = new Point3d(topBarB.PointAt(parametersB[i]));
                    line = new Line(ptA, ptB);
                    bracings.Add(new RoofBracing(line, Index, Count));
                }
            }

            bracings.RemoveAt(0);
            return bracings;
        }
    }
}