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
using BH.oM.Structure.SectionProperties;
using BH.Engine.Structure;
using BH.Engine.RFEM;
using BH.oM.Physical;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ISectionProperty ToBHoM(this rf.CrossSection rfSectionProperty, rf.Material rfMaterial)
        {

            MaterialType materialType = Engine.RFEM.Query.GetMaterialType(rfMaterial);

            if (materialType == MaterialType.Steel)
            {
                ExplicitSection section = new ExplicitSection();
                
                section.CustomData[AdapterIdName] = rfSectionProperty.No;
                //section.Material = Structure.Create.Steel("default steel");
                section.Material = rfMaterial.ToBHoM();
                section.Name = rfSectionProperty.TextID;

                section.Area = rfSectionProperty.AxialArea;
                section.J = rfSectionProperty.TorsionMoment;
                section.Asy = rfSectionProperty.ShearAreaY;
                section.Asz = rfSectionProperty.ShearAreaZ;
                section.Iy = rfSectionProperty.BendingMomentY;
                section.Iz = rfSectionProperty.BendingMomentZ;

                return section;
            }
            else
            {
                Engine.Reflection.Compute.RecordError("dont know how to make" + rfSectionProperty.TextID);
                return null;
            }


        }

    }
}
