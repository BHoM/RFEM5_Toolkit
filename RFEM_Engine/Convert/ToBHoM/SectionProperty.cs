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
            //ISectionProperty bhSectionProperty;

            string tmpDescription = rfSectionProperty.Description;

            MaterialType materialType = Engine.RFEM.Query.GetMaterialType(rfMaterial);

            if (materialType == MaterialType.Steel)
            {
                double dim = Math.Sqrt(rfSectionProperty.AxialArea);
                SteelSection steelSection = Structure.Create.SteelRectangleSection(dim, dim);

                steelSection.CustomData[AdapterId] = rfSectionProperty.No;
                steelSection.Material = rfMaterial.ToBHoM();
                steelSection.Name = rfSectionProperty.TextID;

                //  - - - - read only ! cannot be assigned ! - - - - 
                //steelSection.Area = rfSectionProperty.AxialArea;
                //steelSection.J = rfSectionProperty.TorsionMoment;
                //steelSection.Asy = rfSectionProperty.ShearAreaY;
                //steelSection.Asz = rfSectionProperty.ShearAreaZ;

                Reflection.Compute.RecordWarning("section: "+ rfSectionProperty.TextID + " - Id: " + rfSectionProperty.No + "cannot be read fully! consider resetting parameters");

                return steelSection;
            }
            else
            {
                Reflection.Compute.RecordError("dont know how to make" + rfSectionProperty.TextID);
                return null;
            }


            //if (RFEM.Query.GetProfileType(rfSectionProperty.TextID) == oM.Geometry.ShapeProfiles.ShapeType.ISection)
            //{
            //    steelSection = Structure.Create.SteelISection(200, 10, 80, 80);
            //}
            //else if (RFEM.Query.GetProfileType(rfSectionProperty.TextID) == oM.Geometry.ShapeProfiles.ShapeType.Tube)
            //{
            //    steelSection = Structure.Create.SteelTubeSection(50, 5);
            //}
            //else if (RFEM.Query.GetProfileType(rfSectionProperty.TextID) == oM.Geometry.ShapeProfiles.ShapeType.Rectangle)
            //{
            //    steelSection = Structure.Create.SteelRectangleSection(200, 100);
            //}
            //else if (RFEM.Query.GetProfileType(rfSectionProperty.TextID) == oM.Geometry.ShapeProfiles.ShapeType.Box)
            //{
            //    steelSection = Structure.Create.SteelBoxSection(200, 100, 5);
            //}
            //else
            //{
            //    Reflection.Compute.RecordError("dont know how to make" + rfSectionProperty.TextID);
            //}

            //steelSection.CustomData[AdapterId] = rfSectionProperty.No;
            //steelSection.Name = rfSectionProperty.TextID;


            //return steelSection;
        }

    }
}
