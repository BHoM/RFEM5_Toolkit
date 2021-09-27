/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Spatial.ShapeProfiles;
using rf = Dlubal.RFEM5;
using rf3 = Dlubal.RFEM3;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Adapters.RFEM
{
    public static partial class Compute
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
            IProfile profile = null;

            //if (profileNameArr.Length > 2)
            //{
            //    //this is the case for 'Solid Timber 50/30'
            //    profileValues = profileNameArr[2].Split('/');//parametric sections
            //}
            //else
            //{
            //    if (profileNameArr[0].Contains('/'))
            //        profileValues = profileNameArr[1].Split('/');//parametric sections
            //    else
            //        profileValues = profileNameArr[1].Split('x');//standard sections - Note: can have format "IPE 80" and "IPE 750x137"
            //}

            if (sectionDBProps != null)//section from RFEM Library including sections defined in RFEM model
            {
                switch (profileNameArr[0])
                {
                    case "I":
                    case "IPE":
                    case "HE":
                    case "HEA":
                    case "HEB":
                    case "HEM":
                    case "UB":
                    case "UBP":
                    case "UC":
                    case "HD":
                        v1 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_h).fValue;
                        v2 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_b).fValue;
                        v3 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_t_s).fValue;
                        v4 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_t_g).fValue;
                        v5 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_r).fValue;
                        v6 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_r_1).fValue;
                        profile = Spatial.Create.ISectionProfile(v1, v2, v3, v4, v5, v6);
                        break;
                    case "IS"://parametric
                    case "ITS":
                    case "IUH":
                    case "IUV":
                        v1 = sectionDBProps[0].fValue;
                        v2 = sectionDBProps[1].fValue;
                        v3 = sectionDBProps[2].fValue;
                        v4 = sectionDBProps[3].fValue;
                        //v5 = sectionDBProps[4].fValue;
                        //v6 = sectionDBProps[5].fValue;
                        profile = Spatial.Create.ISectionProfile(v1, v2, v3, v4, 0, 0);
                        break;
                    case "SHS":
                    case "RHS":
                    case "QRO":
                    case "RRO":
                        v1 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_h).fValue;
                        v2 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_b).fValue;
                        v3 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_s).fValue;
                        profile = Spatial.Create.BoxProfile(v1, v2, v3, 0, 0);
                        break;
                    case "TO"://parametric
                    case "HSH":
                    case "HSV":
                        v1 = sectionDBProps[0].fValue;
                        v2 = sectionDBProps[1].fValue;
                        v3 = sectionDBProps[2].fValue;
                        v4 = sectionDBProps[3].fValue;
                        //v5 = sectionDBProps[4].fValue;
                        profile = Spatial.Create.BoxProfile(v1, v2, v3, v4, 0);
                        break;

                    case "SQ-S":
                    case "SQ-R":
                    case "4KT":
                        v1 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_s).fValue;
                        v2 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_b).fValue;
                        v3 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_r).fValue;
                        profile = Spatial.Create.RectangleProfile(v1, v2, v3);
                        break;
                    case "Rectangle"://parametric
                    case "T-Rectangle":
                        v1 = sectionDBProps[0].fValue;
                        v2 = sectionDBProps[1].fValue;
                        v3 = sectionDBProps[2].fValue;
                        profile = Spatial.Create.RectangleProfile(v1, v2, v3);
                        break;

                    case "L":
                    case "UKA":
                    case "LS":
                        v1 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_h).fValue;
                        v2 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_b).fValue;
                        v3 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_s).fValue;
                        v4 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_t_g).fValue;
                        v5 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_r).fValue;
                        v6 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_r_1).fValue;
                        if (v1 == 0)
                            v1 = v2;
                        if (v4 == 0)
                            v4 = v3;
                        profile = Spatial.Create.AngleProfile(v1, v2, v3, v4, v5, v6);
                        break;
                    case "LU"://parametric
                    case "KLU":
                        v1 = sectionDBProps[0].fValue;
                        v2 = sectionDBProps[1].fValue;
                        v3 = sectionDBProps[2].fValue;
                        v4 = sectionDBProps[3].fValue;
                        //v5 = sectionDBProps[4].fValue;
                        //v6 = sectionDBProps[5].fValue;
                        profile = Spatial.Create.AngleProfile(v1, v2, v3, v4, 0, 0);
                        break;

                    case "T":
                    case "TB":
                    case "TPS":
                        v1 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_h).fValue;
                        v2 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_b).fValue;
                        v3 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_t_s).fValue;
                        v4 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_t_g).fValue;
                        v5 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_r).fValue;
                        v6 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_r_1).fValue;
                        profile = Spatial.Create.TSectionProfile(v1, v2, v3, v4, v5, v6);
                        break;
                    case "TS"://parametric
                    case "FB":
                    case "TH":
                    case "TV":
                        v1 = sectionDBProps[0].fValue;
                        v2 = sectionDBProps[1].fValue;
                        v3 = sectionDBProps[2].fValue;
                        v4 = sectionDBProps[3].fValue;
                        //v5 = sectionDBProps[4].fValue;
                        //v6 = sectionDBProps[5].fValue;
                        profile = Spatial.Create.TSectionProfile(v1, v2, v3, v4, 0, 0);
                        break;

                    case "U":
                    case "C":
                    case "UPE":
                    case "UAP":
                    case "CH":
                    case "UPN":
                    case "PFC":
                        v1 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_h).fValue;
                        v2 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_b).fValue;
                        v3 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_t_s).fValue;
                        v4 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_t_g).fValue;
                        v5 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_r).fValue;
                        v6 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_r_1).fValue;
                        profile = Spatial.Create.ChannelProfile(v1, v2, v3, v4, v5, v6);
                        break;
                    case "UU"://parametric
                    case "UM":
                    case "PIH":
                    case "PIV":
                        v1 = sectionDBProps[0].fValue;
                        v2 = sectionDBProps[1].fValue;
                        v3 = sectionDBProps[2].fValue;
                        v4 = sectionDBProps[3].fValue;
                        //v5 = sectionDBProps[4].fValue;
                        //v6 = sectionDBProps[5].fValue;
                        profile = Spatial.Create.ChannelProfile(v1, v2, v3, v4, 0, 0);
                        break;
                    case "CHS":
                    case "RO":
                        v1 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_D).fValue;
                        v2 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_s).fValue;
                        profile = Spatial.Create.TubeProfile(v1, v2);
                        break;
                    case "Pipe"://parametric
                    case "Ring":
                        v1 = sectionDBProps[0].fValue;
                        v2 = sectionDBProps[1].fValue;
                        profile = Spatial.Create.TubeProfile(v1, v2);
                        break;
                    case "RD":
                    case "ROD":
                    case "RB":
                        v1 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_D).fValue;
                        profile = Spatial.Create.CircleProfile(v1);
                        break;
                    case "Round"://parametric 
                    case "Circle":
                    case "T-Circle":
                        v1 = sectionDBProps[0].fValue;
                        profile = Spatial.Create.CircleProfile(v1);
                        break;

                    //case "Z":
                    //case "KZ":
                    //    v1 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_h).fValue;
                    //    v2 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_b).fValue;
                    //    v3 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_t_s).fValue;
                    //    v4 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_t_g).fValue;
                    //    v5 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_r).fValue;
                    //    v6 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_r_1).fValue;
                    //    profile = new ZSectionProfile(v1, v2, v3, v4, v5, v6, "< looks like the only way to create this now is to use the profile curves !! >");
                    //    break;
                    default:
                        Engine.Reflection.Compute.RecordError("Don't know how to make profile: " + profileName);
                        break;
                }
            }

            /* ---TODO: cannot find shape types for these profiles: 
                BH.oM.Geometry.ShapeProfiles.FabricatedBoxProfile
                BH.oM.Geometry.ShapeProfiles.FabricatedISectionProfile
                BH.oM.Geometry.ShapeProfiles.GeneralisedFabricatedBoxProfile
                BH.oM.Geometry.ShapeProfiles.GeneralisedTSectionProfile
                BH.oM.Geometry.ShapeProfiles.KiteProfile
                BH.oM.Geometry.ShapeProfiles.TaperedProfile
                */

            return profile;
        }


        /***************************************************/
    }
}

