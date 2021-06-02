﻿using System;
using Grasshopper.Kernel;
using WarehouseLib.Profiles;
using WarehouseLib.Utilities;


namespace ArqueStructuresTools.Options
{
    public class WarehouseProfilesInputsComponents : GH_Component
    {
        public WarehouseProfilesInputsComponents() : base("Warehouse Profiles", "WaProfiles",
            "Input for each component its corresponding Profile name",
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
            pManager.AddTextParameter("Static column", "SC", "Static column profile name",
                GH_ParamAccess.item, "HEA300");
            pManager.AddTextParameter("Boundary column", "BC", "Boundary column profile name",
                GH_ParamAccess.item, "HEA260");
            pManager.AddTextParameter("Top beam", "TB", "Top beam profile name",
                GH_ParamAccess.item, "IPE300");
            pManager.AddTextParameter("Bottom beam", "BB", "Bottom beam Tekla profile name",
                GH_ParamAccess.item, "IPE300");
            pManager.AddTextParameter("Intermediate beams", "IB", "Intermediate beams profile name",
                GH_ParamAccess.item, "IPE80");
            pManager.AddTextParameter("Roof straps", "RS", "Roof straps profile name", GH_ParamAccess.item,
                "CEBRAU-100X3");
            pManager.AddTextParameter("Facade straps", "FS", "Facade straps profile name", GH_ParamAccess.item,
                "CEBRAU-100X3");
            pManager.AddTextParameter("Roof Cables", "RS", "Roof cables profile name", GH_ParamAccess.item,
                "CHS21.3/2.3");
            pManager.AddTextParameter("Facade Cables", "FC", "Facade cable profile name", GH_ParamAccess.item,
                "CHS21.3/2.3");
            pManager.AddTextParameter("Roof bracing", "RB", "Roof bracing profile name", GH_ParamAccess.item,
                "SHS 70 / 5");
            pManager.AddTextParameter("Columns bracing", "CB", "Columns bracing profile name", GH_ParamAccess.item,
                "SHS 70 / 5");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Warehouse Profiles", "pl", "pl", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var staticColumn = "";
            var boundaryColumn = "";
            var topBeam = "";
            var bottomBeam = "";
            var intermediateBeams = "";
            var roofStraps = "";
            var facadeStraps = "";
            var roofCables = "";
            var facadeCables = "";
            var roofBracing = "";
            var columnsBracing = "";
            if (!DA.GetData(0, ref staticColumn)) return;
            if (!DA.GetData(1, ref boundaryColumn)) return;
            if (!DA.GetData(2, ref topBeam)) return;
            if (!DA.GetData(3, ref bottomBeam)) return;
            if (!DA.GetData(4, ref intermediateBeams)) return;
            if (!DA.GetData(5, ref roofStraps)) return;
            if (!DA.GetData(6, ref facadeStraps)) return;
            if (!DA.GetData(7, ref roofCables)) return;
            if (!DA.GetData(8, ref facadeCables)) return;
            if (!DA.GetData(9, ref roofBracing)) return;
            if (!DA.GetData(10, ref columnsBracing)) return;

            WarehouseProfiles profiles = null;

            try
            {
                profiles = new WarehouseProfiles(new TrimWhiteSpaceFromString(staticColumn).TrimmedString,
                    new TrimWhiteSpaceFromString(boundaryColumn).TrimmedString,
                    new TrimWhiteSpaceFromString(topBeam).TrimmedString,
                    new TrimWhiteSpaceFromString(bottomBeam).TrimmedString,
                    new TrimWhiteSpaceFromString(intermediateBeams).TrimmedString,
                    new TrimWhiteSpaceFromString(roofStraps).TrimmedString,
                    new TrimWhiteSpaceFromString(facadeStraps).TrimmedString,
                    new TrimWhiteSpaceFromString(roofCables).TrimmedString,
                    new TrimWhiteSpaceFromString(facadeCables).TrimmedString,
                    new TrimWhiteSpaceFromString(roofBracing).TrimmedString,
                    new TrimWhiteSpaceFromString(columnsBracing).TrimmedString);
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