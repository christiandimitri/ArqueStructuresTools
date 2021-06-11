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
            pManager.AddNumberParameter("Y Buckling", "YB", "Y buckling length multiplier", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Z Buckling", "ZB", "Z buckling length multiplier", GH_ParamAccess.item, 1);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Axis", "A", "Beam axis", GH_ParamAccess.list);
            pManager.AddNumberParameter("Y Buckling", "YB", "Y buckling length", GH_ParamAccess.item);
            pManager.AddNumberParameter("Z Buckling", "ZB", "Z buckling length", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var beamGoo = new BeamGoo();

            // get input data
            if (!DA.GetData(0, ref beamGoo)) return;
            var bucklingY = 0.0;
            var bucklingZ = 0.0;
            foreach (var bucklingLength in beamGoo.Value.BucklingLengths)
            {
                bucklingY = bucklingLength.BucklingY;
                bucklingZ = bucklingLength.BucklingZ;
            }

            // set output data 
            DA.SetDataList(0, beamGoo.Value.Axis);
            DA.SetData(1,
                bucklingY == 0 ? null : bucklingY);
            DA.SetData(2,
                bucklingZ == 0 ? null : bucklingZ);
        }
    }
}