using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;

namespace ArqueStructuresTools
{
    public class DeconstructCross : GH_Component
    {
        public DeconstructCross() : base("Deconstruct Cross", "Nickname", "Description", "Arque Structures",
            "Utilities")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("98374289-1B21-4D97-A1D1-A9A2A2BF7E71"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new CrossParameter(), "Cross", "", "", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Cross axis", "ca", "ca", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Cross plane", "p", "p", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var crossGoo = new CrossGoo();

            if (!DA.GetData(0, ref crossGoo)) return;
            var axis = crossGoo.Value;

            DA.SetDataList(0, axis.Axis);
            DA.SetData(1, Plane.WorldXY);
        }
    }
}