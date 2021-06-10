using System;
using ArqueStructuresTools.Params;
using Grasshopper.Kernel;

namespace ArqueStructuresTools.Karamba
{
    public class DeconstructKarambaBeam : GH_Component
    {
        public DeconstructKarambaBeam() : base("Deconstruct Karamba3D beam", "DeKaBeam",
            "Deconstruct a Beam into its karamba parts inputs", "Arque Structures", "Beam")
        {
        }

        public override Guid ComponentGuid => new Guid("30A6D320-260C-4C58-BA2E-C5DB0B3496EF");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new BeamParameter(), "Beam", "Beam", "Beam to deconstruct",
                GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Axis", "A", "Beam axis", GH_ParamAccess.list);
            pManager.AddNumberParameter("Y Buckling", "YB", "Y buckling length", GH_ParamAccess.item);
            pManager.AddNumberParameter("Z Buckling", "ZB", "Z buckling length", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var beamGoo = new BeamGoo();

            // get input data
            if (!DA.GetData(0, ref beamGoo)) return;

            // set output data 
            DA.SetDataList(0, beamGoo.Value.Axis);
            DA.SetData(1, beamGoo.Value.BucklingLengths.BucklingY);
            DA.SetData(2, beamGoo.Value.BucklingLengths.BucklingZ);
        }
    }
}