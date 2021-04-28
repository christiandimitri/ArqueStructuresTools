using System;
using System.Collections.Generic;
using WarehouseLib;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace ArqueStructuresTools
{
    public class MonopichTrussComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MonopichTruss class.
        /// </summary>
        public MonopichTrussComponent()
          : base("MonopichTruss", "Nickname",
              "Description",
              "Arque Structures", "Trusses")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("p", "p", "p", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddNumberParameter("l", "l", "l", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("h", "h", "h", GH_ParamAccess.item, 2);
            pManager.AddNumberParameter("mH", "mh", "mh", GH_ParamAccess.item, 3);
            pManager.AddNumberParameter("ch", "ch", "ch", GH_ParamAccess.item, 1.8);
            pManager.AddIntegerParameter("d", "d", "d", GH_ParamAccess.item, 4);
            pManager.AddTextParameter("tt", "tt", "tt", GH_ParamAccess.item, "Pratt");
            pManager.AddTextParameter("at", "at", "at", GH_ParamAccess.item, "Rigid");
            pManager.AddIntegerParameter("bt", "bt", "bt", GH_ParamAccess.item, 0);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TrussParameter());
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane worldXY = Plane.WorldXY;
            double height = 0;
            double length = 0;
            double maxHeight = 0;
            double clearHeight = 0;
            int divisions = 0;
            string trussType = "";
            string articulationType = "";
            int baseType = 0;
            if (!DA.GetData(0, ref worldXY)) return;
            if (!DA.GetData(1, ref length)) return;
            if (!DA.GetData(2, ref height)) return;
            if (!DA.GetData(3, ref maxHeight)) return;
            if (!DA.GetData(4, ref clearHeight)) return;
            if (!DA.GetData(5, ref divisions)) return;
            if (!DA.GetData(6, ref trussType)) return;
            if (!DA.GetData(7, ref articulationType)) return;
            if (!DA.GetData(8, ref baseType)) return;


            var truss = new MonopichedTruss(worldXY, length, height, maxHeight, clearHeight, divisions, trussType, articulationType,baseType );

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
            get { return new Guid("311A180E-3649-40DC-9412-180A948396C5"); }
        }
    }
}