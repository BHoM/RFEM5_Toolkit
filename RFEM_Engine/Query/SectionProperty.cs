using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.SectionProperties;
using BH.Engine.RFEM;
using rf = Dlubal.RFEM5;

namespace BH.Engine.RFEM
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/


        public static Type GetSectionType(rf.CrossSection rfSectionProperty)
        {
            string[] propertyString = rfSectionProperty.TextID.Split('@');

            if (true)
            {
                return typeof(SteelSection);
            }

        }

        /***************************************************/
    }
}
