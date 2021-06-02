using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;

namespace ArqueStructuresTools
{
    public class DeconstructCross : GH_Component
    {
        public DeconstructCross() : base("Deconstruct Cross", "DeCross", "Deconstruct a Cross into its component parts",
            "Arque Structures",
            "Utilities")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("98374289-1B21-4D97-A1D1-A9A2A2BF7E71"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new CrossParameter(), "Cross", "C", "Cross to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Axis", "A", "Cross axis", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Plane", "Pl", "Cross orientation plane", GH_ParamAccess.list);
            pManager.AddTextParameter("Profile", "Pr", "Cross profile name", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var crossGoo = new CrossGoo();

            if (!DA.GetData(0, ref crossGoo)) return;


            DA.SetDataList(0, crossGoo.Value.Axis);
            DA.SetDataList(1, crossGoo.Value.ProfileOrientationPlanes);
            DA.SetData(2, crossGoo.Value.Profile.Name);
        }
    }
}