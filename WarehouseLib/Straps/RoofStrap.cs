using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib
{
    public class RoofStrap : Strap
    {
        public RoofStrap()
        {
        }

        public List<Strap> ConstructRoofStraps(List<Truss> trusses)
        {
            var roofStraps = new List<Strap>();

            for (var i = 0; i < trusses.Count - 1; i++)
            {
                var trussA = trusses[i];
                var trussB = trusses[i + 1];
                for (var j = 0; j < trusses[i].TopNodes.Count; j++)
                {
                    var ptA = trussA.TopNodes[j];
                    var ptB = trussB.TopNodes[j];
                    var axis = new Line(ptA, ptB);
                    var strap = new RoofStrap
                        {Axis = axis, ProfileOrientationPlane = GetTeklaProfileOrientationPlane(trussA, ptA, 0, false)};
                    roofStraps.Add(strap);
                }
            }

            return roofStraps;
        }

        protected override Plane GetTeklaProfileOrientationPlane(Truss truss, Point3d strapPosition, int index,
            bool isBoundary)
        {
            var beam = Curve.JoinCurves(truss.TopBeam.Axis)[0];
            double t;
            beam.ClosestPoint(strapPosition, out t);
            var tangent=beam.TangentAt(t);
            var strapVector = truss._plane.YAxis;
            var normal = Vector3d.CrossProduct(tangent, strapVector);
            var profilePlane = new Plane(strapPosition, normal);
            return profilePlane;
        }
    }
}