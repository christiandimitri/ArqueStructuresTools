using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.Articulations;
using WarehouseLib.Beams;
using WarehouseLib.Columns;
using WarehouseLib.Options;
using WarehouseLib.Trusses;

namespace WarehouseLib.Utilities
{
    public class KarambaTruss
    {
        private readonly Truss _truss;
        public string _porticoType;

        private List<Column> _trussStaticColumns;
        private List<Column> _trussBoundaryColumns;

        public List<Column> Karamba3DStaticColumns;
        public List<Column> Karamba3DBoundaryColumns;

        public List<Point3d> _trussTopNodes;
        public List<Point3d> _trussBottomNodes;

        private Beam _trussTopBeam;
        private Beam _trussBottomBeam;
        private Beam _trussIntermediateBeam;

        public Beam Karamba3DTopBeams;
        public Beam Karamba3DBottomBeams;
        public Beam Karamba3DIntermediateBeams;

        public KarambaTruss(Truss truss)
        {
            // extract truss properties and components
            _truss = truss;
            _porticoType = _truss._porticoType;
            // get truss nodes
            GetTrussNodes();
            // get truss columns
            GetTrussColumns();
            // get truss beams 
            GetTrussBeams();
            // compute karamba properties for each component and add it
            // compute karamba3D columns and properties
            GetKaramba3DColumns();

            // TODO
            // compute karamba3D beams and properties
            GetKaramba3DBeams();
        }

        // truss nodes
        private void GetTrussNodes()
        {
            _trussTopNodes = _truss.TopNodes != null
                ? new List<Point3d>(_truss.TopNodes)
                : new List<Point3d>();
            if (_porticoType == PorticoType.Truss.ToString() || _truss.BottomBeam.Axis != null)
            {
                _trussBottomNodes = _truss.BottomNodes ?? new List<Point3d>();
            }
        }

        // truss columns
        private void GetTrussColumns()
        {
            _trussStaticColumns = _truss.StaticColumns != null
                ? new List<Column>(_truss.StaticColumns)
                : new List<Column>();
            if (_porticoType != PorticoType.Truss.ToString())
            {
                _trussBoundaryColumns = _truss.BoundaryColumns != null
                    ? new List<Column>(_truss.BoundaryColumns)
                    : new List<Column>();
            }
        }

        // truss beams
        private void GetTrussBeams()
        {
            _trussTopBeam = _truss.TopBeam ?? new TopBeam();
            if (_porticoType == PorticoType.Truss.ToString())
            {
                if (_truss.BottomBeam.Axis  !=null)
                {
                    
                }
                _trussBottomBeam = _truss.BottomBeam ?? new BottomBeam();

                _trussIntermediateBeam = _truss.IntermediateBeams ?? new IntermediateBeams();
            }
        }

        // Get Karamba3D all Columns types and properties
        private void GetKaramba3DColumns()
        {
            // Get Karamba3D static columns with buckling length
            var tempStaticColumns = new List<Column>();
            var tempBoundaryColumns = new List<Column>();
            if ((_truss._articulationType == ArticulationType.Rigid.ToString() &&
                 _porticoType == PorticoType.Truss.ToString()))
            {
                for (int i = 0; i < _trussStaticColumns.Count; i++)
                {
                    var column = _trussStaticColumns[i];
                    var bottomNode = _trussBottomNodes[i == 0 ? 0 : _trussBottomNodes.Count - 1];
                    double t;
                    column.Axis.ToNurbsCurve().ClosestPoint(bottomNode, out t);
                    var cl = column.Axis.ToNurbsCurve().Split(t);
                    foreach (var axis in cl)
                    {
                        var tempStaticColumn = new StaticColumn();
                        tempStaticColumn.Axis =
                            new Line(axis.ToNurbsCurve().PointAtStart, axis.ToNurbsCurve().PointAtEnd);
                        var tempCl = GetColumnBucklingLength(tempStaticColumn);
                        tempStaticColumn.BucklingLengths = tempCl.BucklingLengths;
                        tempStaticColumns.Add(tempStaticColumn);
                    }
                }
            }
            else
            {
                foreach (var column in _trussStaticColumns)
                {
                    var tempStaticColumn = new BoundaryColumn();
                    tempStaticColumn.Axis =
                        new Line(column.Axis.ToNurbsCurve().PointAtStart, column.Axis.ToNurbsCurve().PointAtEnd);
                    var tempCl = GetColumnBucklingLength(tempStaticColumn);
                    tempStaticColumn.BucklingLengths = tempCl.BucklingLengths;
                    tempStaticColumns.Add(tempStaticColumn);
                }
            }

            Karamba3DStaticColumns = tempStaticColumns;


            if (_porticoType == PorticoType.Portico.ToString())
            {
                foreach (var column in _trussBoundaryColumns)
                {
                    var tempBoundaryColumn = new BoundaryColumn();
                    tempBoundaryColumn.Axis =
                        new Line(column.Axis.ToNurbsCurve().PointAtStart, column.Axis.ToNurbsCurve().PointAtEnd);
                    var tempCl = GetColumnBucklingLength(tempBoundaryColumn);
                    tempBoundaryColumn.BucklingLengths = tempCl.BucklingLengths;
                    tempBoundaryColumns.Add(tempBoundaryColumn);
                }
            }

            Karamba3DBoundaryColumns = tempBoundaryColumns;
        }

        // Get Karamba3D all Beams types and properties

        private void GetKaramba3DBeams()
        {
            Karamba3DTopBeams = new TopBeam();
            Karamba3DTopBeams.Axis = new List<Curve>(DivideBeamBetweenNodes(_truss.TopNodes, _trussTopBeam));
            Karamba3DTopBeams.BucklingLengths =
                Karamba3DTopBeams.ComputeBucklingLengths(Karamba3DTopBeams, false, double.NaN);
            if (_porticoType == PorticoType.Truss.ToString())
            {
                Karamba3DBottomBeams = new BottomBeam();
                Karamba3DBottomBeams.Axis =
                    new List<Curve>(DivideBeamBetweenNodes(_truss.BottomNodes, _trussBottomBeam));
                // Karamba3DBottomBeams.BucklingLengths= Karamba3DBottomBeams.ComputeBucklingLengths(Karamba3DBottomBeams, false, double.NaN);
                Karamba3DIntermediateBeams = _trussIntermediateBeam;
            }
        }

        // Return a column with its buckling lengths
        private Column GetColumnBucklingLength(Column column)
        {
            BucklingLengths.BucklingLengths bucklingLength;
            if (column is StaticColumn)
            {
                bucklingLength = column.ComputeBucklingLengths(column, true, _truss._facadeStrapsDistance);
            }
            else
            {
                bucklingLength = column.ComputeBucklingLengths(column, true, _truss._facadeStrapsDistance);
            }

            column.BucklingLengths = bucklingLength;
            return column;
        }

        // Return a beam with its buckling lengths


        private List<Curve> DivideBeamBetweenNodes(List<Point3d> nodes, Beam beam)
        {
            var axis = new List<Curve>();
            var tempBaseAxisList = Curve.JoinCurves(beam.Axis);


            var baseAxis = tempBaseAxisList[0];
            for (var i = 1; i < nodes.Count - 1; i++)
            {
                var node = nodes[i];
                double t;
                baseAxis.ClosestPoint(node, out t);
                var tempList = baseAxis.ToNurbsCurve().Split(t).ToList();
                axis.Add(tempList[0]);
                baseAxis = tempList[1];
                if (i == nodes.Count - 1)
                {
                    axis.Add(baseAxis);
                }
            }

            axis.Add(baseAxis);


            return axis;
        }
    }
}