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

        public List<Strap> ConstructRoofStraps(List<Truss> trusses, double distance)
        {
            var roofStraps = new List<Strap>();

            for (var i = 0; i < trusses.Count-1; i++)
            {
                var trussA = trusses[i];
                var trussB = trusses[i + 1];
                for (var j = 0; j < trusses[i].TopNodes.Count; j++)
                {
                    var ptA = trussA.TopNodes[j];
                    var ptB = trussB.TopNodes[j];
                    var axis = new Line(ptA, ptB);
                    var strap = new RoofStrap {Axis = axis};
                    roofStraps.Add(strap);
                }
            }
            return roofStraps;
        }

        protected override Plane GetTeklaProfileOrientationPlane(Truss truss, Point3d strapPosition, int index, bool isBoundary)
        {
            throw new System.NotImplementedException();
        }
    }
}