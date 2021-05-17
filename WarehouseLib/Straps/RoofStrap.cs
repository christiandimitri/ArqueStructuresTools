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

            for (var i = 0; i < trusses.Count; i++)
            {
                for (var j = 0; j < trusses[i].TopNodes.Count; j++)
                {
                    if (i >= trusses.Count - 1) continue;
                    var ptA = trusses[i].TopNodes[j];
                    var ptB = trusses[i + 1].TopNodes[j];
                    var axis = new Line(ptA, ptB);
                    var strap = new RoofStrap {Axis = axis};
                    roofStraps.Add(strap);
                }
            }
            return roofStraps;
        }

        public override List<Strap> ConstructStraps(List<Strap> strapMethod)
        {
            return strapMethod;
        }
    }
}