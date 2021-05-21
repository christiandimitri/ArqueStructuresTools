﻿using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Columns
{
    public class BoundaryColumn : Column
    {
        public BoundaryColumn()
        {
            
        }


        public List<Column> GenerateColumns(List<Point3d> nodes, Plane plane, int index)
        {
            var columns = new List<Column>();
            foreach (var t in nodes)
            {
                var axis = ConstructAxis(t, plane);
                var column = new BoundaryColumn {Axis = axis};
                column.ProfileOrientitionPlane = GetColumnsOrientationPlane(t, plane, index);
                columns.Add(column);
            }
            
            return columns;
        }

        public override Plane GetColumnsOrientationPlane(Point3d node, Plane plane, int index)
        {
            var pt = plane.ClosestPoint(node);
            var vectorX = Vector3d.ZAxis;
            var vectorY = (index == 0) ? -plane.YAxis : plane.YAxis;
            var profilePlane = new Plane(pt, vectorX, vectorY);
            return profilePlane;
        }

    }
}