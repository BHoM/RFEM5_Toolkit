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
using BH.Engine.Spatial;
using rf = Dlubal.RFEM5;
using BH.oM.Structure.MaterialFragments;

namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private List<Bar> ReadBars(List<string> ids = null)
        {
            List<Bar> barList = new List<Bar>();
            rf.Line line;
            ISectionProperty sectionProperty;


            if (ids == null)
            {
                foreach (rf.Member member in modelData.GetMembers())
                {
                    if (member.Type == rf.MemberType.Rigid)
                        continue;

                    line = modelData.GetLine(member.LineNo, rf.ItemAt.AtNo).GetData();

                    sectionProperty = GetSectionProperty(member.StartCrossSectionNo);

                    //check for tapered section
                    if(member.EndCrossSectionNo!=0)
                    {
                        ISectionProperty startSectionProperty = sectionProperty;
                        ISectionProperty endSectionProperty = GetSectionProperty(member.EndCrossSectionNo);
                        sectionProperty = GetTaperedSectionProperty(startSectionProperty, endSectionProperty);
                    }

                    barList.Add(member.FromRFEM(line, sectionProperty));
                }
            }
            else
            {
                foreach(string id in ids)
                {
                    rf.Member member = modelData.GetMember(Int32.Parse(id), rf.ItemAt.AtNo).GetData();

                    if (member.Type == rf.MemberType.Rigid)
                        continue;

                    line = modelData.GetLine(member.LineNo, rf.ItemAt.AtNo).GetData();

                    sectionProperty = GetSectionProperty(member.StartCrossSectionNo);

                    if (member.EndCrossSectionNo != 0)
                    {
                        ISectionProperty startSectionProperty = sectionProperty;
                        ISectionProperty endSectionProperty = GetSectionProperty(member.EndCrossSectionNo);
                        sectionProperty = GetTaperedSectionProperty(startSectionProperty, endSectionProperty);
                    }

                    barList.Add(member.FromRFEM(line, sectionProperty));
                }
            }

            return barList;
        }

        /***************************************************/

        private ISectionProperty GetSectionProperty(int crossSectionNumber)
        {
            ISectionProperty sectionProperty;

            if (!m_sectionDict.TryGetValue(crossSectionNumber, out sectionProperty))
            {
                rf.ICrossSection rfISection = modelData.GetCrossSection(crossSectionNumber, rf.ItemAt.AtNo);
                rf.CrossSection rfSection = rfISection.GetData();
                rf.Material rfMat = modelData.GetMaterial(rfSection.MaterialNo, rf.ItemAt.AtNo).GetData();
                sectionProperty = rfISection.FromRFEM(rfMat);
                m_sectionDict.Add(crossSectionNumber, sectionProperty);
            }

            return sectionProperty;
        }

        private ISectionProperty GetTaperedSectionProperty(ISectionProperty startSectionProperty, ISectionProperty endSectionProperty)
        {
            if (startSectionProperty.Material.Name != endSectionProperty.Material.Name)
                Engine.Reflection.Compute.RecordWarning("Tapered section mixes materials. Only material from start of section is used");

            oM.Spatial.ShapeProfiles.IProfile taperProfile = null;

            if (startSectionProperty.Material.GetType() == typeof(Steel))
            {
                SteelSection startSection = startSectionProperty as SteelSection;
                SteelSection endSection = endSectionProperty as SteelSection;
                taperProfile = BH.Engine.Spatial.Create.TaperedProfile(startSection.SectionProfile, endSection.SectionProfile);
                taperProfile.Name = "TaperedProfile-" + startSection.Name + "-To-" + endSection.Name;
            }

            if (startSectionProperty.Material.GetType() == typeof(Concrete))
            {
                ConcreteSection startSection = startSectionProperty as ConcreteSection;
                ConcreteSection endSection = endSectionProperty as ConcreteSection;
                taperProfile = BH.Engine.Spatial.Create.TaperedProfile(startSection.SectionProfile, endSection.SectionProfile);
                taperProfile.Name = "TaperedProfile-" + startSection.Name + "-To-" + endSection.Name;
            }

            if (startSectionProperty.Material.GetType() == typeof(Timber))
            {
                TimberSection startSection = startSectionProperty as TimberSection;
                TimberSection endSection = endSectionProperty as TimberSection;
                taperProfile = BH.Engine.Spatial.Create.TaperedProfile(startSection.SectionProfile, endSection.SectionProfile);
                taperProfile.Name = "TaperedProfile-" + startSection.Name + "-To-" + endSection.Name;
            }

            return BH.Engine.Structure.Create.SectionPropertyFromProfile(taperProfile, startSectionProperty.Material);

        }
    }
}

