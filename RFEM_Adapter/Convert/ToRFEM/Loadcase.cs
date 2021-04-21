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
using BH.oM.Adapters.RFEM;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static rf.LoadCase ToRFEM(this Loadcase loadcase, string loadcaseId)
        {
            rf.LoadCase rfLoadcase = new rf.LoadCase();
            rfLoadcase.ID = loadcaseId;
            rfLoadcase.ActionCategory = GetLoadCategory(loadcase.Nature);

            // unsure about these ***********
            rfLoadcase.ToSolve = true;
            rfLoadcase.Description = loadcase.Name;
            rfLoadcase.Loading.No = 1;
            rfLoadcase.Loading.Type = rf.LoadingType.LoadCaseType;
            // ******************************

            return rfLoadcase;
        }

        private static rf.ActionCategoryType GetLoadCategory(LoadNature loadNature)
        {
            rf.ActionCategoryType ac = rf.ActionCategoryType.UnknownActionCategory;

            switch (loadNature)
            {
                case LoadNature.Accidental:
                    ac = rf.ActionCategoryType.Accidental;
                    break;
                case LoadNature.Dead:
                    ac = rf.ActionCategoryType.DeadLoad;
                    break;
                case LoadNature.Live:
                    ac = rf.ActionCategoryType.Live;
                    break;
                case LoadNature.Notional:
                    ac = rf.ActionCategoryType.NotionalHorizontalForces;//???
                    break;
                case LoadNature.Prestress:
                    ac = rf.ActionCategoryType.Prestress;
                    break;
                case LoadNature.Seismic:
                    ac = rf.ActionCategoryType.GbSeismic;
                    break;
                case LoadNature.Snow:
                    ac = rf.ActionCategoryType.Snow;
                    break;
                case LoadNature.SuperDead:
                    ac = rf.ActionCategoryType.DeadLoadDL;
                    break;
                case LoadNature.Temperature:
                    ac = rf.ActionCategoryType.TemperatureNonFire;
                    break;
                case LoadNature.Wind:
                    ac = rf.ActionCategoryType.Wind;
                    break;
                case LoadNature.Other:
                default:
                    ac = rf.ActionCategoryType.UnknownActionCategory;
                    break;
            }

            return ac;
        }
    }
}

