using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;
using WarehouseLib.Trusses;

namespace ArqueStructuresTools
{
    public class RoofStrapsComponent : GH_Component
    {
        public RoofStrapsComponent() : base("Construct Roof Straps", "Nickname", "Description", "Arque Structures",
            "Strap")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("A4DCA5FC-E06B-4F13-A17A-79D5B31EB444"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TrussParameter(), "Trusses", "t", "t", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new StrapParameter(), "Roof straps", "rs", "rs", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var trussesGoo = new List<TrussGoo>();

            if (!DA.GetDataList(0, trussesGoo)) return;
            var trusses = new List<Truss>();
            for (var i = 0; i < trussesGoo.Count; i++)
            {
                var trussGoo = trussesGoo[i];
                var truss = trussGoo.Value;
                trusses.Add(truss);
            }

            var roofStraps = new List<StrapGoo>();
            var tempStraps = new RoofStrap().ConstructRoofStraps(trusses);
            foreach (var strap in tempStraps)
            {
                roofStraps.Add(new StrapGoo(strap));
            }

            DA.SetDataList(0, new List<StrapGoo>(roofStraps));
        }
    }
}