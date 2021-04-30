using System;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;

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
            pManager.AddNumberParameter("Length", "l", "l", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Height", "h", "h", GH_ParamAccess.item, 2);
            pManager.AddNumberParameter("Max height", "mh", "mh", GH_ParamAccess.item, 3);
            pManager.AddNumberParameter("Clear height", "ch", "ch", GH_ParamAccess.item, 1.8);
            pManager.AddIntegerParameter("Division", "d", "d", GH_ParamAccess.item, 4);
            pManager.AddTextParameter("Truss type", "t", "t", GH_ParamAccess.item, "Pratt");
            pManager.AddTextParameter("Articulation type", "at", "at", GH_ParamAccess.item, "Rigid");
            pManager.AddIntegerParameter("Base type", "bt", "bt", GH_ParamAccess.item, 0);
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
            var worldXy = Plane.WorldXY;
            double height = 0;
            double length = 0;
            double maxHeight = 0;
            double clearHeight = 0;
            var divisions = 0;
            var trussType = "";
            var articulationType = "";
            var baseType = 0;
            if (!DA.GetData(0, ref worldXy)) return;
            if (!DA.GetData(1, ref length)) return;
            if (!DA.GetData(2, ref height)) return;
            if (!DA.GetData(3, ref maxHeight)) return;
            if (!DA.GetData(4, ref clearHeight)) return;
            if (!DA.GetData(5, ref divisions)) return;
            if (!DA.GetData(6, ref trussType)) return;
            if (!DA.GetData(7, ref articulationType)) return;
            if (!DA.GetData(8, ref baseType)) return;

            var truss = new ArchTruss(worldXy, length, height, maxHeight, clearHeight, divisions, trussType,
                articulationType, baseType);

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