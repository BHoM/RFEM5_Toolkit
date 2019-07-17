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

        public static rf.Member ToRFEM(this Bar bar, int barId, int lineId)
        {
            rf.Member rfBar = new rf.Member();
            rfBar.No = barId;
            rfBar.LineNo = lineId;

            rfBar.StartCrossSectionNo = System.Convert.ToInt32(bar.SectionProperty.CustomData[AdapterId]);

            return rfBar;
        }

    }
}
