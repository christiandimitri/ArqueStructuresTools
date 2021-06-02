using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;

namespace ArqueStructuresTools
{
    public class DeconstructBracing : GH_Component
    {
        public DeconstructBracing() : base("Deconstruct Bracing", "DeBracing", "Deconstruct a Bracing into its component parts", "Arque Structures",
            "Utilities")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("3368D895-929A-475D-B770-67348E76497E"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new BracingParameter(), "Bracing", "B", "Bracing to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Axis", "A", "Bracing axis", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pl", "Bracing orientation plane", GH_ParamAccess.item);
            pManager.AddTextParameter("Profile", "Pr", "Bracing profile name", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var bracingGoo = new BracingGoo();

            if (!DA.GetData(0, ref bracingGoo)) return;
            var bracing = bracingGoo.Value;

            DA.SetData(0, bracing.Axis);
            DA.SetData(1, bracing.ProfileOrientationPlane);
            DA.SetData(2, bracing.Profile.Name);
        }
    }
}