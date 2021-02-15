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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Constraints;
using BH.Engine.Adapter;
using rf = Dlubal.RFEM5;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Adapters.RFEM;

namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private List<RigidLink> ReadLinks(List<string> ids = null)
        {
            List<RigidLink> linkList = new List<RigidLink>();
            rf.Line line;

            if (ids == null)
            {
                rf.Member[] allLinks = modelData.GetMembers().Where(x => x.Type == rf.MemberType.Rigid).ToArray();

                foreach (rf.Member link in allLinks)
                {
                    line = modelData.GetLine(link.LineNo, rf.ItemAt.AtNo).GetData();

                    rf.Point3D sPt = line.ControlPoints.First();
                    rf.Point3D ePt = line.ControlPoints.Last();

                    Node startNode = Engine.Structure.Create.Node(new oM.Geometry.Point() { X = sPt.X, Y = sPt.Y, Z = sPt.Z });
                    Node endNode = Engine.Structure.Create.Node(new oM.Geometry.Point() { X = ePt.X, Y = ePt.Y, Z = ePt.Z });

                    RigidLink bhLink = BH.Engine.Structure.Create.RigidLink(startNode, new Node[] { endNode });
                    LinkConstraint bhLinkConstraint = new LinkConstraint();

                    rf.MemberHinge[] hinges = modelData.GetMemberHinges();
                    if(hinges.Length==0)
                    {
                        //no hinges set, then all fixed
                        bhLink.Constraint.XtoX = true;
                        bhLink.Constraint.YtoY = true;
                        bhLink.Constraint.ZtoZ = true;
                        bhLink.Constraint.XXtoXX = true;
                        bhLink.Constraint.YYtoYY = true;
                        bhLink.Constraint.ZZtoZZ = true;
                    }
                    else
                    {
                        Engine.Reflection.Compute.RecordError("Hinges on Rigid links are not supported");
                    }

                    bhLink.SetAdapterId(typeof(RFEMId), link.No);

                    linkList.Add(bhLink);
                }
            }
            else
            {
                foreach (string id in ids)
                {
                    rf.Member link = modelData.GetMember(Int32.Parse(id), rf.ItemAt.AtNo).GetData();

                    if (link.Type != rf.MemberType.Rigid)
                        continue;

                    line = modelData.GetLine(link.LineNo, rf.ItemAt.AtNo).GetData();

                    rf.Point3D sPt = line.ControlPoints.First();
                    rf.Point3D ePt = line.ControlPoints.Last();

                    Node startNode = Engine.Structure.Create.Node(new oM.Geometry.Point() { X = sPt.X, Y = sPt.Y, Z = sPt.Z });
                    Node endNode = Engine.Structure.Create.Node(new oM.Geometry.Point() { X = ePt.X, Y = ePt.Y, Z = ePt.Z });

                    RigidLink bhLink = BH.Engine.Structure.Create.RigidLink(startNode, new Node[] { endNode });
                    LinkConstraint bhLinkConstraint = new LinkConstraint();

                    rf.MemberHinge[] hinges = modelData.GetMemberHinges();
                    if (hinges.Length == 0)
                    {
                        //no hinges set, then all fixed
                        bhLink.Constraint.XtoX = true;
                        bhLink.Constraint.YtoY = true;
                        bhLink.Constraint.ZtoZ = true;
                        bhLink.Constraint.XXtoXX = true;
                        bhLink.Constraint.YYtoYY = true;
                        bhLink.Constraint.ZZtoZZ = true;
                    }
                    else
                    {
                        Engine.Reflection.Compute.RecordError("Hinges on Rigid links are not supported");
                    }

                    bhLink.SetAdapterId(typeof(RFEMId), link.No);

                    linkList.Add(bhLink);
                }
            }

            return linkList;
        }

        /***************************************************/

    }
}

