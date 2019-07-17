/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.Engine.RFEM;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter
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
                    barIdNum = System.Convert.ToInt32(barList[i].CustomData[AdapterId]);

                    //create line
                    lineIdNum = modelData.GetLineCount() + 1;
                    rf.Line centreLine = Query.GetRFEMLineFromBar(barList[i], lineIdNum);
                    modelData.SetLine(centreLine);


                    rfBars[i] = barList[i].ToRFEM(barIdNum, lineIdNum);
                    modelData.SetMember(rfBars[i]);
                }

                //modelData.SetMembers(rfBars);
            }

            return true;
        }

    }
}
