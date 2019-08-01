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
using BH.Engine.RFEM;
using BH.oM.Physical;
using rf = Dlubal.RFEM5;

namespace BH.Engine.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ISectionProperty ToBHoM(this rf.CrossSection rfSectionProperty, rf.Material rfMaterial)
        {

            MaterialType materialType = Engine.RFEM.Query.GetMaterialType(rfMaterial);

            if (materialType == MaterialType.Steel)
            {
                ExplicitSection section = new ExplicitSection();
                
                section.CustomData[AdapterId] = rfSectionProperty.No;
                //section.Material = Structure.Create.Steel("default steel");
                section.Material = rfMaterial.ToBHoM();
                section.Name = rfSectionProperty.TextID;

                section.Area = rfSectionProperty.AxialArea;
                section.J = rfSectionProperty.TorsionMoment;
                section.Asy = rfSectionProperty.ShearAreaY;
                section.Asz = rfSectionProperty.ShearAreaZ;
                section.Iy = rfSectionProperty.BendingMomentY;
                section.Iz = rfSectionProperty.BendingMomentZ;

                return section;
            }
            else
            {
                Reflection.Compute.RecordError("dont know how to make" + rfSectionProperty.TextID);
                return null;
            }


        }

    }
}
