using System;
using Grasshopper.Kernel;
using WarehouseLib.Options;
using WarehouseLib.Profiles;
using WarehouseLib.Warehouses;

namespace ArqueStructuresTools.TeklaStructures
{
    public class ComputeTeklaWarehouse : GH_Component
    {
        public ComputeTeklaWarehouse() : base("Compute Tekla Warehouse", "Nickname", "Description",
            "Arque Structures", "Tekla Structures")
        {
        }

        public override Guid ComponentGuid => new Guid("CBA42105-44B1-47FD-A286-274AE46E19E1");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new WarehouseParameter(), "Warehouse", "warehouse",
                "warehouse tekla structures manipulator", GH_ParamAccess.item);
            pManager.AddGenericParameter("Elements profile", "ep",
                "The profile's name, for example IPE100", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new WarehouseParameter(), "recomputed warehouse", "rw",
                "Warehouse elements axis recomputed based on the profile sections", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var warehouseGoo = new WarehouseGoo();
            var profileNames =
                new WarehouseProfiles("HEA320", "HEA300", "IPE300", "IPE330", "IPE80");

            if (!DA.GetData(0, ref warehouseGoo)) return;
            if (!DA.GetData(1, ref profileNames)) return;
            var warehouse = warehouseGoo.Value;
            var plane = warehouse._plane;
            var trussInputs = warehouse._trussOptions;
            var warehouseOptions = warehouse._warehouseOptions;

            warehouse = new Warehouse(plane, trussInputs,
                warehouseOptions);

            var teklaWarehouse = new TeklaWarehouse(warehouse, profileNames);
            var newTrussInputs = teklaWarehouse.ComputeTeklaTrussInputs(trussInputs, profileNames);

            warehouse = teklaWarehouse.GetTeklaWarehouse(newTrussInputs, profileNames);

            DA.SetData(0, new WarehouseGoo(warehouse));
        }
    }
}