using System;
using Grasshopper.Kernel;
using WarehouseLib.Articulations;
using WarehouseLib.Options;
using WarehouseLib.Profiles;


namespace ArqueStructuresTools.Options
{
    public class WarehouseProfilesInputsComponents : GH_Component
    {
        public WarehouseProfilesInputsComponents() : base("Warehouse Profiles Inputs", "Nickname", "description",
            "Arque Structures",
            "Utilities")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("E776E0A6-867A-4124-B3A6-700BB3DD256C"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Static columns profile", "scp", "Static columns Tekla profile name",
                GH_ParamAccess.item, "IPE300");
            pManager.AddTextParameter("Boundary columns profile", "bcp", "Boundary columns Tekla profile name",
                GH_ParamAccess.item, "IPE320");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Warehouse Profiles", "pl", "pl", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var staticColumns = "";
            var boundaryColumns = "";
            if (!DA.GetData(0, ref staticColumns)) return;
            if (!DA.GetData(1, ref boundaryColumns)) return;

            WarehouseProfiles profiles = null;

            try
            {
                profiles = new WarehouseProfiles(staticColumns, boundaryColumns);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
                return;
            }

            DA.SetData(0, profiles);
        }
    }
}