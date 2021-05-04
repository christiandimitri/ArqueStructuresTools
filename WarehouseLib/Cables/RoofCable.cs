using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Cables
{
    public class RoofCable : Cable
    {
        public RoofCable(Line axis) : base(axis)
        {
        }

        public List<Cable> ConstructCables(List<Truss> trusses, int index, int count)
        {
            var cables = new List<Cable>();

            var topBarA = Curve.JoinCurves(trusses[index].TopBars)[0];
            var topBarB = Curve.JoinCurves(trusses[index > 0 ? index - 1 : index + 1].TopBars)[0];

            var parametersA = topBarA.DivideByCount(count - 1, true);
            var parametersB = topBarB.DivideByCount(count - 1, true);

            for (int i = 0; i < count; i++)
            {
                Point3d ptA = new Point3d(topBarA.PointAt(parametersA[i]));
                Point3d ptB = (i > 0)
                    ? new Point3d(topBarB.PointAt(parametersB[i - 1]))
                    : new Point3d(topBarA.PointAt(parametersA[i]));
                Line cable = new Line(ptA, ptB);
                if (cable.IsValid) cables.Add(new RoofCable(cable));
                ptB = (i < count-1)
                    ? new Point3d(topBarB.PointAt(parametersB[i + 1]))
                    : new Point3d(topBarA.PointAt(parametersA[i]));
                cable = new Line(ptA, ptB);
                if (cable.IsValid) cables.Add(new RoofCable(cable));
            }
            return cables;
        }
    }
}