using System;
using System.Collections.Generic;
using ArqueStructuresTools.Params;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;
using WarehouseLib.Beams;
using WarehouseLib.BucklingLengths;
using WarehouseLib.Columns;
using WarehouseLib.Options;

namespace ArqueStructuresTools
{
    public class DeconstructKarambaTruss : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructTruss class.
        /// </summary>
        public DeconstructKarambaTruss()
            : base("Deconstruct Karamba3D Truss", "DeKaTruss",
                "Deconstruct a Karamba3D Truss into its component parts",
                "Arque Structures", "Karamba3D")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new KarambaTrussParameter(), "Karamba3D Truss", "Ka3D T",
                "Karamba3D Truss to deconstruct", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new ColumnParameter(), "Static columns", "SC", "Truss static columns",
                GH_ParamAccess.list);
            pManager.AddParameter(new ColumnParameter(), "Boundary columns", "BC", "Truss boundary columns",
                GH_ParamAccess.list);
            pManager.AddParameter(new BeamParameter(), "Top beam", "TB", "Truss top beam", GH_ParamAccess.item);
            pManager.AddParameter(new BeamParameter(), "Bottom beam", "BB", "Truss bottom beam", GH_ParamAccess.item);
            pManager.AddParameter(new BeamParameter(), "Intermediate beams", "IB", "Truss intermediate beams",
                GH_ParamAccess.item);
            pManager.AddPointParameter("Top nodes", "TN", "Truss top nodes", GH_ParamAccess.list);
            pManager.AddPointParameter("Bottom nodes", "BN", "Truss bottom nodes", GH_ParamAccess.list);
            // pManager.AddPointParameter("Boundary nodes", "BN", "Truss boundary nodes", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            KarambaTrussGoo kaTrussGoo = new KarambaTrussGoo();

            if (!DA.GetData(0, ref kaTrussGoo)) return;

            var truss = kaTrussGoo.Value;
            var staticColumnsGoo = new List<ColumnGoo>();
            foreach (var staticColumn in truss.Karamba3DStaticColumns)
            {
                if (staticColumn.Axis != null)
                {
                    staticColumnsGoo.Add(new ColumnGoo(staticColumn));
                }
            }

            var boundaryColumnsGoo = new List<ColumnGoo>();
            if (truss.Karamba3DBoundaryColumns != null)
            {
                foreach (var boundaryColumn in truss.Karamba3DBoundaryColumns)
                {
                    if (boundaryColumn.Axis != null)
                    {
                        boundaryColumnsGoo.Add(new ColumnGoo(boundaryColumn));
                    }
                }
            }

            var topBeamAxisCurves = new List<BeamAxis>();
            var topBeam = new Beam();
            var tempBucklings = new List<BucklingLengths>();
            if (truss.Karamba3DTopBeams.Axis != null)
            {
                for (int i = 0; i < truss.Karamba3DTopBeams.Axis.Count; i++)
                {
                    var axis = new BeamAxis(truss.Karamba3DTopBeams.Axis[i].AxisCurve);
                    topBeamAxisCurves.Add(axis);
                    tempBucklings.Add(truss.Karamba3DTopBeams.BucklingLengths[i]);
                }

                topBeam.Axis = topBeamAxisCurves;
                topBeam.BucklingLengths = tempBucklings;
                topBeam.Position = kaTrussGoo.Value.Karamba3DTopBeams.Position;
            }

            var bottomBeamAxisCurves = new List<BeamAxis>();
            var bottomBeam = new Beam();
            tempBucklings = new List<BucklingLengths>();
            if (truss.Karamba3DBottomBeams.Axis != null)
            {
                for (int i = 0; i < truss.Karamba3DBottomBeams.Axis.Count; i++)
                {
                    var axis = new BeamAxis(truss.Karamba3DBottomBeams.Axis[i].AxisCurve);
                    bottomBeamAxisCurves.Add(axis);
                    tempBucklings.Add(truss.Karamba3DBottomBeams.BucklingLengths[i]);
                }

                bottomBeam.Axis = bottomBeamAxisCurves;
                bottomBeam.BucklingLengths = tempBucklings;
                bottomBeam.Position = kaTrussGoo.Value.Karamba3DBottomBeams.Position;
            }

            var intermediateBeamAxisCurves = new List<BeamAxis>();
            var intermediateBeam = new Beam();
            tempBucklings = new List<BucklingLengths>();
            if (truss.Karamba3DIntermediateBeams.Axis != null)
            {
                for (int i = 0; i < truss.Karamba3DIntermediateBeams.Axis.Count; i++)
                {
                    var axis = new BeamAxis(truss.Karamba3DIntermediateBeams.Axis[i].AxisCurve);
                    intermediateBeamAxisCurves.Add(axis);
                    tempBucklings.Add(truss.Karamba3DIntermediateBeams.BucklingLengths[i]);
                }

                intermediateBeam.Axis = intermediateBeamAxisCurves;
                intermediateBeam.BucklingLengths = tempBucklings;
                intermediateBeam.Position = kaTrussGoo.Value.Karamba3DIntermediateBeams.Position;
            }

            var topBeamGoo = topBeam.Axis != null
                ? new BeamGoo(topBeam)
                : null;
            var bottomBeamGoo = bottomBeam.Axis != null
                ? new BeamGoo(bottomBeam)
                : null;
            var intermediateBeamGoo = intermediateBeam.Axis != null
                ? new BeamGoo(intermediateBeam)
                : null;

            DA.SetDataList(0, staticColumnsGoo);
            DA.SetDataList(1, boundaryColumnsGoo);
            DA.SetData(2, topBeamGoo);
            DA.SetData(3, bottomBeamGoo);
            DA.SetData(4, intermediateBeamGoo);
            DA.SetDataList(5, truss._trussTopNodes);
            DA.SetDataList(6, truss._trussBottomNodes);
            // DA.SetDataList(7, boundaryNodes);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("50CD50F6-3916-4DBC-8AAB-4A0E79C580D0"); }
        }
    }
}