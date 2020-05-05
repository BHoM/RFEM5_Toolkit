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
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry.ShapeProfiles;
using BH.Engine.Structure;
using BH.Engine.RFEM;
using BH.oM.Physical;
using rf = Dlubal.RFEM5;
using rf3 = Dlubal.RFEM3;

namespace BH.Adapter.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ISectionProperty FromRFEM(this rf.ICrossSection rfISectionProperty, rf.Material rfMaterial)
        {
            rf.CrossSection rfSectionProperty = rfISectionProperty.GetData();

            string sectionName = rfSectionProperty.Description;
            rf3.DB_CRSC_PROPERTY[] sectionDBProps = null;
            if (sectionName != "")
            {
                object libraryObj = rfISectionProperty.GetDatabaseCrossSection();
                rf3.IrfCrossSectionDB sectionFromDB = libraryObj as rf3.IrfCrossSectionDB;

                int propCount = sectionFromDB.rfGetPropertyCount();
                sectionDBProps = new rf3.DB_CRSC_PROPERTY[propCount];
                sectionFromDB.rfGetPropertyArrAll(propCount, sectionDBProps);

            }


            IMaterialFragment materialFragment = rfMaterial.FromRFEM();
            IProfile profile = Engine.RFEM.Query.GetSectionProfile(sectionName, sectionDBProps);

            IGeometricalSection geoSection =  Create.SectionPropertyFromProfile(profile, materialFragment, rfSectionProperty.TextID);// this creates the right property if the right material is provided 

            geoSection.CustomData[BH.Adapter.RFEM.Convert.AdapterIdName] = rfSectionProperty.No;
            geoSection.Name = rfSectionProperty.TextID;

            return geoSection;



        }

    }
}
