/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Geometry;
using BH.Adapter.RFEM5;
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

        public static Panel FromRFEM(this rf.Surface surface, ISurfaceProperty surfaceProperty)
        {
            //TODO: this is not currently being called from "ReadPanels" - move relevant conversions to this extension method
            ICurve outline = null;

            string[] boundLineList = surface.BoundaryLineList.Split(',');
            Panel bhPanel = Engine.Structure.Create.Panel(outline, property: surfaceProperty);

            bhPanel.SetAdapterId(typeof(RFEM5Id), surface.No);


            return bhPanel;
        }

        /***************************************************/
    }
}





