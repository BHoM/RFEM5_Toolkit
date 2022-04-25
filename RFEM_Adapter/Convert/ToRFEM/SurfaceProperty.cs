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
using BH.oM.Structure.SurfaceProperties;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static rf.SurfaceStiffness ToRFEM(this ISurfaceProperty surfaceProperty)
        {
            rf.SurfaceStiffness stiffness = new rf.SurfaceStiffness();

            

            if (surfaceProperty is LoadingPanelProperty)
            {
                Engine.Base.Compute.RecordWarning("sorry, Can't do loding panels");

            }
            else
            {

                if (surfaceProperty is ConstantThickness)
                {
                    ConstantThickness constantThickness = surfaceProperty as ConstantThickness;
                    stiffness.Type = rf.OrthotropyType.ConstantThickness;
                    stiffness.Thickness = constantThickness.Thickness;

                    stiffness.MultiplicationFactors.K = 1.0;
                    stiffness.MultiplicationFactors.K33 = 1.0;
                    stiffness.MultiplicationFactors.K44 = 1.0;
                    stiffness.MultiplicationFactors.K55 = 1.0;
                    stiffness.MultiplicationFactors.Kb = 1.0;
                    stiffness.MultiplicationFactors.Ke = 1.0;
                    stiffness.MultiplicationFactors.Km = 1.0;
                    stiffness.MultiplicationFactors.Ks = 1.0;
                }
                else if (surfaceProperty is Ribbed)
                {
                    Ribbed prop = surfaceProperty as Ribbed;
                    stiffness.Type = rf.OrthotropyType.UnidirectionalRibbedPlate;
                    stiffness.Thickness = prop.Thickness;
                    stiffness.GeometricProperties.Height = prop.TotalDepth;
                    stiffness.GeometricProperties.Spacing = prop.Spacing;
                    stiffness.GeometricProperties.Width = prop.StemWidth;
                    stiffness.GeometricProperties.Thickness = prop.Thickness;


                    stiffness.MultiplicationFactors.K = 1.0;
                    stiffness.MultiplicationFactors.K33 = 1.0;
                    stiffness.MultiplicationFactors.K44 = 1.0;
                    stiffness.MultiplicationFactors.K55 = 1.0;
                    stiffness.MultiplicationFactors.Kb = 1.0;
                    stiffness.MultiplicationFactors.Ke = 1.0;
                    stiffness.MultiplicationFactors.Km = 1.0;
                    stiffness.MultiplicationFactors.Ks = 1.0;

                }
                else if (surfaceProperty is Waffle)
                {
                    Waffle prop = surfaceProperty as Waffle;
                    stiffness.Type = rf.OrthotropyType.BidirectionalRibbedPlate;
                    stiffness.Thickness = prop.Thickness;
                    stiffness.GeometricProperties.HeightX = prop.TotalDepthX;
                    stiffness.GeometricProperties.HeightY = prop.TotalDepthY;
                    stiffness.GeometricProperties.SpacingX = prop.SpacingX;
                    stiffness.GeometricProperties.SpacingY = prop.SpacingY;
                    stiffness.GeometricProperties.WidthX = prop.StemWidthX;
                    stiffness.GeometricProperties.WidthY = prop.StemWidthY;
                    stiffness.GeometricProperties.Thickness = prop.Thickness;

                    stiffness.MultiplicationFactors.K = 1.0;
                    stiffness.MultiplicationFactors.K33 = 1.0;
                    stiffness.MultiplicationFactors.K44 = 1.0;
                    stiffness.MultiplicationFactors.K55 = 1.0;
                    stiffness.MultiplicationFactors.Kb = 1.0;
                    stiffness.MultiplicationFactors.Ke = 1.0;
                    stiffness.MultiplicationFactors.Km = 1.0;
                    stiffness.MultiplicationFactors.Ks = 1.0;


                }
                else
                {
                    Engine.Base.Compute.RecordWarning("my responses are limited. I don't know: " + surfaceProperty.Name);
                }

            }
            stiffness.Comment = surfaceProperty.Name;

            return stiffness;

        }


    }
}


