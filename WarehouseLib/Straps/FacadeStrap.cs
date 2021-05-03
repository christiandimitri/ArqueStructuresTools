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

        public List<Strap> ConstructStrapsAxisOnStaticColumns(List<Truss> trusses, double distance)
        {
            var facadeStraps = new List<Strap>();
            for (int i = 0; i < trusses.Count - 1; i++)
            {
                var trussA = trusses[i];
                var trussB = trusses[i + 1];
                for (int j = 0; j < trussA.StaticColumns.Count; j++)
                {
                    var columnA = trussA.StaticColumns[j];
                    var columnB = trussB.StaticColumns[j];
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

        public List<Strap> ConstructStrapsAxisOnBoundaryColumns(List<Truss> trusses, double distance)
        {
            var facadeStraps = new List<Strap>();
            for (int i = 0; i < trusses.Count; i++)
            {
                var trussA = trusses[i];
                for (int j = 0; j < trussA.BoundaryColumns.Count-1; j++)
                {
                    var columnA = trussA.BoundaryColumns[j];
                    var columnB = trussA.BoundaryColumns[j+1];
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