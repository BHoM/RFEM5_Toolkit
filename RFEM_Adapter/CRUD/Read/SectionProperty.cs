/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using rf = Dlubal.RFEM5;
using rf3 = RFEM3;


namespace BH.Adapter.RFEM5
{
    public partial class RFEM5Adapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        //The List<string> in the methods below can be changed to a list of any type of identification more suitable for the toolkit
        //If no ids are provided, the convention is to return all elements of the type

        private List<ISectionProperty> ReadSectionProperties(List<string> ids = null)
        {
           // m_sectionDict.Clear();
            List<ISectionProperty> sectionPropList = new List<ISectionProperty>();

            //ReadSectionFromRFEMLibrary("IPE 200");

            if (ids == null)
            {
                foreach (rf.CrossSection rfSection in modelData.GetCrossSections())
                {

                    int rfSectMaterialNumbe;

                    if (rfSection.MaterialNo==0)
                    {
                        Engine.Base.Compute.RecordWarning("Material number "+rfSection.No+" had no in RFEM Material assigned to it. Insted Material number 1 had been assigned to the Cross Section");
                        rfSectMaterialNumbe = 1;

                    }
                    else
                    {
                        rfSectMaterialNumbe= rfSection.MaterialNo;
                    }

                    
                    //rf.Material rfMaterial = modelData.GetMaterial(rfSection.MaterialNo, rf.ItemAt.AtNo).GetData();
                    rf.Material rfMaterial = modelData.GetMaterial(rfSectMaterialNumbe, rf.ItemAt.AtNo).GetData();

                    rf.ICrossSection rfISection = modelData.GetCrossSection(rfSection.No, rf.ItemAt.AtNo);
                    ISectionProperty section = rfISection.FromRFEM(rfMaterial);

                    sectionPropList.Add(section);

                    int sectionId = rfSection.No;
                    if (!m_sectionDict.ContainsKey(sectionId))
                    {
                        section.Name = rfISection.GetData().Comment;
                        m_sectionDict.Add(sectionId, section);
                    }
                }
            }
            else
            {
                foreach (string id in ids)
                {
                    rf.ICrossSection rfISection = modelData.GetCrossSection(Int32.Parse(id), rf.ItemAt.AtNo);
                    rf.CrossSection rfSection = rfISection.GetData();
                    rf.Material rfMaterial = modelData.GetMaterial(rfSection.MaterialNo, rf.ItemAt.AtNo).GetData();
                    sectionPropList.Add(rfISection.FromRFEM(rfMaterial));
                }
            }


            return sectionPropList;
        }

        /***************************************************/

        private bool ReadSectionFromRFEMLibrary(string sectionName)
        {

            dynamic rfSectionDB = modelData.GetCrossSectionDatabase();// <-- there is no documentation on how to work with this

            rf3.IrfDatabaseCrSc2 dbCrSc = rfSectionDB as rf3.IrfDatabaseCrSc2;// <-- cast rfem5 type to rfem3 type
            if (!dbCrSc.rfIsCrossSectionInDB(sectionName))
                return false;
            rf3.IrfCrossSectionDB csDB = dbCrSc.rfGetCrossSection(sectionName);

            //------------ using RFEM3.IrfCrossSectionDB.rfGetProperty() does work but property types need to be known in advance
            double alpha = csDB.rfGetProperty(rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_A);


            //------------ using RFEM3.IrfCrossSectionDB.rfGetPropertyArr() does work but property types need to be known in advance
            int propertyCount = 5;
            rf3.DB_CRSC_PROPERTY_ID[] propertyIds = new rf3.DB_CRSC_PROPERTY_ID[propertyCount];
            double[] propertyValues = new double[propertyCount];

            propertyIds[0] = rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_b;
            propertyIds[1] = rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_h;
            propertyIds[2] = rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_t_s;
            propertyIds[3] = rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_t_g;
            propertyIds[4] = rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_s;

            csDB.rfGetPropertyArr(propertyCount, ref propertyIds[0], out propertyValues[0]);//should be array


            // ------------ using RFEM3.IrfCrossSectionDB.rfGetPropertyArrAll() does not work ! method signature soes not match documentation
            /*
            int count = csDB.rfGetPropertyCount();
            rf3.DB_CRSC_PROPERTY[] props = new rf3.DB_CRSC_PROPERTY[count];
            count = csDB.rfGetPropertyArrAll(count, out props[0]);//<------- This returns a null reference exception !
            List<string> tmpText = new List<string>();
            for (int i = 0; i < count-1; i++)
                tmpText.Add(props[i].strName);
            */


            // release COM objects as per documentation
            rfSectionDB = null;
            dbCrSc = null;
            csDB = null;

            return true;

        }
    }
}





