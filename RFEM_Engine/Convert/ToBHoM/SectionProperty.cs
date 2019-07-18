using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.Engine.Structure;
using BH.oM.Physical;
using rf = Dlubal.RFEM5;

namespace BH.Engine.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ISectionProperty ToBHoM(this rf.CrossSection rfSectionProperty)
        {
            ISectionProperty bhSectionProperty;

            if (BH.Engine.RFEM.Query.GetSectionType(rfSectionProperty)==typeof(SteelSection))
            {
                bhSectionProperty = BH.Engine.Structure.Create.SteelISection(10, 10, 10, 10);
            }
            else
            {
                bhSectionProperty = null;
            }

            return bhSectionProperty;
        }

    }
}
