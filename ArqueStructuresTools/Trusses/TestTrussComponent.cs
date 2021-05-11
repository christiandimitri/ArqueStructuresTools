using System;
using ArqueStructuresTools.Options;
using WarehouseLib;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib.Trusses;
using WarehouseLib.Options;

namespace ArqueStructuresTools
{
    public class TestTrussComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MonopichTruss class.
        /// </summary>
        public TestTrussComponent()
            : base("Construct Truss", "Nickname",
                "Description",
                "Arque Structures", "Trusses")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        // ReSharper disable once RedundantNameQualifier
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "p", "p", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddGenericParameter("Options", "o", "o", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        // ReSharper disable once RedundantNameQualifier
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TrussParameter(), "Truss", "", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        // ReSharper disable once InconsistentNaming
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.WorldXY;
            var trussInputs = new TrussInputs();
            if (!DA.GetData(0, ref plane)) return;
            if (!DA.GetData(1, ref trussInputs)) return;
            var height = trussInputs.Height;
            var length = trussInputs.Width;
            var maxHeight = trussInputs.MaxHeight;
            var clearHeight = trussInputs.ClearHeight;
            var divisions = trussInputs.Divisions;
            var trussType = trussInputs.TrussType;
            var articulationType = trussInputs.ArticulationType;
            var baseType = trussInputs.BaseType;
            var columnsCount = 2;
            var typology = trussInputs.Typology;
            Truss truss = null;
            switch (typology)
            {
                case 0:
                {
                    truss = new FlatTruss(plane, length, height, maxHeight, clearHeight, divisions, trussType,
                        articulationType, columnsCount);
                    break;
                }
                case 1:
                {
                    truss = new ArchTruss(plane, length, height, maxHeight, clearHeight, divisions, trussType,
                        articulationType, baseType, columnsCount);
                    break;
                }
                case 2:
                {
                    truss = new MonopichedTruss(plane, length, height, maxHeight, clearHeight, divisions,
                        trussType,
                        articulationType, baseType, columnsCount);
                    break;
                }
                case 3:
                {
                    truss = new DoublepichedTruss(plane, 0, height, maxHeight, clearHeight, divisions, trussType,
                        articulationType, length / 2, length / 2, baseType, columnsCount);
                    break;
                }
            }

            DA.SetData(0, new TrussGoo(truss));
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
            get { return new Guid("714CCE00-46CC-4CB7-B257-4A6B77400AB2"); }
        }
    }
}