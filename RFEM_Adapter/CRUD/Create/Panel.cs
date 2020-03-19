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

                    rf.Line[] lns = modelData.GetLines();
                    int[] alt = new int[4];
                    for (int j = 0; j < 4; j++)
                    {
                        boundaryIdArr[j] = lns[j].No;
                    }

                    rfSurfaces[i] = panelList[i].ToRFEM(panelIdNum, boundaryIdArr);

                    if(rfSurfaces[i].IsValid)
                        modelData.SetSurface(rfSurfaces[i]);
                    else
                    {
                        //create test surface from scratch !!

                        //nodes
                        // Allocates array of 4 nodes and sets theirs parameters.
                        rf.Node[] nodes = new rf.Node[4];
                        nodes[0].No = 11;
                        nodes[0].X = 0.0;
                        nodes[0].Y = 0.0;
                        nodes[0].Z = 0.0;

                        nodes[1].No = 12;
                        nodes[1].X = 5.0;
                        nodes[1].Y = 0.0;
                        nodes[1].Z = 0.0;

                        nodes[2].No = 13;
                        nodes[2].X = 5.0;
                        nodes[2].Y = 3.0;
                        nodes[2].Z = 0.0;

                        nodes[3].No = 14;
                        nodes[3].X = 0.0;
                        nodes[3].Y = 3.0;
                        nodes[3].Z = 0.0;

                        // Allocates array of 4 lines and sets theirs parameters.
                        rf.Line[] lines = new rf.Line[4];
                        lines[0].No = 10;
                        lines[0].Type = rf.LineType.PolylineType;
                        lines[0].NodeList = $"{nodes[0].No}, {nodes[1].No}";

                        lines[1].No = 20;
                        lines[1].Type = rf.LineType.PolylineType;
                        lines[1].NodeList = $"{nodes[1].No}, {nodes[2].No}";

                        lines[2].No = 30;
                        lines[2].Type = rf.LineType.PolylineType;
                        lines[2].NodeList = $"{nodes[2].No}, {nodes[3].No}";

                        lines[3].No = 40;
                        lines[3].Type = rf.LineType.PolylineType;
                        lines[3].NodeList = $"{nodes[3].No}, {nodes[0].No}";

                        // Creates material.
                        rf.Material material = new rf.Material
                        {
                            No = 1,
                            TextID = "NameID|Beton C30/37@TypeID|CONCRETE@NormID|DIN 1045-1 - 08"
                        };

                        // Allocates surface variable and sets it's parameters.
                        rf.Surface surfaceData = new rf.Surface
                        {
                            No = 1,
                            GeometryType = rf.SurfaceGeometryType.PlaneSurfaceType,
                            BoundaryLineList = $"{lines[0].No}, {lines[1].No}, {lines[2].No}, {lines[3].No}",
                            MaterialNo = material.No,
                            StiffnessType = rf.SurfaceStiffnessType.StandardStiffnessType
                        };
                        surfaceData.Thickness.Type = rf.SurfaceThicknessType.ConstantThicknessType;
                        surfaceData.Thickness.Constant = 0.2;

                        //SurfaceStiffness stiffness = new SurfaceStiffness
                        //{
                        //    Thickness = 0.2,
                        //    Type = rf.OrthotropyType.DefinedByStiffnessMatrix
                        //};

                        rf.ISurface surface = modelData.SetSurface(surfaceData);


                    }

                }

            }

            return true;
        }

    }
}
