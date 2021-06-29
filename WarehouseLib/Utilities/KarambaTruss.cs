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
        public PorticoType _porticoType;

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
        public List<Point3d> StAndresBottomNodes;
        public List<int> StAndresBottomNodesIndices;

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

            // set st andres nodes from each the truss bottom nodes
            SetStAndresCrossBottomNodes();
        }

        private void SetStAndresCrossBottomNodes()
        {
            if (_truss.StAndresBottomNodes != null && _truss._porticoType == PorticoType.Truss)
            {
                StAndresBottomNodes = new List<Point3d>(_truss.StAndresBottomNodes);
                StAndresBottomNodesIndices = new List<int>(_truss.StAndresBottomNodesIndices);
            }
        }

        // truss nodes
        private void GetTrussNodes()
        {
            _trussTopNodes = _truss.TopNodes ?? new List<Point3d>();
            _trussBottomNodes = _truss.BottomNodes ?? new List<Point3d>();
        }

        // truss columns
        private void GetTrussColumns()
        {
            _trussStaticColumns = _truss.StaticColumns ?? new List<Column>();

            _trussBoundaryColumns = _truss.BoundaryColumns ?? new List<Column>();
        }

        // truss beams
        private void GetTrussBeams()
        {
            if (_truss.TopBeamAxis != null)
            {
                _trussTopBeam = _truss.TopBeam;
            }

            if (_porticoType == PorticoType.Portico) return;
            _trussBottomBeam = _truss.BottomBeam;

            _trussIntermediateBeam = _truss.IntermediateBeams;
        }

        // Get Karamba3D all Columns types and properties
        private void GetKaramba3DColumns()
        {
            // Get Karamba3D static columns with buckling length
            var tempStaticColumns = new List<Column>();
            var tempBoundaryColumns = new List<Column>();
            if ((_truss._articulationType == ArticulationType.Rigid.ToString() &&
                 _truss.BottomBeam.Axis != null))
            {
                for (int i = 0; i < _trussStaticColumns.Count; i++)
                {
                    var column = _trussStaticColumns[i];
                    var bottomNode = _trussBottomNodes[i == 0 ? 0 : _trussBottomNodes.Count - 1];
                    double t;
                    column.Axis.ToNurbsCurve().ClosestPoint(bottomNode, out t);
                    var tempCl = GetColumnBucklingLength(column, true);
                    var cl = column.Axis.ToNurbsCurve().Split(t);
                    for (var j = 0; j < cl.Length; j++)
                    {
                        var axis = cl[j];
                        var tempStaticColumn = new StaticColumn();
                        tempStaticColumn.Axis =
                            new Line(axis.ToNurbsCurve().PointAtStart, axis.ToNurbsCurve().PointAtEnd);
                        if (j == 0) tempStaticColumn.BucklingLengths = tempCl.BucklingLengths;
                        else
                        {
                            tempStaticColumn.BucklingLengths = GetColumnBucklingLength(column, false).BucklingLengths;
                        }

                        tempStaticColumns.Add(tempStaticColumn);
                    }
                }
            }
            else
            {
                for (var i = 0; i < _trussStaticColumns.Count; i++)
                {
                    var column = _trussStaticColumns[i];
                    var tempStaticColumn = new StaticColumn();
                    tempStaticColumn.Axis =
                        new Line(column.Axis.ToNurbsCurve().PointAtStart, column.Axis.ToNurbsCurve().PointAtEnd);
                    var tempCl = GetColumnBucklingLength(tempStaticColumn, true);
                    tempStaticColumn.BucklingLengths = tempCl.BucklingLengths;
                    tempStaticColumns.Add(tempStaticColumn);
                }
            }

            Karamba3DStaticColumns = tempStaticColumns;


            if (_truss.BoundaryColumns != null)
            {
                for (var i = 0; i < _trussBoundaryColumns.Count; i++)
                {
                    var column = _trussBoundaryColumns[i];
                    var tempBoundaryColumn = new BoundaryColumn();
                    tempBoundaryColumn.Axis =
                        new Line(column.Axis.ToNurbsCurve().PointAtStart, column.Axis.ToNurbsCurve().PointAtEnd);
                    var tempCl = GetColumnBucklingLength(tempBoundaryColumn, true);
                    tempBoundaryColumn.BucklingLengths = tempCl.BucklingLengths;
                    tempBoundaryColumns.Add(tempBoundaryColumn);
                }
            }

            Karamba3DBoundaryColumns = tempBoundaryColumns;
        }

        // Get Karamba3D all Beams types and properties
        private void GetKaramba3DBeams()
        {
            Karamba3DTopBeams = new Beam();
            Karamba3DIntermediateBeams = new Beam();
            Karamba3DBottomBeams = new Beam();
            if (_trussTopBeam.Axis != null)
            {
                var axisCurves = _truss.TopBeamAxis;
                var tempAxis = new List<BeamAxis>();
                for (int i = 0; i < axisCurves.Count; i++)
                {
                    tempAxis.Add(new BeamAxis(axisCurves[i].AxisCurve));
                }

                Karamba3DTopBeams.Axis = tempAxis;
                Karamba3DTopBeams.Position = _trussTopBeam.Position;
                if (_porticoType == PorticoType.Truss)
                {
                    Karamba3DTopBeams.BucklingLengths =
                        Karamba3DTopBeams.ComputeTrussBeamBucklingLengthsBetweenNodes(Karamba3DTopBeams, true);
                }
                else
                {
                    Karamba3DTopBeams.BucklingLengths =
                        Karamba3DTopBeams.ComputePorticoBeamBucklingLengths(Karamba3DTopBeams, true);
                }
            }

            if (_trussBottomBeam.Axis != null)
            {
                var axisCurves = _trussBottomBeam.Axis;
                var tempAxis = new List<BeamAxis>();
                for (int i = 0; i < axisCurves.Count; i++)
                {
                    tempAxis.Add(new BeamAxis(axisCurves[i].AxisCurve));
                }

                Karamba3DBottomBeams.Axis = tempAxis;
                // Karamba3DBottomBeams = _trussBottomBeam;
                Karamba3DBottomBeams.BucklingLengths =
                    Karamba3DBottomBeams.ComputeTrussBeamBucklingLengthsBetweenNodes(Karamba3DBottomBeams, true);
                Karamba3DBottomBeams.Position = _trussBottomBeam.Position;
            }

            if (_trussIntermediateBeam.Axis != null)
            {
                Karamba3DIntermediateBeams = _trussIntermediateBeam;
                Karamba3DIntermediateBeams.BucklingLengths =
                    Karamba3DIntermediateBeams.ComputeTrussBeamBucklingLengthsBetweenNodes(Karamba3DIntermediateBeams,
                        false);
                Karamba3DIntermediateBeams.Position = _trussIntermediateBeam.Position;
            }
        }

        // Return a column with its buckling lengths
        private Column GetColumnBucklingLength(Column column, bool hasBucklingLength)
        {
            BucklingLengths.BucklingLengths bucklingLength =
                column.ComputeBucklingLengths(column, true, _truss._facadeStrapsDistance, hasBucklingLength);

            column.BucklingLengths = bucklingLength;
            return column;
        }
    }
}