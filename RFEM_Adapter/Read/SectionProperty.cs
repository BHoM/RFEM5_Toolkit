/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Base;
using BH.oM.Structure.SectionProperties;
using BH.oM.Common.Materials;
using BH.Engine.RFEM;
using rf = Dlubal.RFEM5;


namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        //The List<string> in the methods below can be changed to a list of any type of identification more suitable for the toolkit
        //If no ids are provided, the convention is to return all elements of the type

        private List<ISectionProperty> ReadSectionProperties(List<string> ids = null)
        {
            List<ISectionProperty> sectionPropList = new List<ISectionProperty>();

            //ReadSectionFromRFEMLibrary("IPE100");//<-- intended use but does not return anything useful

            if (ids == null)
            {
                foreach (rf.CrossSection rfSection in modelData.GetCrossSections())
                {
                    rf.Material rfMaterial = modelData.GetMaterial(rfSection.MaterialNo, rf.ItemAt.AtNo).GetData();
                    ISectionProperty section = rfSection.ToBHoM(rfMaterial);

                    sectionPropList.Add(section);

                    int sectionId = rfSection.No;
                    if(!m_sectionDict.ContainsKey(sectionId))
                    {
                        m_sectionDict.Add(sectionId, section);
                    }
                }
            }
            else
            {
                foreach (string id in ids)
                {
                    rf.CrossSection rfSection = modelData.GetCrossSection(Int32.Parse(id), rf.ItemAt.AtNo).GetData();
                    rf.Material rfMaterial = modelData.GetMaterial(rfSection.MaterialNo, rf.ItemAt.AtNo).GetData();
                    sectionPropList.Add(rfSection.ToBHoM(rfMaterial));
                    
                }
            }

            return sectionPropList;
        }

        /***************************************************/

        private void ReadSectionFromRFEMLibrary(string sectionName)
        {

            dynamic rfSectionDB = modelData.GetCrossSectionDatabase();// <-- there is no documentation on how to work with this

            
            Type t = rfSectionDB.GetType();

            System.Reflection.MethodInfo[] mi = t.GetMethods();

            List<string> describe = new List<string>();
            foreach(System.ComponentModel.PropertyDescriptor pd in System.ComponentModel.TypeDescriptor.GetProperties(rfSectionDB))
            {
                describe.Add(pd.Name);
            }
            

        }
    }
}
