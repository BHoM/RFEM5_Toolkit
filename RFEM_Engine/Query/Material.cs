using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.MaterialFragments;
using BH.Engine.RFEM;
using BH.Engine.Structure;
using rf = Dlubal.RFEM5;

namespace BH.Engine.RFEM
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        //Text ID contains the following parameters: 
        //• NameID - Language independent name of the material according to the German terminology.
        //• TypeID - Language independent type of the material.
        //• NormID - Language independent code of the material. 
        //Format: NameID|material ID@TypeID|material type@NormID|material code
        //Example: NameID|Steel S 235@TypeID|STEEL@StandardID|DIN EN 1993-1-1-10 

        public static MaterialType GetMaterialType(rf.Material rfMaterial)
        {
            string[] materialString = rfMaterial.TextID.Split('@');

            switch (materialString[1])
            {
                case "TypeID|STEEL":
                    return MaterialType.Steel;
                case "TypeID|Concrete"://best guess on string
                    return MaterialType.Concrete;
                case "TypeID|Timber"://best guess on string
                    return MaterialType.Timber;
                default:
                    return MaterialType.Steel;
            }

        }


        public static string GetMaterialType(IMaterialFragment material)
        {
            Type materialType = material.GetType();

            if (materialType == typeof(Aluminium))
            {
                return "TypeID|ALUMINIUM";
            }
            if (materialType == typeof(Steel))
            {
                return "TypeID|STEEL";
            }
            if (materialType == typeof(Concrete))
            {
                return "TypeID|CONCRETE";
            }
            if (materialType == typeof(Timber))
            {
                return "TypeID|TIMBER";
            }
            else
            {
                return null;
            }

        }

        public static string GetMaterialName(rf.Material rfMaterial)
        {
            string[] materialString = rfMaterial.TextID.Split('@');

            string materialName = materialString[0].Split('|')[1];//<-- not safe

            return materialName;
        }


        /***************************************************/
    }
}
