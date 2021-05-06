using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib
{
    public class RoofStrap : Strap
    {
        public RoofStrap(Line axis) : base(axis)
        {
        }

        public List<Strap> ConstructRoofStraps(List<Truss> trusses, double distance)
        {
            var roofStraps = new List<Strap>();

            for (var i = 0; i < trusses.Count; i++)
            {
                for (int j = 0; j < trusses[i].TopNodes.Count; j++)
                {
                    if (i < trusses.Count - 1)
                    {
                        Point3d ptA = trusses[i].TopNodes[j];
                        Point3d ptB = trusses[i + 1].TopNodes[j];
                        Line axis = new Line(ptA, ptB);
                        roofStraps.Add(new RoofStrap(axis));
                    }
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