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
                    break;
            }

            return profileType;
        }

        /***************************************************/
    }
}
