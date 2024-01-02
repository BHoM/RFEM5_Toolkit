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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Constraints;
using BH.oM.Geometry;

using rf = Dlubal.RFEM5;
using BH.oM.Structure.MaterialFragments;

namespace BH.Adapter.RFEM5
{
    public partial class RFEM5Adapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private List<Panel> ReadPanels(List<string> ids = null)
        {
            List<Panel> panelList = new List<Panel>();
            ISurfaceProperty surfaceProperty;

            if (ids == null)
            {
                foreach (rf.Surface surface in modelData.GetSurfaces())
                {
                    if(surface.GeometryType != rf.SurfaceGeometryType.PlaneSurfaceType)
                        Engine.Base.Compute.RecordError("Only plane surface types are supported at the moment");

                    //List<Edge> edgeList = GetEdgesFromRFEMSurface(surface);
                    List<Edge> edgeList = GetBoundaryEdges(modelData.GetSurface(surface.No, rf.ItemAt.AtNo).GetData().BoundaryLineList);

                    IMaterialFragment material = modelData.GetMaterial(surface.MaterialNo, rf.ItemAt.AtNo).GetData().FromRFEM();

                    if (surface.StiffnessType == rf.SurfaceStiffnessType.StandardStiffnessType)
                    {
                        surfaceProperty = new ConstantThickness { Thickness = surface.Thickness.Constant, Material = material };
                    }
                    else if (surface.StiffnessType == rf.SurfaceStiffnessType.OrthotropicStiffnessType)
                    {
                        rf.ISurface s = modelData.GetSurface(surface.No, rf.ItemAt.AtNo);
                        rf.IOrthotropicThickness ortho = s.GetOrthotropicThickness();
                        rf.SurfaceStiffness stiffness = ortho.GetData();
                        surfaceProperty = stiffness.FromRFEM(material);
                    }
                    else
                    {
                        surfaceProperty = null;
                        Engine.Base.Compute.RecordError("could not create surface property of type " + surface.StiffnessType.ToString());
                    }
                    
                    List<Opening> openings = new List<Opening>();

                    //if opening exists in panel     
                    if (!(surface.IntegratedOpeningList.Length == 0))
                    {
                        List<int> openingIDList = GetIdListFromString(surface.IntegratedOpeningList);

                        foreach (int i in openingIDList)
                        {
                           
                            rf.Opening rfOpening = modelData.GetOpening(i, rf.ItemAt.AtNo).GetData();
                            List<Edge> edges = GetBoundaryEdges(rfOpening.BoundaryLineList);
                            Opening opening = new Opening { Edges=edges};
                            openings.Add(opening);
                        }
                    }

                    Panel panel = Engine.Structure.Create.Panel(edgeList, openings, surfaceProperty);
                    panel.Name = surface.Comment;
                    panelList.Add(panel);

                }
            }
            else
            {
                foreach (string id in ids)
                {
                    rf.Surface surface = modelData.GetSurface(Int32.Parse(id), rf.ItemAt.AtNo).GetData();
                    if (surface.GeometryType != rf.SurfaceGeometryType.PlaneSurfaceType)
                        Engine.Base.Compute.RecordError("Only plane surface types are supported at the moment");

                    List<Edge> edgeList = GetBoundaryEdges(modelData.GetSurface(surface.No, rf.ItemAt.AtNo).GetData().BoundaryLineList);

                    IMaterialFragment material = modelData.GetMaterial(surface.MaterialNo, rf.ItemAt.AtNo).GetData().FromRFEM();

                    rf.ISurface s = modelData.GetSurface(surface.No, rf.ItemAt.AtNo);
                    rf.IOrthotropicThickness ortho = s.GetOrthotropicThickness();
                    rf.SurfaceStiffness stiffness = ortho.GetData();

                    surfaceProperty = stiffness.FromRFEM(material);
                    List<Opening> openings = new List<Opening>();

                    //if opening exists in panel
                    if (!(surface.IntegratedOpeningList.Length == 0))
                    {
                        List<int> openingIDList = GetIdListFromString(surface.IntegratedOpeningList);

                        foreach (int i in openingIDList)
                        {

                            rf.Opening rfOpening = modelData.GetOpening(i, rf.ItemAt.AtNo).GetData();
                            List<Edge> edges = GetBoundaryEdges(rfOpening.BoundaryLineList);
                            Opening opening = new Opening { Edges = edges };
                            openings.Add(opening);
                        }
                    }

                    Panel panel = Engine.Structure.Create.Panel(edgeList, openings, surfaceProperty);
                    panelList.Add(panel);
                }
            }

            return panelList;
        }


        private List<int> GetIdListFromString(string idsAsString)
        {
            //TODO: replace this GetId.. method with the one from RFEM_Engine.Compute ! ! ! 
            //NOTE: the below only works if RFEM does not use a mix of ',' and '-' delimiters !!
            List<int> idList = new List<int>();

            if (idsAsString.Contains('-') & idsAsString.Contains(','))
            {
                foreach (string part in idsAsString.Split(','))
                {
                    if (!part.Contains('-'))
                    {
                        idList.Add(System.Convert.ToInt32(part));
                    }
                    else
                    {
                        List<int> startEnd = part.Split('-').ToList().ConvertAll(s => Int32.Parse(s));
                        idList.AddRange(Enumerable.Range(startEnd[0], startEnd[1] - startEnd[0] + 1));
                    }
                }
            }
            else if (idsAsString.Contains(','))
            {
                idList = idsAsString.Split(',').ToList().ConvertAll(s => Int32.Parse(s));
            }
            else if (idsAsString.Contains('-'))
            {
                List<int> startEnd = idsAsString.Split('-').ToList().ConvertAll(s => Int32.Parse(s));
                idList = Enumerable.Range(startEnd[0], startEnd[1] - startEnd[0] + 1).ToList();
            }
            else
            {
                idList.Add(System.Convert.ToInt32(idsAsString));
            }

            return idList;

        }


        private List<Edge> GetBoundaryEdges(string boundaryString)
        {
            List<Edge> edgeList = new List<Edge>();
            List<int> boundaryLineIds = GetIdListFromString(boundaryString);

            foreach (int edgeId in boundaryLineIds)
            {


                var modelEdgeType = modelData.GetLine(edgeId, rf.ItemAt.AtNo).GetData().Type;

                //Polyline
                if (modelEdgeType.Equals(rf.LineType.PolylineType))
                {

                    List<oM.Geometry.Point> ptsInEdge = new List<oM.Geometry.Point>();
                    string nodeIdString = modelData.GetLine(edgeId, rf.ItemAt.AtNo).GetData().NodeList;
                    List<int> nodeIds = GetIdListFromString(nodeIdString);

                    foreach (int ptId in nodeIds)
                    {
                        rf.Node rfNode = modelData.GetNode(ptId, rf.ItemAt.AtNo).GetData();
                        ptsInEdge.Add(new oM.Geometry.Point() { X = rfNode.X, Y = rfNode.Y, Z = rfNode.Z });
                    }
                    edgeList.Add(new Edge { Curve = Engine.Geometry.Create.Polyline(ptsInEdge) });
                }

                //Arc-Circular
                else if (modelEdgeType.Equals(rf.LineType.ArcType))
                {

                    List<oM.Geometry.Point> ptsInEdge = new List<oM.Geometry.Point>();
                    string nodeIdString = modelData.GetLine(edgeId, rf.ItemAt.AtNo).GetData().NodeList;
                    List<int> nodeIds = GetIdListFromString(nodeIdString);

                    rf.Node rfNode0 = modelData.GetNode(nodeIds[0], rf.ItemAt.AtNo).GetData();
                    rf.Node rfNode1 = modelData.GetNode(nodeIds[1], rf.ItemAt.AtNo).GetData();
                    rf.Node rfNode2 = modelData.GetNode(nodeIds[2], rf.ItemAt.AtNo).GetData();
                    Point p0 = new oM.Geometry.Point() { X = rfNode0.X, Y = rfNode0.Y, Z = rfNode0.Z };
                    Point p1 = new oM.Geometry.Point() { X = rfNode1.X, Y = rfNode1.Y, Z = rfNode1.Z };
                    Point p2 = new oM.Geometry.Point() { X = rfNode2.X, Y = rfNode2.Y, Z = rfNode2.Z };

                    Arc arc = Engine.Geometry.Create.Arc(p0, p1, p2);

                    edgeList.Add(new Edge { Curve = arc });

                }
                //Circle
                else if (modelEdgeType.Equals(rf.LineType.CircleType))
                {
                    List<oM.Geometry.Point> ptsInEdge = new List<oM.Geometry.Point>();
                    string nodeIdString = modelData.GetLine(edgeId, rf.ItemAt.AtNo).GetData().NodeList;
                    String s = modelData.GetLine(edgeId, rf.ItemAt.AtNo).GetData().NodeList;
                    rf.Point3D[] rfPoints = modelData.GetLine(edgeId, rf.ItemAt.AtNo).GetData().ControlPoints;

                    Point p0 = new oM.Geometry.Point() { X = rfPoints[0].X, Y = rfPoints[0].Y, Z = rfPoints[0].Z };
                    Point p1 = new oM.Geometry.Point() { X = rfPoints[1].X, Y = rfPoints[1].Y, Z = rfPoints[1].Z };
                    Point p2 = new oM.Geometry.Point() { X = rfPoints[2].X, Y = rfPoints[2].Y, Z = rfPoints[2].Z };


                    Circle circle = Engine.Geometry.Create.Circle(p0, p1, p2);

                    edgeList.Add(new Edge { Curve = circle });


                }


                else
                {
                    //Engine.Base.Compute.RecordError("Import of the chosen panel shape is currently not supported!");
                    Engine.Base.Compute.RecordError("The boundary of either a panel or an opening contains an edge of type " + modelEdgeType + " which is currently not supported!");
                };
            }

            return edgeList;
        }

    }
}




