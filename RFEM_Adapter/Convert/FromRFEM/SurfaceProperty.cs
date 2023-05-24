/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Structure;
using BH.oM.Physical;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM5;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM5
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ISurfaceProperty FromRFEM(this rf.SurfaceStiffness rfStiffness, IMaterialFragment material)
        {
            ISurfaceProperty surfaceProperty = null;
            
            
            switch (rfStiffness.Type)
            {
                case rf.OrthotropyType.ConstantThickness:
                    surfaceProperty = new ConstantThickness { Thickness = rfStiffness.Thickness, Material = material };
                    break;
                case rf.OrthotropyType.UnidirectionalRibbedPlate:
                    surfaceProperty = new Ribbed
                    {
                        Thickness = rfStiffness.Thickness,
                        TotalDepth = rfStiffness.GeometricProperties.Height,
                        Spacing = rfStiffness.GeometricProperties.Spacing,
                        StemWidth = rfStiffness.GeometricProperties.Width,
                        Material = material
                    };
                    break;
                case rf.OrthotropyType.BidirectionalRibbedPlate:
                    surfaceProperty = new Waffle
                    {
                        Thickness = rfStiffness.Thickness,
                        TotalDepthX = rfStiffness.GeometricProperties.HeightX,
                        TotalDepthY = rfStiffness.GeometricProperties.HeightY,
                        SpacingX = rfStiffness.GeometricProperties.SpacingX,
                        SpacingY = rfStiffness.GeometricProperties.SpacingY,
                        StemWidthX = rfStiffness.GeometricProperties.WidthX,
                        StemWidthY = rfStiffness.GeometricProperties.WidthY
                    };
                    break;
                case rf.OrthotropyType.UnknownOrthotropyType:
                case rf.OrthotropyType.EffectiveThickness:
                case rf.OrthotropyType.DefinedByStiffnessMatrix:
                case rf.OrthotropyType.Coupling:
                case rf.OrthotropyType.TrapezoidalSheet:
                case rf.OrthotropyType.HollowCoreSlab:
                case rf.OrthotropyType.Grillage:
                case rf.OrthotropyType.UnidirectionalBoxFloor:
                case rf.OrthotropyType.Glass:
                case rf.OrthotropyType.Laminate:
                    surfaceProperty = new ConstantThickness { Thickness = rfStiffness.Thickness, Material = material };
                    Engine.Base.Compute.RecordError("could not create surface property for " + rfStiffness.ID);
                    break;
                default:
                    surfaceProperty = new ConstantThickness { Thickness = rfStiffness.Thickness, Material = material };
                    Engine.Base.Compute.RecordError("could not create surface property for "+rfStiffness.ID);
                    break;
            }


            surfaceProperty.Name = rfStiffness.Comment;
            surfaceProperty.SetAdapterId(typeof(RFEM5Id), rfStiffness.No);
            return surfaceProperty;

        }

    }
}



