/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Common.Materials;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
//using BH.Adapter.Queries;
using BH.oM.Base;
using System.Runtime.InteropServices;
using rf = Dlubal.RFEM5;
using Dlubal.RFEM5;
using System.Reflection;
using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;

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
        public RFEMAdapter(string filePath = "", bool Active = false)
        {
            if (Active)
            {
                AdapterId = BH.Engine.RFEM.Convert.AdapterId;   //Set the "AdapterId" to "SoftwareName_id". Generally stored as a constant string in the convert class in the SoftwareName_Engine

                Config.SeparateProperties = true;   //Set to true to push dependant properties of objects before the main objects are being pushed. Example: push nodes before pushing bars
                Config.MergeWithComparer = true;    //Set to true to use EqualityComparers to merge objects. Example: merge nodes in the same location
                Config.ProcessInMemory = false;     //Set to false to to update objects in the toolkit during the push
                                                    //Config.CloneBeforePush = true;      //Set to true to clone the objects before they are being pushed through the software. Required if any modifications at all, as adding a software ID is done to the objects
                                                    //Config.UseAdapterId = true;         //Tag objects with a software specific id in the CustomData. Requires the NextIndex method to be overridden and implemented


                if (!IsApplicationRunning())
                {
                    app = new Application();

                    if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                        model = app.OpenModel(filePath);
                    else
                        app.CreateModel("testModel");

                    model = app.GetModel(0);

                    model.Activate();

                }
                else
                {
                    try
                    {
                        app = Marshal.GetActiveObject("RFEM5.Application") as IApplication;

                        app.LockLicense();

                        model = app.GetActiveModel();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error creating a new Dlubal RFEM instance. Check whether licenses are available. Error message:\n {e.Message}");
                    }
                }

                modelData = model.GetModelData();

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

        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        private IApplication app = null;
        private IModel model = null;
        private IModelData modelData = null;
        /// <summary>
        /// Level of RFEM application lock: 0=no lock - ready to return UI, 1=licence locked, 2=model locked and ready for modification 
        /// </summary>
        private int lockLevel = 0;


        /*******IModelData data = null;********************************************/


        /***************************************************/
        /**** Private Helper Methods                    ****/
        /***************************************************/


    }
}
