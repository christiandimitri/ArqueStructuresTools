using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib
{
    public class FacadeStrap : Strap
    {
        public FacadeStrap(Line axis) : base(axis)
        {
        }

        public override List<Strap> ConstructStrapsAxis(List<Truss> trusses, double distance)
        {
            var facadeStraps = new List<Strap>();
            for (int i = 0; i < trusses.Count - 1; i++)
            {
                var trussA = trusses[i];
                var trussB = trusses[i + 1];
                for (int j = 0; j < trussA.Columns.Count; j++)
                {
                    var columnA = trussA.Columns[j];
                    var columnB = trussB.Columns[j];
                    var parametersA = columnA.Axis.ToNurbsCurve().DivideByLength(distance, true);
                    var parametersB = columnB.Axis.ToNurbsCurve().DivideByLength(distance, true);
                    for (int k = 0; k < parametersA.Length; k++)
                    {
                        var ptA = columnA.Axis.ToNurbsCurve().PointAt(parametersA[k]);
                        var ptB = columnB.Axis.ToNurbsCurve().PointAt(parametersB[k]);
                        Line line = new Line(ptA, ptB);
                        facadeStraps.Add(new FacadeStrap(line));
                    }
                }
            }

            return facadeStraps;
        }
    }
}