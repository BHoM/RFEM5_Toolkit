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
using BH.oM.Physical;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IMaterialFragment FromRFEM(this rf.Material material)
        {
            //NameID | Steel S 235@TypeID | STEEL@StandardID | DIN EN 1993 - 1 - 1 - 10
            IMaterialFragment bhMaterial = null;

            string[] stringArr = material.TextID.Split('@');
            MaterialType matType = GetMaterialTypeFromString(stringArr[1]);
            string matName = stringArr[0].Split('|')[1];

            switch (matType)
            {
                case MaterialType.Aluminium:
                    bhMaterial = Engine.Structure.Create.Aluminium(matName);
                    break;
                case MaterialType.Steel:
                    bhMaterial = Engine.Structure.Create.Steel(matName);
                    break;
                case MaterialType.Concrete:
                    bhMaterial = Engine.Structure.Create.Concrete(matName);
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

            return bhMaterial;
        }

        private static MaterialType GetMaterialTypeFromString(string RFEMTypeID)
        {
            switch (RFEMTypeID)//this should use the Query namespace !!!
            {
                case "TypeID|STEEL":
                    return MaterialType.Steel;
                case "TypeID|ALUMINIUM":
                    return MaterialType.Aluminium;
                case "TypeID|CONCRETE":
                    return MaterialType.Concrete;
                case "TypeID|TIMBER":
                case "TypeID|CONIFEROUS":
                    return MaterialType.Timber;
                case "TypeID|CABLE":
                    return MaterialType.Cable;
                case "TypeID|GLASS":
                    return MaterialType.Glass;
                case "TypeID|REBAR":
                    return MaterialType.Rebar;
                case "TypeID|TENDON":
                    return MaterialType.Tendon;
                default:
                    return MaterialType.Undefined;
            }
        }

    }
}
