using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;

namespace ArqueStructuresTools
{
    public class DeconstructStrap : GH_Component
    {
        public DeconstructStrap() : base("Deconstruct Strap", "DeStrap", "Deconstruct a Strap into its component parts", "Arque Structures",
            "Utilities")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("D97F3E1F-9B01-4635-A13A-F2FA99734F16"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new StrapParameter(), "Strap", "S", "Strap to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Axis", "A", "Strap axis", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pl", "Strap orientation plane", GH_ParamAccess.item);
            pManager.AddTextParameter("Profile", "Pr", "Strap profile name", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var strapGoo = new StrapGoo();

            if (!DA.GetData(0, ref strapGoo)) return;

            DA.SetData(0, strapGoo.Value.Axis);
            DA.SetData(1, strapGoo.Value.ProfileOrientationPlane);
            DA.SetData(2, strapGoo.Value.Profile.Name);
        }
    }
}