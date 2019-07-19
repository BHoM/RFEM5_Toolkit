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

            string tmpDescription = rfSectionProperty.Description;

            if (BH.Engine.RFEM.Query.GetSectionType(rfSectionProperty)==typeof(SteelSection))
            {
                //bhSectionProperty = BH.Engine.Structure.Create.SteelISection(10, 10, 10, 10);
                bhSectionProperty = new ExplicitSection();
                //bhSectionProperty.Material = Query.GetMaterialFromStoredDict(rfSectionProperty.MaterialNo)
                bhSectionProperty.CustomData[AdapterId] = rfSectionProperty.No;
                //bhSectionProperty.Material = rfSectionProperty.MaterialNo;
                bhSectionProperty.Name = rfSectionProperty.TextID;
                bhSectionProperty.Area = rfSectionProperty.AxialArea;
                bhSectionProperty.J = rfSectionProperty.TorsionMoment;
                bhSectionProperty.Asy = rfSectionProperty.ShearAreaY;
                bhSectionProperty.Asz = rfSectionProperty.ShearAreaZ;


            }
            else
            {
                bhSectionProperty = null;
            }

            return bhSectionProperty;
        }

    }
}
