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
            //If panels exist
            if (panels.Count() > 0)
            {
                int panelIdNum = 0;
                List<Panel> panelList = panels.ToList();
                rf.Surface[] rfSurfaces = new rf.Surface[panelList.Count()];

                for (int i = 0; i < panels.Count(); i++)
                {

                    if (panelList.ElementAt(i).Property == null)
                    {
                        Engine.Base.Compute.RecordError("Could not create surface due to missing property in the panel " + panelList.ElementAt(i).Name);
                        continue;
                    }

                    panelIdNum = GetAdapterId<int>(panelList[i]);

                    //get ids outside of BHoM process - might need to be changed
                    int lastLineId = modelData.GetLastObjectNo(rf.ModelObjectType.LineObject);

                    //Panel Outline
                    List<rf.Line> outline = GenerateLineOutlineList(panelList[i].ExternalEdges);
                    int[] outlineNo = outline.Select(l => l.No).ToArray();
                    rfSurfaces[i] = panelList[i].ToRFEM(panelIdNum, outlineNo);


                    if (rfSurfaces[i].StiffnessType == rf.SurfaceStiffnessType.StandardStiffnessType)
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

                    //If opening exist in panel
                    if (panelList[i].Openings.Count > 0)
                    {
                        int openingId = 0;
                        List<Opening> openingList = panelList[i].Openings;
                        rf.Opening[] rfOpenings = new rf.Opening[openingList.Count];

                        for (int o = 0; o < openingList.Count; o++)
                        {
                            openingId = modelData.GetLastObjectNo(rf.ModelObjectType.OpeningObject);
                            List<rf.Line> openingOutline = GenerateLineOutlineList(openingList[o].Edges);
                            String[] openingOutlineLineNo = (openingOutline.Select(lst => lst.No).ToArray()).Select(l => l.ToString()).ToArray();

                            //Defining Openings
                            rf.Opening opening = new rf.Opening()
                            {
                                No = openingId + 1,
                                InSurfaceNo = rfSurfaces[i].No,
                                BoundaryLineList = String.Join(",", openingOutlineLineNo)
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


        public List<rf.Line> GenerateLineOutlineList(List<Edge> edgeList)
        {
            List<rf.Line> lines = new List<rf.Line>();

            //point Map to avoid double definition of points
            Dictionary<double, Dictionary<double, Dictionary<double, rf.Node>>> pointMap = GenrateMapAndAddPointsToRFEM(edgeList);

            //Defining Nodes
            foreach (Edge e in edgeList)
            {

                rf.Node rfNode;

                if (e.Curve is Polyline)
                {
                    Polyline polyline = e.Curve as Polyline;
                    List<string> outlineNodeList = new List<string>();

                    //If Curve is closed
                    if (polyline.ControlPoints.Last().Equals(polyline.ControlPoints.First()))
                    {
                        for (int j = 0; j < polyline.ControlPoints.Count - 1; j++)
                        {
                            rfNode = pointMap[polyline.ControlPoints[j].X][polyline.ControlPoints[j].Y][polyline.ControlPoints[j].Z];
                            outlineNodeList.Add(rfNode.No.ToString());
                        }
                        outlineNodeList.Add(outlineNodeList[0]);
                    }
                    else
                    {
                        for (int j = 0; j < polyline.ControlPoints.Count; j++)
                        {
                            rfNode = pointMap[polyline.ControlPoints[j].X][polyline.ControlPoints[j].Y][polyline.ControlPoints[j].Z];
                            outlineNodeList.Add(rfNode.No.ToString());
                        }
                    }
                    //Defining rf.Line objects from polylines
                    rf.Line openingOutline = new rf.Line()
                    {
                        No = modelData.GetLastObjectNo(rf.ModelObjectType.LineObject) + 1,
                        Type = rf.LineType.PolylineType,
                        NodeList = String.Join(",", outlineNodeList)
                    };
                    modelData.SetLine(openingOutline);

                    lines.Add(openingOutline);

                }
                else if (e.Curve is Line)
                {
                    List<string> outlineNodeList = new List<string>();
                    Line edgeAsLine = e.Curve as Line;
                    rfNode = pointMap[edgeAsLine.Start.X][edgeAsLine.Start.Y][edgeAsLine.Start.Z];
                    outlineNodeList.Add(rfNode.No.ToString());

                    //Defining Lines
                    rf.Line openingOutline = new rf.Line()
                    {
                        No = modelData.GetLastObjectNo(rf.ModelObjectType.LineObject) + 1,
                        Type = rf.LineType.PolylineType,
                        NodeList = String.Join(",", outlineNodeList)
                    };
                    modelData.SetLine(openingOutline);
                    lines.Add(openingOutline);

                }
                else if (e.Curve is Arc)
                {

                    Arc edgeAsArch = e.Curve as Arc;
                    List<Point> cp = Engine.Geometry.Query.ControlPoints(edgeAsArch);
                    List<String> archPointNoString = new List<string>();

                    List<rf.Node> rfNodeList = new List<rf.Node>() {pointMap[cp[0].X][cp[0].Y][cp[0].Z], pointMap[cp[2].X][cp[2].Y][cp[2].Z], pointMap[cp[4].X][cp[4].Y][cp[4].Z] };
                    archPointNoString=(rfNodeList.Select(n => n.No).ToList()).Select(k => k.ToString()).ToList();
                    

                    //Defining Lines
                    rf.Line arcLine = new rf.Line()
                    {
                        No = modelData.GetLastObjectNo(rf.ModelObjectType.LineObject) + 1,
                        Type = rf.LineType.ArcType,
                        NodeList = String.Join(",", archPointNoString)
                    };
                    modelData.SetLine(arcLine);

                    lines.Add(arcLine);

                }
            }

            return lines;
        }

        //Generating Point map to avoid double definition of points
        private Dictionary<double, Dictionary<double, Dictionary<double, rf.Node>>> GenrateMapAndAddPointsToRFEM(List<Edge> edges)
        {

            Dictionary<double, Dictionary<double, Dictionary<double, rf.Node>>> pointMap = new Dictionary<double, Dictionary<double, Dictionary<double, rf.Node>>>();

            foreach (Edge e in edges)
            {

                List<Point> points = new List<Point>();
                if (e.Curve is Polyline)
                {
                    Polyline polyline = e.Curve as Polyline;

                    points = polyline.ControlPoints;
                }
                else if (e.Curve is Arc)
                {
                    Arc arc = e.Curve as Arc;
                    //points = Engine.Geometry.Query.ControlPoints(arc);
                    points.Add(Engine.Geometry.Query.ControlPoints(arc)[0]);
                    points.Add(Engine.Geometry.Query.ControlPoints(arc)[2]);
                    points.Add(Engine.Geometry.Query.ControlPoints(arc)[4]);

                }

                foreach (Point p in points)
                {
                    double x = p.X;
                    double y = p.Y;
                    double z = p.Z;


                    rf.Node node = new rf.Node()
                    {
                        //No = (int)this.NextFreeId(typeof(Node)),
                        X = x,
                        Y = y,
                        Z = z
                    };

                    if (!pointMap.ContainsKey(x))
                    {

                        node.No = (int)this.NextFreeId(typeof(Node));
                        modelData.SetNode(node);
                        Dictionary<double, rf.Node> pmZ = new Dictionary<double, rf.Node>();
                        pmZ.Add(z, node);
                        Dictionary<double, Dictionary<double, rf.Node>> pmY = new Dictionary<double, Dictionary<double, rf.Node>>();
                        pmY.Add(y, pmZ);
                        pointMap.Add(x, pmY);

                    }
                    else if (!pointMap[x].ContainsKey(y))
                    {

                        node.No = (int)this.NextFreeId(typeof(Node));
                        modelData.SetNode(node);
                        Dictionary<double, rf.Node> pmZ = new Dictionary<double, rf.Node>();
                        pmZ.Add(z, node);
                        Dictionary<double, Dictionary<double, rf.Node>> pmY = new Dictionary<double, Dictionary<double, rf.Node>>();
                        pmY.Add(y, pmZ);
                        pointMap[x] = pmY;



                    }
                    else if (!pointMap[x][y].ContainsKey(z))
                    {
                        node.No = (int)this.NextFreeId(typeof(Node));
                        modelData.SetNode(node);
                        Dictionary<double, rf.Node> pmZ = new Dictionary<double, rf.Node>();
                        pmZ.Add(z, node);
                        pointMap[x][y] = pmZ;

                    }

                }
            }

            return pointMap;
        }


    }

}



