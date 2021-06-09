using System;
using System.Collections.Generic;
using ArqueStructuresTools.Params;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib.Beams;

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
            pManager.AddParameter(new BeamParameter(), "Top beam", "TB", "Truss top beam", GH_ParamAccess.item);
            pManager.AddParameter(new BeamParameter(), "Bottom beam", "BB", "Truss bottom beam", GH_ParamAccess.item);
            pManager.AddParameter(new BeamParameter(), "Intermediate beams", "IB", "Truss intermediate beams",
                GH_ParamAccess.item);
            pManager.AddPointParameter("Top nodes", "TN", "Truss top nodes", GH_ParamAccess.list);
            pManager.AddPointParameter("Bottom nodes", "BN", "Truss bottom nodes", GH_ParamAccess.list);
            pManager.AddParameter(new ColumnParameter(), "Boundary columns", "BC", "Truss boundary columns",
                GH_ParamAccess.list);
            pManager.AddPointParameter("Boundary nodes", "BN", "Truss boundary nodes", GH_ParamAccess.list);
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
                staticColumnsGoo.Add(new ColumnGoo(staticColumn));
            }

            var topBeamGoo = new BeamGoo(truss.GetKaramba3DTopBeams);


            DA.SetDataList(0, staticColumnsGoo);
            DA.SetData(1, topBeamGoo);
            // DA.SetData(2, intermediateBeamsGoo);
            // DA.SetDataList(3, topNodes);
            // DA.SetDataList(4, bottomNodes);
            // DA.SetDataList(5, staticColumnsGoo);
            // DA.SetDataList(6, boundaryColumnsGoo);
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