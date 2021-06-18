using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.Beams;
using WarehouseLib.Bracings;
using WarehouseLib.Cables;
using WarehouseLib.Crosses;
using WarehouseLib.Trusses;
using WarehouseLib.Warehouses;

namespace WarehouseLib.Utilities
{
    public class KarambaWarehouse
    {
        public Warehouse _warehouse { get; set; }
        public List<KarambaTruss> KarambaTrusses { get; set; }
        public List<Truss> Trusses { get; set; }
        public List<Strap> RoofStraps { get; set; }

        public List<Strap> FacadeStrapsX { get; set; }
        public List<Strap> FacadeStrapsY { get; set; }
        public List<Cable> RoofCables { get; set; }
        public List<Bracing> RoofBracings { get; set; }
        public List<Cable> FacadeCables { get; set; }
        public List<Bracing> ColumnsBracings { get; set; }
        public List<Cross> Crosses { get; set; }

        public KarambaWarehouse(Warehouse warehouse)
        {
            _warehouse = warehouse;
            Trusses = _warehouse.Trusses;
            RoofStraps = warehouse.RoofStraps;
            FacadeStrapsX = warehouse.FacadeStrapsX;
            FacadeStrapsY = warehouse.FacadeStrapsY;
            RoofCables = warehouse.RoofCables;
            RoofBracings = warehouse.RoofBracings;
            FacadeCables = warehouse.FacadeCables;
            ColumnsBracings = warehouse.ColumnsBracings;
            Crosses = warehouse.Crosses;
            ReplaceTrussesWithKaramabaTrusses();
        }

        private void ReplaceTrussesWithKaramabaTrusses()
        {
            var karambaTrusses = new List<KarambaTruss>();
            for (var i = 0; i < _warehouse.Trusses.Count; i++)
            {
                var truss = _warehouse.Trusses[i];
                var karambaTruss = new KarambaTruss(truss);
                karambaTrusses.Add(karambaTruss);
            }

            // _warehouse.Trusses = new List<Truss>();
            KarambaTrusses = new List<KarambaTruss>(karambaTrusses);
            ReplaceTrussBottomBeamBucklingLengthByDistanceBetweenStAndre();
        }

        private void ReplaceTrussBottomBeamBucklingLengthByDistanceBetweenStAndre()
        {
            var lengthList = new List<double>();
            // var indices = new List<int>();
            int i = 0;
            int index = 0;
            int k = 0;
            for (i = _warehouse._warehouseOptions.HasBoundary ? 1 : 0;
                _warehouse._warehouseOptions.HasBoundary ? i < KarambaTrusses.Count - 1 : i < KarambaTrusses.Count;
                i++)
            {
                var karambaTruss = KarambaTrusses[i];
                var truss = _warehouse.Trusses[i];
                var tempCloud = new PointCloud(karambaTruss._trussBottomNodes);
                tempCloud.RemoveAt(0);
                if (karambaTruss.StAndresBottomNodes != null)
                {
                    var tempStAndreBottomNodes = new List<Point3d>(karambaTruss.StAndresBottomNodes);
                    tempStAndreBottomNodes.Add(
                        karambaTruss._trussBottomNodes[karambaTruss._trussBottomNodes.Count - 1]);
                    var tempAxisList = new List<Curve>();
                    var tempKarambaAxisList = new List<Curve>(karambaTruss.Karamba3DBottomBeams.Axis);

                    for (k = 0; k < tempStAndreBottomNodes.Count; k++)
                    {
                        var lengths = new List<double>();
                        var tempNode = tempStAndreBottomNodes[k];
                        var tempIndex = tempCloud.ClosestPoint(tempNode);
                        index = tempIndex;
                        lengths = GetBottomBeamBucklingLengths(index, tempKarambaAxisList);
                    }

                    // store lengths until index met

                    // get the sum of the lengths


                    // replace bucklingY lengths with the sum of the lengths
                    // keep old bucklingZ lengths value


                    // add the buckling length to the beam
                }
            }
        }

        private List<double> GetBottomBeamBucklingLengths(int index, List<Curve> tempKarambaAxisList)
        {
            var lengths = new List<double>();
            var tempList = new List<Curve>();
            for (var j = 0; j <= index; j++)
            {
                var axis = tempKarambaAxisList[j];
                tempList.Add(axis);
            }

            var length = Curve.JoinCurves(tempList)[0].GetLength();

            for (int i = 0; i <= index; i++)
            {
                lengths.Add(length);
            }
            
            return lengths;
        }

        // private void AddElementsInListUntilIndex(List<Curve> oldList, List<Curve> newList, int index)
        // {
        //     for (int i = 0; i < UPPER; i++)
        //     {
        //         
        //     }
        // }
        private void RemoveFromIndexAndBackwards(List<Curve> axis, int index)
        {
            for (int i = 0; i < index; i++)
            {
                axis.RemoveAt(i);
            }
        }

        private List<int> ReturnIncludedIndices(List<int> indices)
        {
            var tempList = new List<int>();
            for (int i = 0; i < indices.Count - 1; i++)
            {
                var indiceA = indices[i];
                var indiceB = indices[i + 1];
                for (int j = indiceA; j < indiceB; j++)
                {
                    tempList.Add(j);
                }
            }

            tempList.Add(indices[indices.Count - 1]);

            return tempList;
        }
    }
}