using System;
using System.Collections.Generic;
using ArqueStructuresTools.Params;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;
using WarehouseLib.Beams;

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
            pManager.AddTextParameter("Position", "Po", "Truss beam's position string", GH_ParamAccess.item);
            pManager.AddCurveParameter("Axis", "A", "Beam's axis", GH_ParamAccess.list);
            pManager.AddNumberParameter("Y Buckling", "YB", "Y buckling length", GH_ParamAccess.list);
            pManager.AddNumberParameter("Z Buckling", "ZB", "Z buckling length", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var beamGoo = new BeamGoo();

            // get input data
            if (!DA.GetData(0, ref beamGoo)) return;
            var yMultiplier = 1.0;
            if (!DA.GetData(1, ref yMultiplier)) return;
            var zMultiplier = 1.0;
            if (!DA.GetData(2, ref zMultiplier)) return;
            var bucklingYList = new List<double>();
            var bucklingZList = new List<double>();
            var axisList = new List<Curve>();

            if (beamGoo.Value.BucklingLengths != null)
            {
                for (var index = 0; index < beamGoo.Value.BucklingLengths.Count; index++)
                {
                    var bucklingLength = beamGoo.Value.BucklingLengths[index];
                    var bucklingY = bucklingLength.BucklingY * yMultiplier;
                    bucklingYList.Add(bucklingY);
                    var bucklingZ = bucklingLength.BucklingZ * zMultiplier;
                    bucklingZList.Add(bucklingZ);
                }
            }

            if (beamGoo.Value.SkeletonAxis != null)
            {
                for (int i = 0; i < beamGoo.Value.SkeletonAxis.Count; i++)
                {
                    axisList.Add(beamGoo.Value.SkeletonAxis[i].AxisCurve);
                }
            }

            var position = beamGoo.Value.Position != null ? beamGoo.Value.Position : string.Empty;
            
            // set output data 
            DA.SetData(0, position);
            DA.SetDataList(1, axisList);
            DA.SetDataList(2, bucklingYList);
            DA.SetDataList(3, bucklingZList);
        }
    }
}