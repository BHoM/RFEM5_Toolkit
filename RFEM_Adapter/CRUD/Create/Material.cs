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
using BH.oM.Physical;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private bool CreateCollection(IEnumerable<IMaterialFragment> materialFragments)
        {
            if(materialFragments.Count()>0)
            {
                int idNum = 0;
                List<IMaterialFragment> matList = materialFragments.ToList();
                rf.Material[] rfMaterials = new rf.Material[matList.Count()];

                for (int i = 0; i < matList.Count(); i++)
                {
                    idNum = GetAdapterId<int>(matList[i]);// NextId(matList[i].GetType()));
                    rfMaterials[i] = matList[i].ToRFEM(idNum);


                    List<rf.Material> alreadyExistingMaterialsOFEqualType = modelData.GetMaterials().ToList().FindAll(m => m.Description.Equals(rfMaterials[i].Description));


                    if (alreadyExistingMaterialsOFEqualType.Count==0) {
                        modelData.SetMaterial(rfMaterials[i]);
                    }
                   
                       
                    
                   
                }

                //modelData.SetMaterials(rfMaterials);
            }

            return true;
        }


        /***************************************************/
    }
}


