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
using BH.oM.Structure.Constraints;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM5
{
    public partial class RFEM5Adapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private bool CreateCollection(IEnumerable<Bar> bars)
        {
            if (bars.Count() > 0)
            {
                int barIdNum = 0;
                int lineIdNum = 0;
                List<Bar> barList = bars.ToList();
                rf.Member[] rfBars = new rf.Member[barList.Count()];

                for (int i = 0; i < bars.Count(); i++)
                {
                    barIdNum = GetAdapterId<int>(barList[i]);

                    //create line
                    lineIdNum = modelData.GetLineCount() + 1;
                    rf.Line centreLine = new rf.Line();
                    int startNodeId = GetAdapterId<int>(barList[i].Start);
                    int endNodeId = GetAdapterId<int>(barList[i].End);
                    centreLine.NodeList = String.Join(",", new int[] { startNodeId, endNodeId });
                    centreLine.Type = rf.LineType.PolylineType;
                    modelData.SetLine(centreLine);
                    rfBars[i] = barList[i].ToRFEM(barIdNum, lineIdNum);

                    Boolean otherBarIsUsingSameCS=modelData.GetCrossSections().ElementAtOrDefault(rfBars[i].StartCrossSectionNo-1).TextID == null;
                    if (otherBarIsUsingSameCS)
                    {
                        //Find right CS Number
                        String barMaterialName = barList[i].SectionProperty.Material.Name;
                        var mat= modelData.GetMaterials().Where(k => k.Description.Split(' ')[1].Equals(barMaterialName)).First();
                        int foundMatIndex=modelData.GetMaterials().ToList().IndexOf(mat)+1;
                        rf.CrossSection temporaryCS = barList[i].SectionProperty.ToRFEM( 0, foundMatIndex);
                        rf.CrossSection matchingCS= modelData.GetCrossSections().ToList().First(c=>(c.Description.Equals(temporaryCS.Description)|| c.Comment.Equals(temporaryCS.Description)) &&c.MaterialNo.Equals(temporaryCS.MaterialNo));
                        rfBars[i].StartCrossSectionNo = matchingCS.No;

                    }
           
                    modelData.SetMember(rfBars[i]);
                }

            }

            return true;
        }

    }
}





