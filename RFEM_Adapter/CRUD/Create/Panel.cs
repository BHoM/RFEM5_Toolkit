/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
                   

                    if (panelList.ElementAt(i).Property == null)
                    {
                        Engine.Base.Compute.RecordError("Could not create surface due to missing property in the panel "+ panelList.ElementAt(i).Name);
                        continue;
                    }
                    


                    panelIdNum = GetAdapterId<int>(panelList[i]);

                    //get ids outside of BHoM process - might need to be changed
                    int lastLineId = modelData.GetLastObjectNo(rf.ModelObjectType.LineObject);


                    int[] boundaryIdArr = new int[panelList[i].ExternalEdges.Count()];

                    //create outline
                    List<string> outlineNodeList = new List<string>();
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

                        outlineNodeList.Add(rfNode1.No.ToString());
                    }
                    outlineNodeList.Add(outlineNodeList[0]);

                    rf.Line outline = new rf.Line()
                    {
                        No = lastLineId+1,
                        Type = rf.LineType.PolylineType,
                        NodeList = String.Join(",", outlineNodeList)
                    };
                    modelData.SetLine(outline);


                    rfSurfaces[i] = panelList[i].ToRFEM(panelIdNum, new int[] { outline.No });

                    if(rfSurfaces[i].StiffnessType == rf.SurfaceStiffnessType.StandardStiffnessType)
                    {
                        modelData.SetSurface(rfSurfaces[i]);
                    }
                    else
                    {
                        rf.SurfaceStiffness stiffness = panelList[i].Property.ToRFEM();
                        rfSurfaces[i].Thickness.Constant = stiffness.Thickness;
                        rf.ISurface srf = modelData.SetSurface(rfSurfaces[i]);
                        rf.IOrthotropicThickness ortho = srf.GetOrthotropicThickness();
                        ortho.SetData(stiffness);
                    }


                }

            }

            return true;
        }

    }
}


