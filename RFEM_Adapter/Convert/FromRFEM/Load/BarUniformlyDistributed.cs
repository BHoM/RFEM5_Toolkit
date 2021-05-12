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
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Loads;
using BH.Adapter.RFEM;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BarUniformlyDistributedLoad FromRFEM(this rf.MemberLoad rfMemberLoad, Loadcase bhLoadcase, oM.Base.BHoMGroup<Bar> objectGroup)
        {
            LoadAxis loadAxis;
            bool projected;
            string direction;

            BarUniformlyDistributedLoad barUniformLoad = new BarUniformlyDistributedLoad();
            barUniformLoad.Loadcase = bhLoadcase;
            barUniformLoad.Objects = objectGroup;

            
            GetLoadingParameters(rfMemberLoad, out loadAxis, out projected, out direction);
            barUniformLoad.Axis = loadAxis;
            barUniformLoad.Projected = projected;

            switch (direction)
            {
                case "X":
                    barUniformLoad.Force.X = rfMemberLoad.Magnitude1;
                    break;
                case "Y":
                    barUniformLoad.Force.Y = rfMemberLoad.Magnitude1;
                    break;
                case "Z":
                    barUniformLoad.Force.Z = rfMemberLoad.Magnitude1;
                    break;
                default:
                    break;
            }


            return barUniformLoad;
        }

        /***************************************************/

        private static void GetLoadingParameters(rf.MemberLoad rfMemberLoad, out LoadAxis loadAxis, out bool projected, out string direction)
        {
            loadAxis = LoadAxis.Global;
            projected = false;
            direction = "";

            switch (rfMemberLoad.Direction)
            {
                case rf.LoadDirectionType.GlobalXType:
                    loadAxis = LoadAxis.Global;
                    direction = "X";
                    break;
                case rf.LoadDirectionType.GlobalYType:
                    loadAxis = LoadAxis.Global;
                    direction = "Y";
                    break;
                case rf.LoadDirectionType.GlobalZType:
                    loadAxis = LoadAxis.Global;
                    direction = "Z";
                    break;
                case rf.LoadDirectionType.LocalXType:
                    loadAxis = LoadAxis.Local;
                    direction = "X";
                    break;
                case rf.LoadDirectionType.LocalYType:
                    loadAxis = LoadAxis.Local;
                    direction = "Y";
                    break;
                case rf.LoadDirectionType.LocalZType:
                    loadAxis = LoadAxis.Local;
                    direction = "Z";
                    break;
                case rf.LoadDirectionType.ProjectXType:
                    loadAxis = LoadAxis.Global;
                    projected = true;
                    direction = "X";
                    break;
                case rf.LoadDirectionType.ProjectYType:
                    loadAxis = LoadAxis.Global;
                    projected = true;
                    direction = "Y";
                    break;
                case rf.LoadDirectionType.ProjectZType:
                    loadAxis = LoadAxis.Global;
                    projected = true;
                    direction = "Z";
                    break;
                case rf.LoadDirectionType.PerpendicularZType:
                case rf.LoadDirectionType.UnknownLoadDirectionType:
                case rf.LoadDirectionType.LocalUType:
                case rf.LoadDirectionType.LocalVType:
                    Engine.Reflection.Compute.RecordWarning("Load direction type: " + rfMemberLoad.Direction.ToString() + " is not supported!");
                    break;
                default:
                    break;
            }
        }
    }
}

