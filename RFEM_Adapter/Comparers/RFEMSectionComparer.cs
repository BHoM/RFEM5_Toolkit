/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Structure.SectionProperties;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM5;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM5
{
    public class RFEMSectionComparer : IEqualityComparer<ISectionProperty>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public RFEMSectionComparer()
        {

        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(ISectionProperty section1, ISectionProperty section2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(section1, section2)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(section1, null) || Object.ReferenceEquals(section2, null))
                return false;

            string sectionName1=Convert.RFEMSectionName(section1);
            string sectionName2 = Convert.RFEMSectionName(section2);

            if (sectionName1.Equals(sectionName2) && section1.Material.Name.Equals(section2.Material.Name)) return true;


            return false;

        }

        /***************************************************/

        public int GetHashCode(ISectionProperty section)
        {
            return 0;
        }


        /***************************************************/


    }

}




