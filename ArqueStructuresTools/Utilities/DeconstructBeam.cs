using System;
using System.Collections;
using System.Collections.Generic;
using ArqueStructuresTools.Params;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;

namespace ArqueStructuresTools
{
    public class DeconstructBeam : GH_Component
    {
        public DeconstructBeam() : base("Deconstruct Beam", "DeBeam", "Deconstruct a Beam into its component parts",
            "Arque Structures",
            "Beam")
        {
        }

        public override Guid ComponentGuid => new Guid("1D7C237D-8CEE-4D93-A57D-CD1216FACCBF");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new BeamParameter(), "Beam", "B", "Beam to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Axis", "A", "Column axis", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Plane", "Pl", "Column orientation plane", GH_ParamAccess.item);
            pManager.AddTextParameter("Profile", "Pr", "Column profile name", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            BeamGoo beamGoo = new BeamGoo();

            if (!DA.GetData(0, ref beamGoo)) return;

            var axisCurve = new List<Curve>();
            for (int i = 0; i < beamGoo.Value.Axis.Count; i++)
            {
                axisCurve.Add(beamGoo.Value.Axis[i].AxisCurve);
            }

            DA.SetDataList(0, axisCurve);
            DA.SetData(1, beamGoo.Value.ProfileOrientationPlane);
            DA.SetData(2, beamGoo.Value.Profile.Name);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }
    }
}