/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;

using BH.oM.Common.Materials;
using BH.oM.Structure.Elements;
using BH.oM.Base;
using BH.oM.Data.Requests;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Adapter;
using BH.oM.Adapter.RFEM;
using BH.Engine.Base.Objects;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Constraints;
using BH.Adapter;

using rf = Dlubal.RFEM5;


namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public Fields                             ****/
        /***************************************************/

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        //Add any applicable constructors here, such as linking to a specific file or anything else as well as linking to that file through the (if existing) com link via the API
        [Description("Connect to RFEM")]
        [Input("filePath", "Input the optional file path to RFEM model. Default is to use the currently running instance")]
        [Input("RFEMSettings", "Input the optional RFEM Settings for the adapter. Default is null")]
        [Output("adapter", "Adapter to RFEM")]
        public RFEMAdapter(string filePath = "", RFEMSettings rfemSettings = null, bool active = false)
        {
            BH.Adapter.Modules.Structure.ModuleLoader.LoadModules(this);

            AdapterComparers = new Dictionary<Type, object>
            {
                {typeof(Bar), new BH.Engine.Structure.BarEndNodesDistanceComparer(3) },
                {typeof(Node), new BH.Engine.Structure.NodeDistanceComparer(3) },
                {typeof(ISectionProperty), new BHoMObjectNameOrToStringComparer() },
                {typeof(IMaterialFragment), new BHoMObjectNameComparer() },
                {typeof(LinkConstraint), new BHoMObjectNameComparer() },
            };

            DependencyTypes = new Dictionary<Type, List<Type>>
            {
                //{typeof(Node), new List<Type> { typeof(Constraint6DOF) } },
                {typeof(Bar), new List<Type> { typeof(ISectionProperty), typeof(Node) } },
                {typeof(ISectionProperty), new List<Type> { typeof(IMaterialFragment) } },
                {typeof(RigidLink), new List<Type> { typeof(LinkConstraint), typeof(Node) } },
                {typeof(FEMesh), new List<Type> { typeof(ISurfaceProperty), typeof(Node) } },
                {typeof(ISurfaceProperty), new List<Type> { typeof(IMaterialFragment) } },
                {typeof(Panel), new List<Type> { typeof(ISurfaceProperty) } }
            };


            if (active)
            {
                AdapterIdName = BH.Adapter.RFEM.Convert.AdapterIdName;

                if (!IsApplicationRunning())
                {
                    try
                    {
                        app = new rf.Application();

                        if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                            model = app.OpenModel(filePath);
                        else
                            app.CreateModel("testModel");

                        model = app.GetModel(0);

                        model.Activate();
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Error creating a new Dlubal RFEM instance. Check if licenses are available. Error message:\n {e.Message}");
                    }
                }
                else
                {
                    try
                    {
                        app = Marshal.GetActiveObject("RFEM5.Application") as rf.IApplication;

                        app.LockLicense();

                        model = app.GetActiveModel();
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Error attaching to an existing Dlubal RFEM instance. Look in Windows Task Manager if there is a frozen RFEM process in background and close it.");
                    }
                }

                modelData = model.GetModelData() as rf.IModelData2;//only this version of modeldata supports setting multiple elements at a time - this is not obvious from documentation!!

                app.Show(); // Shows the GUI

                app.UnlockLicense(); // needed here to prevent GUI lock after showing it
            }
        }

        /***************************************************/

        /***************************************************/
        /**** Public  Fields                           ****/
        /***************************************************/



        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static bool IsApplicationRunning()
        {
            var rfemProcesses = System.Diagnostics.Process.GetProcesses()
                         .Where(x => x.ProcessName.ToLower()
                                      .StartsWith("rfem"))
                         .ToList();

            return (rfemProcesses.Count > 0) ? true : false;
        }

        public bool TryToShowApp()
        {
            try
            {
                app.Show();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        private void AppLock()
        {
            if (lockLevel==0)
            {
                app.LockLicense();
                modelData.PrepareModification();
                lockLevel = 2;
            }
            else if (lockLevel==1)
            {
                modelData.PrepareModification();
                lockLevel = 2;
            }
        }

        private void AppUnlock()
        {
            if (lockLevel == 2)
            {
                modelData.FinishModification();
                app.UnlockLicense();
                lockLevel = 0;
            }
            else if (lockLevel == 1)
            {
                app.UnlockLicense();
                lockLevel = 0;
            }
        }

        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        private rf.IApplication app = null;
        private rf.IModel model = null;
        private rf.IModelData modelData = null;

        private Dictionary<Type, int> m_indexDict = new Dictionary<Type, int>();
        private Dictionary<int, IMaterialFragment> m_materialDict = new Dictionary<int, IMaterialFragment>();
        private Dictionary<int, ISectionProperty> m_sectionDict = new Dictionary<int, ISectionProperty>();

        private int lockLevel = 0;

        /***************************************************/

    }
}
