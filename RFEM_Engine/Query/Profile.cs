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
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry.ShapeProfiles;
using BH.Engine.RFEM;
using rf = Dlubal.RFEM5;
using rf3 = Dlubal.RFEM3;

namespace BH.Engine.RFEM
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IProfile GetSectionProfile(string profileName, rf3.DB_CRSC_PROPERTY[] sectionDBProps = null)
        {
            // standard section name: SHS 25x25x2
            // parametric section name: TO 30/30/5/5/5/5
            string[] profileNameArr = profileName.Split(' ');
            string[] profileValues;
            double v1, v2, v3, v4, v5, v6, v7, v8, v9, v10;
            IProfile profile;

            if (profileNameArr.Length > 1)
            {
                //this is the case for 'Solid Timber 50/30'
                profileValues = profileNameArr[2].Split('/');//parametric sections
            }
            else
            {
                if (profileNameArr[0].Contains('x'))
                    profileValues = profileNameArr[1].Split('/');//parametric sections
                else
                    profileValues = profileNameArr[1].Split('x');//standard sections - Note: can have format "IPE 80" and "IPE 750x137"
            }

            for (int i = 0; i < 10; i++)
            {

            }


            switch (profileNameArr[0])
            {
                case "Rectangle":
                case "T-Rectangle"://timber
                    v1 = Convert.ToDouble(profileValues[0]);
                    v2 = Convert.ToDouble(profileValues[1]);
                    
                    profile = Structure.Create.RectangleProfile(v1, v2, 0);
                    return profile;
                case "SHS":
                case "RHS":
                case "QRO":
                case "RRO":
                case "TO":
                case "HSH":
                case "HSV":
                    v1 = Convert.ToDouble(profileValues[0]);
                    v2 = Convert.ToDouble(profileValues[1]);
                    v3 = Convert.ToDouble(profileValues[2]);
                    BoxProfile profile = Structure.Create.BoxProfile(v1, v2, v3, 0, 0);
                    return profile;
                case "L":
                case "LU":
                case "KLU":
                case "LS":
                    AngleProfile profile = new AngleProfile();
                    break;
                case "I":
                case "IPE":
                case "HE":
                case "HEA":
                case "HEB":
                case "UB":
                case "UBP":
                case "UC":
                case "IS":
                case "ITS":
                case "IUH":
                case "IUV":
                    ISectionProfile profile = new ISectionProfile();
                    break;
                case "T":
                case "TB":
                case "TPS":
                case "TS":
                case "FB":
                case "TH":
                case "TV":
                    TSectionProfile profile = new TSectionProfile();
                    break;
                case "U":
                case "UPE":
                case "UAP":
                case "CH":
                case "UPN":
                case "PFC":
                case "C":
                case "UU":
                case "UM":
                case "PIH":
                case "PIV":
                    ChannelProfile profile = new ChannelProfile();
                    break;
                case "CHS":
                case "RO":
                case "Pipe":
                case "Ring":
                    TubeProfile profile = new TubeProfile();
                    break;
                case "RD":
                case "ROD":
                case "RB":
                case "Round":
                case "Circle":
                case "T-Circle":
                    CircleProfile profile = new CircleProfile();
                    break;
                case "Z":
                case "KZ":
                case "Z(A)":
                case "Z(B)":
                case "Z_AM":
                case "Z_BM":
                    ZSectionProfile profile = new ZSectionProfile();
                    break;
                default:
                    break;
            }

            /* ---TODO: cannot find shape types for these profiles: 
                BH.oM.Geometry.ShapeProfiles.FabricatedBoxProfile
                BH.oM.Geometry.ShapeProfiles.FabricatedISectionProfile
                BH.oM.Geometry.ShapeProfiles.GeneralisedFabricatedBoxProfile
                BH.oM.Geometry.ShapeProfiles.GeneralisedTSectionProfile
                BH.oM.Geometry.ShapeProfiles.KiteProfile
                BH.oM.Geometry.ShapeProfiles.TaperedProfile
                */

        }


        /***************************************************/
    }
}
