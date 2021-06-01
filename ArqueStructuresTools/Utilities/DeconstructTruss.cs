using System;
using System.Collections.Generic;
using ArqueStructuresTools.Params;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace ArqueStructuresTools
{
    public class DeconstructTruss : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructTruss class.
        /// </summary>
        public DeconstructTruss()
            : base("Deconstruct Truss", "Nickname",
                "Description",
                "Arque Structures", "Utilities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TrussParameter(), "Truss", "t", "t", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BeamParameter(), "Top bars", "tb", "tb", GH_ParamAccess.item);
            pManager.AddParameter(new BeamParameter(), "Bottom bars", "bb", "bb", GH_ParamAccess.item);
            pManager.AddParameter(new BeamParameter(), "Intermediate bars", "ib", "ib", GH_ParamAccess.item);
            pManager.AddPointParameter("Top nodes", "tn", "tn", GH_ParamAccess.list);
            pManager.AddPointParameter("Bottom nodes", "bn", "bn", GH_ParamAccess.list);
            pManager.AddParameter(new ColumnParameter(), "Static columns", "cb", "cb", GH_ParamAccess.list);
            pManager.AddParameter(new ColumnParameter(), "Boundary columns", "bc", "bc", GH_ParamAccess.list);
            pManager.AddPointParameter("Boundary nodes", "bn", "bn", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            TrussGoo trussGoo = new TrussGoo();

            if (!DA.GetData(0, ref trussGoo)) return;

            var truss = trussGoo.Value;
            var topNodes = truss.TopNodes != null ? truss.TopNodes : new List<Point3d>();
            var bottomNodes = truss.BottomNodes != null ? truss.BottomNodes : new List<Point3d>();
            var boundaryNodes = truss.BoundaryTopNodes != null ? truss.BoundaryTopNodes : new List<Point3d>();
            var staticColumnsGoo = new List<ColumnGoo>();
            var boundaryColumnsGoo = new List<ColumnGoo>();
            var topBeamGoo = (truss.TopBeam != null) ? new BeamGoo(truss.TopBeam) : new BeamGoo();
            var bottomBeamGoo = (truss.BottomBeam != null) ? new BeamGoo(truss.BottomBeam) : new BeamGoo();
            var intermediateBeamsGoo =
                (truss.IntermediateBeams != null) ? new BeamGoo(truss.IntermediateBeams) : new BeamGoo();

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
            DA.SetDataList(7, boundaryNodes);
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
            get { return new Guid("ca8cb4da-4294-4944-9d8d-2b7ad00bf9b1"); }
        }
    }
}