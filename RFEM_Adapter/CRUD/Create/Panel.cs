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
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Geometry;
using BH.Engine.RFEM;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private bool CreateCollection(IEnumerable<Panel> panels)
        {
            if (panels.Count() > 0)
            {
                int panelIdNum = 0;
                List<Panel> panelList = panels.ToList();
                rf.Surface[] rfSurfaces = new rf.Surface[panelList.Count()];

                for (int i = 0; i < panels.Count(); i++)
                {
                    panelIdNum = System.Convert.ToInt32(panelList[i].CustomData[AdapterIdName]);

                    //get ids outside of BHoM process - might need to be changed
                    int lastNodeId = modelData.GetLastObjectNo(rf.ModelObjectType.NodeObject);
                    int lastLineId = modelData.GetLastObjectNo(rf.ModelObjectType.LineObject);


                    int[] boundaryIdArr = new int[panelList[i].ExternalEdges.Count()];

                    //create line
                    //int lineIdNum = modelData.GetLineCount() + 1;
                    int count = 0;
                    foreach (Edge e in panelList[i].ExternalEdges)
                    {
                        //create rfem nodes, i.e. bhom points - NOTE: RFEM will remove the coincident points itself leaving jumps in node numbering ! 1,2,4,6,8,10,...
                        Line edgeAsLine = e.Curve as Line;

                        rf.Node rfNode1 = new rf.Node
                        {
                            No = (int)this.NextFreeId(typeof(Node)),
                            X = edgeAsLine.Start.X,
                            Y = edgeAsLine.Start.Y,
                            Z = edgeAsLine.Start.Z
                        };
                        modelData.SetNode(rfNode1);

                        rf.Node rfNode2 = new rf.Node()
                        {
                            No = (int)this.NextFreeId(typeof(Node)),
                            X = edgeAsLine.End.X,
                            Y = edgeAsLine.End.Y,
                            Z = edgeAsLine.End.Z
                        };
                        modelData.SetNode(rfNode2);

                        rf.Line edge = new rf.Line();
                        lastLineId++;
                        edge.No = lastLineId;
                        edge.NodeList = String.Join(",", new int[] { rfNode1.No, rfNode2.No });
                        edge.Type = rf.LineType.PolylineType;
                        modelData.SetLine(edge);
                        boundaryIdArr[count] = lastLineId;
                        count++;
                    }


                    rfSurfaces[i] = panelList[i].ToRFEM(panelIdNum, boundaryIdArr);
                    modelData.SetSurface(rfSurfaces[i]);
                }

            }

            return true;
        }

    }
}
