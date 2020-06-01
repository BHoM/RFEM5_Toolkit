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
using BH.oM.Geometry;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private bool CreateCollection(IEnumerable<Edge> edges)
        {
            if (edges.Count() > 0)
            {
                int edgeIdNum = 0;

                List<Edge> edgeList = edges.ToList();
                rf.Line[] rfLines = new rf.Line[edgeList.Count()];

                for (int i = 0; i < edges.Count(); i++)
                {
                    edgeIdNum = System.Convert.ToInt32(edgeList[i].CustomData[AdapterIdName]);


                    //create rfem nodes, i.e. bhom points
                    Line edgeAsLine = edgeList[i].Curve as Line;

                    rf.Node rfNode1 = new rf.Node();
                    rfNode1.No = (int)this.NextFreeId(typeof(Node));
                    rfNode1.X = edgeAsLine.Start.X;
                    rfNode1.Y = edgeAsLine.Start.Y;
                    rfNode1.Z = edgeAsLine.Start.Z;
                    modelData.SetNode(rfNode1);

                    rf.Node rfNode2 = new rf.Node();
                    int nodeId1 = (int)this.NextFreeId(typeof(Node));
                    rfNode2.X = edgeAsLine.End.X;
                    rfNode2.Y = edgeAsLine.End.Y;
                    rfNode2.Z = edgeAsLine.End.Z;
                    modelData.SetNode(rfNode1);



                    //create line
                    rf.Line centreLine = new rf.Line();
                    centreLine.No = edgeIdNum;
                    centreLine.NodeList = String.Join(",", new int[] { rfNode1.No, rfNode2.No });
                    centreLine.Type = rf.LineType.PolylineType;
                    modelData.SetLine(centreLine);

                }

                //modelData.SetMembers(rfBars);
            }

            return true;
        }

    }
}
