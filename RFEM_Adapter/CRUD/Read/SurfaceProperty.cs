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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Common.Materials;
using BH.Engine.RFEM;
using rf = Dlubal.RFEM5;
using rf3 = RFEM3;


namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private List<ISurfaceProperty> ReadSurfaceProperties(List<string> ids = null)
        {
            List<ISurfaceProperty> surfacePropList = new List<ISurfaceProperty>();


            if (ids == null)
            {
                foreach (rf.Surface rfSurface in modelData.GetSurfaces())
                {
                    rf.SurfaceThickness srfThickness = rfSurface.Thickness;

                    ISurfaceProperty srfProp = srfThickness.FromRFEM(srfThickness);

                    surfacePropList.Add(srfProp);
                    /*
                    int srfThickId = srfThickness.No;
                    if (!m_sectionDict.ContainsKey(srfThickId))
                    {
                        m_sectionDict.Add(srfThickId, srfProp);
                    }
                    */
                }
            }
            else
            {
                foreach (string id in ids)
                {

                }
            }

            return surfacePropList;
        }

        /***************************************************/

    }
}
