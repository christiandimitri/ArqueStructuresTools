using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib
{
    public class RoofStrap : Strap
    {
        public RoofStrap(Line axis) : base(axis)
        {
        }

        public override List<Strap> ConstructStrapsAxis(List<Truss> trusses, double distance)
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
    }
}