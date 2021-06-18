using System;
using Grasshopper.Kernel;
using WarehouseLib.Utilities;

namespace ArqueStructuresTools.Karamba
{
    public class ComputeKarambaTruss : GH_Component
    {
        public ComputeKarambaTruss() : base("Compute Karamba3D Truss", "CompKaTruss",
            "Compute any Truss into its structural analysis properties", "Arque Structures", "Karamba3D")
        {
        }

        public override Guid ComponentGuid => new Guid("EA3EEB1C-3C2B-4635-BDDD-774E42E8CAA7");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TrussParameter(), "Truss", "T", "Any truss to compute its properties",
                GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new KarambaTrussParameter(), "Karamba Truss", "KaTruss",
                "Karamba3D truss ready to analyse",
                GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var trussGoo = new TrussGoo();

            if (!DA.GetData(0, ref trussGoo)) return;

            KarambaTruss kaTruss = new KarambaTruss(trussGoo.Value);

            DA.SetData(0, new KarambaTrussGoo(kaTruss));
        }
    }
}