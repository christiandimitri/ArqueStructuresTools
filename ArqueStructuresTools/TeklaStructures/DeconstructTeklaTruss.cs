﻿using System;
using System.Collections.Generic;
using ArqueStructuresTools.Params;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace ArqueStructuresTools
{
    public class DeconstructTeklaTruss : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructTruss class.
        /// </summary>
        public DeconstructTeklaTruss()
            : base("Deconstruct Tekla Truss", "DeTekTruss",
                "Deconstruct a Tekla Truss into its component parts",
                "Arque Structures", "Tekla Structures")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TrussParameter(), "Truss", "T", "Truss to deconstruct", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BeamParameter(), "Top beam", "TB", "Truss top beam", GH_ParamAccess.item);
            pManager.AddParameter(new BeamParameter(), "Bottom beam", "BB", "Truss bottom beam", GH_ParamAccess.item);
            pManager.AddParameter(new BeamParameter(), "Intermediate beams", "IB", "Truss intermediate beams", GH_ParamAccess.item);
            pManager.AddPointParameter("Top nodes", "TN", "Truss top nodes", GH_ParamAccess.list);
            pManager.AddPointParameter("Bottom nodes", "BN", "Truss bottom nodes", GH_ParamAccess.list);
            pManager.AddParameter(new ColumnParameter(), "Static columns", "SC", "Truss static columns", GH_ParamAccess.list);
            pManager.AddParameter(new ColumnParameter(), "Boundary columns", "BC", "Truss boundary columns", GH_ParamAccess.list);
            pManager.AddPointParameter("Boundary nodes", "BN", "Truss boundary nodes", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            TrussGoo trussGoo = new TrussGoo();

            if (!DA.GetData(0, ref trussGoo)) return;

            var truss = trussGoo.Value;
            var staticColumnsGoo = new List<ColumnGoo>();
            var boundaryColumnsGoo = new List<ColumnGoo>();
            var topBeamGoo = (truss.TopBeam != null) ? new BeamGoo(truss.TopBeam) : new BeamGoo();
            var bottomBeamGoo = (truss.BottomBeam != null) ? new BeamGoo(truss.BottomBeam) : new BeamGoo();
            var intermediateBeamsGoo =
                (truss.IntermediateBeams != null) ? new BeamGoo(truss.IntermediateBeams) : new BeamGoo();

            if (truss.StaticColumns != null)
            {
                foreach (var cl in truss.StaticColumns)
                {
                    staticColumnsGoo.Add(new ColumnGoo(cl));
                }
            }

            if (truss.BoundaryColumns != null)
            {
                foreach (var cl in truss.BoundaryColumns)
                {
                    boundaryColumnsGoo.Add(new ColumnGoo(cl));
                }
            }

            DA.SetData(0, topBeamGoo);
            DA.SetData(1, bottomBeamGoo);
            DA.SetData(2, intermediateBeamsGoo);
            DA.SetDataList(5, staticColumnsGoo);
            DA.SetDataList(6, boundaryColumnsGoo);
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

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("233B8B96-D2AE-48EF-BF89-8AEEA95C3F96"); }
        }
    }
}