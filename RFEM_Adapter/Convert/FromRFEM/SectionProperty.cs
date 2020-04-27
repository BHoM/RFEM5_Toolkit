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

            // ----- TEST READING SECTIONS FROM LIBRARY -----
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


            MaterialType materialType = Engine.RFEM.Query.GetMaterialType(rfMaterial);
            IProfile profile = Engine.RFEM.Query.GetSectionProfile(sectionName, sectionDBProps);
            
            switch (materialType)
            {
                case MaterialType.Aluminium:
                    AluminiumSection aluSection = Create.AluminiumSectionFromProfile(profile);
                    //return aluSection;
                    return null;


                case MaterialType.Steel:
                    // add switch on steel section type ! ! ! ! ! 

                    ExplicitSection section = new ExplicitSection();
                    
                    section.CustomData[BH.Adapter.RFEM.Convert.AdapterIdName] = rfSectionProperty.No;
                    //section.Material = Structure.Create.Steel("default steel");
                    section.Material = rfMaterial.FromRFEM();
                    section.Name = rfSectionProperty.TextID;

                    section.Area = rfSectionProperty.AxialArea;
                    section.J = rfSectionProperty.TorsionMoment;
                    section.Asy = rfSectionProperty.ShearAreaY;
                    section.Asz = rfSectionProperty.ShearAreaZ;
                    section.Iy = rfSectionProperty.BendingMomentY;
                    section.Iz = rfSectionProperty.BendingMomentZ;

                    return section;
                case MaterialType.Concrete:
                    // add switch on profile type !!!!
                    //Create.ConcreteSectionFromProfile()
                    return null;
                case MaterialType.Timber:
                    //Create.TimberSectionFromProfile();
                    return null;
                case MaterialType.Rebar:
                case MaterialType.Tendon:
                case MaterialType.Glass:
                case MaterialType.Cable:
                case MaterialType.Undefined:
                default:
                    Engine.Reflection.Compute.RecordError("Don't know how to make" + rfSectionProperty.TextID);
                    return null;
            }



        }

    }
}
