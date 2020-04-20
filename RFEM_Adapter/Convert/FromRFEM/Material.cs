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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Physical;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IMaterialFragment FromRFEM(this rf.Material material)
        {
            IMaterialFragment bhMaterial;

            if (material.ModelType == rf.MaterialModelType.IsotropicLinearElasticType)
            {
                bhMaterial = Engine.Structure.Create.Steel("S355 - I am just for testing");
            }
            else if (material.ModelType == rf.MaterialModelType.IsotropicPlastic2D3DType)
            {
                bhMaterial = Engine.Structure.Create.Steel("S355 - I am just for testing");
            }
            else if (material.ModelType == rf.MaterialModelType.IsotropicPlastic2D3DType)
            {
                bhMaterial = Engine.Structure.Create.Steel("S355 - I am just for testing");
            }
            else
            {
                bhMaterial = Engine.Structure.Create.Steel("S355 - I am just for testing");
            }

            return bhMaterial;
        }

        private static MaterialType GetMaterialTypeFromString(string RFEMTextID)
        {
            string[] stringArr = RFEMTextID.Split('@');

            switch (stringArr[1])
            {
                case "TypeID|STEEL":
                    return MaterialType.Steel;
                case "TypeID|ALUMINIUM":
                    return MaterialType.Aluminium;
                case "TypeID|CONCRETE":
                    return MaterialType.Concrete;
                case "TypeID|TIMBER":
                    return MaterialType.Timber;
                case "TypeID|CABLE":
                    return MaterialType.Cable;
                case "TypeID|GLASS":
                    return MaterialType.Glass;
                case "TypeID|REBAR":
                    return MaterialType.Rebar;
                case "TypeID|TENDON":
                    return MaterialType.Tendon;
                default:
                    return MaterialType.Undefined;
            }
        }

    }
}
