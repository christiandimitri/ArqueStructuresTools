using System;
using System.Collections.Generic;
using ArqueStructuresTools.Params;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace ArqueStructuresTools
{
    public class DeconstructReWrittenTruss : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructTruss class.
        /// </summary>
        public DeconstructReWrittenTruss()
            : base("Deconstruct ReTruss", "DeReTruss",
                "Deconstruct a Truss into its component parts",
                "Arque Structures", "Truss")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new ReWrittenTrussParameter(), "Truss", "T", "Truss to deconstruct",
                GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BeamParameter(), "Top beam", "TB", "Truss top beam", GH_ParamAccess.item);
            pManager.AddParameter(new BeamParameter(), "Bottom beam", "BB", "Truss bottom beam", GH_ParamAccess.item);
            pManager.AddParameter(new BeamParameter(), "Intermediate beams", "IB", "Truss intermediate beams",
                GH_ParamAccess.item);
            pManager.AddPointParameter("Top nodes", "TN", "Truss top nodes", GH_ParamAccess.list);
            pManager.AddPointParameter("Bottom nodes", "BN", "Truss bottom nodes", GH_ParamAccess.list);
            pManager.AddParameter(new ColumnParameter(), "Static columns", "SC", "Truss static columns",
                GH_ParamAccess.list);
            pManager.AddParameter(new ColumnParameter(), "Boundary columns", "BC", "Truss boundary columns",
                GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ReWrittenTrussGoo trussGoo = new ReWrittenTrussGoo();

            if (!DA.GetData(0, ref trussGoo)) return;

            var truss = trussGoo.Value;
            var topNodes = truss.TopPoints != null ? truss.TopPoints : new List<Point3d>();
            var bottomNodes = truss.BottomPoints != null ? truss.BottomPoints : new List<Point3d>();
            // var boundaryNodes = truss.BoundaryTopNodes != null ? truss.BoundaryTopNodes : new List<Point3d>();
            var staticColumnsGoo = new List<ColumnGoo>();
            var boundaryColumnsGoo = new List<ColumnGoo>();
            var topBeamGoo = (truss.TopBeam.SkeletonAxis != null) ? new BeamGoo(truss.TopBeam) : null;
            var bottomBeamGoo = (truss.BottomBeamSkeleton != null) ? new BeamGoo(truss.BottomBeam) : null;
            var intermediateBeamsGoo =
                (truss.IntermediateBeamSkeleton != null) ? new BeamGoo(truss.IntermediateBeam) : null;

            if (truss.StaticColumns != null)
            {
                foreach (var cl in truss.StaticColumns)
                {
                    staticColumnsGoo.Add(new ColumnGoo(cl));
                }
            }

            if (truss.BoundaryColumns != null)
            {
                foreach (var cl in truss.BoundaryColumns)
                {
                    boundaryColumnsGoo.Add(new ColumnGoo(cl));
                }
            }

            DA.SetData(0, topBeamGoo);
            DA.SetData(1, bottomBeamGoo);
            DA.SetData(2, intermediateBeamsGoo);
            DA.SetDataList(3, topNodes);
            DA.SetDataList(4, bottomNodes);
            DA.SetDataList(5, staticColumnsGoo);
            DA.SetDataList(6, boundaryColumnsGoo);
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
            get { return new Guid("F085CAB8-0624-4CF8-B7A3-6EA60E1A6743"); }
        }
    }
}