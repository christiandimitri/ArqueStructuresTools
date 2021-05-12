using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;
using WarehouseLib.Options;
using WarehouseLib.Warehouses;

namespace ArqueStructuresTools
{
    public class RectangularWarehouseComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the TestWarehouseComponent class.
        /// </summary>
        public RectangularWarehouseComponent()
            : base("Construct Rectangular Warehouse", "Nickname",
                "Description",
                "Arque Structures", "Warehouses")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        // ReSharper disable once RedundantNameQualifier
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "p", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddGenericParameter("Truss inputs", "ti", "ti", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Porticos count", "pc", "pc", GH_ParamAccess.item, 4);
            pManager.AddTextParameter("Roof bracing type", "rbt", "rbt", GH_ParamAccess.item, "Bracing");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new WarehouseParameter(), "Warehouse", "", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var plane = Plane.WorldXY;
            var trussInputs = new TrussOptions();
            var porticosCount = 0;
            var roofBracingType = "";
            if (!DA.GetData(0, ref plane)) return;
            if (!DA.GetData(1, ref trussInputs)) return;
            if (!DA.GetData(2, ref porticosCount)) return;
            if (!DA.GetData(3, ref roofBracingType)) return;
            Warehouse warehouse = null;

            try
            {
                warehouse = new Warehouse(plane, trussInputs,porticosCount, roofBracingType);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
                return;
            }

            DA.SetData(0, new WarehouseGoo(warehouse));
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
            get { return new Guid("A312F2DD-14A6-4196-8450-87079DA67FB5"); }
        }
    }
}