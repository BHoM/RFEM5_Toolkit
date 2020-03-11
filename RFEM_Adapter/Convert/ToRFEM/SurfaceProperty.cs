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
using BH.oM.Structure.SurfaceProperties;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static rf.SurfaceThickness ToRFEM(this ISurfaceProperty surfaceProperty, int surfacePropertyId, int materialId)
        {
            rf.SurfaceThickness rfSurfaceProperty = new rf.SurfaceThickness();



            if (surfaceProperty is LoadingPanelProperty)
            {
                Engine.Reflection.Compute.RecordWarning("sorry, Can't do loding panels");

            }
            else
            {

                if (surfaceProperty is ConstantThickness)
                {
                    ConstantThickness constantThickness = surfaceProperty as ConstantThickness;
                    rfSurfaceProperty.Type = rf.SurfaceThicknessType.ConstantThicknessType;
                    rfSurfaceProperty.Constant = constantThickness.Thickness;

                }
                else if (surfaceProperty is Waffle)
                {
                    Engine.Reflection.Compute.RecordWarning("sorry, Can't do waffle slabs");
                }
                else if (surfaceProperty is Ribbed)
                {
                    Engine.Reflection.Compute.RecordWarning("sorry, Can't do ribbed slabs");
                }
                else
                {
                    Engine.Reflection.Compute.RecordWarning("my responses are limited. I don't know: " + surfaceProperty.Name);
                }

            }
            return SurfaceThickness;

        }


    }
}
