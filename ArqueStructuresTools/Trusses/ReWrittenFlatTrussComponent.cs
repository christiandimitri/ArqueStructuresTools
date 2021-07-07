using System;
using WarehouseLib;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib.Options;
using WarehouseLib.Trusses;

// ReSharper disable RedundantNameQualifier

namespace ArqueStructuresTools
{
    public class ReWrittenFlatTrussComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the FlatTrussComponent class.
        /// </summary>
        public ReWrittenFlatTrussComponent()
            : base("Construct re_Flat Truss", "Nickname",
                "Description",
                "Arque Structures", "Truss")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "p", "p", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddGenericParameter("Options", "o", "o", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new ReWrittenTrussParameter(), "Flat truss", "t", "t", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        // ReSharper disable once InconsistentNaming
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.WorldXY;
            var trussInputs = new TrussInputs();
            if (!DA.GetData(0, ref plane)) return;
            if (!DA.GetData(1, ref trussInputs)) return;
            var porticoIndex = 0;
            ReWrittenTruss truss = null;
            try
            {
                truss = new ReWrittenFlatTruss(plane, trussInputs);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
                return;
            }

            DA.SetData(0, new ReWrittenTrussGoo(truss));
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
            get { return new Guid("43E4F4FF-1475-4309-B75C-BE8EFE7ACD91"); }
        }
    }
}