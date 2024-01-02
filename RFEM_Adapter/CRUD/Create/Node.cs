/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

        private bool CreateCollection(IEnumerable<Node> nodes)
        {
            if (nodes.Count()>0)
            {
                int nodeIdNum = 0;
                int consIdNum = 0;
                List<Node> nodeList = nodes.ToList();
                rf.Node[] rfNodes = new rf.Node[nodeList.Count()];

                for (int i = 0; i < nodes.Count(); i++)
                {
                    nodeIdNum = GetAdapterId<int>(nodeList[i]);//(NextId(nodeList[i].GetType()));
                    rfNodes[i] = nodeList[i].ToRFEM(nodeIdNum);
                    modelData.SetNode(rfNodes[i]);

                    //set support here if the node contains one ! ! ! ! !
                    if(nodeList[i].Support != null)
                    {
                        consIdNum = System.Convert.ToInt32(NextFreeId(nodeList[i].Support.GetType()));
                        rf.NodalSupport rfConstraint = nodeList[i].Support.ToRFEM(consIdNum, nodeIdNum);
                        modelData.SetNodalSupport(rfConstraint);
                    }
                }

                //modelData.SetNodes(rfemNodes);
            }

            return true;
        }

    }
}




