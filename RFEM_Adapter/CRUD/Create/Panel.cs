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
                    panelIdNum = GetAdapterId<int>(panelList[i]);

                    //get ids outside of BHoM process - might need to be changed
                    int lastLineId = modelData.GetLastObjectNo(rf.ModelObjectType.LineObject);


                    int[] boundaryIdArr = new int[panelList[i].ExternalEdges.Count()];

                    //create outline
                    List<string> outlineNodeList = new List<string>();
                    foreach (Edge e in panelList[i].ExternalEdges)
                    {
                        
                        //create rfem nodes, i.e. bhom points - NOTE: RFEM will remove the coincident points itself leaving jumps in node numbering ! 1,2,4,6,8,10,...

                        rf.Node rfNode1 = new rf.Node();

                        if (e.Curve.GetType().Name.Equals("Polyline"))
                        {

                            Polyline polyline = e.Curve as Polyline;

                            for (int j=0; j<polyline.ControlPoints.Count-1;j++)
                            {
                                rfNode1 = new rf.Node();

                                rfNode1.No = (int)this.NextFreeId(typeof(Node));
                                rfNode1.X = polyline.ControlPoints[j].X;
                                rfNode1.Y = polyline.ControlPoints[j].Y;
                                rfNode1.Z = polyline.ControlPoints[j].Z;

                                modelData.SetNode(rfNode1);
                                outlineNodeList.Add(rfNode1.No.ToString());
                            }

                        }
                        else
                        {
                            Line edgeAsLine = e.Curve as Line;

                            rfNode1.No = (int)this.NextFreeId(typeof(Node));
                            rfNode1.X = edgeAsLine.Start.X;
                            rfNode1.Y = edgeAsLine.Start.Y;
                            rfNode1.Z = edgeAsLine.Start.Z;

                            modelData.SetNode(rfNode1);
                            outlineNodeList.Add(rfNode1.No.ToString());

                        };
                       
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

                    //Openings
                    if (panelList[i].Openings.Count > 0)
                    {
                        List<Opening> openingList = panelList[i].Openings;
                        rf.Opening[] rfOpenings = new rf.Opening[openingList.Count];
                        int openingId = 0;


                        for (int o = 0; o < openingList.Count; o++)
                        {
                            openingId = modelData.GetLastObjectNo(rf.ModelObjectType.OpeningObject);
                            List<string> openingOutlineNodeList = new List<string>();
                            
                            //Defining Nodes
                            foreach (Edge e in openingList[o].Edges)
                            {
                                
                                rf.Node rfNode = new rf.Node();
                                if (e.Curve.GetType().Name.Equals("Polyline"))
                                {

                                    Polyline polyline = e.Curve as Polyline;

                                    for (int j = 0; j < polyline.ControlPoints.Count - 1; j++)
                                    {
                                        rfNode=new rf.Node();

                                        rfNode.No = (int)this.NextFreeId(typeof(Node));
                                        rfNode.X = polyline.ControlPoints[j].X;
                                        rfNode.Y = polyline.ControlPoints[j].Y;
                                        rfNode.Z = polyline.ControlPoints[j].Z;

                                        modelData.SetNode(rfNode);
                                        openingOutlineNodeList.Add(rfNode.No.ToString());

                                    }
                                }
                                else
                                {
                                    Line edgeAsLine = e.Curve as Line;

                                    rfNode.No = (int)this.NextFreeId(typeof(Node));
                                    rfNode.X = edgeAsLine.Start.X;
                                    rfNode.Y = edgeAsLine.Start.Y;
                                    rfNode.Z = edgeAsLine.Start.Z;

                                    modelData.SetNode(rfNode);
                                    openingOutlineNodeList.Add(rfNode.No.ToString());
                                }

                            }
                            openingOutlineNodeList.Add(openingOutlineNodeList[0]);

                            lastLineId = modelData.GetLastObjectNo(rf.ModelObjectType.LineObject);

                            //Defining Lines
                            rf.Line openingOutline = new rf.Line()
                            {
                                No = lastLineId + 1,
                                Type = rf.LineType.PolylineType,
                                NodeList = String.Join(",", openingOutlineNodeList)
                            };
                            modelData.SetLine(openingOutline);

                            //Defining Openings
                            rf.Opening opening = new rf.Opening()
                            {
                                No = openingId+1,
                                InSurfaceNo = rfSurfaces[i].No,
                                BoundaryLineList = String.Join(",", openingOutline.No)
                            };

                            rfOpenings[o] = opening;
                            rfSurfaces[i].IntegratedOpeningList = String.Join(",", new int[] { opening.No });

                            modelData.SetOpening(opening);
                        }
                    }

                }
            }

            return true;
        }

    }
}


