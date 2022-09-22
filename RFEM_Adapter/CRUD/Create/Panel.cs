/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
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

                //Panels
                for (int i = 0; i < panels.Count(); i++)
                {

                    if (panelList.ElementAt(i).Property == null)
                    {
                        Engine.Base.Compute.RecordError("Could not create surface due to missing property in the panel " + panelList.ElementAt(i).Name);
                        continue;
                    }

                    panelIdNum = GetAdapterId<int>(panelList[i]);

                    //get ids outside of BHoM process - might need to be changed
                    // int lastLineId = modelData.GetLastObjectNo(rf.ModelObjectType.LineObject);
                    var nodes = modelData.GetNodes();
                    //create Panel outline
                    List<rf.Line> outlineNodeList = GenerateOutlineLines(panelList[i].ExternalEdges);
                    int[] numberArray = outlineNodeList.Select(line => line.No).ToArray();
                    rfSurfaces[i] = panelList[i].ToRFEM(panelIdNum, numberArray);
                    //rfSurfaces[i].MaterialNo=modelData.GetMaterials().ToList().Find(m => m.Description.Split(' ')[1].Equals(panelList[i].Property.Material.Name)).No;
                    rfSurfaces[i].MaterialNo = modelData.GetMaterials().ToList().Find(m => m.Description.Split(':')[1].Equals(" " + panelList[i].Property.Material.Name)).No;



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

                    //Openings
                    if (panelList[i].Openings.Count > 0)
                    {
                        List<Opening> openingList = panelList[i].Openings;

                        for (int o = 0; o < openingList.Count; o++)
                        {
                            List<rf.Line> openingLineList = GenerateOutlineLines(openingList[o].Edges);
                            List<String> openingLineListString = (openingLineList.Select(line => line.No).ToList()).Select(g => g.ToString()).ToList();

                            //Defining Openings
                            rf.Opening opening = new rf.Opening()
                            {
                                No = modelData.GetLastObjectNo(rf.ModelObjectType.OpeningObject) + 1,
                                InSurfaceNo = rfSurfaces[i].No,
                                BoundaryLineList = String.Join(",", openingLineListString)
                            };
                            rfSurfaces[i].IntegratedOpeningList = String.Join(",", new int[] { opening.No });
                            modelData.SetOpening(opening);
                        }
                    }
                }
            }

            return true;
        }

        private List<rf.Line> GenerateOutlineLines(List<Edge> edgeList)
        {

            //List<String> openingLineStringList = new List<String>();
            List<rf.Line> lineList = new List<rf.Line>();

            for (int e = 0; e < edgeList.Count; e++)
            {

                List<Point> points;
                if (edgeList[e].Curve is Arc)
                {
                    Point start = Engine.Geometry.Query.IControlPoints(edgeList[e].Curve)[0];
                    Point mid = Engine.Geometry.Query.IControlPoints(edgeList[e].Curve)[2];
                    Point End = Engine.Geometry.Query.IControlPoints(edgeList[e].Curve)[4];

                    points = new List<Point>() { start, mid, End };
                }
                else if (edgeList[e].Curve is Circle)
                {
                    List<string> pointList = new List<string>();
                    Circle circle = edgeList[e].Curve as Circle;
                    List<Point> pts = Engine.Geometry.Query.IControlPoints(edgeList[e].Curve);

                    //access relevant controlpoints for the describtion of the rf.circle
                    Point p0 = Engine.Geometry.Query.IControlPoints(edgeList[e].Curve)[0];
                    Point p1 = Engine.Geometry.Query.IControlPoints(edgeList[e].Curve)[1];
                    Point p2 = Engine.Geometry.Query.IControlPoints(edgeList[e].Curve)[2];

                    points = new List<Point>() { p0, p1, p2 };
                }
                else
                {
                    points = Engine.Geometry.Query.IControlPoints(edgeList[e].Curve);
                }

                //Adding Points and lines to RFEM
                if (e == 0)
                {
                    List<string> pointList = new List<string>();
                    if ((!points.First().Equals(points.Last())))

                    {
                        pointList = points.Select(x => AddPointToModelData(x)).ToList();
                        lineList.Add(AddLineToModelData(pointList, edgeList[e]));
                    }
                    else
                    {
                        points.Remove(points.Last());
                        pointList = points.Select(x => AddPointToModelData(x)).ToList();
                        pointList.Add(pointList.First());
                        lineList.Add(AddLineToModelData(pointList, edgeList[e]));
                    }

                }
                else if (e == (edgeList.Count - 1))
                {

                    List<string> pointList = new List<string>();
                    pointList.Add("" + GetNodeFromModelDate((Engine.Geometry.Query.IControlPoints(edgeList[e - 1].Curve)).Last()).No);

                    for (int i = 1; i < points.Count - 1; i++)
                    {
                        pointList.Add(AddPointToModelData(points[i]));
                    }
                    pointList.Add("" + GetNodeFromModelDate((Engine.Geometry.Query.IControlPoints(edgeList[0].Curve)).First()).No);
                    lineList.Add(AddLineToModelData(pointList, edgeList[e]));
                }
                else
                {
                    List<string> pointList = new List<string>();
                    pointList.Add("" + GetNodeFromModelDate((Engine.Geometry.Query.IControlPoints(edgeList[e - 1].Curve)).Last()).No);

                    for (int i = 1; i < points.Count; i++)
                    {
                        pointList.Add(AddPointToModelData(points[i]));
                    }
                    lineList.Add(AddLineToModelData(pointList, edgeList[e]));
                }

            }

            return lineList;
        }

        private string AddPointToModelData(Point p)
        {

            List<rf.Node> nodes = modelData.GetNodes().Where(n => (n.X.Equals(p.X) && n.Y.Equals(p.Y) && n.Z.Equals(p.Z))).ToList();

            if (nodes.Count>0) { return nodes.First().No.ToString(); }

            var rfNodes = modelData.GetNodes().ToList();


            rf.Node node = new rf.Node()
            {
                No = modelData.GetLastObjectNo(rf.ModelObjectType.NodeObject) + 1,
                X = p.X,
                Y = p.Y,
                Z = p.Z
            };

            modelData.SetNode(node);

            return node.No.ToString();

        }

        private rf.Node GetNodeFromModelDate(Point p)
        {

            List<rf.Node> node = modelData.GetNodes().Where(n => (n.X.Equals(p.X) && n.Y.Equals(p.Y) && n.Z.Equals(p.Z))).ToList();

            return node.FirstOrDefault();
        }

        private rf.Line AddLineToModelData(List<String> nodeString, Edge e)
        {

            if (e.Curve is Polyline)
            {
                //Defining Polyline
                rf.Line polyline = new rf.Line()
                {
                    No = modelData.GetLastObjectNo(rf.ModelObjectType.LineObject) + 1,
                    Type = rf.LineType.PolylineType,
                    NodeList = String.Join(",", nodeString)
                };
                modelData.SetLine(polyline);

                return polyline;
            }
            else if (e.Curve is Line)
            {

                //Defining Line
                rf.Line line = new rf.Line()
                {
                    No = modelData.GetLastObjectNo(rf.ModelObjectType.LineObject) + 1,
                    Type = rf.LineType.PolylineType,
                    NodeList = String.Join(",", nodeString)
                };
                modelData.SetLine(line);

                return line;

            }
            else if (e.Curve is Arc)
            {
                //Defining Arc
                rf.Line arc = new rf.Line()
                {
                    No = modelData.GetLastObjectNo(rf.ModelObjectType.LineObject) + 1,
                    Type = rf.LineType.ArcType,
                    NodeList = String.Join(",", nodeString)
                };
                modelData.SetLine(arc);

                return arc;
            }
            else if (e.Curve is Circle)
            {
                //Defining Circle
                rf.Line circle = new rf.Line()
                {
                    No = modelData.GetLastObjectNo(rf.ModelObjectType.LineObject) + 1,
                    Type = rf.LineType.CircleType,
                    NodeList = String.Join(",", nodeString)
                };
                modelData.SetLine(circle);

                return circle;
            }

            else
            {
                return new rf.Line();
            }

        }

    }
}




