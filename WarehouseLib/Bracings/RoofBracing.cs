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

        public override List<Bracing> ConstructBracings(List<Truss> trusses, int count, int index)
        {
            var bracings = new List<Bracing>();
            count /= 2;
            for (int j = 0; j < trusses[index].TopBars.Count; j++)
            {
                var curveA = trusses[index].TopBars[j];
                var curveB = trusses[index > 0 ? index - 1 : index + 1].TopBars[j];
                var parametersA = curveA.DivideByCount(count, true);
                var parametersB = curveB.DivideByCount(count, true);

                for (int i = 1; i < count; i++)
                {
                    var ptA = new Point3d(curveA.PointAt(parametersA[i]));
                    var ptB = new Point3d(curveB.PointAt(parametersB[i]));
                    var axis = new Line(ptA, ptB);
                    var bracing = new RoofBracing {Axis = axis};
                    if (bracing.Axis.IsValid) bracings.Add(bracing);
                }
            }

            return bracings;
        }

        public List<Bracing> ConstructWarrenStudsBracings(List<Truss> trusses, int count, int index)
        {
            var bracings = new List<Bracing>();
            count /= 2;
            for (int j = 0; j < trusses[index].TopBars.Count; j++)
            {
                var curveA = trusses[index].TopBars[j];
                var curveB = trusses[index > 0 ? index - 1 : index + 1].TopBars[j];
                var parametersA = curveA.DivideByCount(count, true);
                var parametersB = curveB.DivideByCount(count, true);

                for (int i = 0; i < count; i++)
                {
                    var ptA = new Point3d(curveA.PointAt(parametersA[i]));
                    var ptB = new Point3d(curveB.PointAt(parametersB[i]));
                    var axis = new Line(ptA, ptB);
                    var bracing = new RoofBracing();
                    bracing.Axis = axis;
                    bracings.Add(bracing);
                    ptA = new Point3d(curveA.PointAt(parametersA[i]));
                    ptB = new Point3d(curveB.PointAt(parametersB[i + 1]));
                    axis = new Line(ptA, ptB);

                    if (i % 2 == 0)
                    {
                        bracing = new RoofBracing();
                        bracing.Axis = axis;
                        bracings.Add(bracing);
                    }

                    if (i % 2 == 1)
                    {
                        ptA = new Point3d(curveA.PointAt(parametersA[i + 1]));
                        ptB = new Point3d(curveB.PointAt(parametersB[i]));
                        axis = new Line(ptA, ptB);
                        bracing = new RoofBracing();
                        bracing.Axis = axis;
                        bracings.Add(bracing);
                    }
                }
            }

            bracings.RemoveAt(0);
            return bracings;
        }
    }
}