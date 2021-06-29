using System;
using System.Collections.Generic;
using ArqueStructuresTools.Params;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;
using WarehouseLib.Beams;

namespace ArqueStructuresTools
{
    public class HalfEdgeTruss : GH_Component
    {
        public HalfEdgeTruss() : base("HalfEdge Truss", "H-ETruss", "Construct a half-edge Truss", "Arque Structures",
            "Utilities")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("1B8D1430-F6C9-4B41-BBEB-2E5E2623A4E3"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new KarambaTrussParameter(), "Truss", "Truss", "Truss to compute its half-edge",
                GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var trussGoo = new KarambaTrussGoo();

            if (!DA.GetData(0, ref trussGoo)) return;

            var topBeam = new Beam(trussGoo.Value.Karamba3DTopBeams);
            var bottomBeam = new Beam(trussGoo.Value.Karamba3DBottomBeams);
            var interBeam = new Beam(trussGoo.Value.Karamba3DIntermediateBeams);

            
            // DA.SetData(0, topBeam);
            // DA.SetData(1, bottomBeam);
            // DA.SetData(2, interBeam);
        }
    }
}
