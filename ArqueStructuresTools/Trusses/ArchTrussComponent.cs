using System;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;
using WarehouseLib.Options;
using WarehouseLib.Trusses;

namespace ArqueStructuresTools
{
    public class ArchTrussComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ArchTrussComponent class.
        /// </summary>
        public ArchTrussComponent()
            : base("Construct Arch Truss", "Nickname",
                "Description",
                "Arque Structures", "Trusses")
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
            pManager.AddParameter(new TrussParameter(), "Arch truss", "t", "t", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var plane = Plane.WorldXY;
            var trussInputs = new TrussOptions();
            if (!DA.GetData(0, ref plane)) return;
            if (!DA.GetData(1, ref trussInputs)) return;
            var porticoIndex = 0;
            Truss truss = null;
            try
            {
                truss = new ArchTruss(plane, trussInputs);
                if (trussInputs._porticoType == PorticoType.Portico.ToString())
                    truss.ConstructPorticoFromTruss(truss, porticoIndex);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
                return;
            }

            DA.SetData(0, new TrussGoo(truss));
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
            get { return new Guid("273ce80d-9f41-4d50-8a02-0291b75a4aaa"); }
        }
    }
}