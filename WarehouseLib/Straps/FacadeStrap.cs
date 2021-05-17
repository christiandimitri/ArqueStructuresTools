using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Straps
{
    public class FacadeStrap : Strap
    {
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
                    for (var k = 0; k < parametersA.Length; k++)
                    {
                        var ptA = columnA.Axis.ToNurbsCurve().PointAt(parametersA[k]);
                        var ptB = columnB.Axis.ToNurbsCurve().PointAt(parametersB[k]);
                        Line axis = new Line(ptA, ptB);
                        var strap = new FacadeStrap {Axis = axis};
                        facadeStraps.Add(strap);
                    }
                }
            }

            return facadeStraps;
        }

        public List<Strap> ConstructStrapsAxisOnBoundaryColumns(List<Truss> trusses, double distance, bool hasBoundary)
        {
            var facadeStraps = new List<Strap>();
            foreach (var truss in trusses)
            {
                for (var j = 0;
                    (hasBoundary == true) ? j < truss.BoundaryColumns.Count - 1 : j < 1;
                    j++)
                {
                    var columnA = (hasBoundary == true) ? truss.BoundaryColumns[j] : truss.StaticColumns[j];
                    var columnB = (hasBoundary == true) ? truss.BoundaryColumns[j + 1] : truss.StaticColumns[j + 1];

                    var parametersA = columnA.Axis.ToNurbsCurve().DivideByLength(distance, true);
                    var parametersB = columnB.Axis.ToNurbsCurve().DivideByLength(distance, true);
                    var tempArray = new int[] {parametersA.Length, parametersB.Length};
                    for (var k = 0; k < tempArray.Min(); k++)
                    {
                        var ptA = columnA.Axis.ToNurbsCurve().PointAt(parametersA[k]);
                        var ptB = columnB.Axis.ToNurbsCurve().PointAt(parametersB[k]);
                        var axis = new Line(ptA, ptB);
                        var strap = new FacadeStrap {Axis = axis};
                        facadeStraps.Add(strap);
                    }
                }
            }

            return facadeStraps;
        }

        public override List<Strap> ConstructStraps(List<Strap> strapList)
        {
            return strapList;
        }
    }
}