/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using rf = Dlubal.RFEM5;
using rf3 = Dlubal.RFEM3;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Adapters.RFEM
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Get list of Ids from RFEM string")]
        [Input("rfemIdString", "String with RFEM ids")]
        [Output("idList", "List of ids")]
        public static List<string> GetIdListFromString(string rfemIdString)
        {
            List<string> idList = new List<string>();

            string[] firstSplit = rfemIdString.Split(',');
            int startId, endId;

            foreach (string item in firstSplit)
            {
                string[] rangeSplit = item.Split('-');
                if (rangeSplit.Length>1)
                {
                    int.TryParse(rangeSplit[0], out startId);
                    int.TryParse(rangeSplit[1], out endId);
                    idList.AddRange(Enumerable.Range(startId, endId - startId).Select(x => x.ToString()).ToList());
                }
                else if(rangeSplit.Length==1)
                {
                    idList.Add(rangeSplit[0]);
                }
            }

            return idList;
        }

        /***************************************************/
    }
}

