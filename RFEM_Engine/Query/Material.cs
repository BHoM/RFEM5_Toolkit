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
using BH.Engine.Structure;
using rf = Dlubal.RFEM5;

namespace BH.Engine.Adapters.RFEM
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        //Text ID contains the following parameters: 
        //• NameID - Language independent name of the material according to the German terminology.
        //• TypeID - Language independent type of the material.
        //• NormID - Language independent code of the material. 
        //Format: NameID|material ID@TypeID|material type@NormID|material code
        //Example: NameID|Steel S 235@TypeID|STEEL@StandardID|DIN EN 1993-1-1-10 

        public static MaterialType GetMaterialType(rf.Material rfMaterial)
        {
            string[] materialString = rfMaterial.TextID.Split('@');

            if(materialString.Count()<2)
            {
                Engine.Reflection.Compute.RecordWarning("Don't know how to make" + rfMaterial.TextID + ". Steel created instead!");
                return MaterialType.Steel;
            }

            switch (materialString[1])
            {
                case "TypeID|STEEL":
                    return MaterialType.Steel;
                case "TypeID|ALUMINIUM":
                    return MaterialType.Aluminium;
                case "TypeID|CONCRETE":
                    return MaterialType.Concrete;
                case "TypeID|TIMBER":
                case "TypeID|CONIFEROUS":
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
                    Engine.Reflection.Compute.RecordWarning("Don't know how to make: " + materialString[1]);
                    return MaterialType.Undefined;
            }

        }


        public static string GetMaterialType(IMaterialFragment material)
        {
            Type materialType = material.GetType();

            if (materialType == typeof(Aluminium))
            {
                return "TypeID|ALUMINIUM";
            }
            if (materialType == typeof(Steel))
            {
                return "TypeID|STEEL";
            }
            if (materialType == typeof(Concrete))
            {
                return "TypeID|CONCRETE";
            }
            if (materialType == typeof(Timber))
            {
                return "TypeID|TIMBER";
            }
            else
            {
                return null;
            }

        }

        public static string GetMaterialName(rf.Material rfMaterial)
        {
            string materialName;
            string[] materialString = rfMaterial.TextID.Split('@');
            if (materialString.Length < 2)
                materialName = rfMaterial.Description;
            else
                materialName = materialString[0].Split('|')[1];

            return materialName;
        }


        /***************************************************/
    }
}
