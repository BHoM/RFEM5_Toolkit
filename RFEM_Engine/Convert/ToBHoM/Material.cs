using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Physical;
using rf = Dlubal.RFEM5;

namespace BH.Engine.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IMaterialFragment ToBHoM(this rf.Material material)
        {
            IMaterialFragment bhMaterial;

            if (material.ModelType == rf.MaterialModelType.IsotropicLinearElasticType)
            {
                bhMaterial = Engine.Structure.Create.Steel("S355 - I am just for testing");
            }
            else if (material.ModelType == rf.MaterialModelType.IsotropicPlastic2D3DType)
            {
                bhMaterial = Engine.Structure.Create.Steel("S355 - I am just for testing");
            }
            else if (material.ModelType == rf.MaterialModelType.IsotropicPlastic2D3DType)
            {
                bhMaterial = Engine.Structure.Create.Steel("S355 - I am just for testing");
            }
            else
            {
                bhMaterial = Engine.Structure.Create.Steel("S355 - I am just for testing");
            }

            return bhMaterial;
        }

    }
}
