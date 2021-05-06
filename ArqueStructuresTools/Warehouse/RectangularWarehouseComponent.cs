﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;
using WarehouseLib.Warehouses;

namespace ArqueStructuresTools
{
    public class RectangularWarehouseComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the TestWarehouseComponent class.
        /// </summary>
        public RectangularWarehouseComponent()
            : base("Construct Rectangular Warehouse", "Nickname",
                "Description",
                "Arque Structures", "Warehouses")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        // ReSharper disable once RedundantNameQualifier
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "p", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddNumberParameter("Length", "L", "L", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Width", "w", "w", GH_ParamAccess.item, 5);
            pManager.AddNumberParameter("Height", "h", "h", GH_ParamAccess.item, 4);
            pManager.AddNumberParameter("Max height", "mh", "h", GH_ParamAccess.item, 6);
            pManager.AddNumberParameter("Clear height", "ch", "h", GH_ParamAccess.item, 3.5);
            pManager.AddIntegerParameter("Typology", "T", "T", GH_ParamAccess.item, 2);
            pManager.AddIntegerParameter("Count", "C", "C", GH_ParamAccess.item, 3);
            pManager.AddTextParameter("Truss type", "tt", "tt", GH_ParamAccess.item, "Warren");
            pManager.AddIntegerParameter("Columns count", "cc", "cc", GH_ParamAccess.item, 2);
            pManager.AddTextParameter("Roof bracing type", "rbt", "rbt", GH_ParamAccess.item, "Bracing");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new WarehouseParameter(), "Warehouse", "", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var plane = Plane.WorldXY;
            double length = 15;
            double width = 10;
            double height = 4;
            double maxHeight = 6;
            var clearHeight = 3.5;
            var typology = 2;
            var count = 3;
            var trussType = "";
            var columsCount = 0;
            var roofBracingType = "";
            if (!DA.GetData(0, ref plane)) return;
            if (!DA.GetData(1, ref length)) return;
            if (!DA.GetData(2, ref width)) return;
            if (!DA.GetData(3, ref height)) return;
            if (!DA.GetData(4, ref maxHeight)) return;
            if (!DA.GetData(5, ref clearHeight)) return;
            if (!DA.GetData(6, ref typology)) return;
            if (!DA.GetData(7, ref count)) return;
            if (!DA.GetData(8, ref trussType)) return;
            if (!DA.GetData(9, ref columsCount)) return;
            if (!DA.GetData(10, ref roofBracingType)) return;
            Warehouse warehouse = null;

            try
            {
                warehouse = new Warehouse(plane, length, width, height, maxHeight, clearHeight, typology, count,
                    trussType, columsCount, roofBracingType);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
                return;
            }

            DA.SetData(0, new WarehouseGoo(warehouse));
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
            get { return new Guid("A312F2DD-14A6-4196-8450-87079DA67FB5"); }
        }
    }
}