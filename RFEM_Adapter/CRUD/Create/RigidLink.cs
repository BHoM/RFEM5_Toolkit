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
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM5
{
    public partial class RFEM5Adapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private bool CreateCollection(IEnumerable<RigidLink> links)
        {
            if (links.Count() > 0)
            {
                int linkIdNum = 0;
                int lineIdNum = 0;
                List<RigidLink> linkList = links.ToList();
                rf.Member[] rfLinks = new rf.Member[linkList.Count()];

                for (int i = 0; i < links.Count(); i++)
                {
                    linkIdNum = GetAdapterId<int>(linkList[i]);

                    //check for multiple secondary nodes
                    if(linkList[i].SecondaryNodes.Count>1)
                        Engine.Base.Compute.RecordWarning("Multiple secondary nodes detected! Link no. " + linkIdNum + " was created using only the first secondary node!");


                    //create line
                    lineIdNum = modelData.GetLineCount() + 1;
                    rf.Line centreLine = new rf.Line();
                    int startNodeId = GetAdapterId<int>(linkList[i].PrimaryNode);
                    int endNodeId = GetAdapterId<int>(linkList[i].SecondaryNodes[0]);
                    centreLine.NodeList = String.Join(",", new int[] { startNodeId, endNodeId });
                    centreLine.Type = rf.LineType.PolylineType;
                    modelData.SetLine(centreLine);

                    rf.Member rfLink = new rf.Member();
                    rfLink.No = linkIdNum;
                    rfLink.LineNo = lineIdNum;
                    rfLink.Type = rf.MemberType.Rigid;

                    LinkConstraint lc = linkList[0].Constraint;
                    if(lc != null)
                    {
                        if (lc.XtoX != true || lc.YtoY != true || lc.ZtoZ != true || lc.XXtoXX != true || lc.YYtoYY != true || lc.ZZtoZZ != true)
                        {
                            Engine.Base.Compute.RecordWarning("Hinges on RigidLinks are not supported. Member no. " + linkIdNum + " created as fixed!");
                        }
                    }

                    modelData.SetMember(rfLink);
                }

                //modelData.SetMembers(rfBars);
            }

            return true;
        }

    }
}



