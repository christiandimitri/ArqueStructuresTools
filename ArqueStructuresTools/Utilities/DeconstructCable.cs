using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;

namespace ArqueStructuresTools
{
    public class DeconstructCable : GH_Component
    {
        public DeconstructCable() : base("Deconstruct Cable", "DeCable", "Deconstruct a Cable into its component parts", "Arque Structures",
            "Utilities")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("6BAB6CFB-5256-4312-AE29-E6E770EDA48F"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new CableParameter(), "Cable", "C", "Cable to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Axis", "A", "Cable axis", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pl", "Cable orientation plane", GH_ParamAccess.item);
            pManager.AddTextParameter("Profile", "Pr", "Cable profile name", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var cableGoo = new CableGoo();

            if (!DA.GetData(0, ref cableGoo)) return;
            var cable = cableGoo.Value;

            DA.SetData(0, cable.Axis);
            DA.SetData(1, cable.ProfileOrientationPlane);
            DA.SetData(2, cable.Profile.Name);
        }
    }
}