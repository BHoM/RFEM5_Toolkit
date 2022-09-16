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
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Physical;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM;
using rf = Dlubal.RFEM5;
using BH.Engine.Adapters.RFEM;

namespace BH.Adapter.RFEM
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
            MaterialType matType = material.GetMaterialType();// Engine.Adapters.RFEM.Query.GetMaterialType(material);
            string matName = Engine.Adapters.RFEM.Query.GetMaterialName(material);
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

            bhMaterial.SetAdapterId(typeof(RFEMId), material.No);
            return bhMaterial;
        }

        //private static double[] getMaterialStrength(rf.Material material)
        //{
        //    string[] strengthArray = new string[] { "0", "0" };

        //    //Non RFEM Libary Material
        //    if (material.TextID.Equals(""))
        //    {

        //        string[] materialStringArr = material.Description.Split(':');
        //        string materialGradeString = "";

        //        switch (materialStringArr[0])
        //        {
        //            case "CONCRETE":
        //                materialGradeString = materialStringArr[1].Substring(2);
        //                strengthArray = materialGradeString.Split('/');
        //                break;
        //            case "STEEL":
        //                //Upper boundary for Reading the yield stress from name. Check for Rebar
        //                int upperBoundary = materialStringArr[1].Substring(0, 2) == " B" ? materialStringArr[1].Length - 3 : materialStringArr[1].Length - 2;
        //                strengthArray[0] = materialStringArr[1].Substring(2, upperBoundary).Split(null)[0];
        //                break;
        //            case "TIMBER":
        //                break;
        //            case "ALUMINIUM":
        //                break;
        //            default:
        //                break;

        //        }

        //    }// RFEM Libary Material
        //    else
        //    {
        //        string[] materialStringArr = material.TextID.Split('@');
        //        string materialGradeString = "";

        //        switch (materialStringArr[1])
        //        {
        //            case "TypeID|CONCRETE":
        //                materialGradeString = materialStringArr[0].Split(null)[1];
        //                strengthArray = materialGradeString.Substring(1).Split('/');
        //                break;
        //            case "TypeID|STEEL":
        //                strengthArray[0] = materialStringArr[0].Split(null)[2]; ;
        //                break;
        //            case "TypeID|TIMBER":

        //                break;
        //            case "TypeID|ALUMINIUM":

        //                break;
        //            default:
        //                break;

        //        }

        //    }

        //    return new double[] { System.Convert.ToDouble(strengthArray[0]) * 1e6, System.Convert.ToDouble(strengthArray[1]) * 1e6 };
        //}

    }
}


