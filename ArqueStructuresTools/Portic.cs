using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace ArqueStructuresTools
{
    public class Portic : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Portic()
          : base("Portic/Truss", "Pr",
              "Description",
              "Arque Structures", "Portics")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            // You can often supply default values when creating parameters.
            // All parameters must have the correct access type. If you want 
            // to import lists or trees of values, modify the ParamAccess flag.
            pManager.AddPlaneParameter("plane", "pl", "description", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddIntegerParameter("typology", "ty", "description", GH_ParamAccess.item, 3);
            pManager.AddIntegerParameter("base type", "bT", "description", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("support type", "sT", "description", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("span left", "sL", "description", GH_ParamAccess.item, 5000);
            pManager.AddIntegerParameter("span right", "sR", "description", GH_ParamAccess.item, 5000);
            pManager.AddIntegerParameter("max height", "mH", "description", GH_ParamAccess.item, 3500);
            pManager.AddIntegerParameter("clear height", "cH", "description", GH_ParamAccess.item, 2700);
            pManager.AddIntegerParameter("left height", "lH", "description", GH_ParamAccess.item, 3000);
            pManager.AddIntegerParameter("right height", "rH", "description", GH_ParamAccess.item, 3000);
            pManager.AddIntegerParameter("subdivision count", "sC", "description", GH_ParamAccess.item, 4);
            pManager.AddIntegerParameter("min length", "miL", "description", GH_ParamAccess.item, 1500);
            pManager.AddIntegerParameter("max length", "maL", "description", GH_ParamAccess.item, 2000);
            pManager.AddIntegerParameter("portic type", "pT", "description", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("truss type", "tT", "description", GH_ParamAccess.item, 0);
            // If you want to change properties of certain parameters, 
            // you can use the pManager instance to access them by index:
            //pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Use the pManager object to register your output parameters.
            // Output parameters do not have default values, but they too must have the correct access type.
            pManager.AddPointParameter("upper points", "uP", "upper base points", GH_ParamAccess.list);
            pManager.AddCurveParameter("upper curves", "uC", "upper curves points", GH_ParamAccess.list);
            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            //pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.WorldXY;
            int typology = 3;
            int baseType = 0;
            int supType = 1;
            int spanLeft = 5000;
            int spanRight = 5000;
            int maxHeight = 3500;
            int clearHeight = 2700;
            int clHeight = 3000;
            int crHeight = 3000;
            int subdCount = 4;
            int minLength = 1500;
            int maxLength = 2000;
            int porticType = 0;
            int trussType = 0;
            if (!DA.GetData(0, ref plane)) return;
            if (!DA.GetData(1, ref typology)) return;
            if (!DA.GetData(2, ref baseType)) return;
            if (!DA.GetData(3, ref supType)) return;
            if (!DA.GetData(4, ref spanLeft)) return;
            if (!DA.GetData(5, ref spanRight)) return;
            if (!DA.GetData(6, ref maxHeight)) return;
            if (!DA.GetData(7, ref clearHeight)) return;
            if (!DA.GetData(8, ref clHeight)) return;
            if (!DA.GetData(9, ref crHeight)) return;
            if (!DA.GetData(10, ref subdCount)) return;
            if (!DA.GetData(11, ref minLength)) return;
            if (!DA.GetData(12, ref maxLength)) return;
            if (!DA.GetData(13, ref porticType)) return;
            if (!DA.GetData(14, ref trussType)) return;
            List<Point3d> upperBasePoints = new List<Point3d>();
            List<Curve> upperBaseCurves = new List<Curve>();
            if(typology > 3)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Topology type selection should be between 0 and 3");
            }
            // straight typology
            if(typology == 0)
            {
                List<Point3d> straightPoints = Straight.StraightPoints.UpperBasePoints(plane, spanLeft, maxHeight);
                upperBasePoints = straightPoints;
                List<Curve> straightBaseCurves = Duopich.DuopichCurves.UpperBaseCurves(straightPoints);
                upperBaseCurves = straightBaseCurves;

            }
            // arch typology
            else if (typology == 1)
            {
                List<Point3d> archPoints = Arch.ArchPoints.UpperBasePoints(plane, spanLeft, maxHeight, ref clHeight, ref crHeight);
                upperBasePoints = archPoints;
                List<Curve> archBaseCurves = Arch.ArchCurves.UpperBaseCurves(archPoints);
                upperBaseCurves = archBaseCurves;
            }
            // monopich typology
            else if(typology ==2)
            {
                List<Point3d> monopichPoints = Monopich.MonopichPoints.UpperBasePoints(plane, spanLeft, ref clHeight,ref crHeight);
                upperBasePoints = monopichPoints;
                List<Curve> monopichBaseCurves = Monopich.MonopichCurves.UpperBaseCurves(monopichPoints);
                upperBaseCurves = monopichBaseCurves;

            }
            // duopich typology
            else if(typology == 3)
            {
                List<Point3d> duopichPoints = Duopich.DuopichPoints.UpperBasePoints(plane, spanLeft, spanRight, maxHeight, ref clHeight, ref crHeight);
                upperBasePoints = duopichPoints;
                List<Curve> duopichBaseCurves = Duopich.DuopichCurves.UpperBaseCurves(duopichPoints);
                upperBaseCurves = duopichBaseCurves;

            }

            DA.SetDataList(0, upperBasePoints);
            DA.SetDataList(1, upperBaseCurves);
        }

        

        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("967e44ae-d6a3-4d84-b116-3e592554f49e"); }
        }
    }
}
