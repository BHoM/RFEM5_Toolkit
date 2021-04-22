/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Physical.Materials;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.SectionProperties;
using BH.Engine.Adapter;
using BH.Engine.Geometry;
using BH.oM.Adapters.RFEM;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void ToRFEM(this BarUniformlyDistributedLoad load, int loadId, int loadcaseId)
        {
            rf.MemberLoad rfLoad = new rf.MemberLoad();
            rfLoad.No = loadId;
            rfLoad.Distribution = rf.LoadDistributionType.UniformType;
            rfLoad.RelativeDistances = false;

            if(load.Force.Length() == 0 & load.Moment.Length() == 0)
            {
                Engine.Reflection.Compute.RecordWarning("Zero forces set. No load pushed!");
                return;
            }

            if(load.Force.Length() != 0)
            {
                rfLoad.Type = rf.LoadType.ForceType;
                if(load.Force.Z!=0)
                {
                    rfLoad.Direction = rf.LoadDirectionType.GlobalZType;
                    rfLoad.Magnitude1 = load.Force.Z;//from documentation it looks like you need to create a load for each component! only .magnityde1 is used!
                }

            }

            if(load.Moment.Length() != 0)
            {
                rfLoad.Type = rf.LoadType.MomentType;

            }
            //check if force, moment or both
            rfLoad.Type = rf.LoadType.ForceType;
            //rfLoad.Type = rf.LoadType.MomentType;
            //rfLoad.Type = rf.LoadType.TemperatureType;
            //rfLoad.Magnitude1 = load.Force;

            //rfLoad.ObjectList = load.Objects.AdapterIds(typeof(RFEMId)).ToString();

        }
    }
}