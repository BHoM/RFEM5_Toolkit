/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System.ComponentModel;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.MaterialFragments;
using BH.Engine.Structure;
using BH.oM.Base.Attributes;
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
        [Description("Get list of Ids from RFEM string.")]
        [Input("rfMaterial", "String with RFEM ids.")]
        [Output("idList", "List of ids.")]
        public static MaterialType MaterialType(this rf.Material rfMaterial)
        {
            //string[] materialStringArr = rfMaterial.TextID.Split('@');

         
            //string materialString = rfMaterial.TextID == "" ? rfMaterial.Description.Split(':')[0] :materialStringArr[1].Split('|')[1];

            string materialString = "";
            string[] materialStringArr;

            if (rfMaterial.Equals(null))
            {
                Engine.Base.Compute.RecordWarning("Material was Null and has been set to Steel");
                return oM.Structure.MaterialFragments.MaterialType.Steel; ; //A suitable return - you could `return null;` here instead if needed
            }
            else
            {

                materialStringArr = rfMaterial.TextID.Split('@');
                materialString = rfMaterial.TextID == "" ? rfMaterial.Description.Split(':')[0] : materialStringArr[1].Split('|')[1];
            }


            switch (materialString)
            {

                case "TypeID|STEEL":
                case "STEEL":
                    return oM.Structure.MaterialFragments.MaterialType.Steel;
                case "ALUMINIUM":
                    return oM.Structure.MaterialFragments.MaterialType.Aluminium;
                case "CONCRETE":
                    return oM.Structure.MaterialFragments.MaterialType.Concrete;
                case "TIMBER":
                case "CONIFEROUS":
                    return oM.Structure.MaterialFragments.MaterialType.Timber;
                case "CABLE":
                    return oM.Structure.MaterialFragments.MaterialType.Cable;
                case "GLASS":
                    return oM.Structure.MaterialFragments.MaterialType.Glass;
                case "TREBAR":
                    return oM.Structure.MaterialFragments.MaterialType.Rebar;
                case "TENDON":
                    return oM.Structure.MaterialFragments.MaterialType.Tendon;
                default:
                    Engine.Base.Compute.RecordWarning("Don't know how to make: " + materialString[1]);
                    return oM.Structure.MaterialFragments.MaterialType.Undefined;
            }

        }


        /***************************************************/
    }
}


