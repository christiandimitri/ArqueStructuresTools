using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using WarehouseLib.Articulations;
using WarehouseLib.Beams;
using WarehouseLib.Columns;
using WarehouseLib.Connections;
using WarehouseLib.Nodes;
using WarehouseLib.Options;

namespace WarehouseLib.Trusses
{
    public abstract class ReWrittenTruss
    {
        // store inputs as local variables
        public TrussInputs _inputs;
        public Plane _plane;
        public double _width;
        public double _height;
        public double _maxHeight;
        public double _clearHeight;
        public double _columnsCount;
        public int _divisions;
        public ConnectionType _connectionType;
        public string _articulationType;
        public double _facadeStrapsDistance;
        public PorticoType _porticoType;

        // initialize a truss with a plane and inputs
        public ReWrittenTruss(Plane plane, TrussInputs inputs)
        {
            _inputs = inputs;
            _plane = plane;
            _width = inputs.Width;
            _height = inputs.Height;
            _maxHeight = inputs.MaxHeight;
            _clearHeight = inputs.ClearHeight;
            _divisions = inputs.Divisions;
            _connectionType = inputs.TrussType;
            _articulationType = inputs._articulationType;
            _columnsCount = inputs.ColumnsCount;
            _divisions = inputs.Divisions;
            _facadeStrapsDistance = inputs.FacadeStrapsDistance;
            _porticoType = inputs.PorticoType;
            ConstructBeamsSkeleton();
            ConstructTruss();
        }

        // store truss properties

        // store the columns Skeleton

        public List<Curve> StaticColumnsSkeleton;
        public List<Curve> BoundaryColumnsSkeleton;

        // store the columns

        public List<Column> StaticColumns;
        public List<Column> BoundaryColumns;

        // store the beams

        public Beam TopBeam;
        public Beam BottomBeam;
        public Beam IntermediateBeam;

        // store the starting points

        public List<Point3d> StartingPoints;

        // store the subdivided points
        public List<Point3d> TopPoints;
        public List<Point3d> BottomPoints;

        // store the beams skeleton curves

        public List<Curve> TopBeamSkeleton;
        public List<Curve> BottomBeamSkeleton;
        public List<Curve> IntermediateBeamSkeleton;

        // store the beams axis

        public List<BeamAxis> TopBeamAxis;
        public List<BeamAxis> BottomBeamAxis;
        public List<BeamAxis> IntermediateBeamAxis;

        // store the truss nodes
        public List<Node> TopNodes;
        public List<Node> BottomNodes;

        // initialize a truss with another one

        public ReWrittenTruss(ReWrittenTruss truss)
        {
            _inputs = truss._inputs;
            StaticColumnsSkeleton = truss.StaticColumnsSkeleton;
            BoundaryColumnsSkeleton = truss.BoundaryColumnsSkeleton;
            StaticColumns = truss.StaticColumns;
            BoundaryColumns = truss.BoundaryColumns;
            TopBeam = truss.TopBeam;
            BottomBeam = truss.BottomBeam;
            IntermediateBeam = truss.IntermediateBeam;
            StartingPoints = truss.StartingPoints;
            TopBeamSkeleton = truss.TopBeamSkeleton;
            BottomBeamSkeleton = truss.BottomBeamSkeleton;
            IntermediateBeamSkeleton = truss.IntermediateBeamSkeleton;
        }

        // construct starting points
        protected List<Point3d> ConstructStartingPoints(Plane plane, double leftLength, double rightLength,
            double leftHeight,
            double centerHeight,
            double rightHeight)
        {
            var ptA = plane.PointAt(-leftLength, 0, leftHeight);
            var ptB = plane.PointAt(0, 0, centerHeight);
            var ptC = plane.PointAt(rightLength, 0, rightHeight);
            var startingPoints = new List<Point3d> {ptA, ptB, ptC};
            return startingPoints;
        }

        // construct beams skeleton
        protected abstract void ConstructBeamsSkeleton();

        // gets the top beams nodes and axis

        // top beam nodes and axis
        protected Beam GetTopBeamDivisions(Curve skeleton, double[] divisionParams)
        {
            var topBeamElements = new Beam().GenerateTopBeamDivisions(skeleton, divisionParams);

            return topBeamElements;
        }

        // gets bottom beam nodes and axis
        protected Beam GetBottomBeamDivisions(Curve skeleton, List<Point3d> topNodes, Plane _plane,
            double _maxHeight)
        {
            var bottomBeamElements = new Beam().GenerateBottomBeamDivisions(skeleton, topNodes, _plane, _maxHeight);
            return bottomBeamElements;
        }

        // gets the intermediate beam axis
        public Beam GetIntermediateBeamAxis(List<Point3d> _topPoints, List<Point3d> _bottomPoints)
        {
            var intermediateBeam = new Beam().GenerateIntermediateBeamAxis(_inputs, _topPoints,
                _bottomPoints, _articulationType);
            
            intermediateBeam.SkeletonAxis.ForEach(axis => IntermediateBeamSkeleton.Add(axis.AxisCurve));
            var intermediateBeamAxis = new List<BeamAxis>();
            intermediateBeam.Axis.ForEach(axis => intermediateBeamAxis.Add(axis));

            return intermediateBeam;
        }

        // recompute divisions * 2 to make divisions 
        protected virtual int RecomputeDivisonsCount()
        {
            var divisions = _divisions;
            if (_connectionType == ConnectionType.Warren)
            {
                divisions *= 2;
            }
            else if (_divisions % 2 == 1 && _connectionType != ConnectionType.Warren) divisions += 1;

            return divisions;
        }

        // construct truss method
        protected virtual void ConstructTruss()
        {
            TopPoints = new List<Point3d>();
            BottomPoints = new List<Point3d>();
            TopBeamAxis = new List<BeamAxis>();
            BottomBeamAxis = new List<BeamAxis>();
            TopNodes = new List<Node>();
            BottomNodes = new List<Node>();

            var initParams =
                TopBeamSkeleton[0].DivideByCount(RecomputeDivisonsCount(), true);

            var newParams = RecomputeParams(initParams.ToList());

            var topElements = GetTopBeamDivisions(TopBeamSkeleton[0], newParams.TopParams.ToArray());
            TopPoints.AddRange(topElements.GetNodesPoints());
            TopPoints.ForEach(pt => TopNodes.Add(new Node(pt)));
            Debug.WriteLine("top points");
            TopBeamAxis.AddRange(topElements.SkeletonAxis);

            var tempTopElements = GetTopBeamDivisions(TopBeamSkeleton[0], newParams.BottomParams.ToArray());

            var bottomElements = GetBottomBeamDivisions(BottomBeamSkeleton[0], tempTopElements.GetNodesPoints(), _plane,
                _maxHeight);
            BottomPoints.AddRange(bottomElements.GetNodesPoints());
            BottomBeamAxis.AddRange(bottomElements.SkeletonAxis);
            BottomPoints.ForEach(pt => BottomNodes.Add(new Node(pt)));

            IntermediateBeamAxis = GetIntermediateBeamAxis(TopPoints, BottomPoints).Axis;

            TopBeam = new Beam();
            BottomBeam = new Beam();
            IntermediateBeam = new Beam();

            TopBeam.SetBeamAxisNodes(TopBeamAxis, TopNodes, new List<BeamAxis> {new BeamAxis(TopBeamSkeleton[0])});
            BottomBeam.SetBeamAxisNodes(BottomBeamAxis, BottomNodes,
                new List<BeamAxis> {new BeamAxis(BottomBeamSkeleton[0])});
            IntermediateBeam.SetBeamAxisNodes(IntermediateBeamAxis, new List<Node>(),
                new List<BeamAxis>(IntermediateBeamAxis));
        }


        // initializes a curve params divisions struct
        public struct DivisionParamsOnBeam
        {
            public List<double> TopParams;
            public List<double> BottomParams;
        }

        // recompute params by connection type
        protected virtual DivisionParamsOnBeam RecomputeParams(List<double> initParams)
        {
            var recomputedParams = new DivisionParamsOnBeam();
            recomputedParams.TopParams = new List<double>();
            recomputedParams.BottomParams = new List<double>();
            if (_connectionType == ConnectionType.Warren)
            {
                for (int i = 0; i < initParams.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        recomputedParams.TopParams.Add(initParams[i]);
                    }
                    else
                    {
                        recomputedParams.BottomParams.Add(initParams[i]);
                    }
                }

                if (_articulationType != ArticulationType.Articulated.ToString())
                {
                    recomputedParams.BottomParams.Insert(0, initParams[0]);
                    recomputedParams.BottomParams.Add(initParams[initParams.Count - 1]);
                }
            }
            else if (_connectionType == ConnectionType.WarrenStuds)
            {
                for (int i = 0; i < initParams.Count; i++)
                {
                    recomputedParams.TopParams.Add(initParams[i]);

                    if (i % 2 == 1)
                    {
                        recomputedParams.BottomParams.Add(initParams[i]);
                    }
                }

                if (_articulationType != ArticulationType.Articulated.ToString())
                {
                    recomputedParams.BottomParams.Insert(0, initParams[0]);
                    recomputedParams.BottomParams.Add(initParams[initParams.Count - 1]);
                }
            }
            else
            {
                recomputedParams.TopParams = initParams.ToList();
                recomputedParams.BottomParams = initParams.ToList();
            }

            return recomputedParams;
        }
    }
}