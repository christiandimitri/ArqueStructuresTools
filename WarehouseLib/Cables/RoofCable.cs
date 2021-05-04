using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rhino.Geometry;

namespace WarehouseLib.Cables
{
    public class RoofCable : Cable
    {
        public int Count;
        public int Index;

        public RoofCable(Line axis, int index, int count) : base(axis)
        {
            Count = count;
            Index = index;
        }

        public override List<Cable> ConstructCables(List<Truss> trusses)
        {
            var cables = new List<Cable>();

            var topBarA = Curve.JoinCurves(trusses[Index].TopBars)[0];
            var topBarB = Curve.JoinCurves(trusses[Index > 0 ? Index - 1 : Index + 1].TopBars)[0];

            var parametersA = topBarA.DivideByCount(Count - 1, true);
            var parametersB = topBarB.DivideByCount(Count - 1, true);

            for (int i = 0; i < Count; i++)
            {
                Point3d ptA = new Point3d(topBarA.PointAt(parametersA[i]));
                Point3d ptB = (i > 0)
                    ? new Point3d(topBarB.PointAt(parametersB[i - 1]))
                    : new Point3d(topBarA.PointAt(parametersA[i]));
                Line cable = new Line(ptA, ptB);
                if (cable.IsValid) cables.Add(new RoofCable(cable, Count, Index));
                ptB = (i < Count - 1)
                    ? new Point3d(topBarB.PointAt(parametersB[i + 1]))
                    : new Point3d(topBarA.PointAt(parametersA[i]));
                cable = new Line(ptA, ptB);
                if (cable.IsValid) cables.Add(new RoofCable(cable, Count, Index));
            }

            return cables;
        }
    }
}