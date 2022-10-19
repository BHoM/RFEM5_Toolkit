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
using BH.oM.Physical.Materials;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using rf = Dlubal.RFEM5;
using BH.Engine.Structure;

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

            name = sectionProperty.DescriptionOrName();

            HashSet<string> standardCSNames = new HashSet<string>() { "HE", "CHS", "IPE", "L", "UPE", "PFC", "RHS", "TUB", "TUC", "UB", "UBP", "UC"};
                

            if (!standardCSNames.Contains(sectionProperty.Name.Split(' ')[0]))
            {


                String matString = Engine.Base.Query.PropertyValue(sectionProperty, "Material").ToString().Equals("BH.oM.Structure.MaterialFragments.Concrete") ? "Concrete" : "Steel";
                String shapeString = Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.Shape").ToString();

                double diameter, width, height, thickness;

                switch (shapeString)
                {
                    case "Circle":

                        diameter = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.Diameter")) * 1000;
                        name = "Circle " + diameter;

                        break;

                    case "Rectangle":

                        height = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.Height")) * 1000;
                        width = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.Width")) * 1000;
                        name = "Rectangle " + width + "/" + height;

                        break;

                    case "Tube":

                        diameter = System.Convert.ToInt32(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.Diameter")) * 1000;
                        thickness = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.Thickness")) * 1000;
                        String prefix = matString.Equals("Steel") ? "Pipe " : "Ring ";
                        name = prefix + diameter + "/" + thickness;

                        break;

                    case "Box":

                        height = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.Height")) * 1000;
                        width = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.Width")) * 1000;
                        thickness = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.Thickness")) * 1000;
                        if (thickness == 0)
                        {
                            thickness = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.WebThickness"));
                            Engine.Base.Compute.RecordWarning("Flange Thickness hab been changed to " + thickness + "!");
                            Engine.Base.Compute.RecordWarning("Weld Size have been ingored!");
                        }

                        prefix = matString.Equals("Steel") ? "TO " : "HK ";
                        name = prefix + height + "/" + width + "/" + thickness + "/" + thickness + "/" + thickness + "/" + thickness;
                        Engine.Base.Compute.RecordWarning("Outer and inner radius of Box profile have been set to 0!");
                        break;

                    case "ISection":

                        height = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.Height")) * 1000;
                        width = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.Width")) * 1000;
                        //Check if I section symetrical, if not width is not a property and is 0
                        if (width == 0)
                        {
                            double topFlWidth = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.TopFlangeWidth")) * 1000;
                            double botFlWidth = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.BotFlangeWidth")) * 1000;
                            double webThicknesss = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.WebThickness")) * 1000;
                            double topFlThickness = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.TopFlangeThickness")) * 1000;
                            double botFlThickness = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.BotFlangeThickness")) * 1000;
                            double weldSize = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.WeldSize")) * 1000;
                            name = "IU " + height + "/" + topFlWidth + "/" + topFlThickness + "/" + webThicknesss + "/" + botFlWidth + "/" + botFlThickness + "/" + weldSize + "/" + weldSize;
                        }
                        else
                        {

                            double webThicknesss = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.WebThickness")) * 1000;
                            double flangeThicknesss = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.FlangeThickness")) * 1000;

                            prefix = matString.Equals("Steel") ? "IS " : "ITS ";
                            name = prefix + height + "/" + width + "/" + webThicknesss + "/" + flangeThicknesss;
                            Engine.Base.Compute.RecordWarning("Root and toe radius of the ISection have been set to 0!");
                        }

                        break;


                    case "Tee":

                        height = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.Height")) * 1000;
                        width = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.Width")) * 1000;
                        double webThickness = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.WebThickness")) * 1000;
                        double flangeThickness = System.Convert.ToDouble(Engine.Base.Query.PropertyValue(sectionProperty, "SectionProfile.FlangeThickness")) * 1000;
                        prefix = matString.Equals("Steel") ? "TS " : "FB ";
                        name = prefix + height + "/" + width + "/" + webThickness + "/" + flangeThickness;
                        Engine.Base.Compute.RecordWarning("Root and toe radius of the ISection have been set to 0!");
                        break;

                    default:

                        name = sectionProperty.DescriptionOrName();
                        break;
                }
            }

            //TODO 
            // The CrossSection class does both use the attribute name and discribtion. 
            // The Grasshopper interface does only know name
            rfSectionProperty.Description = name;
            rfSectionProperty.TextID = name;
            rfSectionProperty.Comment = sectionProperty.Name;

            return rfSectionProperty;

        }


    }
}


