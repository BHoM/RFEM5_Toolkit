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
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Physical;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private bool CreateCollection(IEnumerable<ISectionProperty> sectionProperties)
        {
            if (sectionProperties.Count() > 0)
            {
                int idNum = 0;
                int matNumId = 0;
                List<ISectionProperty> secList = sectionProperties.ToList();
                rf.CrossSection[] rfCrossSections = new rf.CrossSection[secList.Count()];

                for (int i = 0; i < secList.Count(); i++)
                {

                    idNum = GetAdapterId<int>(secList[i]);// NextId(secList[i].GetType()));
                   // matNumId = GetAdapterId<int>(secList[i]);
                    matNumId = modelData.GetMaterials().ToList().IndexOf(modelData.GetMaterials().ToList().Find(m => m.Description.Split(' ')[1].Equals(secList[i].Material.Name))) + 1;
                    bool sectionAlredyInDict= m_sectionDict.Values.Any(s => s.Name.Equals(secList[i]));

                    if (!sectionAlredyInDict)
                    {
                        m_sectionDict.Add(m_sectionDict.Count+1, secList[i]);
                        rfCrossSections[i] = secList[i].ToRFEM(idNum, matNumId);
                        modelData.SetCrossSection(rfCrossSections[i]); 
                    }


                    rfCrossSections[i] = secList[i].ToRFEM(idNum, matNumId);
                    //modelData.SetCrossSection(rfCrossSections[i]);
                }

                //modelData.SetCrossSections(rfCrossSections);
            }

            return true;
        }


        /***************************************************/
    }
}


