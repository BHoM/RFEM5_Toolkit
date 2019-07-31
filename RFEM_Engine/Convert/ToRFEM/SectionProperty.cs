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

        public static rf.CrossSection ToRFEM(this ISectionProperty sectionProperty, int sectionPropertyId, int materialId)
        {
            rf.CrossSection rfSectionProperty = new rf.CrossSection();
            rfSectionProperty.No = sectionPropertyId;
            rfSectionProperty.MaterialNo = materialId;
            rfSectionProperty.TextID = sectionProperty.Name;
            rfSectionProperty.Description = sectionProperty.Name + " | " + "show standard/norm";
            rfSectionProperty.AxialArea = sectionProperty.Area;
            rfSectionProperty.TorsionMoment = sectionProperty.J;
            rfSectionProperty.ShearAreaY = sectionProperty.Asy;
            rfSectionProperty.ShearAreaZ = sectionProperty.Asz;


            if (sectionProperty is SteelSection)
            {
                SteelSection steelSection = sectionProperty as SteelSection;
                //steelSection.SectionProfile.Shape... not sure this is directly readable from RFEM sections
                //steelSection.Fabrication = SteelFabrication.Welded; ... not sure if directly readable from rfem

            }
            else if (sectionProperty is ConcreteSection)
            {
                Reflection.Compute.RecordWarning("my responses are limited. I only speak steel sections at the moment. I dont know: " + sectionProperty.Name);
            }
            else if (sectionProperty is ExplicitSection)
            {
                Reflection.Compute.RecordWarning("my responses are limited. I only speak steel sections at the moment. I dont know: " + sectionProperty.Name);
            }
            else
            {
                Reflection.Compute.RecordWarning("my responses are limited. I only speak steel sections at the moment. I dont know: " + sectionProperty.Name);
            }

            return rfSectionProperty;

        }


    }
}
