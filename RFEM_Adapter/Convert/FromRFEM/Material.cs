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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Physical;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM5;
using rf = Dlubal.RFEM5;
using BH.Engine.Adapters.RFEM5;

namespace BH.Adapter.RFEM5
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IMaterialFragment FromRFEM(this rf.Material material)
        {

            IMaterialFragment bhMaterial = null;

            string[] stringArr = material.TextID.Split('@');
            MaterialType matType = material.DetermineMaterialType();// Engine.Adapters.RFEM.Query.GetMaterialType(material);
            //string matName = Engine.Adapters.RFEM.Query.GetMaterialName(material);
            string matName = material.TextID == "" ? material.Description.Split(':')[1] : material.Description;
            String[] matParaArray = material.Comment.Split('|');

            switch (matType)
            {
                case MaterialType.Aluminium:
                    bhMaterial = Engine.Structure.Create.Aluminium(matName);
                    break;
                case MaterialType.Steel:

                    double yieldStress = (matParaArray.Length > 1) ? Double.Parse(matParaArray[1]) : 0;
                    double ulitimateStess = (matParaArray.Length > 1) ? Double.Parse(matParaArray[2]) : 0;

                    bhMaterial = Engine.Structure.Create.Steel(matName, material.ElasticityModulus, material.PoissonRatio, material.ThermalExpansion, material.SpecificWeight * 0.1, 0, yieldStress, ulitimateStess);

                    break;
                case MaterialType.Concrete:

                    double cylinderStrenth = (matParaArray.Length > 1) ? Double.Parse(matParaArray[1]) : 0;
                    double cubeStrength = (matParaArray.Length > 1) ? Double.Parse(matParaArray[2]) : 0;

                    bhMaterial = Engine.Structure.Create.Concrete(matName, material.ElasticityModulus, material.PoissonRatio, material.ThermalExpansion, material.SpecificWeight * 0.1, 0, cubeStrength, cylinderStrenth);

                    break;
                case MaterialType.Timber://TODO: as this uses vector over double assumption is the the below turns Timber into an incorrect Isotropic material !!!
                    BH.oM.Geometry.Vector young = new oM.Geometry.Vector() { X = material.ElasticityModulus, Y = material.ElasticityModulus, Z = material.ElasticityModulus };
                    BH.oM.Geometry.Vector poissons = new oM.Geometry.Vector() { X = material.PoissonRatio, Y = material.PoissonRatio, Z = material.PoissonRatio };
                    BH.oM.Geometry.Vector shear = new oM.Geometry.Vector() { X = material.ShearModulus, Y = material.ShearModulus, Z = material.ShearModulus };
                    BH.oM.Geometry.Vector thermal = new oM.Geometry.Vector() { X = material.ThermalExpansion, Y = material.ThermalExpansion, Z = material.ThermalExpansion };
                    bhMaterial = Engine.Structure.Create.Timber(matName, young, poissons, shear, thermal, material.SpecificWeight, 0.05);
                    break;
                case MaterialType.Rebar:
                case MaterialType.Tendon:
                case MaterialType.Glass:
                case MaterialType.Cable:
                case MaterialType.Undefined:
                default:
                    break;
            }

            bhMaterial.Name = matName;

            bhMaterial.SetAdapterId(typeof(RFEM5Id), material.No);
            return bhMaterial;
        }

        private static MaterialType DetermineMaterialType(this rf.Material rfMaterial)
        {
       

            string materialString = "";
            string[] materialStringArr;

            if (rfMaterial.Equals(null))
            {
                Engine.Base.Compute.RecordWarning("Material was Null and has been set to Steel");
                return oM.Structure.MaterialFragments.MaterialType.Steel; ; //A suitable return - you could `return null;` here instead if needed
            }
            else
            {

                materialStringArr = rfMaterial.TextID.Split('@');
                materialString = rfMaterial.TextID == "" ? rfMaterial.Description.Split(':')[0] : materialStringArr[1].Split('|')[1];
            }


            switch (materialString)
            {

                case "TypeID|STEEL":
                case "STEEL":
                    return oM.Structure.MaterialFragments.MaterialType.Steel;
                case "ALUMINIUM":
                    return oM.Structure.MaterialFragments.MaterialType.Aluminium;
                case "CONCRETE":
                    return oM.Structure.MaterialFragments.MaterialType.Concrete;
                case "TIMBER":
                case "CONIFEROUS":
                    return oM.Structure.MaterialFragments.MaterialType.Timber;
                case "CABLE":
                    return oM.Structure.MaterialFragments.MaterialType.Cable;
                case "GLASS":
                    return oM.Structure.MaterialFragments.MaterialType.Glass;
                case "TREBAR":
                    return oM.Structure.MaterialFragments.MaterialType.Rebar;
                case "TENDON":
                    return oM.Structure.MaterialFragments.MaterialType.Tendon;
                default:
                    Engine.Base.Compute.RecordWarning("Don't know how to make: " + materialString[1]);
                    return oM.Structure.MaterialFragments.MaterialType.Undefined;
            }

        }


        /***************************************************/
    }

}






