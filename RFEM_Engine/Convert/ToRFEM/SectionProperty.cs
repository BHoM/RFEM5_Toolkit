using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Physical.Materials;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using rf = Dlubal.RFEM5;

namespace BH.Engine.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static rf.CrossSection ToRFEM(this ISectionProperty sectionProperty, int sectionPropertyId)
        {
            rf.CrossSection rfSectionProperty = new rf.CrossSection();
            rfSectionProperty.No = sectionPropertyId;
            rfSectionProperty.MaterialNo = 1;//material number must be unique and should be looked up in the currently used material list
            rfSectionProperty.Description = sectionProperty.Name;

            
            if (sectionProperty is SteelSection)
            {
                SteelSection steelSection = sectionProperty as SteelSection;
            }
            else
            {
                Reflection.Compute.RecordWarning("my responses are limited. I only speak steel sections at the moment. I dont know: " + sectionProperty.Name);
            }

            return rfSectionProperty;

        }


    }
}
