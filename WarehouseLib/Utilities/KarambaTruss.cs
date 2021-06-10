using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.Articulations;
using WarehouseLib.Beams;
using WarehouseLib.Columns;
using WarehouseLib.Trusses;

namespace WarehouseLib.Utilities
{
    public class KarambaTruss
    {
        private readonly Truss _truss;

        private List<Column> _trussStaticColumns;
        private List<Column> _trussBoundaryColumns;

        public List<Column> Karamba3DStaticColumns;
        public List<Column> Karamba3DBoundaryColumns;

        public List<Point3d> _trussTopNodes;
        public List<Point3d> _trussBottomNodes;

        private Beam _trussTopBeam;
        private Beam _trussBottomBeam;
        private Beam _trussIntermediateBeam;

        public Beam GetKaramba3DTopBeams;
        public List<Beam> Karamba3DBottomBeams;
        public List<Beam> Karamba3DIntermediateBeams;

        public KarambaTruss(Truss truss)
        {
            // extract truss properties and components
            _truss = truss;

            // get truss nodes
            GetTrussNodes();

            // get truss columns
            GetTrussColumns();

            GetTrussTopBeam();
            // GetTrussBottomBeam();

            // compute karamba properties for each component and add it
            GetKaramba3DColumns();
            // TODO
            GetKaramba3DBeams();
        }

        // truss nodes
        private void GetTrussTopNodes()
        {
            _trussTopNodes = _truss.TopNodes ?? new List<Point3d>();
        }

        private void GetTrussBottomNodes()
        {
            _trussBottomNodes = _truss.BottomNodes ?? new List<Point3d>();
        }

        private void GetTrussNodes()
        {
            GetTrussTopNodes();
            GetTrussBottomNodes();
        }

        // truss columns
        private void GetTrussColumns()
        {
            _trussStaticColumns = _truss.StaticColumns != null
                ? new List<Column>(_truss.StaticColumns)
                : new List<Column>();
            _trussBoundaryColumns = _truss.BoundaryColumns != null
                ? new List<Column>(_truss.BoundaryColumns)
                : new List<Column>();
        }

        // truss beams
        private void GetTrussTopBeam()
        {
            _trussTopBeam = _truss.TopBeam ?? new TopBeam();
        }

        // private void GetTrussBottomBeam()
        // {
        //     _trussBottomBeam = _truss.BottomBeam ?? new BottomBeam();
        //     Karamba3DBottomBeams = new List<Beam>();
        //     Karamba3DBottomBeams.Add(_trussBottomBeam);
        // }

        // Get Karamba3D all Columns types and properties
        private void GetKaramba3DColumns()
        {
            
            // Get Karamba3D static columns with buckling length
            var tempStaticColumns = new List<Column>();

            for (var i = 0; i < _trussStaticColumns.Count; i++)
            {
                double t;
                var staticColumn = _trussStaticColumns[i];

                staticColumn.Axis.ToNurbsCurve()
                    .ClosestPoint(_trussBottomNodes[i == 0 ? 0 : _trussBottomNodes.Count - 1], out t);
                var cl = _truss._articulationType == ArticulationType.Rigid.ToString()
                    ? staticColumn.Axis.ToNurbsCurve().Split(t).ToList()
                    : new List<Curve>{staticColumn.Axis.ToNurbsCurve()};
                var tempColumns = new List<Column>();
                foreach (var axis in cl)
                {
                    var column = new StaticColumn();
                    column.Axis = new Line(axis.PointAtStart, axis.PointAtEnd);
                    column.BucklingLengths = GetColumnBucklingLength(column).BucklingLengths;
                    tempColumns.Add(column);
                }

                tempStaticColumns.AddRange(tempColumns);
            }
            Karamba3DStaticColumns = tempStaticColumns;
            
            
        }

        // Get Karamba3D all Beams types and properties

        private void GetKaramba3DBeams()
        {
            GetKaramba3DTopBeams = _trussTopBeam;
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

        private Beam GetBeamsBucklingLength(Beam beam)
        {
            BucklingLengths.BucklingLengths bucklingLength;

            if (GetKaramba3DTopBeams != null)
            {
                bucklingLength = beam.ComputeBucklingLengths(beam, false, Double.NaN);
            }

            return beam;
        }
    }
}