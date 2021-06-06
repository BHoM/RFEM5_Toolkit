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
            int lcNo = loadcase.Number;

            if (loadcase.Number == 0)
                int.TryParse(loadcaseId, out lcNo);

            rfLoadcase.Loading.No = lcNo;
            rfLoadcase.Description = loadcase.Name;
            rfLoadcase.ActionCategory = GetLoadCategory(loadcase.Nature);
            rfLoadcase.ToSolve = true;
            
            //// unsure about these ***********
            //rfLoadcase.ToSolve = true;
            //rfLoadcase.Loading.Type = rf.LoadingType.LoadCaseType;
            //// ******************************

            return rfLoadcase;
        }

        private static rf.ActionCategoryType GetLoadCategory(LoadNature loadNature)
        {
            switch (loadNature)
            {
                case LoadNature.Dead:
                    return rf.ActionCategoryType.DeadLoad;
                case LoadNature.SuperDead:
                    return rf.ActionCategoryType.OtherCategory;//TODO: What is super dead load in RFEM terminology??
                case LoadNature.Live:
                    return rf.ActionCategoryType.Live;
                case LoadNature.Wind:
                    return rf.ActionCategoryType.Wind;
                case LoadNature.Seismic:
                    return rf.ActionCategoryType.Earthquake;
                case LoadNature.Temperature:
                    return rf.ActionCategoryType.UniformTemperatures;
                case LoadNature.Snow:
                    return rf.ActionCategoryType.Snow;
                case LoadNature.Accidental:
                    return rf.ActionCategoryType.Accidental;
                case LoadNature.Prestress:
                    return rf.ActionCategoryType.Prestress;
                case LoadNature.Notional:
                    return rf.ActionCategoryType.OtherCategory;
                case LoadNature.Other:
                    return rf.ActionCategoryType.OtherCategory;
                default:
                    return rf.ActionCategoryType.UnknownActionCategory;
            }

        }
    }
}

