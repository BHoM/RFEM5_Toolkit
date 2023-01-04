/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
            barUniformLoad.SetAdapterId(typeof(RFEMId), rfMemberLoad.No);
            
            GetLoadingParameters(rfMemberLoad, out loadAxis, out projected, out direction);
            barUniformLoad.Axis = loadAxis;
            barUniformLoad.Projected = projected;

            switch (direction)
            {
                case "FX":
                    barUniformLoad.Force.X = rfMemberLoad.Magnitude1;
                    break;
                case "FY":
                    barUniformLoad.Force.Y = rfMemberLoad.Magnitude1;
                    break;
                case "FZ":
                    barUniformLoad.Force.Z = rfMemberLoad.Magnitude1;
                    break;
                case "UX":
                    barUniformLoad.Moment.X = rfMemberLoad.Magnitude1;
                    break;
                case "UY":
                    barUniformLoad.Moment.Y = rfMemberLoad.Magnitude1;
                    break;
                case "UZ":
                    barUniformLoad.Moment.Z = rfMemberLoad.Magnitude1;
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

            switch (rfMemberLoad.Type)
            {
                case rf.LoadType.ForceType:
                    direction = "F";
                    break;
                case rf.LoadType.MomentType:
                    direction = "U";
                    break;
                case rf.LoadType.TemperatureType:
                case rf.LoadType.AxialStrainType:
                case rf.LoadType.AxialDisplacementType:
                case rf.LoadType.PrecamberType:
                case rf.LoadType.InitialPrestressType:
                case rf.LoadType.EndPrestressType:
                case rf.LoadType.DisplacementType:
                case rf.LoadType.RotationLoadType:
                case rf.LoadType.FullPipeContentType:
                case rf.LoadType.PartialPipeContentType:
                case rf.LoadType.PipeInternalPressureType:
                case rf.LoadType.RotaryMotionType:
                case rf.LoadType.BuoyancyType:
                case rf.LoadType.UnknownLoadType:
                    Engine.Base.Compute.RecordWarning("Load type: " + rfMemberLoad.Type.ToString() + " is not supported!");
                    break;
                default:
                    break;
            }

            switch (rfMemberLoad.Direction)
            {
                case rf.LoadDirectionType.GlobalXType:
                    loadAxis = LoadAxis.Global;
                    direction += "X";
                    break;
                case rf.LoadDirectionType.GlobalYType:
                    loadAxis = LoadAxis.Global;
                    direction += "Y";
                    break;
                case rf.LoadDirectionType.GlobalZType:
                    loadAxis = LoadAxis.Global;
                    direction += "Z";
                    break;
                case rf.LoadDirectionType.LocalXType:
                    loadAxis = LoadAxis.Local;
                    direction += "X";
                    break;
                case rf.LoadDirectionType.LocalYType:
                    loadAxis = LoadAxis.Local;
                    direction += "Y";
                    break;
                case rf.LoadDirectionType.LocalZType:
                    loadAxis = LoadAxis.Local;
                    direction += "Z";
                    break;
                case rf.LoadDirectionType.ProjectXType:
                    loadAxis = LoadAxis.Global;
                    projected = true;
                    direction += "X";
                    break;
                case rf.LoadDirectionType.ProjectYType:
                    loadAxis = LoadAxis.Global;
                    projected = true;
                    direction += "Y";
                    break;
                case rf.LoadDirectionType.ProjectZType:
                    loadAxis = LoadAxis.Global;
                    projected = true;
                    direction += "Z";
                    break;
                case rf.LoadDirectionType.PerpendicularZType:
                case rf.LoadDirectionType.UnknownLoadDirectionType:
                case rf.LoadDirectionType.LocalUType:
                case rf.LoadDirectionType.LocalVType:
                    Engine.Base.Compute.RecordWarning("Load direction type: " + rfMemberLoad.Direction.ToString() + " is not supported!");
                    break;
                default:
                    break;
            }

        }
    }
}



