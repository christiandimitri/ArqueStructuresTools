﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.Columns;
using WarehouseLib.Connections;
using WarehouseLib.Options;

// ReSharper disable VirtualMemberCallInConstructor

namespace WarehouseLib.Trusses
{
    public class DoublepichTruss : PichedTruss
    {
        public TrussInputs Inputs;

        public DoublepichTruss(Plane plane, TrussInputs inputs) : base(plane, inputs)
        {
            Inputs = inputs;
            GenerateTopBars();
            StaticColumns =
                new List<Column>(new StaticColumn().GenerateColumns(StartingNodes, plane));
            ChangeBaseByType(inputs.BaseType);
            ConstructTruss(inputs.Divisions);
            ChangeArticulationAtColumnsByType(inputs._articulationType);
            ConstructBeams(false, (inputs.BaseType == 1) ? true : false);
        }

        protected override void RecomputeNodes(int index)
        {
            List<Point3d> tempTopList = new List<Point3d>();
            List<Point3d> tempBottomList = new List<Point3d>();
            for (int i = 0; i < TopNodes.Count; i++)
            {
                if (_connectionType == ConnectionType.Warren.ToString())
                {
                    if (i % 2 == 0)
                    {
                        tempTopList.Add(TopNodes[i]);
                    }
                    else if (i % 2 == 1)
                    {
                        tempBottomList.Add(BottomNodes[i]);
                    }
                }
                else if (_connectionType == ConnectionType.WarrenStuds.ToString())
                {
                    tempTopList.Add(TopNodes[i]);
                    if (i % 2 == 1 || i == TopNodes.Count - 1 || i == 0)
                    {
                        tempBottomList.Add(BottomNodes[i]);
                    }
                }
                else if (_connectionType == ConnectionType.Howe.ToString() || _connectionType == ConnectionType.Pratt.ToString())
                {
                    tempTopList.Add(TopNodes[i]);
                    tempBottomList.Add(BottomNodes[i]);
                }
            }

            if (_connectionType == ConnectionType.Warren.ToString())
            {
                tempBottomList.Insert(0, BottomNodes[0]);
                tempBottomList.Add(BottomNodes[BottomNodes.Count - 1]);
                if (!tempBottomList.Contains(BottomNodes[index]))
                {
                    tempBottomList.Insert((index / 2) + 1, BottomNodes[index]);
                }
                else if (!tempTopList.Contains(TopNodes[index]))
                {
                    tempTopList.Insert((index / 2) + 1, TopNodes[index]);
                }
            }

            if (_connectionType == ConnectionType.WarrenStuds.ToString())
            {
                if (!tempBottomList.Contains(BottomNodes[index]))
                {
                    tempBottomList.Insert((index / 2) + 1, BottomNodes[index]);
                }
                else if (!tempTopList.Contains(TopNodes[index]))
                {
                    tempTopList.Insert((index / 2) + 1, TopNodes[index]);
                }
            }

            TopNodes = new List<Point3d>(tempTopList);
            BottomNodes = new List<Point3d>(tempBottomList);
        }

        public override void GenerateTopBars()
        {
            StartingNodes = GetStartingPoints(_plane, Inputs.Width/2, Inputs.Width/2, _height, _maxHeight, _height);
            var barA = new Line(StartingNodes[0], StartingNodes[1]);
            var barB = new Line(StartingNodes[1], StartingNodes[2]);
            TopBeamAxisCurves = new List<Curve> {barA.ToNurbsCurve(), barB.ToNurbsCurve()};
        }

        protected override void GenerateBottomNodes(Curve crv)
        {
            GenerateVerticalBottomNodes(crv);
        }
    }
}