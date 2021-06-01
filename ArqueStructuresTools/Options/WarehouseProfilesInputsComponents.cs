using System;
using Grasshopper.Kernel;
using WarehouseLib.Profiles;
using WarehouseLib.Utilities;


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
                GH_ParamAccess.item, "HEA300");
            pManager.AddTextParameter("Boundary columns profile", "bcp", "Boundary columns Tekla profile name",
                GH_ParamAccess.item, "HEA260");
            pManager.AddTextParameter("Top beams profile", "tbp", "Top beams Tekla profile name",
                GH_ParamAccess.item, "IPE300");
            pManager.AddTextParameter("Bottom beams profile", "bcp", "Bottom beams Tekla profile name",
                GH_ParamAccess.item, "IPE300");
            pManager.AddTextParameter("Intermediate beams profile", "bcp", "Intermediate beams Tekla profile name",
                GH_ParamAccess.item, "IPE80");
            pManager.AddTextParameter("Roof straps", "rs", "rs", GH_ParamAccess.item, "CEBRAU-100X3");
            pManager.AddTextParameter("Facade straps", "fs", "fs", GH_ParamAccess.item, "CEBRAU-100X3");
            pManager.AddTextParameter("Roof Cables", "fs", "fs", GH_ParamAccess.item, "CHS21.3/2.3");
            pManager.AddTextParameter("Facade Cables", "fs", "fs", GH_ParamAccess.item, "CHS21.3/2.3");
            pManager.AddTextParameter("Roof bracing", "rb", "rb", GH_ParamAccess.item, "SHS 70 / 5");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Warehouse Profiles", "pl", "pl", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var staticColumns = "";
            var boundaryColumns = "";
            var topBeams = "";
            var bottomBeams = "";
            var intermediateBeams = "";
            var roofStraps = "";
            var facadeStraps = "";
            var roofCables = "";
            var facadeCables = "";
            var roofBracing = "";
            if (!DA.GetData(0, ref staticColumns)) return;
            if (!DA.GetData(1, ref boundaryColumns)) return;
            if (!DA.GetData(2, ref topBeams)) return;
            if (!DA.GetData(3, ref bottomBeams)) return;
            if (!DA.GetData(4, ref intermediateBeams)) return;
            if (!DA.GetData(5, ref roofStraps)) return;
            if (!DA.GetData(6, ref facadeStraps)) return;
            if (!DA.GetData(7, ref roofCables)) return;
            if (!DA.GetData(8, ref facadeCables)) return;
            if (!DA.GetData(8, ref roofBracing)) return;

            WarehouseProfiles profiles = null;

            try
            {
                profiles = new WarehouseProfiles(new TrimWhiteSpaceFromString(staticColumns).TrimmedString,
                    new TrimWhiteSpaceFromString(boundaryColumns).TrimmedString,
                    new TrimWhiteSpaceFromString(topBeams).TrimmedString,
                    new TrimWhiteSpaceFromString(bottomBeams).TrimmedString,
                    new TrimWhiteSpaceFromString(intermediateBeams).TrimmedString,
                    new TrimWhiteSpaceFromString(roofStraps).TrimmedString,
                    new TrimWhiteSpaceFromString(facadeStraps).TrimmedString,
                    new TrimWhiteSpaceFromString(roofCables).TrimmedString,
                    new TrimWhiteSpaceFromString(facadeCables).TrimmedString,
                    new TrimWhiteSpaceFromString(roofBracing).TrimmedString);
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