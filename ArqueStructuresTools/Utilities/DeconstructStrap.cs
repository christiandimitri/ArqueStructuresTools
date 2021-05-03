using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;

namespace ArqueStructuresTools
{
    public class DeconstructStrap : GH_Component
    {
        public DeconstructStrap() : base("Deconstruct Strap", "Nickname", "Description", "Arque Structures",
            "Utilities")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("D97F3E1F-9B01-4635-A13A-F2FA99734F16"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new StrapParameter(), "Strap", "", "", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Strap axis", "s", "s", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Strap plane", "p", "p", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var strapGoo = new StrapGoo();

            if (!DA.GetData(0, ref strapGoo)) return;
            var strap = strapGoo.Value;

            DA.SetData(0, strap.Axis);
            DA.SetData(1, Plane.WorldXY);
        }
    }
}