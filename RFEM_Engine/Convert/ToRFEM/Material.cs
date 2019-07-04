using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Physical.Materials;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using rf = Dlubal.RFEM5;

namespace BH.Engine.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static rf.Material ToRFEM(this IMaterialFragment materialFragment, int materialId)
        {
            rf.Material rfMaterial = new rf.Material();
            rfMaterial.No = materialId;
            rfMaterial.Description = materialFragment.Name;
            rfMaterial.SpecificWeight = materialFragment.Density;

            if (materialFragment is IIsotropic)
            {
                IIsotropic material = materialFragment as IIsotropic;
                rfMaterial.ThermalExpansion = material.ThermalExpansionCoeff;
                rfMaterial.PoissonRatio = material.PoissonsRatio;
                rfMaterial.ElasticityModulus = material.YoungsModulus;
            }
            else
            {
                Reflection.Compute.RecordWarning("Upsie Daisy! Isotropic materials only for now! cannot make " + materialFragment.Name);
            }

            return rfMaterial;

        }

    }
}
