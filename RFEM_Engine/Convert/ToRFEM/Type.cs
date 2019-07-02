using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using rf = Dlubal.RFEM5;

namespace BH.Engine.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        //Add methods for converting From BHoM to the specific software types, if possible to do without any BHoM calls

        public static rf.ModelObjectType ToRFEM(this Type bhObjectType)
        {
            string typeString = bhObjectType.Name;
            
            switch (typeString)
            {
                case "Node":
                    return rf.ModelObjectType.NodeObject;
                case "Bar":
                    return rf.ModelObjectType.MemberObject;
                case "Material":
                    return rf.ModelObjectType.MaterialObject;
                case "SectionProperty":
                    return rf.ModelObjectType.CrossSectionObject;
                default:
                    return rf.ModelObjectType.UnknownObject;
            }
        }
    }
}
