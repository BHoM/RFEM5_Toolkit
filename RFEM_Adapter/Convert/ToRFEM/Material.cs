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
using BH.oM.Physical.Materials;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static rf.Material ToRFEM(this IMaterialFragment materialFragment, int materialId)
        {
            rf.Material rfMaterial = new rf.Material();
            rfMaterial.No = materialId;
            rfMaterial.Description = materialFragment.Name;
            rfMaterial.SpecificWeight = materialFragment.Density * 10; //translate from kg/m3 to kN/m3

            if (materialFragment is IIsotropic)
            {
                IIsotropic material = materialFragment as IIsotropic;
                rfMaterial.ThermalExpansion = material.ThermalExpansionCoeff;
                rfMaterial.PoissonRatio = material.PoissonsRatio;
                rfMaterial.ElasticityModulus = material.YoungsModulus;
                rfMaterial.ModelType = rf.MaterialModelType.IsotropicLinearElasticType;//--consider other types depending on analysis type
            }
            else
            {
                Reflection.Compute.RecordWarning("Upsie Daisy! Isotropic materials only for now! cannot make " + materialFragment.Name);
            }

            return rfMaterial;

        }

    }
}
