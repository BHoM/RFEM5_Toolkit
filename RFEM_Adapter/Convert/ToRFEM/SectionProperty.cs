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
using BH.oM.Physical.Materials;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static rf.CrossSection ToRFEM(this ISectionProperty sectionProperty, int sectionPropertyId, int materialId)
        {
            rf.CrossSection rfSectionProperty = new rf.CrossSection();
            string name;
            rfSectionProperty.No = sectionPropertyId;
            rfSectionProperty.MaterialNo = materialId;
            rfSectionProperty.AxialArea = sectionProperty.Area;
            rfSectionProperty.TorsionMoment = sectionProperty.J;
            rfSectionProperty.ShearAreaY = sectionProperty.Asy;
            rfSectionProperty.ShearAreaZ = sectionProperty.Asz;
            rfSectionProperty.BendingMomentY = sectionProperty.Iy;
            rfSectionProperty.BendingMomentZ = sectionProperty.Iz;

            if(sectionProperty.Name != "")
            {
                name = sectionProperty.Name;
            }
            else
            {
                switch (sectionProperty)
                {
                    case SteelSection s:
                        if (s.SectionProfile.Name != "")
                            name = s.SectionProfile.Name;
                        else
                            name = s.SectionProfile.Shape.ToString();
                        break;
                    case AluminiumSection s:
                        if (s.SectionProfile.Name != "")
                            name = s.SectionProfile.Name;
                        else
                            name = s.SectionProfile.Shape.ToString();
                        break;
                    case GenericSection s:
                        if (s.SectionProfile.Name != "")
                            name = s.SectionProfile.Name;
                        else
                            name = s.SectionProfile.Shape.ToString();
                        break;
                    case TimberSection s:
                        if (s.SectionProfile.Name != "")
                            name = s.SectionProfile.Name;
                        else
                            name = s.SectionProfile.Shape.ToString();
                        break;
                    case ConcreteSection s:
                        if (s.SectionProfile.Name != "")
                            name = s.SectionProfile.Name;
                        else
                            name = s.SectionProfile.Shape.ToString();
                        break;
                    default:
                        Engine.Reflection.Compute.RecordWarning("No name provided for section No:" + sectionPropertyId.ToString() + ". Name set to Id number.");
                        name = "section id: " + sectionPropertyId.ToString();
                        break;
                }
            }

            rfSectionProperty.Description = name;
            rfSectionProperty.TextID = name;

            return rfSectionProperty;

        }


    }
}
