using System;
using Grasshopper.Kernel;

namespace ArqueStructuresTools.TeklaStructures
{
    public class RecomputeForTeklaStructures : GH_Component
    {
        public RecomputeForTeklaStructures() : base("Recompute Tekla Warehouse", "Nickname", "Description",
            "Arque Structures", "Tekla Structures")
        {
        }

        public override Guid ComponentGuid => new Guid("CBA42105-44B1-47FD-A286-274AE46E19E1");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new WarehouseParameter(), "Warehouse", "warehouse",
                "warehouse tekla structures manipulator", GH_ParamAccess.item);
            pManager.AddTextParameter("Static columns profile", "scp",
                "The static columns profile name, for example IPE100", GH_ParamAccess.item, "HEA320");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new WarehouseParameter(), "recomputed warehouse", "rw",
                "Warehouse elements axis recomputed based on the profile sections", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var warehouseGoo = new WarehouseGoo();
            var staticColumnsProfileName = "";

            if (!DA.GetData(0, ref warehouseGoo)) return;
            if (!DA.GetData(1, ref staticColumnsProfileName)) return;

            DA.SetData(0, warehouseGoo);
        }
    }
}