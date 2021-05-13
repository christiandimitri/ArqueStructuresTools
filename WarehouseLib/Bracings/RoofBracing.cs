using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Bracings
{
    public class RoofBracing : Bracing
    {
        public RoofBracing()
        {
        }

        public override List<Bracing> ConstructBracings(List<Truss>trusses, int count, int index)
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
                var axis = new Line(ptA, ptB);
                var bracing = new RoofBracing();
                bracing.Axis = axis;
                if (bracing.Axis.IsValid) bracings.Add(bracing);
            }

            return bracings;
        }
        
        public List<Bracing> ConstructWarrenStudsBracings(List<Truss> trusses, int count, int index)
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
                var axis = new Line(ptA, ptB);
                var bracing = new RoofBracing();
                bracing.Axis = axis;
                bracings.Add(bracing);
                ptA = new Point3d(topBarA.PointAt(parametersA[i]));
                ptB = new Point3d(topBarB.PointAt(parametersB[i + 1]));
                axis = new Line(ptA, ptB);

                if (i % 2 == 0)
                {
                    bracing = new RoofBracing();
                    bracing.Axis = axis;
                    bracings.Add(bracing);
                }
                if (i % 2 == 1)
                {
                    ptA = new Point3d(topBarA.PointAt(parametersA[i + 1]));
                    ptB = new Point3d(topBarB.PointAt(parametersB[i]));
                    axis = new Line(ptA, ptB);
                    bracing = new RoofBracing();
                    bracing.Axis = axis;
                    bracings.Add(bracing);
                }
            }

            bracings.RemoveAt(0);
            return bracings;
        }
    }
}