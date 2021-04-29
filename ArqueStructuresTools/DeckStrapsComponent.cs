using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using WarehouseLib;

namespace ArqueStructuresTools
{
    public class DeckStrapsComponent : GH_Component
    {
        public override Guid ComponentGuid
        {
            get { return new Guid("A4DCA5FC-E06B-4F13-A17A-79D5B31EB444"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TrussParameter());
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new StrapParameter());
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<TrussGoo> trussesGoo = new List<TrussGoo>();

            if (!DA.GetDataList(0, trussesGoo)) return;


            DA.SetDataList(0, new List<StrapGoo>());
        }
    }
}