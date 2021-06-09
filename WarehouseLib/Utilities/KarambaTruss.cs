using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
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

        private Beam _trussTopBeam;
        private Beam _trussBottomBeam;
        private Beam _trussIntermediateBeam;

        public List<Beam> GetKaramba3DTopBeams;
        public List<Beam> Karamba3DBottomBeams;
        public List<Beam> Karamba3DIntermediateBeams;

        public KarambaTruss(Truss truss)
        {
            // extract truss properties and components
            _truss = truss;
            GetTrussColumns();
            GetTrussTopBeam();
            GetTrussBottomBeam();
            
            // compute karamba properties for each component and add it
            GetKaramba3DColumns();
            // GetKaramba3DTopBeams();
        }

        private void GetTrussColumns()
        {
            _trussStaticColumns = _truss.StaticColumns != null
                ? new List<Column>(_truss.StaticColumns)
                : new List<Column>();
            _trussBoundaryColumns = _truss.BoundaryColumns != null
                ? new List<Column>(_truss.BoundaryColumns)
                : new List<Column>();
        }

        // construct karamba columns output "axis", "buckling length", "articulation"
        private void GetTrussTopBeam()
        {
            _trussTopBeam = _truss.TopBeam ?? new TopBeam();
            GetKaramba3DTopBeams = new List<Beam>();
            GetKaramba3DTopBeams.Add(_trussTopBeam);
        }
        
        private void GetTrussBottomBeam()
        {
            _trussBottomBeam = _truss.BottomBeam ?? new BottomBeam();
            Karamba3DBottomBeams = new List<Beam>();
            Karamba3DBottomBeams.Add(_trussBottomBeam);
        }
        private void GetKaramba3DColumns()
        {
            var tempStaticColumns = new List<Column>();
            if (_truss._articulationType == Articulations.ArticulationType.Rigid.ToString())
            {
                foreach (var staticColumn in _trussStaticColumns)
                {
                    double t;
                    var cl = staticColumn.Axis.ToNurbsCurve().Split(_truss._clearHeight).ToList();
                    var tempColumns = new List<Column>();
                    foreach (var axis in cl)
                    {
                        var column = new StaticColumn();
                        column.Axis = new Line(axis.PointAtStart, axis.PointAtEnd);
                        column.BucklingLengths = GetColumnsBucklingLength(column).BucklingLengths;
                        tempColumns.Add(column);
                    }

                    tempStaticColumns.AddRange(tempColumns);
                }
            }

            Karamba3DStaticColumns = tempStaticColumns;
        }

        // Return a column with its buckling lengths
        private Column GetColumnsBucklingLength(Column column)
        {
            BucklingLengths.BucklingLengths bucklingLength;
            if (column is StaticColumn)
            {
                bucklingLength = column.ComputeBucklingLengths(column, false, double.NaN);
            }
            else
            {
                bucklingLength = column.ComputeBucklingLengths(column, true, _truss._facadeStrapsDistance);
            }

            column.BucklingLengths = bucklingLength;
            return column;
        }

        private List<Beam> GetBeamsBucklingLength(List<Beam> beams)
        {
            return beams;
        }
    }
}