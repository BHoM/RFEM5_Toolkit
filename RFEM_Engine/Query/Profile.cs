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

namespace BH.Engine.RFEM
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IProfile GetSectionProfile(string profileName)
        {
            string[] profileNameArr = profileName.Split(' ');
            string[] profileValues;

            switch (GetProfileType(profileName))
            {
                case ShapeType.Rectangle:
                    profileValues = profileNameArr[1].Split('/');
                    double b = Convert.ToDouble(profileValues[0]);
                    double h = Convert.ToDouble(profileValues[1]);
                    RectangleProfile profile = Structure.Create.RectangleProfile(h, b, 0);
                    return profile;
                case ShapeType.Box:
                    profileValues = profileNameArr[1].Split('/');
                    double b = Convert.ToDouble(profileValues[0]);
                    double h = Convert.ToDouble(profileValues[1]);
                    double t = Convert.ToDouble(profileValues[1]);
                    BoxProfile profile = Structure.Create.BoxProfile(h, b, t, 0, 0);
                    return profile;
                case ShapeType.Angle:
                    AngleProfile profile = new AngleProfile();
                    break;
                case ShapeType.ISection:
                    ISectionProfile profile = new ISectionProfile();
                    break;
                case ShapeType.Tee:
                    TSectionProfile profile = new TSectionProfile();
                    break;
                case ShapeType.Channel:
                    ChannelProfile profile = new ChannelProfile();
                    break;
                case ShapeType.Tube:
                    TubeProfile profile = new TubeProfile();
                    break;
                case ShapeType.Circle:
                    CircleProfile profile = new CircleProfile();
                    break;
                case ShapeType.Zed:
                    ZSectionProfile profile = new ZSectionProfile();
                    break;
                case ShapeType.FreeForm:
                    FreeFormProfile profile = new FreeFormProfile();
                    break;
                case ShapeType.DoubleAngle:
                    break;
                case ShapeType.DoubleISection:
                    break;
                case ShapeType.DoubleChannel:
                    break;
                case ShapeType.Cable:
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

        public static ShapeType GetProfileType(string profileName)
        {
            string typeString = profileName.Split(' ')[0];
            
            ShapeType profileType;

            switch (typeString)
            {
                case "RHS":
                    profileType = ShapeType.Rectangle;
                    break;
                case "SHS":
                    profileType = ShapeType.Box;
                    break;
                case "IPE":
                    profileType = ShapeType.ISection;
                    break;
                case "T":
                    profileType = ShapeType.Tee;
                    break;
                case "UPE":
                    profileType = ShapeType.Channel;
                    break;
                case "CHS":
                    profileType = ShapeType.Tube;
                    break;
                case "L":
                    profileType = ShapeType.Angle;
                    break;
                case "RD":
                    profileType = ShapeType.Circle;
                    break;
                case "Z":
                    profileType = ShapeType.Zed;
                    break;
                //case ShapeType.FreeForm:
                //case ShapeType.DoubleAngle:
                //case ShapeType.DoubleISection:
                //case ShapeType.DoubleChannel:
                //case ShapeType.Cable:

                default:
                    profileType = ShapeType.ISection;
                    Engine.Reflection.Compute.RecordError("Don't know how to make" + profileName);
                    break;
            }

            return profileType;
        }

        /***************************************************/
    }
}
