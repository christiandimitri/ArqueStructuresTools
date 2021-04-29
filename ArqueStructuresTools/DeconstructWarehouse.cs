using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace ArqueStructuresTools
{
    public class DeconstructWarehouse : GH_Component
    {
        public DeconstructWarehouse() : base("DeconstructWarehouse", "Nickname", "Description", "Arque Structures",
            "Utilities")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("45D2DC47-A714-4DFD-A19A-B281CFF1F870"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new WarehouseParameter());
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TrussParameter());
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            WarehouseGoo warehouseGoo = new WarehouseGoo();

            if (!DA.GetData(0, ref warehouseGoo)) return;

            var warehouse = warehouseGoo.Value;

            var trusses = new List<TrussGoo>();
            foreach (var truss in warehouse.Trusses)
            {
                trusses.Add(new TrussGoo(truss));
            }
            DA.SetDataList(0, new List<TrussGoo>(trusses));
        }
    }
}